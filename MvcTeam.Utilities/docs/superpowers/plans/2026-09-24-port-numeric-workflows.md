# Port LAT Numeric Workflows Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

> **NO GIT ACTIONS.** The user runs all git commands themselves (see `~/.claude/CLAUDE.md`). Do not commit, add, stage, or stash. Read-only `git status`/`git diff` are fine.

**Goal:** Bring the 18 numeric CRM workflow activities (plus their base class) from `CRM-Numeric-Workflow-Utilities-main` into `MvcTeam.Utilities.Workflows`.

**Architecture:** Copy the 19 `.cs` files into a new `Workflows/Numeric/` folder, change only the namespace, register them in the old-style `MvcTeam.Utilities.csproj` via explicit `<Compile Include>` entries, then build and exercise them.

**Tech Stack:** C# / .NET Framework 4.6.1, `System.Activities` (CodeActivity), Microsoft.Xrm.Sdk 8.2, MSBuild (Visual Studio 18 Community).

## Global Constraints

- Target project: `MvcTeam.Utilities/MvcTeam.Utilities.csproj` (assembly `MvcTeam.Utilities`, signed with its own `MvcTeam.snk`).
- Namespace: `LAT.WorkflowUtilities.Numeric` -> `MvcTeam.Utilities.Workflows`. No other code changes; CRM `[Input]`/`[Output]` labels stay identical to upstream.
- Files go in `MvcTeam.Utilities/Workflows/Numeric/` (19 files: `WorkFlowActivityBase` + 18 activities).
- Do NOT port: `LATKey.snk`, `Settings.Designer.cs`, LAT `AssemblyInfo.cs`, tests, upstream `.sln`/README.
- Only add new files and edit the one csproj. The working tree has many unrelated uncommitted changes; do not touch them.
- Never modify `CRM-Numeric-Workflow-Utilities-main/` (read-only source).
- No secrets in code. Temporary scripts go in the scratchpad dir, not the project.

**MSBuild path (used below):**
`MSB="/c/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe"`

---

### Task 1: Copy files, change namespace, register in csproj

**Files:**
- Create: `MvcTeam.Utilities/Workflows/Numeric/*.cs` (19 files)
- Modify: `MvcTeam.Utilities/MvcTeam.Utilities.csproj` (insert before line 113, `<Compile Include="Properties\AssemblyInfo.cs" />`)

**Interfaces:**
- Produces: 18 public activity classes in namespace `MvcTeam.Utilities.Workflows` (`AbsValue`, `Add`, `Average`, `Divide`, `IntegerToWords`, `IsStringNumeric`, `Max`, `Min`, `Multiply`, `NthRoot`, `RaiseToThePower`, `RandomNumber`, `RandomNumberBetween`, `Round`, `Subtract`, `ToDecimal`, `ToInteger`, `Truncate`), each deriving from `abstract class WorkFlowActivityBase : CodeActivity`.

- [ ] **Step 1: Confirm target folder does not exist and no name collisions**

Run (from repo root `D:/Matt/AI Codings/MvcTeam_Utilities`):
```bash
ls MvcTeam.Utilities/Workflows/Numeric 2>&1
grep -rnwE "class (AbsValue|Add|Average|Divide|IntegerToWords|IsStringNumeric|Max|Min|Multiply|NthRoot|RaiseToThePower|RandomNumber|RandomNumberBetween|Round|Subtract|ToDecimal|ToInteger|Truncate|WorkFlowActivityBase)" MvcTeam.Utilities.Workflows MvcTeam.Utilities.Core --include=*.cs
```
Expected: `ls` says "No such file or directory"; grep prints nothing.

- [ ] **Step 2: Copy the 19 source files**

```bash
SRC="CRM-Numeric-Workflow-Utilities-main/LAT.WorkflowUtilities.Numeric"
DST="MvcTeam.Utilities/Workflows/Numeric"
mkdir -p "$DST"
for f in AbsValue Add Average Divide IntegerToWords IsStringNumeric Max Min Multiply NthRoot RaiseToThePower RandomNumber RandomNumberBetween Round Subtract ToDecimal ToInteger Truncate WorkFlowActivityBase; do
  cp "$SRC/$f.cs" "$DST/$f.cs"
done
ls "$DST" | wc -l
```
Expected: `19`

- [ ] **Step 3: Change the namespace in the copies only**

```bash
sed -i 's/^namespace LAT\.WorkflowUtilities\.Numeric$/namespace MvcTeam.Utilities/' MvcTeam.Utilities/Workflows/Numeric/*.cs
grep -c "^namespace MvcTeam.Utilities.Workflows$" MvcTeam.Utilities/Workflows/Numeric/*.cs | grep -v ":1$"
grep -rn "LAT\." MvcTeam.Utilities/Workflows/Numeric/
```
Expected: both greps print nothing (every file has exactly one new namespace line; no `LAT.` left). If the first grep lists a file, its namespace line is formatted differently (e.g. trailing `\r` or a brace on the same line) - fix that file by hand.

- [ ] **Step 4: Register the files in the csproj**

