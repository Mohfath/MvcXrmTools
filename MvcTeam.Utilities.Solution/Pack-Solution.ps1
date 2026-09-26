<#
.SYNOPSIS
    Packs the freshly built MvcTeam.Utilities.* assemblies into the Dataverse solution zip. Runs after a Release build.

.DESCRIPTION
    Source of truth is the Solution\ folder next to this script (solution.xml, customizations.xml, [Content_Types].xml).
    For every plugin assembly registered in customizations.xml the script:
      1. takes the matching MvcTeam.Utilities.<Name>.dll from -BinDir and reads its real version and public key token;
      2. writes that version into the assembly's FullName, every plugin type's AssemblyQualifiedName and the
         "<Assembly> (<version>)" workflow group name;
      3. compares the workflow activity types inside the DLL with the registered plugin types. A new step gets a new
         PluginTypeId and FriendlyName GUID; a step that no longer exists is removed; existing ids are kept so CRM
         upgrades in place. When that changes anything, Solution\customizations.xml is updated so the ids stay stable;
      4. sets the solution <Version> and each root component's schemaName to the versions above.
    The solution version is the highest assembly version, so it rises with every build (see Update-AssemblyVersion.ps1).
    The zip is written to -OutputDir as <UniqueName>_<major>_<minor>_<managed|unmanaged>.zip and checked afterwards.
    Only Release output is the real package; any other -Configuration (Debug) gets a _<Configuration> suffix, because
    Debug assemblies trace every input value and must not be mistaken for the package to install.
    The version-only edits go to a staging copy; the tracked source files only change when steps are added or removed.
    Any problem throws, so powershell exits 1 and the build fails.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$BinDir,
    [Parameter(Mandatory = $true)][string]$PackagesDir,
    [Parameter(Mandatory = $true)][string]$OutputDir,
    [Parameter(Mandatory = $true)][string]$StagingDir,
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceDir = Join-Path $here 'Solution'
$BinDir = [IO.Path]::GetFullPath($BinDir)
$PackagesDir = [IO.Path]::GetFullPath($PackagesDir)
$OutputDir = [IO.Path]::GetFullPath($OutputDir)
$StagingDir = [IO.Path]::GetFullPath($StagingDir)

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

# ---- helpers -------------------------------------------------------------------------------------------------

# Files are read and written without a BOM and without touching line endings; the originals have none.
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Read-XmlKeepingWhitespace([string]$path) {
    $doc = New-Object System.Xml.XmlDocument
    $doc.PreserveWhitespace = $true
    $doc.LoadXml([IO.File]::ReadAllText($path, $utf8NoBom).TrimStart([char]0xFEFF))
    return $doc
}

function Save-Xml([System.Xml.XmlDocument]$doc, [string]$path) {
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Encoding = $utf8NoBom
    $settings.OmitXmlDeclaration = $true
    $settings.Indent = $false
    $settings.NewLineHandling = [System.Xml.NewLineHandling]::None
    $writer = [System.Xml.XmlWriter]::Create($path, $settings)
    try { $doc.Save($writer) } finally { $writer.Close() }
}

# Reflection-only loading never runs the assemblies; dependencies come from the NuGet packages folder.
$dependencies = @{}
Get-ChildItem -Path $PackagesDir -Recurse -Filter '*.dll' -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match 'net452|net35' } |
    ForEach-Object { $dependencies[$_.BaseName.ToLowerInvariant()] = $_.FullName }
[AppDomain]::CurrentDomain.add_ReflectionOnlyAssemblyResolve({
    param($sender, $e)
    $name = ($e.Name -split ',')[0].ToLowerInvariant()
    if ($dependencies.ContainsKey($name)) { return [Reflection.Assembly]::ReflectionOnlyLoadFrom($dependencies[$name]) }
    try { return [Reflection.Assembly]::ReflectionOnlyLoad($e.Name) } catch { return $null }
})

function Test-IsWorkflowActivity($type) {
    $base = $type.BaseType
    while ($base -ne $null) {
        if ($base.FullName -eq 'System.Activities.CodeActivity') { return $true }
        $base = $base.BaseType
    }
    return $false
}

function Get-ActivityTypeNames([string]$dllPath) {
    $assembly = [Reflection.Assembly]::ReflectionOnlyLoadFrom($dllPath)
    try { $types = $assembly.GetTypes() }
    catch [Reflection.ReflectionTypeLoadException] { throw "Could not read the types of ${dllPath}: $($_.Exception.LoaderExceptions[0].Message)" }
    return @($types | Where-Object { $_.IsPublic -and -not $_.IsAbstract -and (Test-IsWorkflowActivity $_) } |
        ForEach-Object { $_.FullName } | Sort-Object)
}

# ---- load the tracked solution source ------------------------------------------------------------------------

foreach ($file in 'solution.xml', 'customizations.xml', '[Content_Types].xml') {
    if (-not (Test-Path -LiteralPath (Join-Path $sourceDir $file))) { throw "Missing solution source file: $sourceDir\$file" }
}

