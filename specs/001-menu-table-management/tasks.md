---
description: "Dependency-ordered implementation tasks for Menu and Table Management"
---

# Tasks: Menu and Table Management

**Input**: `specs/001-menu-table-management/` — approved `spec.md`, Blazor-based `plan.md`,
`research.md`, `data-model.md`, `contracts/api.md`, `contracts/ui.md`, `quickstart.md`, and
`.specify/memory/constitution.md`; overall scope remains in `requirements.md`.

**Prerequisites**: Review approval before execution. This file is a work list, not authorization
to deploy or alter unrelated databases. No implementation has been performed by generating it.

**Validation**: .NET builds and focused manual checks only. Do not generate automated tests unless
explicitly requested or necessary to diagnose a specific bug, with that necessity recorded.

**Organization**: Shared setup/foundation first, then the three P1 user stories in spec order,
then final integration. One existing ASP.NET Core 8 Web SDK project hosts controllers, Blazor
Interactive Server, Identity HTTP pages and Bootstrap. No new web host, repository framework,
mediator or separate client build. No ordering, POS, payments, reporting, delivery or removal.

## Format: `[ID] [P?] [Story] Description`

Every task has a sequential ID and exact repository-relative file paths. `[P]` identifies work
on separate files that may overlap only once its stated prerequisites are complete. `[US1]`,
`[US2]`, `[US3]` refer to the approved stories. Unmarked tasks execute in listed order unless
the dependency section permits otherwise. Record evidence/failures beneath the relevant task
in this file; a checkbox is complete only after its described verification succeeds.

## Path Conventions

All source paths below are under the existing solution; do not regenerate its project template.
`CafeManagement.Api/` is the combined API/Blazor host. `Components/` contains Razor components;
`Pages/Account/` contains ordinary HTTP Razor Pages. `Infrastructure/Persistence/Migrations/`
is generated only during implementation. `artifacts/cafe/` is ignored build output, not source.

## Phase 1: Setup (Shared Infrastructure)

**Goal**: Verify environment/baseline and introduce only the approved dependencies/configuration.

- [X] T001 Inspect `CafeManagement.sln`, `CafeManagement.Api/CafeManagement.Api.csproj`, `CafeManagement.Api/Program.cs` and `.gitignore`; verify SDK/SQL/certificate prerequisites from `specs/001-menu-table-management/quickstart.md`, run baseline restore/build and manually request existing `/WeatherForecast` and Development Swagger, recording actual output/blockers under this task before changing source (Constitution II/VI).
  - Initial attempt 2026-10-27 was blocked; resolved by the successful retry below. The specification-quality checklist passes
    16/16, the starting working tree was clean, and existing build/local-state ignore rules apply.
  - `dotnet --list-sdks` reports only 8.0.400; the installed ASP.NET Core and .NET runtimes are
    8.0.8. The quickstart's serviced .NET 8 prerequisite is not satisfied. Microsoft's support
    policy currently lists 8.0.33 and requires current servicing patches:
    https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core.
  - `dotnet restore CafeManagement.sln` succeeded with all projects up to date. No baseline build
    or WeatherForecast/Swagger runtime checks were performed after identifying the prerequisite
    blocker. A serviced .NET 8 SDK/runtime must be made available before retrying this task.
  - `dotnet dev-certs https --check` with normal Windows user-store access found a valid localhost
    development certificate expiring 2026-10-30; trust and phone reachability remain unverified.
    The sandbox-only certificate check did not see that certificate and is not the final result.
  - SQL service MSSQL$MSSQLSERVER01 is running; database connectivity/permissions are unverified.
    No source/package/configuration changes, certificate modifications, SQL writes, or tasks
    T002–T005 were performed. No task is marked complete; no commit or push is authorized by
    this incomplete validation result.
  - Prerequisite retry 2026-10-27: Microsoft release metadata identified SDK 8.0.427 Windows x64,
    including runtime 8.0.33. Downloaded from builds.dotnet.microsoft.com to the Windows temporary
    directory; its published SHA-512 matched and Authenticode verified Microsoft Corporation.
    Administrator installation was explicitly approved by the user.
  - The official installer failed with exit code 112 (0x70: insufficient disk space). Drive C:
    had approximately 0.045 GB free afterward. Installation did not resolve the prerequisite:
    `dotnet --version` and `--list-sdks` still report 8.0.400, and `--list-runtimes` still reports
    Microsoft.AspNetCore.App, Microsoft.NETCore.App and Microsoft.WindowsDesktop.App 8.0.8.
  - At that failed retry, T001 remained blocked pending disk space and a serviced SDK installation.
    No new restore/build/runtime checks were attempted after the installation failure; the prior
    successful baseline restore remains the only restore result. No application code or target
    framework changed; T002–T005 remain unstarted. No commit or push was performed.
  - Successful retry 2026-10-27 after the approved cleanup: C: initially had approximately
    4.646 GiB free (4.509 GiB immediately before installation). Reused the cached official
    SDK 8.0.427 Windows x64 installer after rechecking its SHA-512 against Microsoft release
    metadata and its valid Microsoft Authenticode signature. Approved administrator installation
    completed with exit code 0; C: had 3.111 GiB free immediately afterward.
  - `dotnet --version` now reports 8.0.427. `dotnet --list-sdks` lists 8.0.400 and 8.0.427;
    `dotnet --list-runtimes` lists Microsoft.AspNetCore.App, Microsoft.NETCore.App and
    Microsoft.WindowsDesktop.App at both 8.0.8 and 8.0.33. The project still targets net8.0.
  - Post-installation `dotnet dev-certs https --check` passed with the existing certificate.
    A check attempted while installation was still copying SDK files encountered a transient
    missing runtimeconfig error; the completed installation and all subsequent checks passed.
    Normal HTTPS requests below passed certificate validation without a bypass. The existing
    certificate expires 2026-10-30; phone reachability remains a later feature validation concern.
  - Read-only integrated-authentication connection to localhost\MSSQLSERVER01/master succeeded:
    SQL Server version 16.0.1000.6 and CREATE ANY DATABASE permission = 1. No database was created
    or modified. The local connection used TrustServerCertificate=True; production TLS and the
    dedicated CafeManagement database remain later setup work.
  - `dotnet restore CafeManagement.sln` succeeded; `dotnet build CafeManagement.sln --no-restore`
    succeeded with 0 warnings and 0 errors (19.89 seconds). Existing bin/obj output remains ignored.
  - Ran the unchanged application's https launch profile in Development. HTTPS GET
    /WeatherForecast returned 200 with five forecasts; /swagger/index.html and
    /swagger/v1/swagger.json returned 200, with OpenAPI 3.0.1 documenting /WeatherForecast.
    Stopped the validation host afterward. C: temporarily showed 1.650 GiB free after validation;
    the final check after shutdown showed 3.517 GiB free, with neither baseline port listening.
  - T001 is complete. Only this task's documentation changed; no application source, configuration,
    target framework or dependency references changed. T002–T005 remain unstarted. No automated
    tests, commit or push were performed; further work awaits review.
- [X] T002 Add compatible serviced 8.0.x EF Core SqlServer/Design and Identity.EntityFrameworkCore references plus QRCoder 1.8.1 to `CafeManagement.Api/CafeManagement.Api.csproj`, and pin compatible local dotnet-ef in `.config/dotnet-tools.json`; retain net8.0 and existing Swashbuckle unless a demonstrated compatibility issue requires a documented change; restore successfully before proceeding (plan Technical Context).
- [X] T003 [P] Define nonsecret connection/public-origin/bootstrap options in `CafeManagement.Api/Infrastructure/Identity/BootstrapOptions.cs` and `CafeManagement.Api/Application/PublicMenuOptions.cs`; add UserSecretsId metadata in `CafeManagement.Api/CafeManagement.Api.csproj` if needed, document local secret keys from `specs/001-menu-table-management/quickstart.md`, validate an absolute trusted HTTPS PublicWebBaseUrl, and keep actual passwords/connection secrets outside committed configuration (after T002; FR-001/006).
- [X] T004 [P] Add pinned Bootstrap 5.3.8 CSS and its license under `CafeManagement.Api/wwwroot/lib/bootstrap/css/bootstrap.min.css` and `CafeManagement.Api/wwwroot/lib/bootstrap/LICENSE`, with minimal responsive overrides in `CafeManagement.Api/wwwroot/css/site.css`; verify local files/attribution, avoiding a runtime CDN or unnecessary JS plugins (after T001; FR-007).
- [X] T005 Run `dotnet restore CafeManagement.sln`, `dotnet tool restore`, and `dotnet build CafeManagement.sln --no-restore` against `CafeManagement.sln` and `.config/dotnet-tools.json`; verify dependency/SDK compatibility and continued exclusion of secrets/build artifacts by `.gitignore`, recording evidence here (after T002–T004; Constitution II/VI).


### Setup execution evidence (T002–T005, 2026-10-27)

- T002: Checked official NuGet v3 package metadata and pinned EF Core SqlServer/Design,
  Identity.EntityFrameworkCore and the local dotnet-ef tool to compatible stable 8.0.33;
  QRCoder is pinned to 1.8.1. EF Design is PrivateAssets=all. Target remains net8.0;
  Swashbuckle.AspNetCore remains 6.4.0. Solution restore and local tool restore succeeded
  before T003 began. No other framework, frontend project or build toolchain was added.
