# Quickstart and Manual Validation: Menu and Table Management

Future execution guide after implementation is authorized and completed. Blazor components,
Identity pages, DbContext, migrations and bootstrap mode do not exist yet. No setup/build/run
command below was executed in this planning revision; this is not proof of a working feature.

## Prerequisites

- Serviced .NET 8 SDK/runtime; compatible 8.0.x EF/Identity packages and local dotnet-ef tool.
- Reachable SQL Server and permission to create/use a dedicated CafeManagement database.
  A named SQL service was observed previously; version, connectivity and permissions unverified.
- Local HTTPS certificate; a trusted phone-reachable HTTPS origin for physical QR scanning.
- Standard server connectivity/circuit transport for Blazor Interactive Server. A proxy used
  later must support its persistent connections; no proxy/deployment configuration created here.
- Bootstrap static styles/assets and license pinned locally in wwwroot/lib/bootstrap during
  implementation, plus QRCoder package. No separate frontend runtime or asset build required.
- Operator-supplied connection and bootstrap secrets outside committed configuration.

Canonical design: [plan](plan.md), [research](research.md), [data model](data-model.md),
[API contract](contracts/api.md) and [UI contract](contracts/ui.md).

## Future configuration and setup

Use the repository root in PowerShell. Verify the installed SDK/runtime:

```powershell
dotnet --info
```

Supply local user-secrets or process environment configuration for ConnectionStrings:CafeDatabase,
PublicWebBaseUrl and Bootstrap:Email / Bootstrap:Password / Bootstrap:Role. The Bootstrap prefix
here names the existing operator-account bootstrap options, not the CSS library. Environment
key separators are double underscores, e.g. ConnectionStrings__CafeDatabase. Use role Admin
for the first account; a separate operator-provisioned Cashier may validate denied admin access.
Do not put actual credentials into saved commands, HTTP examples, screenshots or committed files.

### Setup phase configuration contract (T003)

The project now has a UserSecretsId, which identifies a local development secret store; it is
not a credential. No secret values are included in the repository or provisioned by setup.
Use `--project CafeManagement.Api` with `dotnet user-secrets` commands, or provide the process
environment keys below through your local secret tooling. Avoid saving real values in scripts
or shell history. User secrets are a development convenience, not an encrypted production vault.

| Configuration key | Environment key | Intended consumer |
| --- | --- | --- |
| ConnectionStrings:CafeDatabase | ConnectionStrings__CafeDatabase | Framework connection-string configuration; SQL registration in T010. |
| PublicWebBaseUrl | PublicWebBaseUrl | PublicMenuOptions bound from root server configuration. |
| Bootstrap:Email | Bootstrap__Email | BootstrapOptions, only for the explicit operator command. |
| Bootstrap:Password | Bootstrap__Password | BootstrapOptions; Identity enforces password policy when provisioning. |
| Bootstrap:Role | Bootstrap__Role | BootstrapOptions; Admin or Cashier, with no implicit role default. |

PublicWebBaseUrl must be an operator-controlled absolute HTTPS origin, optionally including a
port and trailing slash, without embedded credentials, an application path, query or fragment.
For desktop-only development the existing origin is `https://localhost:7128`; this does not
provide phone reachability. Never derive the QR origin from a request Host header. Structural
validation cannot prove operator trust, DNS reachability or certificate trust on a phone.

The options classes and validation are defined in T003. Binding/startup validation belongs to
the later approved host integration (T020), and bootstrap invocation remains T013. Setup does
not change normal startup, create accounts, connect to a database or expose feature routes.
Bootstrap CSS 5.3.8 is stored locally with its MIT license under wwwroot/lib/bootstrap; asset
serving and component references are wired in T019/T020 rather than during setup.

The previously observed named local service suggests `.\MSSQLSERVER01` as a possible Windows-
integrated local SQL host. This is not a verified connection. Use a dedicated database only.
Keep any certificate-trust exception strictly local; do not weaken external SQL/HTTPS security.

After implementation adds the required package references and local tool manifest:

```powershell
dotnet restore CafeManagement.sln
dotnet build CafeManagement.sln --no-restore
dotnet tool restore
dotnet ef migrations list --project CafeManagement.Api --startup-project CafeManagement.Api --context CafeDbContext
```

Inspect generated initial migration SQL and confirm the target database before applying it.
No migrations or database writes are part of planning. After approving the actual migration:

```powershell
dotnet ef database update --project CafeManagement.Api --startup-project CafeManagement.Api --context CafeDbContext
```

The planned operator-only bootstrap mode must create only missing roles and the supplied account,
then exit. It must not expose a web endpoint or silently reset passwords/promote existing users.
After implementing that mode and providing local bootstrap credentials:

```powershell
dotnet run --project CafeManagement.Api -- --bootstrap-user
```

Remove bootstrap credentials from the environment after provisioning. Normal startup must not
run operator bootstrap or automatically migrate an ordinary production database.

## Build, run and publish checks

The same .NET project builds API controllers, Razor components and HTTP account pages:

```powershell
dotnet build CafeManagement.sln -c Release
dotnet run --project CafeManagement.Api --launch-profile https
```

