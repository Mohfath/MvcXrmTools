<#
.SYNOPSIS
    Sets the group and display name of the MvcTeam.Utilities workflow steps in CRM.

.DESCRIPTION
    Run this after registering or updating MvcTeam.Utilities.dll with the Plugin Registration Tool.
    In the process designer, "Add Step" shows one submenu per group, listing the steps by name.

    Reads CRM_CONNECTION_STRING from the .env file in the solution folder (see .env.example).
    Requires the Microsoft.Xrm.Tooling.CrmConnector.PowerShell module (Windows PowerShell 5.1):
        Install-Module Microsoft.Xrm.Tooling.CrmConnector.PowerShell -Scope CurrentUser

.EXAMPLE
    .\Set-WorkflowGroups.ps1 -WhatIf
    Shows what would change without saving anything.

.EXAMPLE
    .\Set-WorkflowGroups.ps1
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [string]$EnvFile = (Join-Path $PSScriptRoot '..\..\.env')
)

$ErrorActionPreference = 'Stop'

$AssemblyName = 'MvcTeam.Utilities'

# Edit this table to rename a step or move it to another group.
$Steps = [ordered]@{
    'MvcTeam.Utilities.Workflows.AddPriceListItem'             = @{ Group = 'MvcTeam Utilities'; Name = 'Add Price List Item' }
    'MvcTeam.Utilities.Workflows.CloneChildren'                = @{ Group = 'MvcTeam Utilities'; Name = 'Clone Children' }
    'MvcTeam.Utilities.Workflows.CloneRecord'                  = @{ Group = 'MvcTeam Utilities'; Name = 'Clone Record' }
    'MvcTeam.Utilities.Workflows.CurrencyConvert'              = @{ Group = 'MvcTeam Utilities'; Name = 'Currency Convert' }
    'MvcTeam.Utilities.Workflows.DeleteRecord'                 = @{ Group = 'MvcTeam Utilities'; Name = 'Delete Record' }
    'MvcTeam.Utilities.Workflows.GetRecordId'                  = @{ Group = 'MvcTeam Utilities'; Name = 'Get Record Id' }
    'MvcTeam.Utilities.Workflows.JsonParser'                   = @{ Group = 'MvcTeam Utilities'; Name = 'JSON Parser' }
    'MvcTeam.Utilities.Workflows.QueryValues'                  = @{ Group = 'MvcTeam Utilities'; Name = 'Query Values' }
    'MvcTeam.Utilities.Workflows.QueryGetResults'              = @{ Group = 'MvcTeam Utilities'; Name = 'Query Get Results' }
    'MvcTeam.Utilities.Workflows.QueryRunWorkflowOnResults'    = @{ Group = 'MvcTeam Utilities'; Name = 'Query Run Workflow On Results' }
    'MvcTeam.Utilities.Workflows.RecalculateAllRollups'        = @{ Group = 'MvcTeam Utilities'; Name = 'Recalculate All Rollups' }
    'MvcTeam.Utilities.Workflows.ResizeAnnotationImage'        = @{ Group = 'MvcTeam Utilities'; Name = 'Resize Annotation Image' }
    'MvcTeam.Utilities.Workflows.RunWorkflowWithForLoop'       = @{ Group = 'MvcTeam Utilities'; Name = 'Run Workflow With For Loop' }
    'MvcTeam.Utilities.Workflows.SetUserSettings'             = @{ Group = 'MvcTeam Security'; Name = 'Set User Settings' }
    'MvcTeam.Utilities.Workflows.StringFunctions'                 = @{ Group = 'MvcTeam Utilities'; Name = 'String Functions' }
    'MvcTeam.Utilities.Workflows.UpdateFieldDynamically'       = @{ Group = 'MvcTeam Utilities'; Name = 'Update Field Dynamically' }
    'MvcTeam.Utilities.Workflows.ConvertNumberToPersianString' = @{ Group = 'MvcTeam Persian'; Name = 'Convert Number To Persian String' }
    'MvcTeam.Utilities.Workflows.GetDatePersianParts'          = @{ Group = 'MvcTeam Persian'; Name = 'Get Date Persian Parts' }
    'MvcTeam.Utilities.Workflows.ValidateNationalCode'         = @{ Group = 'MvcTeam Persian'; Name = 'Validate National Code' }
    'MvcTeam.Utilities.Workflows.FormatPersianDateTime'        = @{ Group = 'MvcTeam Persian'; Name = 'Format Persian DateTime' }
    'MvcTeam.Utilities.Workflows.IsUserMemberOfTeam'           = @{ Group = 'MvcTeam Security'; Name = 'Is User Member Of Team' }
    'MvcTeam.Utilities.Workflows.AddBusinessDays'              = @{ Group = 'MvcTeam Date & Time'; Name = 'Add Business Days' }
    'MvcTeam.Utilities.Workflows.BusinessDaysBetween'          = @{ Group = 'MvcTeam Date & Time'; Name = 'Business Days Between' }
    'MvcTeam.Utilities.Workflows.GetInitiatingUser'            = @{ Group = 'MvcTeam Security'; Name = 'Get Initiating User' }
    'MvcTeam.Utilities.Workflows.IsUserHasRole'                = @{ Group = 'MvcTeam Security'; Name = 'Is User Has Role' }
    'MvcTeam.Utilities.Workflows.IsUserTeamsHaveRole'          = @{ Group = 'MvcTeam Security'; Name = 'Is User Teams Have Role' }
}