- T003: Added BootstrapOptions and PublicMenuOptions, with no credentials or default staff
  role. Added nonsecret UserSecretsId metadata and documented all connection/public-origin/
  bootstrap configuration keys in quickstart.md. Framework connection-string configuration
  is reused; no extra connection abstraction or actual local secret values were created.
  Lightweight in-memory validation of the built class accepted three HTTPS origins and rejected
  ten blank/relative/HTTP/credential-bearing/path/query/fragment/whitespace cases. Empty bootstrap
  options were rejected. The PowerShell validation command required loading ASP.NET shared
  framework assemblies first; no application defect or test project was introduced.
  Options binding/startup validation remains T020; operator provisioning remains T013.
- T004: Downloaded only Bootstrap 5.3.8 CSS and its MIT license from the official twbs/bootstrap
  v5.3.8 tag. Verified version/license headers and license text; CSS is 232111 bytes, SHA-256
  D85327D99C7A3EE1F9B5D0500D1370ACEA3AD2DB39C163C2F51F232BAEDBDEDE.
  Sources: https://raw.githubusercontent.com/twbs/bootstrap/v5.3.8/dist/css/bootstrap.min.css
  and https://raw.githubusercontent.com/twbs/bootstrap/v5.3.8/LICENSE.
  Added only small wrapping/image/focus CSS defaults. No runtime CDN, JS plugins, package
  manager, generated frontend files or UI screens. Static serving/layout references remain
  T019/T020; this setup check does not claim a rendered mobile feature has been validated.
- T005: Using SDK 8.0.427, dotnet restore CafeManagement.sln and dotnet tool restore succeeded.
  dotnet build CafeManagement.sln --no-restore succeeded with 0 warnings and 0 errors
  (13.06 seconds); dotnet ef --version reports 8.0.33. Resolved packages match the exact pins.
  Existing HTTPS launch profile still starts in Development; WeatherForecast returns HTTP 200
  with five forecasts, Swagger UI returns HTTP 200 and its OpenAPI document includes
  WeatherForecast. HTTPS certificate validation passed without bypass. Validation host stopped.
- Extended .gitignore with .env.* to protect local secret variants. Checked ignore coverage for
  .env/.env.local, bin/obj, artifacts and .pfx. Reviewed source/configuration changes for secret
  values and unintended changes; no passwords, credentialed connection strings, tokens,
  generated build output or local state belong in the setup commit. No database writes,
  migration, account creation, automated tests or changes to Program.cs/sample endpoints.
- T001 evidence is preserved. Only T001–T005 are marked complete; T006 onward remains unstarted.
  No Spec Kit extension hooks file exists. The setup phase is complete; later work requires review.

**Checkpoint**: Baseline is known; dependencies restore and current solution builds. No migration
or account bootstrap until the target database and local secrets have been verified.

## Phase 2: Foundational (Blocking Prerequisites)

**Goal**: Shared schema, short-lived persistence, trusted authorization and co-hosted UI/account shell.
**Critical**: All stories require the T024 foundation checkpoint. Domain records serve both admin
management and public browsing, so their shared schema belongs here rather than duplicated per story.

- [X] T006 [P] Create `CafeManagement.Api/Domain/MenuCategory.cs` with "Id int identity PK; Name nvarchar(120) required" and "One category has zero or more products; name duplicates allowed" from `data-model.md`; keep it free of deletion/deactivation fields (after Phase 1; FR-002/009/011).
- [X] T007 Create `CafeManagement.Api/Domain/Product.cs` with "Id int identity PK; CategoryId int required; Name nvarchar(120) required; Price decimal(18,2) required; IsAvailable bit required", the required category relation, "product name duplicates allowed" and "CHECK Price > 0"; do not invent an availability default or currency table (after T006; FR-003/009/010/011).
- [X] T008 [P] Create `CafeManagement.Api/Domain/CafeTable.cs` with "Id int identity PK; Label nvarchar(50) required; NormalizedLabel nvarchar(50) required; PublicCode uniqueidentifier required"; preserve one immutable unique PublicCode per table and no menu ownership/deactivation fields (after Phase 1; FR-005/006/009/011).
- [X] T009 Map entities in `CafeManagement.Api/Infrastructure/Persistence/Configurations/MenuCategoryConfiguration.cs`, `ProductConfiguration.cs` and `CafeTableConfiguration.cs`: "SQL columns are NOT NULL with nonempty-after-space-trim checks", "No unique index on category/product names", "CategoryId FK -> MenuCategory.Id, index on FK, delete restricted", "CHECK Price > 0", and "Unique index on NormalizedLabel (binary comparison) and unique index on PublicCode"; inspect the mapping for every required/max-length/precision constraint (after T006–T008; FR-004/011).
- [X] T010 Add `CafeManagement.Api/Infrastructure/Persistence/CafeDbContext.cs` derived from IdentityDbContext and additive registration in `CafeManagement.Api/Program.cs`: register one SQL Server IDbContextFactory consistently with HTTP-scoped Identity stores, Admin/Cashier roles and Identity cookie services; verify DI options do not conflict and factory-created contexts can be disposed per operation (after T009; FR-001/004).

### Foundation execution evidence (T006–T010, 2026-10-27)

- Confirmed T001–T005 complete and the starting working tree clean; specification-quality
  checklist remains 16/16. Reviewed exact task dependencies, approved specification, plan,
  research, data model, API/UI contracts, quickstart and Constitution. No extension hooks exist.
- T006–T008: Added only MenuCategory, Product and CafeTable. Categories have a product collection;
  products have the required category FK/navigation, decimal price and explicitly required
  availability input (no SQL/business availability default). Names and labels are required C#
  properties. New tables receive distinct nonempty random GUID public codes with private setters;
  EF marks persisted PublicCode read-only (AfterSaveBehavior.Throw), so label edits cannot rotate it.
  No deletion/deactivation, order/payment/POS/reporting fields or currency table were introduced.
- T009: Inspected SQL Server DDL generated entirely offline by dotnet ef dbcontext script
  --project CafeManagement.Api --no-build. Confirmed int identity keys, nvarchar(120) names,
  nvarchar(50) label/normalized label, required columns, decimal(18,2), bit availability,
  nonblank-after-space-trim checks, Price > 0, CategoryId indexed FK with ON DELETE NO ACTION,
  unique normalized-label index using Latin1_General_100_BIN2 and unique public-code index.
  Category/product names have no unique index. No availability default or stored QR image.
  This command inspected the model only: no migration files, database connection or SQL execution.
  Application trimming/Unicode whitespace checks and collision/conflict handling remain later tasks.
- T010: CafeDbContext derives from IdentityDbContext, retains base Identity mappings and applies
  the three entity configurations. Added one AddDbContextFactory registration using the external
  ConnectionStrings:CafeDatabase key, plus IdentityUser/IdentityRole EF stores and cookie services.
  Identity roles can be managed through RoleManager; actual Admin/Cashier provisioning remains T013.
  No separate AddDbContext/options registration conflicts with the singleton factory; Identity
  HTTP stores resolve the scoped context supplied by AddDbContextFactory. No connection fallback,
  credentials, account bootstrap, migrations or startup database writes were added.
- Restore succeeded; dotnet build CafeManagement.sln --no-restore succeeded with 0 warnings and
  0 errors (2.66 seconds), using the existing net8.0 project and dependencies. dotnet ef dbcontext
  info --project CafeManagement.Api --no-build constructed the SQL Server context via the actual
  application host without database configuration or access.
- Lightweight in-memory inspection used EF Design's existing AppServiceProviderFactory to obtain
  the actual host provider. Verified UserManager/RoleManager/SignInManager resolution, independent
  factory contexts, one scoped context reused within a scope and different contexts across scopes.
  Disposed one factory context and confirmed the other remained usable. No test files/projects,
  database operations or alternate application registrations were introduced for this check.
- Started the existing HTTPS Development profile; WeatherForecast returned HTTP 200/five forecasts,
  Swagger UI returned 200 and its OpenAPI document retained WeatherForecast. Normal certificate
  validation passed without bypass; validation host stopped. Existing middleware/routes/sample
  code/configuration remain intact apart from the required additive service registrations.
- Only T001–T010 are complete. Stop before T011: no migration/snapshot generation, schema changes,
  database writes, role/account creation, login pages or Blazor UI implementation occurred.
  Live SQL constraint enforcement and full authentication/circuit behavior remain later checks.