Use the existing https://localhost:7128 profile: /account/login, /admin/categories,
/admin/products, /admin/tables, /menu/t/{actual-public-code}, /swagger and /WeatherForecast.
One host serves everything. There is no second frontend process or development proxy. Component
routes resolve directly on refresh; unknown /api paths must not become HTML page fallbacks.

After successful local review, validate the .NET publish output without deploying it:

```powershell
dotnet publish CafeManagement.Api -c Release --no-restore -o artifacts/cafe
```

Review output for compiled application/static Bootstrap assets and absence of secrets. This is
an implementation-stage validation command, not an instruction executed now. Keep the existing
ignore rules for build/publish artifacts. Actual deployment requires separate authorization.

The public origin configured in PublicWebBaseUrl must be reachable and trusted by the phone;
localhost refers to the phone itself when scanned there. A phone demo may require separately
approved HTTPS binding/hosting/certificate configuration. Do not open firewalls or expose shared
services as a side effect of this guide. Real-phone connectivity includes the interactive circuit.

## Manual validation scenarios

These are acceptance/recovery checks, not tasks.md or mandatory automated tests.

| Review | Actions | Expected result |
| --- | --- | --- |
| Preserve API | Open Development Swagger; request /WeatherForecast before/after integration. | Existing five-forecast behavior remains; API/Blazor routing coexist. |
| Login/logout | GET /account/login; submit valid form, use admin screen, POST sign out. | HttpOnly cookie changes on HTTP responses; full navigation creates fresh circuit; no interactive cookie-write errors. |
| Staff roles | Try admin pages/API while anonymous and as Cashier. | Pages deny/redirect appropriately; /api returns 401/403 without HTML redirects; no protected service data/change. |
| HTTP antiforgery | Omit/alter account hidden token; obtain /api/auth/antiforgery and try JSON write with missing/wrong/valid RequestVerificationToken header and cookie. | Invalid pairing -> 400/no write; valid pairing plus authorized user permits valid write. |
| Direct-service access | Review component handlers/services and attempt forbidden UI operations with non-admin state. | Every management operation checks trusted principal and current Identity user/stamp/role, not only displayed controls. |
| Context lifetime | Open two admin tabs; perform repeated reads/edits and overlapping requests. | No circuit-retained DbContext, concurrent-context error or cross-user tracked state; SQL constraints still enforced. |
| Menu persistence | Create two categories/products; edit name/price/availability; reopen and restart host. | Accepted values retained; SC-001/FR-004. |
| Inputs | Try EGP25.50, duplicate category/product names; reject blank names/labels, zero/negative/25.501 prices and duplicate table label. | Approved values accepted; invalid unchanged; EGP formatted to two decimals; SC-008. |
| Table identity | Create Table 1/2, attempt case/space-variant duplicate; render both QR images, rename one and rescan old image. | Unique normalized labels/codes; old QR still opens renamed correct table; SC-002. |
| Anonymous menu | Scan each table from signed-out phone with no staff session. | Categories/available products/prices/table label visible, no login; SC-003. |
| Visibility | Mark product unavailable, reload menu and inspect public API response; reopen Admin view; restore availability. | Hidden publicly, still editable by Admin, restored next load; SC-007. |
| Empty/errors | All unavailable/empty category; malformed/unknown code; simulate local read/save failure. | Clear safe messages, no false save or incorrect table; SC-006. |
| Responsive | Browse at 360, 390 and 1280px; keyboard/focus through category controls/forms. | Readable Bootstrap layout, usable controls, no horizontal page scroll; SC-004. |
| Connection loss | Temporarily disconnect browser network during read/edit, then reconnect or reload. | Reconnect state visible, writes not replayed automatically; reload current saved values before another explicit save. |
| Auth changes | Login/logout with full navigation; review revalidation with another open tab and controlled local security-stamp invalidation. | Current page rebuilds identity; stale circuits do not bypass service checks. Ordinary other-tab logout is not claimed as instant global revocation. |
| Prerender/deep links | Direct-load and refresh admin/menu routes; observe data operations across prerender/reconnect. | Reads only during initialization, no duplicate writes or mistaken table; no route masking. |
| Scope | Review screens and Swagger. | No deletion/deactivation, ordering, payments, POS or reporting routes/screens; SC-007. |

Use only local/disposable records for failure checks. Do not stop shared SQL services or alter
unrelated databases. Desktop width checks do not replace a physical phone QR journey. Record
actual commands/results, failures and skipped checks honestly; no check has been executed now.

## Recovery and completion

Keep the Git baseline. Review migration rollback against a verified database; do not drop a
populated/shared database as routine cleanup. Interactive circuit loss can lose unsaved edits;
re-read persisted data rather than replaying uncertain mutations or claiming offline support.

Readiness requires successful .NET builds/publish validation and applicable manual outcomes.
No new automated tests by default; only explicit request or necessity to diagnose a specific
issue permits them, with reason recorded. Package/SQL/certificate/circuit issues must be reported
as blocked checks, never success. Plan stops before tasks, implementation or deployment.
