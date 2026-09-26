# MvcXrmTools — MvcTeam Utilities

Custom Workflow Activity (step) library for **Microsoft Dynamics CRM 2016 On-Premise**, with first-class Persian / Iran support (Jalali calendar, Iran time, Persian number words, national-code validation). 145 documented steps in 8 categories, released under MIT. Everyone using it is a CRM workflow designer, not a developer: input labels, defaults and error messages are what they see.

The team's docs and README are in **Persian**. Code, comments and this file are English. The user cannot read Persian in the terminal, so always answer and ask in English.

## Working rules for this repo
- Ask before building when something is ambiguous. Keep changes small and touch only what was asked.
- Never commit without user direct order.
- No secrets in code. The connection string belongs in `.env` (gitignored); `.env.example` shows the shape.
- Before saying something is done, build/run it and show the output.
- Docs describe the **intended** behaviour. If you find a code bug while documenting, report it to the user; do not change the code or write the bug into the docs unless they approve.

## Target platform (do not upgrade casually)
- CRM 2016 on-prem, **CRM SDK 8.2.0.2**, **.NET Framework 4.6.1**, classic (non-SDK-style) `.csproj` files, C# language features as supported by the VS toolchain.
- The SDK 9 toolkits we port from use messages that don't exist on SDK 8 (multi-select option sets, app modules, `DeleteRecordChangeHistory`). Those steps can't be ported.
- Assemblies run in the CRM workflow sandbox: **no extra DLLs**. That is why JSON is read with `JsonReaderWriterFactory` (`JsonPathReader`) instead of Newtonsoft, and why there is no ILMerge in the build.
- Every assembly is strong-name signed with `MvcTeam.snk`.

## Solution layout (`MvcTeam.Utilities.sln`)
```
MvcTeam.Utilities.Core/          shared project (.shproj/.projitems), no DLL of its own; imported by every step project
MvcTeam.Utilities.DateAndTime/   DateTime_*   (47 steps + Common/ helpers)
MvcTeam.Utilities.Email/         Email_*      (15)
MvcTeam.Utilities.Note/          Note_*       (10)
MvcTeam.Utilities.Numeric/       Numeric_*    (18)
MvcTeam.Utilities.Other/         Utilities_*  (14 compiled + 1 uncompiled class, see below)
MvcTeam.Utilities.Persian/       Persian_*    (4)
MvcTeam.Utilities.Security/      Security_*   (5)
MvcTeam.Utilities.Strings/       String_*     (32)
MvcTeam.Utilities.Solution/      packaging project: turns the Release build into the Dataverse solution zip
Build/Update-AssemblyVersion.ps1 pre-build version stamper
Build/MVCUtilities_<major>_<minor>_managed.zip   generated Dataverse solution (output of the packaging project)
docs/<category>/                 Persian step docs (+ images/)
README.md, THIRD-PARTY-NOTICES.md
```
- Each step project compiles the shared Core files through `<Import Project="..\MvcTeam.Utilities.Core\MvcTeam.Utilities.Core.projitems" Label="Shared" />`. **Core code is compiled into every DLL**, so Core changes affect all eight assemblies.
- The `.projitems` file lists exactly which Core files compile. `Models/CrmEmail.cs`, `Models/CrmEmailSignature.cs`, `Services/App_Add_Signature_To_Email.cs` and `Services/App_Mark_Email_As_Read.cs` are `<None>` (deliberately not compiled).
- A new Core file must be added to `MvcTeam.Utilities.Core.projitems` or it won't build into the DLLs.
- A new step file must be added to its project's `<Compile Include=...>` list (classic csproj: files are not globbed).
- `MvcTeam.Utilities/` (repo root) is an untracked leftover of the old single-project layout (`bin/obj`, empty `Models`, a couple of empty folders). It is not in the solution. Ignore it.
- `Dynamics-365-Workflow-Tools/`, `WorkflowElements-master/`, `CRM-Email-Workflow-Utilities-main/` are upstream reference copies of the toolkits we port from. They are reference only, not part of the build, and graphify ignores them.
- `packages/` holds the restored NuGet packages the csprojs reference by HintPath (CRM SDK 8.2.0.2, Microsoft.IdentityModel, plus legacy ones).