- [X] T011 Generate and inspect the initial migration named `InitialMenuAndIdentity` and `CafeDbContextModelSnapshot.cs` in `CafeManagement.Api/Infrastructure/Persistence/Migrations/`; use the pinned tool, review SQL/Down migration against `data-model.md`, and verify only Identity plus the three domain tables and required constraints are introduced (after T010; FR-004/011).
- [X] T012 Confirm the dedicated CafeManagement database target and connection secrets, then apply the reviewed migration from `CafeManagement.Api/Infrastructure/Persistence/Migrations/` using the commands in `specs/001-menu-table-management/quickstart.md`; verify tables/indexes/FK/checks and record the verified target here without credentials; do not drop/change an unrelated database or auto-migrate normal startup (after T011; Constitution IV).
- [X] T013 Add operator-only `--bootstrap-user` handling in `CafeManagement.Api/Infrastructure/Identity/IdentityBootstrapper.cs` and `CafeManagement.Api/Program.cs`; use secret Bootstrap options, UserManager password hashing and role services to create only missing roles/supplied account, exit afterward, never reset/promote existing users silently, then provision local Admin and a separate Cashier for access checks (after T012; FR-001).
- [X] T014 Implement the small trusted staff check in `CafeManagement.Api/Application/StaffAccess.cs`: require authenticated server principal plus current Identity user/security stamp/Admin membership in a newly disposed service scope; accept no roles/principal from browser payload and retain no scoped Identity store across circuit actions; verify its failure outcomes are usable by HTTP and component callers (after T013; FR-001).
- [X] T015 [P] Add `CafeManagement.Api/Infrastructure/Identity/IdentityRevalidatingAuthenticationStateProvider.cs` using the .NET 8 Identity revalidation pattern with short Identity scopes; invalidate stale circuit authentication without claiming immediate cross-tab logout, and never use retained HttpContext/UserManager/DbContext from interactive components (after T013; FR-001).

### Migration and staff security evidence (T011–T015, 2026-10-27)

- Verified T001–T010 complete, clean starting Git state and 16/16 passing requirements checklist.
  Reviewed exact dependencies and approved specification/plan/model/contracts/Constitution.
- T011: Pinned EF tool 8.0.33 generated 20261027013745_InitialMenuAndIdentity, its designer and
  CafeDbContextModelSnapshot. Reviewed Up/Down plus forward/rollback SQL offline. Only seven
  standard Identity tables and MenuCategories/Products/CafeTables are added, with approved
  column types, required fields, checks, FK and indexes. Rollback drops only these feature tables;
  no rollback was executed. has-pending-model-changes reports no model/snapshot differences.
- T012: User explicitly confirmed localhost\MSSQLSERVER01/CafeManagement with Windows integrated
  authentication. Read-only preflight confirmed that database did not exist. Stored the local
  connection setting only in the existing external development user-secrets store and applied
  the reviewed migration successfully. Encrypt=True/TrustServerCertificate=True is restricted
  to this local development connection, not committed configuration or production guidance.
  Verified the migration history entry, ten feature/Identity tables plus EF history, all five
  enabled/trusted check constraints, unique normalized-label/public-code indexes and Products
  category FK with NO_ACTION. No unrelated database or existing application data was changed.
- T013: Added explicit --bootstrap-user mode that exits without starting HTTP hosting, validates
  supplied options, creates missing Admin/Cashier roles and the supplied new account via Identity,
  and uses a transaction for role/user/membership writes. Existing accounts are never reset or
  promoted; existing-role mismatch fails safely. Normal startup never migrates or provisions.
  User explicitly approved random local development credentials. Provisioned one Admin and one
  separate Cashier; passwords are outside the repository under local user-secret keys
  LocalDevelopmentAccounts:Admin:Password and LocalDevelopmentAccounts:Cashier:Password, with
  corresponding Email keys. Bootstrap credentials existed only in child-process environments
  during provisioning and were cleared afterward. No passwords were printed or committed.
  Missing options returned exit 1 without hosting. Repeat runs succeeded without changing account
  count/password-hash/security-stamp fingerprints; an existing-account role mismatch returned
  exit 1 with no account mutation. Identity password verification succeeded for both accounts.
- T014: StaffAccess returns Allowed/Unauthenticated/Forbidden for HTTP/component adapters and
  uses a new async Identity scope for every authenticated operation. Actual host service checks
  allowed current Admin, rejected anonymous/Cashier/stale stamp and rejected a Cashier principal
  with an added Admin role claim, proving membership is checked against the database. No request
  payload/role is accepted as authoritative, and no Identity store/context is retained.
- T015: Added the .NET 8 RevalidatingServerAuthenticationStateProvider pattern with a 30-minute
  interval and fresh Identity scope per security-stamp check. Direct service checks accepted
  the current stamp and rejected stale/unknown identity. No browser/circuit integration or
  immediate cross-tab logout guarantee is claimed; host registration and UI integration remain
  T020 and full circuit behavior remains later validation. Validation commands initially loaded
  SQL Client's reference assembly and used shallow principal copies; correcting the command's
  Windows runtime assembly and independent identities resolved these diagnostic-command issues.
- dotnet restore CafeManagement.sln passed; final dotnet build CafeManagement.sln --no-restore
  passed with 0 warnings and 0 errors (2.23 seconds). Model/snapshot check passed. Ordinary HTTPS
  Development startup preserved WeatherForecast (200/five records), Swagger UI (200) and OpenAPI
  documentation, without bypassing certificate validation. Validation web host stopped.
- No automated test files, extra dependencies, entity/configuration changes, new business features
  or tasks beyond T015. No extension hook file exists. Credentials, local configuration, database
  files and build artifacts remain outside the commit; only reviewed source/migrations/task evidence
  are intended for Git. T016 onward remains unstarted.

- [X] T016 [P] Add `CafeManagement.Api/Controllers/AuthController.cs` with authenticated GET `/api/auth/me` returning email/roles and no-store GET `/api/auth/antiforgery` returning requestToken plus framework cookie; configure RequestVerificationToken consistently and keep authentication cookie HttpOnly; no public registration or JSON login endpoint (after T010; `contracts/api.md` session contract).
- [X] T017 [P] Add `CafeManagement.Api/Pages/Account/Login.cshtml` and `Login.cshtml.cs` for ordinary HTTP GET/POST email/password login with antiforgery, generic credential/lockout failure, safe local returnUrl and full redirect after SignInManager cookie issuance; use Admin default `/admin/categories`, authenticated non-admin denial, and no client role selector/registration (after T013; FR-001).
- [X] T018 [P] Add `CafeManagement.Api/Pages/Account/Logout.cshtml` and `Logout.cshtml.cs` for authenticated antiforgery-protected POST logout with cleared cookie and full redirect to login; forbid GET sign-out and do not perform cookie mutations from interactive handlers (after T013; FR-001).
- [X] T019 [P] Add `CafeManagement.Api/Components/App.razor`, `Routes.razor`, `_Imports.razor` and `Layout/AdminLayout.razor` with local Bootstrap assets, .NET 8 component routing/cascading authentication, staff navigation and HTTP logout form; keep public routes outside Admin enforcement and leave account pages outside interactive rendering (after T004/T015/T017/T018; FR-001/007).
- [X] T020 Integrate the host additively in `CafeManagement.Api/Program.cs`: Razor Pages, Razor components/Interactive Server, revalidating auth state, concrete staff/options services, static files, Identity cookie options and Admin policy; authentication then authorization then antiforgery; map controllers/pages/components without swallowing `/api/*`, existing `/WeatherForecast` or Swagger, and return 401/403 for APIs rather than HTML redirects (after T014–T019; FR-001/007/008).

### HTTP accounts and Blazor host evidence (T016–T020, 2026-10-27)

- Verified T001–T015 complete, clean starting Git state and 16/16 passing quality checklist.
  Reviewed exact tasks/dependencies and approved specification, plan, research, model, contracts,
  quickstart and Constitution. No extension hook file exists.
- T016: Added GET /api/auth/me (authenticated email/current roles) and anonymous/staff GET
  /api/auth/antiforgery (framework cookie/request token); both use no-store responses. Header
  RequestVerificationToken is configured consistently. No registration or JSON login API exists.
- T017: Added ordinary HTTP Razor Page login with automatic Razor Pages antiforgery validation,
  email/password fields, generic credential/lockout failure and full-navigation cookie issuance.
  Valid Admin redirects to a local returnUrl or /admin/categories; an external returnUrl was
  rejected in favor of the default. Cashier authenticates then receives a 403 access-denied view
  on the same HTTP page, with sign-out; no dashboard or role selector was introduced.
- T018: Added authenticated POST logout with Razor Pages antiforgery validation and full redirect
  to /account/login. Authenticated GET logout returns 405. Missing POST token returns 400 and
  leaves the session authenticated; a valid fresh RequestVerificationToken permits logout and
  subsequent /api/auth/me returns 401. Account pages do not load Blazor/enhance their HTTP forms.
- T019: Added App/Routes/_Imports and AdminLayout with local Bootstrap/site CSS, framework
  Blazor script, AuthorizeRouteView, staff navigation/session identity and an AntiforgeryToken
  HTTP logout form. Public routes are not placed under an Admin default policy/layout. Account
  pages remain Razor Pages. Future routable screens select their approved Interactive Server
  mode/layout in their story tasks; no temporary home/admin/menu screen or validation route was
  created. /admin/categories is intentionally not implemented until its later story task.
- T020: Added .NET 8 Razor Pages and Razor components/Interactive Server registration, cascading
  auth, the scoped revalidating provider and StaffAccess, bound options, Admin policy and static
  serving. Authentication precedes authorization and antiforgery; controllers/pages/components
  retain separate endpoint mapping, with no blanket HTML fallback. Identity cookie is HttpOnly,
  Secure, SameSite=Lax and nonpersistent; API login/access-denied events return 401/403 without
  redirect. Existing factory/Identity stores, operator bootstrap and sample API remain intact.
  Configured PublicWebBaseUrl=https://localhost:7128 only in local user secrets; this is desktop
  development configuration and does not claim phone reachability. Invalid HTTP origin supplied
  by a temporary process override failed startup with OptionsValidationException as intended.