Use the Edit tool on `MvcTeam.Utilities/MvcTeam.Utilities.csproj`.

old_string:
```
    <Compile Include="Properties\AssemblyInfo.cs" />
```
new_string:
```
    <Compile Include="Workflows\Numeric\AbsValue.cs" />
    <Compile Include="Workflows\Numeric\Add.cs" />
    <Compile Include="Workflows\Numeric\Average.cs" />
    <Compile Include="Workflows\Numeric\Divide.cs" />
    <Compile Include="Workflows\Numeric\IntegerToWords.cs" />
    <Compile Include="Workflows\Numeric\IsStringNumeric.cs" />
    <Compile Include="Workflows\Numeric\Max.cs" />
    <Compile Include="Workflows\Numeric\Min.cs" />
    <Compile Include="Workflows\Numeric\Multiply.cs" />
    <Compile Include="Workflows\Numeric\NthRoot.cs" />
    <Compile Include="Workflows\Numeric\RaiseToThePower.cs" />
    <Compile Include="Workflows\Numeric\RandomNumber.cs" />
    <Compile Include="Workflows\Numeric\RandomNumberBetween.cs" />
    <Compile Include="Workflows\Numeric\Round.cs" />
    <Compile Include="Workflows\Numeric\Subtract.cs" />
    <Compile Include="Workflows\Numeric\ToDecimal.cs" />
    <Compile Include="Workflows\Numeric\ToInteger.cs" />
    <Compile Include="Workflows\Numeric\Truncate.cs" />
    <Compile Include="Workflows\Numeric\WorkFlowActivityBase.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
```
Verify: `grep -c "Workflows\\\\Numeric" MvcTeam.Utilities/MvcTeam.Utilities.csproj` -> `19`.

---

### Task 2: Build and verify behaviour

**Files:**
- Read-only: `MvcTeam.Utilities/bin/Debug/MvcTeam.Utilities.dll` (build output)
- Scratch (outside project): `<scratchpad>/NumericCheck.cs`, `<scratchpad>/NumericCheck.exe`

**Interfaces:**
- Consumes: the 18 activity classes and their public `InArgument`/`OutArgument` properties from Task 1 (e.g. `Add.Number1`, `Add.Number2`, `Add.RoundDecimalPlaces` -> `Add.Sum`).

- [ ] **Step 1: Restore packages check**

```bash
ls packages/Microsoft.CrmSdk.CoreAssemblies.8.2.0.2/lib/net452/Microsoft.Xrm.Sdk.dll packages/Microsoft.CrmSdk.Workflow.8.2.0.2/lib/net452/Microsoft.Xrm.Sdk.Workflow.dll
```
Expected: both paths listed (packages are already present).

- [ ] **Step 2: Build the project**

```bash
MSB="/c/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/MSBuild.exe"
"$MSB" MvcTeam.Utilities/MvcTeam.Utilities.csproj -p:Configuration=Debug -p:RunCodeAnalysis=false -v:minimal -nologo 2>&1 | tail -40
```
Expected: `Build succeeded.` with `0 Error(s)`. Warnings are acceptable. `RunCodeAnalysis=false` avoids the csproj's dead `..\..\..\..\Desktop\RuleSet1.ruleset` path; it is a command-line override only, the csproj is not changed.

If it fails: read the errors. Likely causes are a missing `using` (add it to that copied file only) or an accessibility error on `WorkFlowActivityBase` (the constructor is `internal`, fine within one assembly). Fix in the copied file, rebuild. If the failure is pre-existing and unrelated to `Workflows\Numeric`, stop and report it; do not fix unrelated code.

- [ ] **Step 3: Confirm the 18 activities are in the built assembly**

```bash
powershell -NoProfile -Command "[Reflection.Assembly]::LoadFrom((Resolve-Path 'MvcTeam.Utilities/bin/Debug/MvcTeam.Utilities.dll')) | Out-Null; [AppDomain]::CurrentDomain.GetAssemblies() | ? { \$_.GetName().Name -eq 'MvcTeam.Utilities' } | % { \$_.GetTypes() } | ? { \$_.BaseType -and \$_.BaseType.Name -eq 'WorkFlowActivityBase' } | % { \$_.FullName } | sort"
```
Expected: 18 lines, each `MvcTeam.Utilities.Workflows.<Name>`. (If reflection load fails on missing CRM SDK assemblies, add `-ReferencePath`-style loading of `packages/.../Microsoft.Xrm.Sdk.dll` first, or skip to Step 4 which exercises the same types.)

- [ ] **Step 4: Write a scratch harness that runs activities without CRM**

Create `<scratchpad>/NumericCheck.cs` (scratchpad dir from the session, never inside the project). `RealProxy` stubs the CRM interfaces so `WorkFlowActivityBase.Execute` can construct its `LocalWorkflowContext`:

