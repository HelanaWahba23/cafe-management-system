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

- [ ] T016 [P] Add `CafeManagement.Api/Controllers/AuthController.cs` with authenticated GET `/api/auth/me` returning email/roles and no-store GET `/api/auth/antiforgery` returning requestToken plus framework cookie; configure RequestVerificationToken consistently and keep authentication cookie HttpOnly; no public registration or JSON login endpoint (after T010; `contracts/api.md` session contract).
- [ ] T017 [P] Add `CafeManagement.Api/Pages/Account/Login.cshtml` and `Login.cshtml.cs` for ordinary HTTP GET/POST email/password login with antiforgery, generic credential/lockout failure, safe local returnUrl and full redirect after SignInManager cookie issuance; use Admin default `/admin/categories`, authenticated non-admin denial, and no client role selector/registration (after T013; FR-001).
- [ ] T018 [P] Add `CafeManagement.Api/Pages/Account/Logout.cshtml` and `Logout.cshtml.cs` for authenticated antiforgery-protected POST logout with cleared cookie and full redirect to login; forbid GET sign-out and do not perform cookie mutations from interactive handlers (after T013; FR-001).
- [ ] T019 [P] Add `CafeManagement.Api/Components/App.razor`, `Routes.razor`, `_Imports.razor` and `Layout/AdminLayout.razor` with local Bootstrap assets, .NET 8 component routing/cascading authentication, staff navigation and HTTP logout form; keep public routes outside Admin enforcement and leave account pages outside interactive rendering (after T004/T015/T017/T018; FR-001/007).
- [ ] T020 Integrate the host additively in `CafeManagement.Api/Program.cs`: Razor Pages, Razor components/Interactive Server, revalidating auth state, concrete staff/options services, static files, Identity cookie options and Admin policy; authentication then authorization then antiforgery; map controllers/pages/components without swallowing `/api/*`, existing `/WeatherForecast` or Swagger, and return 401/403 for APIs rather than HTML redirects (after T014–T019; FR-001/007/008).
- [ ] T021 Define only the minimal shared service failure/field-error shape in `CafeManagement.Api/Application/OperationError.cs` and HTTP mapping in `CafeManagement.Api/Contracts/ApiErrors.cs`, wiring safe ProblemDetails/validation responses in `CafeManagement.Api/Program.cs`; cover denied/not-found/invalid/conflict and generic 500 without secret disclosure, not a generic application framework (after T020; FR-001/004/008/011).
- [ ] T022 Finish accessible Bootstrap HTTP account presentation in `CafeManagement.Api/Pages/_ViewImports.cshtml`, `Pages/Account/Login.cshtml` and `Pages/Account/Logout.cshtml`; ensure full form posts include antiforgery and no enhanced same-circuit cookie interception, with readable errors and keyboard labels (after T017–T020; FR-001).
- [ ] T023 Build `CafeManagement.sln`, run its existing HTTPS profile from `CafeManagement.Api/Properties/launchSettings.json` and verify Swagger, `/WeatherForecast`, static CSS, HTTP account pages and component host coexist without runtime DI/options/route errors; record any real failure before adding feature services (after T021/T022; Constitution II/VI).
- [ ] T024 Manually verify `CafeManagement.Api/Pages/Account/Login.cshtml`, `Logout.cshtml` and `Controllers/AuthController.cs`: valid/invalid credentials, Admin/Cashier session roles, cookie flags, missing/invalid CSRF and logout/new HTTP principal; inspect service/circuit authorization lifetimes and record evidence here; management redirects are not accepted as complete feature screens until their story phases (after T023; FR-001 foundation checkpoint).

**Checkpoint**: Shared foundation builds and authentication works. Management authorization must
still be validated against real story endpoints/components in their subsequent checkpoints.

## Phase 3: User Story 1 — Maintain the Menu (Priority: P1, first useful increment)

**Goal**: Authorized Admin creates/edits categories/products, positive EGP prices and availability.
**Independent verification**: Two categories and two products can be saved, edited and reopened,
including unavailable Admin-visible products; no tables, customer ordering or payments needed.

