<#
.SYNOPSIS
    Stamps a timestamp-based version into AssemblyInfo.cs. Runs as the project's pre-build event.

.DESCRIPTION
    Keeps the Major.Minor already in AssemblyVersion and sets the other two parts from the UTC build time:
        Build    = days since 2000-01-01
        Revision = HHmm
    e.g. 1.9.0.0 -> 1.9.9764.1205. AssemblyVersion and AssemblyFileVersion always get the same value.
    Change Major.Minor by editing AssemblyInfo.cs by hand; the script preserves them.

    Runs before compile, so the DLL that is built carries the new version. Two builds in the same UTC minute
    get the same version (the file is then left untouched). UTC is used so daylight-saving changes and
    different machines can't make the version go backwards.

    Any problem (file missing, attribute missing) throws, so powershell exits 1 and the build fails.

.PARAMETER AssemblyInfoPath
    Path to Properties\AssemblyInfo.cs.

.PARAMETER UtcNow
    Build time to stamp. Defaults to the current UTC time; only overridden for testing.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$AssemblyInfoPath,
    [datetime]$UtcNow = [DateTime]::UtcNow
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $AssemblyInfoPath)) {
    throw "AssemblyInfo file not found: $AssemblyInfoPath"
}

# Decode without stripping the BOM (it stays as U+FEFF and is written back as the same bytes),
# so encoding and line endings survive the round trip.
$encoding = New-Object System.Text.UTF8Encoding($false)
$text = $encoding.GetString([IO.File]::ReadAllBytes($AssemblyInfoPath))

# ^ anchors to the start of a line, so the commented-out example in the file's header is ignored.
$current = [regex]::Match($text, '(?m)^\[assembly:\s*AssemblyVersion\("(\d+)\.(\d+)[^"]*"\)\]')
if (-not $current.Success) {
    throw "No AssemblyVersion attribute found in $AssemblyInfoPath"
}
$major = $current.Groups[1].Value
$minor = $current.Groups[2].Value

$build = [int]($UtcNow.Date - (New-Object DateTime(2000, 1, 1))).TotalDays
$revision = [int]$UtcNow.ToString('HHmm')
$version = "$major.$minor.$build.$revision"

$updated = $text
foreach ($attribute in 'AssemblyVersion', 'AssemblyFileVersion') {
    $pattern = '(?m)^(\[assembly:\s*' + $attribute + '\(")[^"]*("\)\])'
    if (-not [regex]::IsMatch($updated, $pattern)) {
        throw "No $attribute attribute found in $AssemblyInfoPath"
    }
    $updated = $updated -replace $pattern, ('${1}' + $version + '${2}')
}

if ($updated -ceq $text) {
    Write-Host "Assembly version already $version, nothing to update."
    exit 0
}

[IO.File]::WriteAllBytes($AssemblyInfoPath, $encoding.GetBytes($updated))
Write-Host "Assembly version updated: $version"