- dotnet restore CafeManagement.sln passed; dotnet build CafeManagement.sln --no-restore passed
  with 0 warnings and 0 errors (5.20 seconds). Model/snapshot check reports no pending changes.
  Actual host services resolve StaffAccess and IdentityRevalidatingAuthenticationStateProvider;
  Admin policy allows Admin/denies Cashier. Its API forbidden cookie event returns 403 without
  Location. A live anonymous /api/auth/me request returned 401 without redirect.
- Focused HTTP checks using existing accounts passed for both roles: valid login, generic invalid
  credentials, rejected missing login CSRF, current session email/roles, no-store token/session
  responses, secure session-cookie flags, GET logout rejection, rejected missing logout CSRF,
  valid header-token logout and cleared session. The invalid-credentials check used Admin once
  per run followed by valid login; no account lockout was triggered. No credentials were printed.
- Bootstrap CSS, site CSS and /_framework/blazor.web.js returned 200; /_blazor/negotiate exposed
  Interactive Server transports. Unknown /api route returned 404 without component HTML.
  WeatherForecast returned five records; Swagger UI/OpenAPI remained available over HTTPS with
  normal certificate validation. Validation web host stopped. Component shell compilation and
  transport readiness are verified; actual management/menu rendering and connected-circuit
  authorization remain later feature checks, not claimed complete here.
- Read-only before/after metadata/account fingerprints matched: schema/table/migration counts,
  account/role/membership counts, password hashes and security stamps unchanged. No database
  reset/recreation, migrations, provisioning, account reset, new dependencies or test files.
  Diagnostic-command fixes handled expected redirects, JSON arrays and empty-header values in
  PowerShell; no application defect was hidden by those command corrections.
- Only T001–T020 are complete. T021 onward remains unstarted; shared API errors (T021), account
  presentation refinement (T022) and their complete integration checkpoints remain later work.
  Credentials/local settings/build artifacts are excluded from the reviewed source commit.

- [X] T021 Define only the minimal shared service failure/field-error shape in `CafeManagement.Api/Application/OperationError.cs` and HTTP mapping in `CafeManagement.Api/Contracts/ApiErrors.cs`, wiring safe ProblemDetails/validation responses in `CafeManagement.Api/Program.cs`; cover denied/not-found/invalid/conflict and generic 500 without secret disclosure, not a generic application framework (after T020; FR-001/004/008/011).
- [X] T022 Finish accessible Bootstrap HTTP account presentation in `CafeManagement.Api/Pages/_ViewImports.cshtml`, `Pages/Account/Login.cshtml` and `Pages/Account/Logout.cshtml`; ensure full form posts include antiforgery and no enhanced same-circuit cookie interception, with readable errors and keyboard labels (after T017–T020; FR-001).
- [X] T023 Build `CafeManagement.sln`, run its existing HTTPS profile from `CafeManagement.Api/Properties/launchSettings.json` and verify Swagger, `/WeatherForecast`, static CSS, HTTP account pages and component host coexist without runtime DI/options/route errors; record any real failure before adding feature services (after T021/T022; Constitution II/VI).
- [X] T024 Manually verify `CafeManagement.Api/Pages/Account/Login.cshtml`, `Logout.cshtml` and `Controllers/AuthController.cs`: valid/invalid credentials, Admin/Cashier session roles, cookie flags, missing/invalid CSRF and logout/new HTTP principal; inspect service/circuit authorization lifetimes and record evidence here; management redirects are not accepted as complete feature screens until their story phases (after T023; FR-001 foundation checkpoint).

### Foundation completion evidence (T021–T024, 2026-10-27)

- Verified T001–T020 complete, clean starting working tree and 16/16 passing requirements
  checklist. Reviewed exact dependencies, approved plan/model/contracts and Constitution;
  no extension hooks are present. No tasks beyond T024 were executed.
- T021: Added only OperationError kind/message/code/field/field-error data and ApiErrors mapping.
  Verified unauthenticated/forbidden/not-found/invalid/conflict/unexpected map to 401/403/404/
  400/409/500 with application/problem+json; validation preserves safe field feedback, duplicate
  table-label conflict preserves errorCode/field, and unexpected errors suppress supplied text
  and metadata. Model-state mapping replaces raw framework errors with safe field messages.
  Added API-only status/exception handling in Program.cs; generic failures log a correlation ID
  without logging raw exception/configuration text from that boundary. No result framework,
  new business service or endpoint was introduced. HTTP 401/404 ProblemDetails checks passed.
  A temporary second host used a process-only nonexistent database target to trigger a read
  failure in existing /api/auth/me: returned generic HTTP 500 ProblemDetails without exception,
  server/database or credential disclosure. No database was created or modified by that host;
  real local connection configuration remained unchanged. EF's own server diagnostics remained
  server-side. Direct model-state/mapping checks cover cases whose business APIs do not exist yet.
- T022: Added shared Razor tag-helper imports and responsive Bootstrap account presentation,
  heading/label associations, native required inputs, per-field descriptions and live error
  summary. Account forms explicitly render antiforgery tokens and disable enhanced submission;
  automatic duplicate generation is disabled where Html.AntiForgeryToken is explicit. No
  interactive account rendering or cookie mutation was introduced. Login/access-denied forms
  render normally; logout still preserves its existing GET 405 and successful POST redirect.
  Logout.cshtml has a matching safe fallback form, without changing existing handler behavior.
  Source/HTTP markup checks confirmed labels, error associations and no Blazor account script.
- T023: Restore succeeded; final dotnet build CafeManagement.sln --no-restore succeeded with
  0 warnings and 0 errors (1.57 seconds). Initial compilation caught an incorrect .NET 8 JSON
  response overload; it was corrected before the successful build/runtime checks. EF reports
  no pending model/snapshot changes. Existing HTTPS Development profile starts without DI,
  options or routing errors; Bootstrap/site CSS, account pages, framework Blazor script,
  Interactive Server negotiation, Swagger and five-record WeatherForecast coexist successfully.
  Unknown /api routes return JSON 404, not component HTML. Both validation hosts were stopped.
- T024: Existing Admin and Cashier passed valid login, generic invalid credentials, correct
  email/role session response, HttpOnly/Secure/SameSite=Lax session-cookie flags, no-store auth
  responses and safe external-returnUrl handling. Missing and invalid login/logout CSRF were
  rejected with 400; rejected logout preserved authentication, GET logout returned 405, and a
  fresh valid header token allowed POST logout. Subsequent /api/auth/me returned a new anonymous
  HTTP principal/401. Cashier authenticates but receives the approved 403 management denial.
  One invalid credential attempt per account was followed by valid login; no lockout triggered.
  Read-only before/after fingerprints verified schema/migration/account/role/membership counts,
  password hashes and security stamps unchanged. No credentials were printed or committed.
- Reviewed StaffAccess and circuit provider: each authenticated check creates/disposes a short
  Identity scope; no HttpContext/UserManager/DbContext is retained by a circuit helper. Staff
  checks use current database membership/stamp. Revalidation remains 30-minute stamp checking,
  with no immediate cross-tab sign-out claim. No schema/account reset, migration/provisioning,
  database cleanup, new dependencies or automated test files. Lightweight validation command
  corrections handled PowerShell JSON byte content and a reserved variable; no failed check
  was counted as passed until corrected and rerun successfully.
- T001–T024 are complete. Management/menu screens remain later story tasks, and login's Admin
  redirect is not evidence of their completion. Physical-phone UI and full connected-screen
  checks remain later feature validation. Only scoped source/presentation/task evidence changed;
  existing entities, migrations, accounts, authentication handlers and Blazor components remain.

**Checkpoint**: Shared foundation builds and authentication works. Management authorization must
still be validated against real story endpoints/components in their subsequent checkpoints.

## Phase 3: User Story 1 — Maintain the Menu (Priority: P1, first useful increment)

**Goal**: Authorized Admin creates/edits categories/products, positive EGP prices and availability.
**Independent verification**: Two categories and two products can be saved, edited and reopened,
including unavailable Admin-visible products; no tables, customer ordering or payments needed.

- [X] T025 [P] [US1] Define category request/response and editable view models in `CafeManagement.Api/Contracts/CategoryContracts.cs`: "enforce name length 1–120" after trim, "reject whitespace-only input" and "name duplicates allowed"; no editable ID/deactivation fields (after T024; FR-002/009/011).
- [X] T026 [P] [US1] Define product request/response and editable view models in `CafeManagement.Api/Contracts/ProductContracts.cs`: required existing CategoryId, "enforce name length 1–120", duplicate names allowed, explicit IsAvailable, price "greater than zero", "at most two fractional digits" and "at most 9999999999999999.99"; HTTP price invariant string, service/UI decimal and response currency EGP/two places, no silent rounding (after T024; FR-003/010/011).
- [X] T027 [US1] Implement category create/list/get/edit in `CafeManagement.Api/Application/MenuService.cs` using StaffAccess on every operation, per-operation disposed factory context, DTO projections/trim/whitespace validation and persisted duplicate-name allowance; no delete/deactivate methods (after T025/T021; FR-001/002/004/009/011).
- [X] T028 [US1] Extend `CafeManagement.Api/Application/MenuService.cs` for product create/list/get/edit including unavailable products, required category FK validation, exact decimal scale/range and availability changes; validate before SaveChanges, preserve atomicity and map missing category/record and failure outcomes without partial changes (after T026/T027; FR-001/003/004/010/011).
- [X] T029 [P] [US1] Add `CafeManagement.Api/Controllers/AdminCategoriesController.cs` with Admin-authorized GET list/item, POST and PUT routes under `/api/admin/categories`, explicit unsafe-request antiforgery, documented DTOs/statuses/Location and shared service mapping; no entity binding/removal endpoints (after T027; FR-001/002/004/009).