- [ ] T025 [P] [US1] Define category request/response and editable view models in `CafeManagement.Api/Contracts/CategoryContracts.cs`: "enforce name length 1–120" after trim, "reject whitespace-only input" and "name duplicates allowed"; no editable ID/deactivation fields (after T024; FR-002/009/011).
- [ ] T026 [P] [US1] Define product request/response and editable view models in `CafeManagement.Api/Contracts/ProductContracts.cs`: required existing CategoryId, "enforce name length 1–120", duplicate names allowed, explicit IsAvailable, price "greater than zero", "at most two fractional digits" and "at most 9999999999999999.99"; HTTP price invariant string, service/UI decimal and response currency EGP/two places, no silent rounding (after T024; FR-003/010/011).
- [ ] T027 [US1] Implement category create/list/get/edit in `CafeManagement.Api/Application/MenuService.cs` using StaffAccess on every operation, per-operation disposed factory context, DTO projections/trim/whitespace validation and persisted duplicate-name allowance; no delete/deactivate methods (after T025/T021; FR-001/002/004/009/011).
- [ ] T028 [US1] Extend `CafeManagement.Api/Application/MenuService.cs` for product create/list/get/edit including unavailable products, required category FK validation, exact decimal scale/range and availability changes; validate before SaveChanges, preserve atomicity and map missing category/record and failure outcomes without partial changes (after T026/T027; FR-001/003/004/010/011).
- [ ] T029 [P] [US1] Add `CafeManagement.Api/Controllers/AdminCategoriesController.cs` with Admin-authorized GET list/item, POST and PUT routes under `/api/admin/categories`, explicit unsafe-request antiforgery, documented DTOs/statuses/Location and shared service mapping; no entity binding/removal endpoints (after T027; FR-001/002/004/009).
- [ ] T030 [P] [US1] Add `CafeManagement.Api/Controllers/AdminProductsController.cs` with the approved GET list/item, POST and PUT `/api/admin/products` contract, Admin policy/unsafe-request antiforgery and invariant price DTO mapping, showing all products only to Admin (after T028; FR-001/003/004/010/011).
- [ ] T031 [P] [US1] Add `CafeManagement.Api/Components/Pages/Admin/Categories.razor` with Admin route authorization, Interactive Server, Bootstrap list/Add/Edit EditForm, field errors and loading/empty/saving states; obtain trusted principal from AuthenticationStateProvider and call MenuService directly (after T027; FR-001/002/004/007).
- [ ] T032 [P] [US1] Add `CafeManagement.Api/Components/Pages/Admin/Products.razor` with Admin/Interactive Server, all-product list and form for name/category/decimal EGP price/availability, IDs distinguishing duplicate category names, retained input on failure and duplicate-submit prevention; no circuit-retained context or loopback HTTP (after T028; FR-001/003/004/010/011).
- [ ] T033 [US1] Register MenuService in `CafeManagement.Api/Program.cs` and confirm `CafeManagement.Api/Components/Layout/AdminLayout.razor` navigation reaches category/product pages; build `CafeManagement.sln` and manually exercise list/item/create/edit through both component and Swagger/controller paths (after T029–T032; FR-002/003/004).
- [ ] T034 [US1] Manually verify the input/security/save scenarios for `CafeManagement.Api/Components/Pages/Admin/Categories.razor`, `Products.razor` and `Controllers/AdminProductsController.cs`: EGP25.50, blank/whitespace/overlength, 0/negative/25.501 prices, duplicate names, nonexistent category, unavailable Admin visibility, denied anonymous/Cashier calls and failed-save feedback; record outcomes here (after T033; FR-001/004/008/010/011, SC-005/006/008).
- [ ] T035 [US1] Reopen edited categories/products, restart the host and verify persisted names/price/availability via `CafeManagement.Api/Components/Pages/Admin/Categories.razor` and `Products.razor`; record SC-001 and the applicable quickstart outcomes in this file before declaring the menu-management increment complete (after T034; FR-004, SC-001).

**Checkpoint**: First useful MVP is US1 administration, not the whole QR/customer feature.
All stories remain P1; sequence reflects implementation dependency, not a downgraded priority.

## Phase 4: User Story 2 — Manage Tables and Their QR Codes (Priority: P1)

**Goal**: Manage unique table labels and view a stable table-specific QR image/destination.
**Independent verification**: Table persistence/uniqueness and decoded QR destinations can be
checked without customer ordering or a populated menu. Full scan-to-menu acceptance intentionally
waits for US3 and T051; do not claim the customer QR journey complete before that integration.