```csharp
using System;
using System.Activities;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Workflows;

class Stub<T> : RealProxy
{
    public Stub() : base(typeof(T)) { }
    public override IMessage Invoke(IMessage msg)
    {
        var call = (IMethodCallMessage)msg;
        var rt = ((System.Reflection.MethodInfo)call.MethodBase).ReturnType;
        object ret = null;
        if (rt == typeof(IOrganizationService)) ret = new Stub<IOrganizationService>().GetTransparentProxy();
        else if (rt.IsValueType && rt != typeof(void)) ret = Activator.CreateInstance(rt);
        return new ReturnMessage(ret, null, 0, call.LogicalCallContext, call);
    }
}

static class Program
{
    static int failures;

    static IDictionary<string, object> Run(Activity a, IDictionary<string, object> args)
    {
        var inv = new WorkflowInvoker(a);
        inv.Extensions.Add<IWorkflowContext>(() => (IWorkflowContext)new Stub<IWorkflowContext>().GetTransparentProxy());
        inv.Extensions.Add<ITracingService>(() => (ITracingService)new Stub<ITracingService>().GetTransparentProxy());
        inv.Extensions.Add<IOrganizationServiceFactory>(() => (IOrganizationServiceFactory)new Stub<IOrganizationServiceFactory>().GetTransparentProxy());
        return inv.Invoke(args);
    }

    static void Check(string name, object actual, object expected)
    {
        bool ok = Equals(actual, expected);
        if (!ok) failures++;
        Console.WriteLine("{0,-28} {1}  (got {2}, expected {3})", name, ok ? "PASS" : "FAIL", actual, expected);
    }

    static int Main()
    {
        Check("Add 2+3", Run(new Add(), new Dictionary<string, object> { { "Number1", 2m }, { "Number2", 3m }, { "RoundDecimalPlaces", -1 } })["Sum"], 5m);
        Check("Add rounding 1.234+1", Run(new Add(), new Dictionary<string, object> { { "Number1", 1.234m }, { "Number2", 1m }, { "RoundDecimalPlaces", 1 } })["Sum"], 2.2m);
        Check("Subtract 10-4", Run(new Subtract(), new Dictionary<string, object> { { "Number1", 10m }, { "Number2", 4m }, { "RoundDecimalPlaces", -1 } })["Difference"], 6m);
        Check("Multiply 6*7", Run(new Multiply(), new Dictionary<string, object> { { "Number1", 6m }, { "Number2", 7m }, { "RoundDecimalPlaces", -1 } })["Product"], 42m);
        Check("Divide 9/3", Run(new Divide(), new Dictionary<string, object> { { "Dividend", 9m }, { "Divisor", 3m }, { "RoundDecimalPlaces", -1 } })["Quotient"], 3m);
        Console.WriteLine(failures == 0 ? "ALL PASS" : failures + " FAILED");
        return failures;
    }
}
```

Before compiling, confirm the exact argument names used above (`Difference`, `Product`, `Dividend`, `Divisor`, `Quotient`) against the copied files:
```bash
grep -nE "InArgument|OutArgument" MvcTeam.Utilities/Workflows/Numeric/{Subtract,Multiply,Divide}.cs
```
Adjust the dictionary keys in the harness to match the property names printed. (`Add` is confirmed: `Number1`, `Number2`, `RoundDecimalPlaces` -> `Sum`.)

- [ ] **Step 5: Compile and run the harness**

```bash
SCR="<scratchpad dir>"
CSC="/c/Program Files/Microsoft Visual Studio/18/Community/MSBuild/Current/Bin/Roslyn/csc.exe"
BIN="MvcTeam.Utilities/bin/Debug"
SDK="packages/Microsoft.CrmSdk.CoreAssemblies.8.2.0.2/lib/net452"
WF="packages/Microsoft.CrmSdk.Workflow.8.2.0.2/lib/net452"
cp "$BIN/MvcTeam.Utilities.dll" "$SDK/Microsoft.Xrm.Sdk.dll" "$WF/Microsoft.Xrm.Sdk.Workflow.dll" "packages/Microsoft.IdentityModel.7.0.0/lib/net35/microsoft.identitymodel.dll" "$SCR/"
"$CSC" -nologo -out:"$SCR/NumericCheck.exe" -r:"$SCR/MvcTeam.Utilities.dll" -r:"$SCR/Microsoft.Xrm.Sdk.dll" -r:"$SCR/Microsoft.Xrm.Sdk.Workflow.dll" -r:System.Activities.dll -r:System.Runtime.Serialization.dll -r:System.ServiceModel.dll "$SCR/NumericCheck.cs"
"$SCR/NumericCheck.exe"
```
Expected output ends with `ALL PASS` and five `PASS` lines. Any `FAIL` means an activity's behaviour changed vs upstream: compare with the source file and fix the port (namespace-only changes should never alter results).

- [ ] **Step 6: Report**

Show the user: the build's `Build succeeded / 0 Error(s)` line, the harness output, and `git status --short MvcTeam.Utilities/Workflows/Numeric MvcTeam.Utilities/MvcTeam.Utilities.csproj` (read-only) so they can see exactly which files are new/changed. Do NOT commit or stage anything; tell the user the changes are ready for them to commit.