if (Test-Path -LiteralPath $StagingDir) { Remove-Item -LiteralPath $StagingDir -Recurse -Force }
New-Item -ItemType Directory -Path $StagingDir | Out-Null

$customizations = Read-XmlKeepingWhitespace (Join-Path $sourceDir 'customizations.xml')
$solution = Read-XmlKeepingWhitespace (Join-Path $sourceDir 'solution.xml')
$sourceChanged = $false
$versions = @()
$stagedDlls = @()
$summary = @()

foreach ($assemblyNode in $customizations.SelectNodes('/ImportExportXml/SolutionPluginAssemblies/PluginAssembly')) {
    $assemblyName = ($assemblyNode.GetAttribute('FullName') -split ',')[0].Trim()
    $dll = Join-Path $BinDir "$assemblyName.dll"
    if (-not (Test-Path -LiteralPath $dll)) { throw "Built assembly not found: $dll (build the solution in Release first)." }

    $name = [Reflection.AssemblyName]::GetAssemblyName($dll)
    $version = $name.Version.ToString()
    $token = ($name.GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') }) -join ''
    if ($assemblyNode.GetAttribute('FullName') -notmatch "PublicKeyToken=$token") {
        throw "$assemblyName is signed with token $token, which differs from the solution's registered token."
    }
    $versions += $name.Version
    $fullName = "$assemblyName, Version=$version, Culture=neutral, PublicKeyToken=$token"
    $assemblyNode.SetAttribute('FullName', $fullName)

    # Plugin types: keep ids of steps that still exist, add new steps, drop steps that are gone
    $typesNode = $assemblyNode.SelectSingleNode('PluginTypes')
    $existing = @{}
    foreach ($typeNode in $typesNode.SelectNodes('PluginType')) { $existing[$typeNode.GetAttribute('Name')] = $typeNode }
    $actual = Get-ActivityTypeNames $dll
    $added = @($actual | Where-Object { -not $existing.ContainsKey($_) })
    $removed = @($existing.Keys | Where-Object { $actual -notcontains $_ } | Sort-Object)

    foreach ($gone in $removed) {
        $node = $existing[$gone]
        $before = $node.PreviousSibling
        if ($before -ne $null -and $before.NodeType -eq [System.Xml.XmlNodeType]::Whitespace) { [void]$typesNode.RemoveChild($before) }
        [void]$typesNode.RemoveChild($node)
        $existing.Remove($gone)
        $sourceChanged = $true
    }
    foreach ($new in $added) {
        $template = @($typesNode.SelectNodes('PluginType'))[0]
        $node = $template.CloneNode($true)
        $node.SetAttribute('Name', $new)
        $node.SetAttribute('PluginTypeId', [guid]::NewGuid().ToString())
        $node.SelectSingleNode('FriendlyName').InnerText = [guid]::NewGuid().ToString()
        # Keep the list alphabetical, as CRM exports it
        $next = @($typesNode.SelectNodes('PluginType') | Where-Object { [string]::CompareOrdinal($_.GetAttribute('Name'), $new) -gt 0 })[0]
        $gap = $template.PreviousSibling.CloneNode($true)
        if ($next -ne $null) {
            [void]$typesNode.InsertBefore($node, $next.PreviousSibling)
            [void]$typesNode.InsertBefore($gap, $node)
        }
        else {
            $last = $typesNode.LastChild
            [void]$typesNode.InsertBefore($gap, $last)
            [void]$typesNode.InsertBefore($node, $last)
        }
        $sourceChanged = $true
    }

    foreach ($typeNode in $typesNode.SelectNodes('PluginType')) {
        $typeNode.SetAttribute('AssemblyQualifiedName', "$($typeNode.GetAttribute('Name')), $fullName")
        $typeNode.SelectSingleNode('WorkflowActivityGroupName').InnerText = "$assemblyName ($version)"
    }

    # Root component of the solution
    $id = '{' + $assemblyNode.GetAttribute('PluginAssemblyId').ToLowerInvariant() + '}'
    $root = $solution.SelectSingleNode("/ImportExportXml/SolutionManifest/RootComponents/RootComponent[@type='91' and @id='$id']")
    if ($root -eq $null) { throw "solution.xml has no root component $id for $assemblyName." }
    $root.SetAttribute('schemaName', $fullName)

    # Stage the DLL where the solution expects it (FileName holds the path inside the zip)
    $entryName = $assemblyNode.SelectSingleNode('FileName').InnerText.TrimStart('/')
    $target = Join-Path $StagingDir ($entryName -replace '/', '\')
    New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force | Out-Null
    Copy-Item -LiteralPath $dll -Destination $target
    $stagedDlls += [pscustomobject]@{ Entry = $entryName; Path = $target }

    $summary += ('  {0} {1}: {2} step(s){3}{4}' -f $assemblyName, $version, $actual.Count,
        $(if ($added.Count) { ", +$($added -join ', +')" } else { '' }),
        $(if ($removed.Count) { ", -$($removed -join ', -')" } else { '' }))
}

$solutionVersion = ($versions | Sort-Object | Select-Object -Last 1).ToString()
$solution.SelectSingleNode('/ImportExportXml/SolutionManifest/Version').InnerText = $solutionVersion

# The tracked customizations.xml keeps the ids of the plugin types, so it is updated when steps are added or removed.
# Its version strings are only a placeholder; the staged copy has the real ones.
$stagedCustomizations = Join-Path $StagingDir 'customizations.xml'
$stagedSolution = Join-Path $StagingDir 'solution.xml'
Save-Xml $customizations $stagedCustomizations
Save-Xml $solution $stagedSolution
Copy-Item -LiteralPath (Join-Path $sourceDir '[Content_Types].xml') -Destination (Join-Path $StagingDir '[Content_Types].xml')

if ($sourceChanged) {
    $registry = Read-XmlKeepingWhitespace (Join-Path $sourceDir 'customizations.xml')
    foreach ($assemblyNode in $registry.SelectNodes('/ImportExportXml/SolutionPluginAssemblies/PluginAssembly')) {
        $assemblyName = ($assemblyNode.GetAttribute('FullName') -split ',')[0].Trim()
        $staged = $customizations.SelectSingleNode("/ImportExportXml/SolutionPluginAssemblies/PluginAssembly[@PluginAssemblyId='$($assemblyNode.GetAttribute('PluginAssemblyId'))']")
        $assemblyNode.PluginTypes.InnerXml = $staged.PluginTypes.InnerXml -replace 'Version=\d+\.\d+\.\d+\.\d+', 'Version=2.0.0.0' -replace '\(\d+\.\d+\.\d+\.\d+\)', '(2.0.0.0)'
        $assemblyNode.SetAttribute('FullName', ($assemblyNode.GetAttribute('FullName') -replace 'Version=\d+\.\d+\.\d+\.\d+', 'Version=2.0.0.0'))
    }
    Save-Xml $registry (Join-Path $sourceDir 'customizations.xml')
    Write-Host 'Plugin types changed: Solution\customizations.xml was updated (keep it in git).'
}

# ---- zip -----------------------------------------------------------------------------------------------------

$unique = $solution.SelectSingleNode('/ImportExportXml/SolutionManifest/UniqueName').InnerText
$managed = $solution.SelectSingleNode('/ImportExportXml/SolutionManifest/Managed').InnerText
$parts = $solutionVersion.Split('.')
$zipName = '{0}_{1}_{2}_{3}{4}.zip' -f $unique, $parts[0], $parts[1], $(if ($managed -eq '1') { 'managed' } else { 'unmanaged' }), $(if ($Configuration -ne 'Release') { "_$Configuration" } else { '' })
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
$zipPath = Join-Path $OutputDir $zipName
$tempZip = "$zipPath.tmp"
if (Test-Path -LiteralPath $tempZip) { Remove-Item -LiteralPath $tempZip -Force }

$entries = @(
    [pscustomobject]@{ Entry = 'customizations.xml'; Path = $stagedCustomizations },
    [pscustomobject]@{ Entry = '[Content_Types].xml'; Path = (Join-Path $StagingDir '[Content_Types].xml') },
    [pscustomobject]@{ Entry = 'solution.xml'; Path = $stagedSolution }
) + $stagedDlls

$archive = [System.IO.Compression.ZipFile]::Open($tempZip, 'Create')
try {
    foreach ($entry in $entries) {
        [void][System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $entry.Path, $entry.Entry, [System.IO.Compression.CompressionLevel]::Optimal)
    }
}
finally { $archive.Dispose() }

# ---- check what was written ----------------------------------------------------------------------------------

$check = [System.IO.Compression.ZipFile]::OpenRead($tempZip)
try {
    if ($check.Entries.Count -ne $entries.Count) { throw "Zip has $($check.Entries.Count) entries, expected $($entries.Count)." }
    $sha = [System.Security.Cryptography.SHA256]::Create()
    foreach ($entry in $entries) {
        $zipEntry = $check.GetEntry($entry.Entry)
        if ($zipEntry -eq $null) { throw "Zip is missing $($entry.Entry)." }
        $stream = $zipEntry.Open()
        try { $inZip = [BitConverter]::ToString($sha.ComputeHash($stream)) } finally { $stream.Dispose() }
        $onDisk = [BitConverter]::ToString($sha.ComputeHash([IO.File]::ReadAllBytes($entry.Path)))
        if ($inZip -ne $onDisk) { throw "Zip entry $($entry.Entry) differs from the staged file." }
    }
}
finally { $check.Dispose() }

foreach ($xmlFile in $stagedCustomizations, $stagedSolution) { [void](Read-XmlKeepingWhitespace $xmlFile) }

Move-Item -LiteralPath $tempZip -Destination $zipPath -Force
$summary | ForEach-Object { Write-Host $_ }
Write-Host "Dataverse solution $unique $solutionVersion packed: $zipPath"
