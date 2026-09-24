# Port LAT numeric workflows into MvcTeam.Utilities.Workflows

## Goal
Bring all numeric workflow activities from `CRM-Numeric-Workflow-Utilities-main` into the main project (`MvcTeam.Utilities/MvcTeam.Utilities.csproj`).

## Decisions
- **Base class:** port `WorkFlowActivityBase` verbatim (tracing, timing, DEBUG argument logging).
- **Tests:** not ported. The solution has no test project.
- **Scope:** workflows + base class only.

## Design
- **Location:** `MvcTeam.Utilities/Workflows/Numeric/`, alongside `Persian/`, `Security/`, `Utilities/`.
- **Files (19):** `WorkFlowActivityBase` plus the 18 activities: `AbsValue`, `Add`, `Average`, `Divide`, `IntegerToWords`, `IsStringNumeric`, `Max`, `Min`, `Multiply`, `NthRoot`, `RaiseToThePower`, `RandomNumber`, `RandomNumberBetween`, `Round`, `Subtract`, `ToDecimal`, `ToInteger`, `Truncate`.
- **Namespace:** `LAT.WorkflowUtilities.Numeric` -> `MvcTeam.Utilities.Workflows` (matches existing workflows).
- **Code changes:** namespace only. Logic and CRM `[Input]`/`[Output]` labels stay identical to upstream.
- **Project file:** add 19 `<Compile Include="Workflows\Numeric\*.cs" />` entries to `MvcTeam.Utilities.csproj` (old-style csproj, no globbing). Required references (`Microsoft.Xrm.Sdk`, `Microsoft.Xrm.Sdk.Workflow`, `System.Activities`, `System.ServiceModel`) already exist.
- **Not ported:** `LATKey.snk`, `Settings.Designer.cs`, LAT `AssemblyInfo`, tests, upstream `.sln`/README. Main project keeps `MvcTeam.snk`.

## Risks
- Short class names (`Add`, `Max`, `Min`, `Round`, `Truncate`) could collide with existing types. Checked: no collisions in `MvcTeam.Utilities.Workflows` or `MvcTeam.Utilities.Core`.
- Working tree has many uncommitted deletions/moves. Only new files and the one csproj are touched.

## Verification
- Build `MvcTeam.Utilities.csproj` with MSBuild; must compile with no new errors.
- Exercise at least one activity's logic (e.g. `Add`) without a CRM connection if practical; show output.