### Menu contracts/service/category API evidence (T025–T029, 2026-10-27)

- Confirmed T001–T024 complete, clean starting tree and 16/16 passing quality checklist;
  reviewed exact dependencies and approved specification, plan, research, model, contracts,
  quickstart and Constitution. No extension hook file exists.
- T025: Added category HTTP input/edit model and response DTO with trimmed 1–120 character,
  nonblank validation; no editable ID or deactivation field. Duplicate names remain allowed.
- T026: Added separate HTTP product input, decimal edit model/details and invariant string-price
  response mapping with fixed EGP/two-place output. Availability/category/price must be supplied;
  names are nonblank/trimmed/max120. HTTP price accepts digits and up to two fractional digits,
  no sign/grouping/exponent; decimal values must be positive, scale <=2 and <=9999999999999999.99.
  Rejected excess scale (including trailing-zero scale three) without rounding. Wire checks
  accepted 25.50/the maximum and rejected zero/negative/exponent/group/sign/excess-scale/overflow.
- T027–T028: MenuService checks StaffAccess on every category/product read/write, creates and
  asynchronously disposes one factory context per operation, projects DTOs and allows duplicate
  names. Product admin reads include unavailable records. Missing addressed record maps to
  NotFound; missing category/invalid input maps to field validation; unexpected authorization/
  persistence failures return a safe OperationError. Each accepted mutation uses one SaveChanges;
  rejected edits do not save or partially change a record. No deletion/deactivation methods.
- T029: Added Admin-policy category list/item/create/edit controller with explicit POST/PUT
  antiforgery, DTO/status documentation, 201 Location and shared error mapping. No entity input
  binding/removal route. The controller constructs the shared concrete service from the existing
  factory/StaffAccess registrations so this task is functional without prematurely executing
  T033's shared MenuService registration; Program.cs remains unchanged.
- Restore passed; final solution build passed with 0 warnings/0 errors (2.29 seconds). EF reports
  no pending model/snapshot changes. No migration or dependency/configuration change occurred.
- HTTPS category checks passed: anonymous 401; Cashier GET/POST 403; missing/invalid CSRF 400;
  Admin create 201 with Location, trimming, independent duplicate names, edit/re-read, blank/
  overlong rejected edits preserving saved values, nonpositive ID 400/missing ID 404, DELETE 405
  and Swagger category routes. Existing accounts were used without resetting/provisioning.
- Actual host service checks using server-generated Identity principals passed product create,
  duplicate names, trim, availability/price edits, new-context persistence, unavailable admin list,
  missing product/category outcomes, absent availability and price scale/range rejection.
  Re-reading after rejected edits preserved category/price/availability. All eight service
  operations rejected anonymous/Cashier callers. Product response formatted 25.60 with EGP.
- Validation created only new labelled samples: categories 1/2 (T029) and category 3 (T028),
  products 1/2. Samples are retained; no existing records were edited/deleted. After stopping
  the web host, read-only SQL confirmed three categories/two products persisted, with two
  existing accounts/two roles/one migration unchanged. No database reset or cleanup was performed.
- Reviewed diff/secret exclusions. Only the selected contracts/service/controller/task evidence
  changed; existing auth, entities, mappings, migrations, components and host remain intact.
  No automated test files, product endpoints, UI screens, POS/orders/payments/reports or tasks
  beyond T029. T030 onward remains unstarted; full component/controller integration is later work.

- [X] T030 [P] [US1] Add `CafeManagement.Api/Controllers/AdminProductsController.cs` with the approved GET list/item, POST and PUT `/api/admin/products` contract, Admin policy/unsafe-request antiforgery and invariant price DTO mapping, showing all products only to Admin (after T028; FR-001/003/004/010/011).
- [X] T031 [P] [US1] Add `CafeManagement.Api/Components/Pages/Admin/Categories.razor` with Admin route authorization, Interactive Server, Bootstrap list/Add/Edit EditForm, field errors and loading/empty/saving states; obtain trusted principal from AuthenticationStateProvider and call MenuService directly (after T027; FR-001/002/004/007).
- [X] T032 [P] [US1] Add `CafeManagement.Api/Components/Pages/Admin/Products.razor` with Admin/Interactive Server, all-product list and form for name/category/decimal EGP price/availability, IDs distinguishing duplicate category names, retained input on failure and duplicate-submit prevention; no circuit-retained context or loopback HTTP (after T028; FR-001/003/004/010/011).
- [X] T033 [US1] Register MenuService in `CafeManagement.Api/Program.cs` and confirm `CafeManagement.Api/Components/Layout/AdminLayout.razor` navigation reaches category/product pages; build `CafeManagement.sln` and manually exercise list/item/create/edit through both component and Swagger/controller paths (after T029–T032; FR-002/003/004).
- [X] T034 [US1] Manually verify the input/security/save scenarios for `CafeManagement.Api/Components/Pages/Admin/Categories.razor`, `Products.razor` and `Controllers/AdminProductsController.cs`: EGP25.50, blank/whitespace/overlength, 0/negative/25.501 prices, duplicate names, nonexistent category, unavailable Admin visibility, denied anonymous/Cashier calls and failed-save feedback; record outcomes here (after T033; FR-001/004/008/010/011, SC-005/006/008).

### Product API and menu screens evidence (T030–T034, 2026-10-27)

- Resumed the usage-limit interruption by inspecting the entire Git diff and all five existing
  changed files before editing. Preserved the interrupted product controller, both admin pages,
  category-controller DI adjustment and MenuService registration. T001–T029 were already complete;
  quality checklist passed 16/16, and no extension hook file exists. No further application-code
  changes were needed during the resumed session; only this verified task evidence was added.
- T030: AdminProductsController implements GET list/item and POST/PUT with Admin policy, explicit
  unsafe-request antiforgery, ProductInput mapping, fixed EGP/two-place invariant string responses,
  status documentation/Location and shared failure mapping. Admin listing includes unavailable
  products. No DELETE/deactivation, unrelated API or entity binding was added.
- T031–T032: Admin-authorized Interactive Server category/product pages use Bootstrap, direct
  MenuService calls and the current AuthenticationStateProvider principal per operation. Forms
  provide add/edit, validation summaries/field feedback, loading/empty/saving states, retained
  input on failure and disabled controls plus saving guards against duplicate submissions.
  Product forms use nullable decimal/category/availability input without inventing defaults;
  selectors distinguish category duplicates by ID. Components retain no context/Identity store
  or loopback HTTP client. Invalid submission clears earlier success feedback before validation.
- T033: Registered scoped MenuService and changed the category controller to consume that shared
  registration. Existing staff navigation reaches both real pages. Real-browser checks on the
  live HTTPS host exercised category and product list/item/create/edit through interactive form
  events; separate HTTP controller checks exercised the same product operations and inspected
  Swagger/OpenAPI routes. Existing source and five interrupted files remained intact.
- T034 component checks passed: EGP25.50 and two-place display; blank/whitespace/overlength names;
  zero/negative/25.501 prices; duplicate category/product names; explicit category IDs; availability
  changes and unavailable Admin visibility. Invalid values remained in forms with feedback and no
  false success. A browser-only tampered category option triggered the real missing-category
  service failure, displayed its field error and retained entered product values without saving.
  A disposable second host using a process-only nonexistent database target verified safe category
  failed-save feedback, retained input and no success/exception disclosure. It never created or
  modified a database or changed the real connection configuration.
- Product API checks passed: GET/list/item, POST 201 plus Location, PUT 200 and re-read, exact EGP
  formatting, duplicates allowed, blank/whitespace/overlength/price/category/availability failures
  returning 400, rejected edits preserving saved values, missing/invalid CSRF rejected and DELETE
  returning 405. Anonymous category/product APIs returned 401; Cashier GET/POST returned 403.
  Anonymous component navigation required login; Cashier component navigation displayed denial.
- Restore passed. Initial build encountered the old validation process locking apphost.exe;
  verified its path and stopped only that project's leftover process. Rebuild succeeded with
  0 warnings and 0 errors (1.95 seconds). Model/snapshot check reports no pending changes.
  No dependency, entity/configuration/migration, account or authentication-handler changes.
- Browser diagnostics used disposable headless Edge contexts and in-memory commands, not test
  files/projects. Corrected diagnostic BOM decoding and waiting for editor replacement. Browser
  navigation initially verified the trusted local certificate; Node API requests required a
  local diagnostic-context certificate override, with no application/certificate setting change.
  Failed attempts were not counted as passing until the relevant checks were completed successfully.
- Validation retained newly labelled categories 4–6 and products 3–6; category 4 came from the
  first interrupted browser-check attempt. Original categories 1–3 and products 1–2 were preserved.
  Final read-only counts: six categories/six products, two existing accounts/two roles/one migration.
  No sample cleanup, database reset or account provisioning/reset was performed. Validation hosts
  were stopped afterward. Credentials/build output/local configuration remain outside the commit.