### Build and versioning
- Build (VS MSBuild, there is no `msbuild` on PATH): `"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" MvcTeam.Utilities.sln -p:Configuration=Debug -v:minimal -nologo`. Output goes to `Build\bin\<Configuration>\`.
- Every project has the same `PreBuildEvent` running `Build/Update-AssemblyVersion.ps1` on its `Properties/AssemblyInfo.cs`. It keeps the Major.Minor already in the file (currently `2.0`) and sets `Build` = days since 2000-01-01 and `Revision` = UTC `HHmm`. **Every build rewrites the `AssemblyInfo.cs` files**, so they show as modified in git after a build; that is expected. Two builds in the same UTC minute give the same version. To change Major.Minor edit `AssemblyInfo.cs` by hand.
### Dataverse solution (packaging project)
- `MvcTeam.Utilities.Solution` builds nothing itself. After **every** build (Debug or Release) it runs `Pack-Solution.ps1`, which packs the eight fresh DLLs from `Build/bin/<Configuration>` into `Build/MVCUtilities_<major>_<minor>_managed.zip` (name from `solution.xml`'s UniqueName/Version/Managed, so it overwrites the same file). Only the **Release** zip is the package to install; a Debug build writes `..._managed_Debug.zip` instead (Debug assemblies trace every input value).
- Source of truth is `MvcTeam.Utilities.Solution/Solution/` (`solution.xml`, `customizations.xml`, `[Content_Types].xml`), tracked in git. DLLs are not tracked; they are copied from `Build/bin/Release` at pack time.
- The script writes each DLL's real version into the assembly FullName, every plugin type's AssemblyQualifiedName, the `<Assembly> (<version>)` group name and the solution root components, and sets the **solution `<Version>` to the highest assembly version** (so it rises with every build). Those version edits go to a staging copy only; the tracked `customizations.xml` keeps a `2.0.0.0` placeholder.
- It also compares the workflow activity types in each DLL with the registered plugin types: a **new step** gets a new PluginTypeId/FriendlyName GUID, a **removed step** is dropped, existing GUIDs are kept so CRM upgrades in place. In that case `Solution/customizations.xml` is rewritten: commit it. So adding or deleting a step needs no manual solution edit; just build Release.
- The zip is checked after writing (entries, SHA-256 of every file, XML parses). It is a hand-packed **managed** solution: import it into a dev org first before production. A managed zip is normally exported from an org; if that ever becomes a problem, switch `<Managed>` to 0 in `solution.xml` for an unmanaged package.
- A changed assembly signing key, a new assembly project, or publisher/other solution metadata are not automated: edit `Solution/*.xml` (a new assembly needs a `PluginAssembly` + root component with new GUIDs).
- Debug builds define `DEBUG`, which makes `WorkFlowActivityBase` trace every input/output argument. Release does not.
- `MvcTeam.Utilities.Other.csproj` also has `RunCodeAnalysis` with a ruleset path that points outside the repo; it is harmless on this machine.

## How a step is built
Two styles exist. Prefer style 1 for new steps unless the step needs the raw `CodeActivityContext`.

1. **`WorkFlowActivityBase` steps** (Numeric, String, Email, Note, most DateTime): `public class Category_Name : WorkFlowActivityBase`, constructor `base(typeof(Category_Name))`, override `ExecuteCrmWorkFlowActivity(context, localContext)`. The base class builds `LocalWorkflowContext` (OrganizationService created for `WorkflowExecutionContext.UserId`, tracing service), traces entry/exit/duration and rethrows `FaultException<OrganizationServiceFault>`.
2. **Plain `CodeActivity` steps** (Utilities, Security, Persian, Add/BusinessDaysBetween): resolve `IWorkflowContext`, `IOrganizationServiceFactory` and `ITracingService` themselves.

Conventions that apply to both:
- **Class names are `<Category>_<StepName>` in the global namespace** (no `namespace` block). CRM registers workflow activities by full type name, so renaming a class or moving it into a namespace breaks existing workflows. Helper types live in `MvcTeam.Utilities.Services`, `.Models`, `.Workflows` and (DateAndTime only) `.Workflows.Common`. The Persian calendar is `MD.PersianDateTime`.
- Category = the prefix and the doc folder: `Utilities`→`utilities`, `Security`→`security`, `Note`→`note`, `String`→`string`, `Numeric`→`numeric`, `DateTime`→`datetime`, `Email`→`email`, `Persian`→`persian`.
- **Logic goes in Core** (`CrmService` or a static helper); the step stays thin: read inputs, validate, call, set outputs.
- `[RequiredArgument]` only takes effect in the CRM designer. A dynamic value can still arrive empty at run time, so validate in code and throw `InvalidPluginExecutionException` with a clear message ("X is required.").
- `[Default(...)]` is also designer-only. In tests and harnesses pass every input explicitly.
- Labels in `[Input("...")]` / `[Output("...")]` are user-facing and stable: keep the original labels when porting, and never rename them casually (workflows bind to them).
- Record URLs ("Record Url (Dynamic)") are parsed only through `CrmService.GetRecordFromUrl` / `DynamicUrlParser` (handles `etc=` and `etn=`, `%7b..%7d` braces). Never parse `etc`/`id` positionally by hand.
- **Which service to use:** default is the workflow user (`CreateOrganizationService(context.UserId)` or `InitiatingUserId`) so CRM permissions apply. Use the system service (`CreateOrganizationService(null)`) only where the check must not depend on the caller's rights: role/team checks, holiday queries, rollup recalculation, `Get Record Id`. `SetUserSettings` and clone steps deliberately run as the workflow user.
- Fix bugs rather than copying them from upstream. Error text should tell the designer what to change.
- Steps that use randomness use `SecureRandom` (not `System.Random`); regex steps use `SafeRegex` (2-second timeout, friendly error); number parsing uses `NumberText`.

## Persian / Iran specifics
- **`IranTime`** converts UTC ⇄ Iran time. Iran dropped daylight saving in 2022; from 2022-09-21 19:30 UTC the offset is a fixed +03:30, applied in code so a server with outdated tz data still agrees. Earlier dates use the server's `Iran Standard Time` history. CRM passes dates to steps as UTC.
- Two DateTime families: (a) the `Get*` / `IsBusinessDay` / `IsSameDay` steps with **Evaluate As User Local** (user's CRM time zone via `LocalTimeFromUtcTimeRequest`; if the user has no time zone, falls back to Iran time); (b) the newer Iran-time steps (`AddBusinessDays`, `BusinessDaysBetween`, Persian steps) that always work in Iran time.
- Business days: `BusinessDayCalculator` (weekend days as `"4|5"`, 0=Sunday…6=Saturday; default Thu|Fri; holidays are the `holiday` column of a FetchXml query, paged via `CrmService.GetHolidayDates`). The older `DateDiffBusiness*`, `IsBusinessDay`, `GetNumberOfBusinessDays` use `BusinessDayLogic` / `BusinessMinuteLogic` with a CRM calendar entity and treat Saturday/Sunday as the weekend. That inconsistency is known and has not been unified.
- `PersianDateFormatter` formats with .NET-style patterns independent of server culture (`ق.ظ`/`ب.ظ`, optional Persian digits). `PersianDateTime` (`MD.PersianDateTime`, third-party, MDSSoft) is the Jalali calendar core.
- `NumberText` reads user-typed numbers: accepts Persian/Arabic digits and marks, `.`/`/` as decimal, and is culture-invariant. `AppConvertNumberToLetters` turns numbers into Persian words (max 15 digits). `NationalCodeValidator` validates the Iranian 10-digit national code.

## Query / clone helpers (Core)
- `QueryResultTable` runs a system view, personal view or FetchXml (priority in that order), pages through all results, and renders HTML table / CSV / list / typed first-cell values. A filter "primary key not-null" on a link to the current record's entity is narrowed to the current record.
- `RecordCloner` clones records and child records (skips update-only fields, copies status after create, rebuilds activity parties). `CrmService.RunWorkflowOnRecords` starts workflows 100 at a time with `ExecuteMultiple`.

## Documentation (`docs/`)
- Persian, one page per step: `docs/<category>/<kebab-class-suffix>.md` (e.g. `Email_CcTeam` → `docs/email/cc-team.md`). Whole page wrapped in `<div dir="rtl">`. Step names, parameter labels and technical terms (Workflow, Lookup, GUID, Record URL) stay in English; prose is Persian.
- Template: follow `docs/note/*.md` (parameters, output, how it works, notes such as "runs as the workflow user", example). Ported steps translate the upstream doc, copy media into `docs/<category>/images/` with descriptive kebab names and end with a credit line (e.g. Demian Rasko, Ms-PL, github.com/demianrasko/Dynamics-365-Workflow-Tools). Write from **our** code, not the upstream text (labels/behaviour differ). Steps with no source doc get no credit line unless provenance is known.
- Every page has back-links (`<!-- nav-top -->`, `<!-- nav-bottom -->`) and appears in the README step tables. README tables were generated from the pages by a script (not in the repo); regenerate instead of hand-editing when adding many pages.
- **Every doc page and image must be listed in `MvcTeam.Utilities.sln`** as solution items under nested folders `docs > <category> > images`, plus `README.md` and `THIRD-PARTY-NOTICES.md` under "Solution Items" (and this `CLAUDE.md`). The `.sln` has a UTF-8 BOM and CRLF line endings: preserve both. Regenerate the folder blocks from disk with a script rather than hand-editing.
- A doc must correspond to a compiled step class. Verify links and images resolve after writing (0 broken links was the last verified state).
- Style is confirmed one category at a time by the user; do not mass-produce pages without a review checkpoint.

### Known doc gaps / drift (as of 2026-09-26)
- `String_CapitalizeFirst` has no doc page.
- `docs/utilities/workflow-add-signature-to-email.md` documents `Utilities_WorkflowAddSignatureToEmail`, which is **not compiled** (its file is in no csproj; it depends on an uncompiled helper that calls a method that doesn't exist). Pending the user's decision to keep or delete.
- `docs/numeric/{is-string-numeric,to-decimal,to-integer}.md` claim Persian/Arabic digits aren't recognised and that parsing follows the server culture; the code (`NumberText`) now accepts them and is culture-invariant.

## Porting steps from other toolkits
Sources so far: Dynamics-365-Workflow-Tools (Demian Rasko, Ms-PL), WorkflowElements (Aiden Kaskela, MIT), CRM-Email-Workflow-Utilities (Jason Lattimer, MIT), Ultimate Workflow Toolkit (Andrii Butenko, MIT), LAT.WorkflowUtilities. Licences and credits live in `README.md` and `THIRD-PARTY-NOTICES.md`; keep them updated when adding a ported step. When asked to "add" a step:
1. Ask before choosing group/category, defaults or behaviour changes.
2. Add the class to the right project (and its `<Compile>` list), keep original labels, put logic in Core.
3. Adapt to SDK 8.2 / CRM 2016; drop or rewrite anything that needs SDK 9 or an online service.
4. Build, test, then add the doc page and solution items.

The old `Deploy/Set-WorkflowGroups.ps1` (assigned steps to CRM workflow groups: MvcTeam Utilities / Persian / Security / Date & Time) is not in the repo any more. Ask the user before recreating it.

## Testing
There is **no test project in the repo**. Earlier work used a scratchpad harness (a fake `IOrganizationService` plus `WorkflowInvoker`) that lives only in temporary session folders and references DLL paths from the old layout. If it is gone, rebuild a small standalone console harness outside the repo (or propose adding a test project, and ask first). For bug fixes: write the failing test first, then fix. Probe real outputs with the harness before writing claims in docs.

## Known code quirks (reported, not necessarily fixed)
- Three round-to-time steps (`RoundToHour`, `RoundToHalfHour`, `RoundToQuarterHour`) pass `typeof(DateTime_DateDiffMinutes)` to the base constructor; only trace text is affected.
- `String_Substring`: a start past the end returns null (with a trace), a length past the end is cut at the end, a negative length throws; an empty Length means "to the end".
- Note steps throw `new ArgumentNullException("Note cannot be null")` (message passed as the parameter name).
- `DateTime_DateDiffMonths` computes a `day` value it never uses.
- The Email project has five near-duplicate `Cc*`/`Email*` pairs (same logic, `cc` vs `to`).
- `Email_*QueueMembers` check the CRM version string starting with "5" (CRM 2011) to decide how to get queue members.
- `Core/Services/App_Add_Signature_To_Email.cs` (uncompiled) calls `CrmService.GetEmailSignatureForUser`, which does not exist. `Other/Services/CrmService.cs` is in no csproj.
- Known behaviour changes that already shipped: `SetUserSettings`/StringFunctions namespace fixes mean CRM workflows using steps registered under the old type name need the assembly re-registered and the step re-added.

## Tooling notes
- `graphify-out/` (gitignored) holds a knowledge graph (`graph.html`, `GRAPH_REPORT.md`, `graph.json`). For architecture questions prefer `graphify query "<question>"` before grepping. Rebuild with `/graphify` (use `--update` for incremental).
- Memory notes for Claude sessions are outside the repo (`~/.claude/projects/...`); this file is the shared source of truth for the project.