function Read-EnvFile([string]$Path) {
    if (-not (Test-Path $Path)) { throw "Env file not found: $Path. Copy .env.example to .env and fill it in." }
    $values = @{}
    foreach ($line in Get-Content $Path) {
        $line = $line.Trim()
        if ($line -eq '' -or $line.StartsWith('#')) { continue }
        $key, $value = $line -split '=', 2
        $values[$key.Trim()] = $value.Trim().Trim('"', "'")
    }
    return $values
}

$connectionString = (Read-EnvFile $EnvFile)['CRM_CONNECTION_STRING']
if ([string]::IsNullOrWhiteSpace($connectionString)) { throw "CRM_CONNECTION_STRING is missing from $EnvFile" }

if (-not (Get-Module -ListAvailable Microsoft.Xrm.Tooling.CrmConnector.PowerShell)) {
    throw 'Module Microsoft.Xrm.Tooling.CrmConnector.PowerShell is not installed. Run: Install-Module Microsoft.Xrm.Tooling.CrmConnector.PowerShell -Scope CurrentUser'
}
Import-Module Microsoft.Xrm.Tooling.CrmConnector.PowerShell

$conn = Get-CrmConnection -ConnectionString $connectionString
if (-not $conn.IsReady) { throw "Could not connect to CRM: $($conn.LastCrmError)" }
Write-Host "Connected to $($conn.ConnectedOrgFriendlyName)"

$query = New-Object Microsoft.Xrm.Sdk.Query.QueryExpression 'plugintype'
$query.ColumnSet = New-Object Microsoft.Xrm.Sdk.Query.ColumnSet 'typename', 'name', 'workflowactivitygroupname'
$query.Criteria.AddCondition('typename', 'In', [object[]]@($Steps.Keys))
$query.Criteria.AddCondition('isworkflowactivity', 'Equal', [object[]]@($true))
$assemblyLink = $query.AddLink('pluginassembly', 'pluginassemblyid', 'pluginassemblyid')
$assemblyLink.LinkCriteria.AddCondition('name', 'Equal', [object[]]@($AssemblyName))

$found = $conn.RetrieveMultiple($query).Entities

foreach ($type in $found) {
    $typeName = $type['typename']
    $wanted = $Steps[$typeName]
    if ($type['workflowactivitygroupname'] -eq $wanted.Group -and $type['name'] -eq $wanted.Name) {
        Write-Host "  unchanged  $typeName"
        continue
    }
    if ($PSCmdlet.ShouldProcess($typeName, "Set group '$($wanted.Group)' and name '$($wanted.Name)'")) {
        $update = New-Object Microsoft.Xrm.Sdk.Entity 'plugintype', $type.Id
        $update['workflowactivitygroupname'] = $wanted.Group
        $update['name'] = $wanted.Name
        $conn.Update($update)
        Write-Host "  updated    $typeName -> [$($wanted.Group)] $($wanted.Name)"
    }
}

$missing = $Steps.Keys | Where-Object { $_ -notin @($found | ForEach-Object { $_['typename'] }) }
foreach ($typeName in $missing) {
    Write-Warning "Not registered in CRM under assembly '$AssemblyName': $typeName"
}