- Only T001–T034 are marked complete. T035 (restart/reopen increment checkpoint) and all later
  work remain unstarted; this evidence does not declare the full menu-management increment done.

- [X] T035 [US1] Reopen edited categories/products, restart the host and verify persisted names/price/availability via `CafeManagement.Api/Components/Pages/Admin/Categories.razor` and `Products.razor`; record SC-001 and the applicable quickstart outcomes in this file before declaring the menu-management increment complete (after T034; FR-004, SC-001).

### Menu-management checkpoint evidence (T035, 2026-10-27)

- Verified T001–T034 complete, clean master tracking origin/master, and the 16/16 specification
  checklist gate. Reviewed T035, SC-001/FR-004, approved planning/contracts, applicable quickstart
  checks and Constitution. No Spec Kit extension hooks exist. Only T035 was executed.
- SC-001/FR-004 passed: one existing Admin created two labelled categories and two products
  through the live Interactive Server forms, edited both category/product names, and changed
  product prices and availability. Reopened all four editors before and after completely stopping
  and restarting the HTTPS host. All saved values matched the API: 2/2 categories and 2/2 products
  retained (100%), including category associations, EGP 36.75/47.60 and true/false availability.
  Unavailable products remained visible/editable to Admin. Direct navigation and refresh passed.
- Applicable quickstart input/error checks passed: whitespace-only category names and zero,
  negative and 25.501 prices showed validation feedback, retained input and cleared success text.
  A browser-only invalid category option exercised the server's missing-category failure; the
  field message appeared, entered values remained and the prior saved product was unchanged.
  API edits rejected blank/whitespace/overlength category/product names, invalid prices,
  missing/nonexistent category and missing availability with safe 400 ProblemDetails.
  Re-reading both categories/products confirmed rejected changes had not persisted.
- Applicable quickstart permissions/antiforgery checks passed: anonymous category/product
  read/create/edit APIs returned 401; Cashier equivalents returned 403. Both roles were denied
  management forms. Admin POST/PUT requests with missing/incorrect CSRF tokens returned 400.
  Missing records returned 404 and DELETE returned 405. Reviewed per-operation current Identity
  user/stamp/Admin checks and short-lived factory contexts; no UI-only authorization dependency.
  Valid Admin login and ordinary HTTP sign-out passed; subsequent protected API access was 401.
- Existing WeatherForecast still returned five forecasts; Swagger UI/schema and category/product
  routes worked. Restore succeeded; dotnet build CafeManagement.sln --no-restore passed with
  0 warnings and 0 errors (13.00 seconds). EF has-pending-model-changes reported no model changes.
- Preserved original categories/products 1–6 using read-only before/after data fingerprints;
  Identity account/password/security-stamp and role-membership fingerprints were unchanged.
  Retained new validation categories 7–8 and products 7–8, labelled T035 1793069315323.
  Final counts: eight categories, eight products, two existing accounts, two roles, one migration.
  No deletion, database reset, migration, account provisioning/reset or application changes.
- Validation used disposable headless Edge sessions and in-memory diagnostic commands, without
  adding automated tests/files. Node-backed requests used a disposable diagnostic-context HTTPS
  certificate override only; application/certificate configuration was unchanged. A nested
  PowerShell here-string initially prevented the read-only SQL diagnostic from parsing; corrected
  command quoting before taking the baseline. No application defect was found. Hosts were stopped.
- US1 menu administration checkpoint is complete. Table/QR/public-menu/phone, reconnect and
  later cross-cutting quickstart checks remain for their own tasks; none is claimed passed here.
  T036 and all later tasks remain unchecked and were not implemented.

**Checkpoint**: First useful MVP is US1 administration, not the whole QR/customer feature.
All stories remain P1; sequence reflects implementation dependency, not a downgraded priority.

## Phase 4: User Story 2 — Manage Tables and Their QR Codes (Priority: P1)

**Goal**: Manage unique table labels and view a stable table-specific QR image/destination.
**Independent verification**: Table persistence/uniqueness and decoded QR destinations can be
checked without customer ordering or a populated menu. Full scan-to-menu acceptance intentionally
waits for US3 and T051; do not claim the customer QR journey complete before that integration.

- [X] T036 [P] [US2] Define `CafeManagement.Api/Contracts/TableContracts.cs` for editable Label and read-only Id/PublicCode/menuUrl: "enforce ... label length 1–50 after trimming", nonblank, "Set NormalizedLabel from trimmed Label.ToUpperInvariant(); reject normalized length over 50" and immutable server-generated PublicCode; no client-editable normalization/code fields (after T024; FR-005/006/011).
- [X] T037 [US2] Implement Admin-protected table list/get/create/edit in `CafeManagement.Api/Application/TableService.cs` with per-operation factory contexts, trimmed display label/invariant normalized unique key and immutable random GUID code; precheck labels and map index race to duplicate_table_label conflict, retry a code collision without assigning an existing table's code (after T036; FR-001/004/005/006/011).
- [X] T038 [US2] Add `CafeManagement.Api/Application/TableQrService.cs` using QRCoder PngByteQRCode and StaffAccess, trusted configured HTTPS PublicWebBaseUrl plus `/menu/t/{publicCode}`, stable rendering after rename and repeated access, returning PNG bytes without stored images or unchecked Host-header URLs (after T037/T003; FR-001/005/006).
- [X] T039 [US2] Add `CafeManagement.Api/Controllers/AdminTablesController.cs` with authorized GET list/item/QR PNG plus POST/PUT table routes, antiforgery for writes, proper 201/200/404 and duplicate-label 409 mapping; no DELETE/deactivation/QR-rotation routes (after T038; FR-001/005/006/009/011).

### Table API execution evidence (T036–T039, 2026-10-27)

- Confirmed T001–T035 complete, starting working tree clean and specification checklist 16/16.
  Reviewed approved spec, plan, research, data model, API/UI contracts, quickstart and Constitution.
  No extension hooks exist. Only T036–T039 were implemented; existing source remains unchanged.
- T036: Added Label-only input/edit model, nonblank/trimmed and normalized length validation
  (1–50), and a response containing Id/Label/PublicCode/MenuUrl. HTTP attempts to supply ID,
  normalization or public code did not control persisted identity. No editable code/rotation field.
- T037: Added Admin-protected list/get/create/edit with a fresh disposed factory context per
  operation, invariant uppercase normalization and label prechecks. Existing SQL unique indexes
  remain authoritative. Reviewed SQL 2601/2627 index-specific handling: label conflicts return
  duplicate_table_label with field label; creation retries only proven PublicCode-index collisions
  with a fresh entity/random GUID, at most three attempts. Existing codes are never reassigned.
  The rare GUID-collision retry branch was inspected, not forced during live validation.
- T038: QRCoder PngByteQRCode returns in-memory PNG bytes after TableService's current
  Identity user/security-stamp/Admin check. Destinations use configured HTTPS PublicWebBaseUrl
  and /menu/t/{publicCode}; no request Host/forwarding-header origin or saved image. Two PNGs
  decoded to their distinct exact table URLs. A rename, repeated reads and a complete host restart
  preserved codes/URLs and byte-identical PNG hashes. Forwarded-host/protocol tampering did not
  change menuUrl. QR responses are image/png with no-store.
- T039: GET list/item/QR, POST 201 with item Location and PUT 200 passed on the live HTTPS host.
  Blank/whitespace/null/overlength labels returned 400; case/space duplicates and conflicting
  edits returned safe 409 with duplicate_table_label/label. Rejected writes left rows unchanged.
  Two simultaneous case-variant creates returned one 201 and one 409 with one persisted row.
  Anonymous list/item/QR/create/edit returned 401; Cashier equivalents returned 403. Admin
  POST/PUT with missing/invalid antiforgery returned 400. Invalid IDs returned 400, unknown
  records/QR returned 404 and DELETE returned 405. No removal/deactivation/rotation endpoints.
- Controller composition uses the already registered factory, StaffAccess and options to make
  these API endpoints usable now. Shared TableService/TableQrService DI registration and switching
  controller injection to those registrations remain T041 with component integration; Program.cs
  and the navigation/layout were not changed ahead of that task.
- Restore succeeded. dotnet build CafeManagement.sln --no-restore passed with 0 warnings and
  0 errors (6.59 seconds). EF has-pending-model-changes reported no changes. Existing category/
  product components/APIs, WeatherForecast's five records and Swagger/schema remained usable.
  No migration, dependency update, account provisioning/reset or database recreation was needed.
- Read-only before/after fingerprints confirmed all eight existing categories/products and the
  two Identity accounts/password/security stamps/role memberships unchanged; two roles and one
  migration remain. Retained only new labelled validation tables 1–3 (T039 1793069907131 and
  T039 race 1793070101237); no records were deleted. Validation hosts were stopped afterward.
- Used in-memory diagnostics/disposable browser sessions; no automated test files or project.
  Node requests used a disposable diagnostic-context certificate override only. The installed
  browser lacked BarcodeDetector; the isolated jsQR decoder initially encountered an upstream
  404 and then a UMD-binding error. Corrected the diagnostic URL/binding and successfully decoded
  both PNGs without adding an application dependency or weakening application HTTPS settings.
  Decoder source: https://raw.githubusercontent.com/cozmo/jsQR/master/dist/jsQR.js,
  SHA-256 3325b0888fa4745c4e6940897d8c4f426fbaae76901fcbfe1871a04e90a51655.
