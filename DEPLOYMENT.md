# Deployment and validation — cash-only portfolio

Current delivery uses ASP.NET Core8, Blazor Server/Bootstrap, SQL Server/EF Core and Identity. Payments are cash only at the cafe counter or pickup; no provider credentials are required. No hosting resources or public exposure were created.

## Local run

Preserve existing User Secrets/database/accounts. Follow README.md for first setup only. Do not bootstrap accounts, reapply migrations or reset data when resuming this workspace.

```powershell
dotnet build CafeManagement.sln --no-restore
dotnet build CafeManagement.sln -c Release --no-restore
dotnet publish CafeManagement.Api -c Release --no-build --no-restore -o artifacts/new-inactive-publish
dotnet run --project CafeManagement.Api --launch-profile https
```

Choose a fresh output directory; never overwrite active host files. HTTPS profile uses localhost7128. Preserve the current database connection and PublicWebBaseUrl; no migration is needed for provider removal.

## Approved isolated staging plan

Use a separate staging database with demonstration data and separate Admin/Cashier accounts; do not expose the development database or copy real customer/Identity records. Confirm target and backup before additive migrations on a new chosen staging database. Use existing explicit bootstrap command only for this fresh selected database; keep passwords private and remove bootstrap password afterward.

Prefer a single directly hosted trusted HTTPS endpoint with WebSockets for Blazor Server. Set PublicWebBaseUrl to that exact origin. Production does not load development User Secrets automatically: supply ConnectionStrings__CafeDatabase privately through the platform. The app ignores old Paymob configuration; no online route can be reenabled with keys.

If using a TLS-terminating reverse proxy, configure and verify trusted proxy forwarding before secure cookies/HTTPS redirection/rate limiting; arbitrary forwarded headers are not trusted by the current source. Persist and protect the hosting identity's Data Protection key ring across restarts/releases for Identity cookies. Keep secrets/private keys outside Git and app publish output. Restrict SQL access to the application and do not expose database ports.

Keep Production error behavior and verify staff authorization/private capability access. Do not enable logging of passwords, cookies, private tracking links or customer request data. Verify publish/version, valid TLS, WebSockets and the public menus before Android testing. Hosting/cost/public exposure/tunnels still require explicit approval; no firewall or trust changes are automatic.

Retain immutable publish outputs for rollback. Stop only verified owned processes. Do not roll back database migrations by deleting records or restoring over the existing database without a reviewed plan.

## Validation evidence

On2026-10-30 the cash-only scope passed full Debug and Release builds (zero warnings/errors) and fresh Release publish to artifacts/cash-scope-20261030-publish. Twelve read-only HTTP checks passed on isolated loopback HTTPS7277 using the existing validation database copy: pickup/table/login, protected staff cash/status/reports, missing private capability, and all five removed online routes404 even with legacy Paymob Enabled=true. TLS validation stayed enabled. No database write/migration/cleanup was executed.

Prior accepted cash evidence covers26 focused safety checks, repeat/concurrent receipts, exact actor/time/EGP, rejection guards, historical bills and320/768/1280 UI. The cash panel/controller baseline is preserved; only cash guidance/provider dependencies changed. Prior customer/POS/reporting/native-navigation evidence remains applicable. No provider or phone result was fabricated.

The prior current dependency audit reported no vulnerable packages; no dependencies changed in this scope revision. This is not a complete security certification. Exact archive SHA-256 hashes and source/diff/secret checks must pass before commit. The user subsequently approved mobile-browser emulation instead of real Android acceptance;55 checks and final coverage review passed. Physical hardware/camera/network remain explicitly untested, not active blockers. See FINAL-ACCEPTANCE.md and EXTERNAL-ACCEPTANCE.md.