- [ ] T036 [P] [US2] Define `CafeManagement.Api/Contracts/TableContracts.cs` for editable Label and read-only Id/PublicCode/menuUrl: "enforce ... label length 1–50 after trimming", nonblank, "Set NormalizedLabel from trimmed Label.ToUpperInvariant(); reject normalized length over 50" and immutable server-generated PublicCode; no client-editable normalization/code fields (after T024; FR-005/006/011).
- [ ] T037 [US2] Implement Admin-protected table list/get/create/edit in `CafeManagement.Api/Application/TableService.cs` with per-operation factory contexts, trimmed display label/invariant normalized unique key and immutable random GUID code; precheck labels and map index race to duplicate_table_label conflict, retry a code collision without assigning an existing table's code (after T036; FR-001/004/005/006/011).
- [ ] T038 [US2] Add `CafeManagement.Api/Application/TableQrService.cs` using QRCoder PngByteQRCode and StaffAccess, trusted configured HTTPS PublicWebBaseUrl plus `/menu/t/{publicCode}`, stable rendering after rename and repeated access, returning PNG bytes without stored images or unchecked Host-header URLs (after T037/T003; FR-001/005/006).
- [ ] T039 [US2] Add `CafeManagement.Api/Controllers/AdminTablesController.cs` with authorized GET list/item/QR PNG plus POST/PUT table routes, antiforgery for writes, proper 201/200/404 and duplicate-label 409 mapping; no DELETE/deactivation/QR-rotation routes (after T038; FR-001/005/006/009/011).
- [ ] T040 [US2] Add `CafeManagement.Api/Components/Pages/Admin/Tables.razor` with Admin/Interactive Server, Bootstrap list/create/edit form, field conflict messages and inline authorized QR PNG/menu link preview; trusted circuit principal calls services directly, no rotation/print/download workflow (after T038; FR-001/005/006/009/011).
- [ ] T041 [US2] Register table/QR services in `CafeManagement.Api/Program.cs`, connect the table page in `CafeManagement.Api/Components/Layout/AdminLayout.razor`, and build `CafeManagement.sln`; verify controller and component management agree on outputs/errors (after T039/T040; FR-005/006).
- [ ] T042 [US2] Manually review `CafeManagement.Api/Components/Pages/Admin/Tables.razor` and `Controllers/AdminTablesController.cs`: distinct labels, blank/overlength/duplicate/case-space variants, label-update conflict, unknown ID, Admin-only read/write/QR access and denied CSRF writes; verify no failed change persisted (after T041; FR-001/004/005/008/011, SC-005/008).
- [ ] T043 [US2] Generate QR images for two tables through `CafeManagement.Api/Application/TableQrService.cs`, decode/compare destinations, rename/restart/reopen and verify each code is stable and distinct with the correct table record; record the QR-management checkpoint here and leave full menu-scan acceptance to T051 (after T042; FR-004/005/006).

## Phase 5: User Story 3 — Browse the Menu on a Phone (Priority: P1)

**Goal**: Signed-out mobile customer browses the current available menu with correct table context.
**Independent verification**: Given two manually created sample tables/categories/products, no
staff session is needed for browsing; ordering/payment/POS services are unnecessary.

- [ ] T044 [P] [US3] Define the public projection in `CafeManagement.Api/Contracts/PublicMenuContracts.cs`: table label/publicCode, fixed EGP, category ID/name and available product ID/name/price only; omit internal table IDs, Identity/normalization and unavailable-product data, keeping empty category/product arrays explicit (after T024; FR-006/007/008/010/011).
- [ ] T045 [US3] Add `CafeManagement.Api/Application/PublicMenuService.cs` with anonymous per-operation factory reads, safe GUID lookup, correctly selected shared menu/table and server-side IsAvailable filter; return explicit invalid/unknown table/empty/read-failure outcomes, deterministic ID ordering, decimal EGP values and no side effects during initialization/prerender (after T044; FR-006/007/008/010/011).
- [ ] T046 [US3] Add `CafeManagement.Api/Controllers/PublicMenuController.cs` for anonymous GET `/api/menu/tables/{publicCode}`, no-store menu response, approved price-string projection and 400/404/problem responses; never invoke staff checks, return hidden data or choose a fallback table (after T045; FR-006/007/008/010).
- [ ] T047 [US3] Add `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` and `Components/Layout/PublicLayout.razor` with anonymous Interactive Server, direct public-service calls, visible table, Bootstrap category/product content and EGP two-place prices; implement loading/empty/invalid-table/read-error/retry-read states and keyboard/mobile usability without staff shell or login preflight (after T045; FR-006/007/008/010/011).
- [ ] T048 [US3] Register PublicMenuService in `CafeManagement.Api/Program.cs`, confirm anonymous menu deep links in `CafeManagement.Api/Components/Routes.razor` and build `CafeManagement.sln`; verify both public controller and direct component reads enforce identical filtering/table semantics (after T046/T047; FR-006/007/010).
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