- No application defects/blockers were found. The table component, public menu destination and
  physical-phone scan journey are not implemented or claimed complete; T040 onward remain unchecked.

- [X] T040 [US2] Add `CafeManagement.Api/Components/Pages/Admin/Tables.razor` with Admin/Interactive Server, Bootstrap list/create/edit form, field conflict messages and inline authorized QR PNG/menu link preview; trusted circuit principal calls services directly, no rotation/print/download workflow (after T038; FR-001/005/006/009/011).
- [X] T041 [US2] Register table/QR services in `CafeManagement.Api/Program.cs`, connect the table page in `CafeManagement.Api/Components/Layout/AdminLayout.razor`, and build `CafeManagement.sln`; verify controller and component management agree on outputs/errors (after T039/T040; FR-005/006).
- [X] T042 [US2] Manually review `CafeManagement.Api/Components/Pages/Admin/Tables.razor` and `Controllers/AdminTablesController.cs`: distinct labels, blank/overlength/duplicate/case-space variants, label-update conflict, unknown ID, Admin-only read/write/QR access and denied CSRF writes; verify no failed change persisted (after T041; FR-001/004/005/008/011, SC-005/008).
- [X] T043 [US2] Generate QR images for two tables through `CafeManagement.Api/Application/TableQrService.cs`, decode/compare destinations, rename/restart/reopen and verify each code is stable and distinct with the correct table record; record the QR-management checkpoint here and leave full menu-scan acceptance to T051 (after T042; FR-004/005/006).

### Table management checkpoint evidence (T040–T043, 2026-10-27)

- Confirmed T001–T039 complete, clean master tracking origin/master and 16/16 checklist gate.
  Reviewed the exact task dependencies and approved spec, plan, contracts, data model, research,
  quickstart and Constitution. No extension hooks exist. Only T040–T043 were implemented.
- T040: Added Admin-authorized Interactive Server Tables.razor with a Bootstrap list/create/edit
  form, field/summary validation, retained input on conflicts, loading/empty/saving/error feedback
  and guarded saves. It obtains the trusted circuit principal from AuthenticationStateProvider
  and calls TableService/TableQrService directly. Saved records show a labelled inline PNG and
  the trusted menu URL; unsaved text is not presented as a newly saved QR label. New-table action
  clears the preview. No context, HTTP loopback, QR rotation, print/download or removal workflow.
- T041: Registered both concrete services as scoped and changed AdminTablesController to inject
  those same registrations. Existing AdminLayout Tables navigation already targets /admin/tables;
  it now reaches the real screen, verified alongside category/product navigation. No layout edit
  was needed. Component-generated PNG bytes matched the controller endpoint byte for byte.
- T042: Live UI created two distinct trimmed labels, edited/reopened both and accepted a label
  exactly 50 characters long. Blank/whitespace/51-character edits displayed validation messages.
  Duplicate create and case/space-variant conflicting edits displayed the label conflict message,
  retained entered values, cleared success feedback and left saved rows unchanged. Matching API
  checks returned 400 validation and 409 duplicate_table_label/label, with no failed write persisted.
  A valid HTTP edit matched the component record and could not assign a supplied public code.
- Anonymous/Cashier table pages denied access without rendering the form or QR preview; list,
  item, create, edit and QR APIs returned 401/403 respectively. Admin POST/PUT with missing or
  invalid antiforgery returned 400. Unknown item/edit/QR IDs returned 404; nonpositive IDs returned
  400 and DELETE returned 405. Components use the framework circuit/form protections and service
  authorization; HTTP writes retain controller antiforgery validation. No authorization or cookie
  handling changes were made. A 390px table-page check showed no horizontal page overflow.
- T043: Decoded both inline service-generated PNGs to their exact configured HTTPS table URLs.
  The two public codes/destinations were distinct. Renamed both records, fully stopped/restarted
  the host, reopened both editors and decoded again: labels persisted, each code/URL stayed with
  its correct table, and PNG hashes stayed byte-identical to their pre-rename values. The API
  matched component values/PNGs after restart; direct navigation and refresh also passed.
- Restore succeeded. Initial build caught CS0542 from an injected property named Tables matching
  the Razor component class; renamed it TableManager. Rebuild passed with 0 warnings/0 errors
  (6.69 seconds). EF has-pending-model-changes reported no changes. Existing category/product
  screens/APIs, five-forecast WeatherForecast behavior and Swagger/schema remained usable.
- Read-only before/after fingerprints preserved original tables 1–3 (including codes), all eight
  categories/products and both Identity accounts/password/security stamps/role memberships.
  Retained new validation tables 4–5, labelled T043 1793070665121. Final counts: five tables,
  eight categories, eight products, two accounts, two roles and one migration. No reset, deletion,
  new migration, dependency change or account provisioning/reset. Validation hosts were stopped.
- Used disposable Edge sessions and in-memory diagnostics, with no automated tests/files added.
  Corrected reused diagnostic helpers that expected product success text and a single h2; resumed
  with the same new rows rather than creating extras. Node-backed requests used only a disposable
  diagnostic-context certificate override; application/certificate settings were unchanged.
  Isolated QR decoding reused the jsQR source/hash recorded under T036–T039. All affected checks
  were completed successfully after correcting the diagnostic helpers and component name.
- US2 table/QR-management checkpoint is complete. Public customer menu implementation and full
  signed-out phone scan/browse acceptance remain T044 onward and T051; they are not claimed passed.
  T044 and all later tasks remain unchecked. No ordering/POS/payment or unrelated features added.

## Phase 5: User Story 3 — Browse the Menu on a Phone (Priority: P1)

**Goal**: Signed-out mobile customer browses the current available menu with correct table context.
**Independent verification**: Given two manually created sample tables/categories/products, no
staff session is needed for browsing; ordering/payment/POS services are unnecessary.

- [X] T044 [P] [US3] Define the public projection in `CafeManagement.Api/Contracts/PublicMenuContracts.cs`: table label/publicCode, fixed EGP, category ID/name and available product ID/name/price only; omit internal table IDs, Identity/normalization and unavailable-product data, keeping empty category/product arrays explicit (after T024; FR-006/007/008/010/011).
- [X] T045 [US3] Add `CafeManagement.Api/Application/PublicMenuService.cs` with anonymous per-operation factory reads, safe GUID lookup, correctly selected shared menu/table and server-side IsAvailable filter; return explicit invalid/unknown table/empty/read-failure outcomes, deterministic ID ordering, decimal EGP values and no side effects during initialization/prerender (after T044; FR-006/007/008/010/011).
- [X] T046 [US3] Add `CafeManagement.Api/Controllers/PublicMenuController.cs` for anonymous GET `/api/menu/tables/{publicCode}`, no-store menu response, approved price-string projection and 400/404/problem responses; never invoke staff checks, return hidden data or choose a fallback table (after T045; FR-006/007/008/010).
- [X] T047 [US3] Add `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` and `Components/Layout/PublicLayout.razor` with anonymous Interactive Server, direct public-service calls, visible table, Bootstrap category/product content and EGP two-place prices; implement loading/empty/invalid-table/read-error/retry-read states and keyboard/mobile usability without staff shell or login preflight (after T045; FR-006/007/008/010/011).
- [X] T048 [US3] Register PublicMenuService in `CafeManagement.Api/Program.cs`, confirm anonymous menu deep links in `CafeManagement.Api/Components/Routes.razor` and build `CafeManagement.sln`; verify both public controller and direct component reads enforce identical filtering/table semantics (after T046/T047; FR-006/007/010).

### Public menu implementation evidence (T044–T048, 2026-10-27)

- Confirmed T001–T043 complete, clean master tracking origin/master and the 16/16 checklist gate.
  Reviewed exact task dependencies and approved spec, plan, contracts, data model, research,
  quickstart and Constitution. No extension hooks exist. Only T044–T048 were implemented.
- T044: Added public-only table label/code, category ID/name and available product ID/name/price
  records. Services/components retain decimal prices; HTTP projection formats invariant two-place
  price strings and fixed EGP. Public responses contain no internal table ID, normalized label,
  Identity data, category association/availability flags or unavailable-product records.
- T045: Added an anonymous read-only service with GUID parsing, exact table-code lookup,
  factory context creation/disposal per call, AsNoTracking projections and deterministic ID order.
  The available-product predicate executes in SQL before projection; no staff-access dependency,
  entity serialization, fallback table, initialization writes, application cache or polling.
  Empty category/product arrays are explicit. Invalid/unknown links and read errors have safe
  distinct outcomes; API cancellation is passed through rather than misreported as success.
- T046: Anonymous GET /api/menu/tables/{publicCode} returned the exact approved public projection
  for all five existing tables: correct label/code, shared eight categories and four available
  products. Compared full JSON with Admin data filtered by IsAvailable; all four unavailable
  products were absent. Prices retained two decimal places. Success/error responses were no-store;
  malformed/unknown codes returned 400/404 ProblemDetails. Public POST returned 405.
- T047: Added a separate public layout and anonymous Interactive Server menu page calling the
  service directly, with visible table, Bootstrap category anchors/product lists, two-place EGP
  prices, loading/empty-category/empty-menu/error/retry states and no staff shell/account controls.
  Parameter changes trigger fresh reads; a load sequence prevents an older table read replacing
  a later navigation. Signed-out direct loads for two tables, refresh, interactive reload, category
  navigation and keyboard link focus passed. Empty categories displayed their explicit message.
  Public browser requests did not call staff auth/login preflight endpoints.
- Navigation review found fragment-only category anchors resolved against the host's root base
  URL. Corrected anchors to retain /menu/t/{publicCode} before their category fragment and repeated
  the checks successfully. Also corrected a diagnostic focus assertion that ran during reload
  repaint. No automated test file/project was introduced to diagnose this issue.
- A disposable second host used a process-only nonexistent database target, never creating a
  database or changing real/local secret configuration. Its public API returned generic 500
  ProblemDetails without SQL/credentials/stack details; the page showed safe read-error feedback
  and a working retry-read action without stale table/products. No SQL service was stopped.
- T048: Registered PublicMenuService as scoped. Existing Routes.razor already permits anonymous
  components and honors the explicit public layout; no route/auth rewrite was needed. API and
  component values/filtering matched. Anonymous Admin APIs/QR remained 401, Cashier Admin access
  remained 403, while Cashier could also browse the public menu. Existing Admin category/product/
  table navigation, five-forecast WeatherForecast behavior and Swagger/schema remained usable.
- Restore and initial build passed (0 warnings/errors, 12.13 seconds). After the anchor correction,
  rebuild passed with 0 warnings/errors (5.96 seconds). EF has-pending-model-changes reported no
  changes. No dependencies, migrations, account handling or existing Admin source were changed.
- Read-only before/after fingerprints matched for all five tables/codes, eight categories,
  eight products, two Identity accounts/password/security stamps/role memberships, two roles and
  one migration. No data was created, edited or deleted. Validation hosts were stopped afterward.
- Validation used disposable Edge contexts/in-memory commands, with a diagnostic-context HTTPS
  certificate override only for Node-backed requests; application/certificate settings unchanged.
  No secrets, generated artifacts or automated tests belong in this change. Dedicated T049/T050
  scenario/mobile reviews and T051 physical-phone QR acceptance remain unperformed and unchecked;
  no full-feature acceptance is claimed. T049 and all later tasks remain outside this execution.

- [ ] T049 [US3] Manually verify `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` and `Controllers/PublicMenuController.cs` signed out: two-table context, EGP precision, unavailable filtering in returned data, restore availability/changed price on next reload, empty/all-unavailable menu, malformed/unknown links and safe read failures; record outcomes here (after T048 with sample data; FR-006/007/008/010/011, SC-003/006/007/008).
- [ ] T050 [US3] Check `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` at 360/390/1280px for readable names/prices, usable category controls, focus/keyboard access and zero horizontal page scrolling; record responsive results here without adding an automated suite (after T049; FR-007, SC-004).

## Phase 6: Polish & Cross-Cutting Validation

**Goal**: Validate the complete feature, circuit recovery, preservation and publish readiness.
No unrelated cleanup, speculative optimization or additional business features.

- [ ] T051 Scan both table QR codes from a real signed-out phone using the verified reachable HTTPS origin configured through `CafeManagement.Api/Application/PublicMenuOptions.cs`; open `Components/Pages/Menu/TableMenu.razor`, rename one table using `Components/Pages/Admin/Tables.razor` and rescan its prior QR, confirming correct table/no login; record SC-002/003 outcomes or exact connectivity blockers here (after T043/T050; FR-006/007).
- [ ] T052 Add the minimal .NET 8 reconnect status UI in `CafeManagement.Api/Components/Shared/ReconnectStatus.razor` and wire it in `Components/App.razor`; review the three admin page save handlers and `Components/Pages/Menu/TableMenu.razor` for disconnect feedback, disabled/guarded submits, no automatic write replay and record reload after an uncertain save; manually interrupt/reconnect the browser and record findings (after all stories; FR-004/008).
- [ ] T053 Review `CafeManagement.Api/Application/StaffAccess.cs`, `Infrastructure/Identity/IdentityRevalidatingAuthenticationStateProvider.cs`, `Program.cs` and account pages against `contracts/api.md`: verify API and component authorization, current-user/stamp/role checks, token rejection, cookie flags, safe redirects/errors and full-navigation identity changes; use local controlled stamp invalidation/two-tab checks without claiming ordinary logout instantly revokes every circuit (after T052; FR-001/008, SC-005/006).
- [ ] T054 Validate SQL/data/circuit behavior through `CafeManagement.Api/Infrastructure/Persistence/CafeDbContext.cs`, entity configurations and migration: simultaneous duplicate-label requests yield one accepted write/one conflict, FK/price/name constraints hold, independent tabs do not share tracked state, and restart retains all values/codes; record manual results here without touching unrelated data (after T053; FR-004/006/011).
- [ ] T055 Run `dotnet build CafeManagement.sln -c Release` and `dotnet publish CafeManagement.Api -c Release --no-restore -o artifacts/cafe`; inspect published local Bootstrap assets and compiled pages, recheck original `/WeatherForecast`/Swagger/ports plus direct menu/admin paths and unknown `/api/*` responses, and confirm `.gitignore` excludes output/secrets; record evidence here without deploying (after T054; Constitution II/VI).
- [ ] T056 Review all acceptance scenarios in `specs/001-menu-table-management/spec.md` against this file's evidence and `quickstart.md`; confirm FR-001–FR-011/SC-001–SC-008 coverage, no deletion/deactivation/orders/POS/payments/reports endpoints/screens, no unnecessary generated tests, and no unapproved source removal; summarize completed/blocked/skipped validations in `specs/001-menu-table-management/tasks.md` for user review, never mark blocked checks passed (after T055).

## Dependencies & Execution Order

### Phase Dependencies

```text
Phase 1 Setup (T001–T005)
  -> Phase 2 Foundation (T006–T024)
     -> US1 Menu management (T025–T035)
     -> US2 Table/QR management (T036–T043)
     -> US3 Anonymous menu (T044–T050)
        -> Phase 6 Full QR integration/recovery/security/publish (T051–T056)
```

Default execution is the displayed sequential order. US1 provides sample menu records and US2
provides sample tables for US3's easiest manual review. US3 logic depends on shared schema,
not admin screens: a reviewed disposable sample dataset can independently exercise its journey.
US2's complete scan acceptance requires US3; its earlier checkpoint verifies management and
stable QR destinations only. T051 is the full integration gate, avoiding a false independence claim.

### Intra-task and Shared-File Dependencies

- Phase 1: T001 first; T002 finishes before T003; T004 can overlap package setup; T005 joins them.
- Foundation: model tasks precede configuration/context/migration. Only a verified dedicated DB
  permits T012; bootstrap follows. Shared Program.cs changes in T010/T013/T020/T021 are serial.
- T019 requires authentication-state support and HTTP account pages; T024 blocks every story.
- Within a story: contracts -> services -> controllers/components -> registration/build -> review.
  The explicit per-task prerequisites define any safe overlap, not the `[P]` marker alone.
- Serialise Program.cs/AdminLayout/Routes changes (T033/T041/T048 and final integration), migration
  operations and database writes that might interfere with a manual acceptance scenario.
- All stories must finish their checkpoints before T051–T056, which run sequentially.

### Parallel Opportunities

`[P]` means separate files at a dependency-ready boundary, not permission to run all marked work
at once. No cross-story parallel changes to shared startup/layout/services. Domain schema and
foundation remain shared integration work; tasks marked parallel still need their listed inputs.

## Parallel Example: User Story 1

After T024, T025 category contracts and T026 product contracts use different files. After menu
services are ready, T029/T030 controller files can be prepared alongside T031/T032 component
files. T027/T028 both edit MenuService.cs and must remain sequential; T033 is their join/build.

## Parallel Example: User Story 2

After T024, T036 table contracts can be drafted independently of US1 page work. Within US2,
T039 controller and T040 table component use separate files after T038. Their shared registration
in T041 waits for both and is serialized with other Program.cs changes.

## Parallel Example: User Story 3

T044 public projection can be drafted after T024. After T045, T046 public controller and T047
menu component/layout are separate-file work; T048 joins them. Availability and QR integration
reviews wait until sample menu/table data exists and the relevant changes are registered.

## Implementation Strategy

### MVP First (User Story 1)

Complete setup/foundation and US1; demonstrate Admin login, category/product editing and durable
EGP/availability data before spending time on QR/UI polish. This is a useful first increment,
not completion of the requested feature. No deployment is implied by that demonstration.

### Incremental Delivery

Add US2 table/QR management, then US3 anonymous menu, then the real-phone integrated QR journey.
Use each checkpoint to detect integration/auth/DI problems early. Keep the overall three-day
constraint visible and report schedule pressure rather than deleting security controls or adding
unrequested features. No fixed completion-time promise is inferred from the task count.

### Boundaries and Blockers

No unanswered business decision blocks these tasks. Execution still requires usable serviced
.NET8/package restores, a verified dedicated SQL target/permissions, local bootstrap secrets,
and a trusted phone-reachable HTTPS origin with functioning circuits. These are checks, not
conditions proven by planning. Record any unresolved prerequisite as blocked without performing
unrelated machine-wide changes. The documented .NET8 support-lifecycle risk remains in plan.md.

Only `tasks.md` is generated now; no source, migrations, packages, accounts, databases, validation
outputs or other planning artifacts are created/modified by this command. Proceeding to execution
requires user review. Suggested next review command is `$speckit-analyze`, not automatic implementation.
