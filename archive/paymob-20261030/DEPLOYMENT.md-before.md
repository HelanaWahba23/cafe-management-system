# Portfolio delivery and isolated staging

The application is a single ASP.NET Core 8/Blazor Server/SQL Server project. Current accepted behavior and evidence are indexed in README.md and the feature validation documents. Online payments are sandbox-only and remain externally unaccepted. No hosting resources or public exposure have been created.

## Local validation

Use the existing configured database and accounts for ordinary development; do not bootstrap accounts or run migrations just to resume. Preserve local User Secrets. For a fresh environment, follow README.md and review the target database before applying migrations.

```powershell
dotnet restore CafeManagement.sln
dotnet build CafeManagement.sln --no-restore
dotnet build CafeManagement.sln -c Release --no-restore
dotnet publish CafeManagement.Api -c Release --no-build --no-restore -o artifacts/review-publish
dotnet list CafeManagement.sln package --vulnerable --include-transitive
```

Choose a fresh inactive output directory; never overwrite an active host's publish folder. Published Production hosts do not load local development User Secrets automatically. Supply secrets through the hosting platform's private configuration, with keys such as `ConnectionStrings__CafeDatabase` and `Paymob__SecretKey`; never put values in source, shell history or public deployment logs.

## Preferred staging plan — requires hosting approval

1. Use an approved isolated Windows/.NET HTTPS staging host with an independently selected SQL Server database and no access to the development database. Prefer a clean staging database and manually created demonstration menu/tables; do not copy real Identity accounts, customer data or password hashes into public staging. Apply only reviewed migrations to that selected database after backup/target confirmation. Never reset the existing database.
2. Provision separate staging Admin and Cashier accounts with the existing explicit bootstrap command, one role at a time. Keep initial passwords in private configuration and remove the bootstrap password afterward. Ordinary startup must not provision users. Do not publish a shared administrator password in the portfolio README.
3. Bind the app directly to a valid trusted HTTPS endpoint. Configure `PublicWebBaseUrl` to that exact origin. Set `Paymob:Enabled=false` until eligibility, credentials and callback reachability are confirmed. Configure the callback base only for the selected staging host. SQL access should be restricted to the application; database ports must not be exposed publicly.
4. Enable WebSockets for Blazor Server and verify the interactive circuit remains connected. Do not deploy only the static wwwroot directory. Prefer a single app instance for this portfolio. If a reverse proxy terminates TLS, configure trusted proxy forwarding before HTTPS redirection, secure cookies and rate limiting; the current source does not configure forwarded headers. Do not trust arbitrary forwarded headers or publish behind an unreviewed proxy configuration.
5. Preserve the hosting identity's Data Protection key ring across restarts/releases; it protects Identity cookies and the saved provider client secret. Restrict access to that identity and protect keys at rest. A new ephemeral key ring can invalidate existing cookies/protected checkout data. Follow [Microsoft's Data Protection guidance](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-8.0). Keep private keys outside Git and publish output.
6. In public staging use Production error handling and private configuration. Exclude request/query/body/SQL-sensitive logging; return URLs, HMACs, customer capabilities and provider responses must not reach public logs. Keep Swagger limited to local Development. Verify authorization, no-store/no-referrer behavior, valid TLS and externally reachable callbacks before creating any intention.
7. Record the actual commit/build, staging origin, database selection and accepted results. Retain a prior immutable publish for rollback; stop only verified owned processes. Do not roll back database migrations by deleting records or restoring over the existing database without a separate reviewed plan.

Hosting account signup, cost, deployment, proxy configuration and public exposure require approval. This plan does not claim an actual staging deployment or authorize a tunnel. A temporary tunnel to a **separate test host/database** is a fallback only after explicit approval; see EXTERNAL-ACCEPTANCE.md. Never point it at the current development host by default.

## Workflow validation and phone session

Reuse the recorded completed functional evidence for menu/table management, dine-in/pickup checkout and private tracking, Cashier fulfillment/cash receipt, Admin reports, responsive layout and native navigation. Repeat affected checks after new changes, not whole completed features on every resume.

The remaining genuine provider/Android checklist and evidence columns are in EXTERNAL-ACCEPTANCE.md. Physical-device checks require a real Android operator and approved reachable HTTPS. Desktop emulation cannot close those tasks. The local environment has no discovered `adb` executable; no connected phone was accessed. Keep those tasks unchecked until actual outcomes are recorded.

## Validation performed on 30 October 2026

- Spec Kit prerequisites loaded Feature 004 explicitly without modifying persisted feature selection. All five requirement checklists passed 16/16; no extension hook configuration was present.
- Full working-tree Debug and Release builds: zero warnings/errors. Release publish: `artifacts/portfolio-finalization-publish`, not deployed publicly.
- Current dependency vulnerability audit: no vulnerable packages reported by configured sources. This is an advisory check, not a comprehensive security certification.
- Targeted secret-pattern scan of197 versioned/pending source and documentation files found no private-key blocks, live Paymob keys or GitHub tokens. No values were printed; this is not a comprehensive credential-leak certification. `git diff --check` passed.
- Nine focused non-success inquiry-correlation checks passed without provider requests or database writes. The earlier four intercepted inquiry-authentication requests remain valid.
- Fifteen read-only HTTP/security checks passed against the new publish on loopback HTTPS port 7276, with Paymob disabled and the existing validation copy selected: public pickup/table/return documents, forged return query informational only, privacy headers, anonymous report/POS/cash denials, missing/invalid private capability denial, missing-CSRF initiation/reconciliation denial, unavailable callback and malformed JSON refusal. TLS validation remained enabled.
- The temporary host was stopped after verifying its PID, exact DLL and port. No existing host was stopped, no order/payment/catalog record was written, no migration/cleanup ran, and no firewall/certificate/tunnel changes occurred.

The inquiry-correlation and initiation-error UI fixes were made before the operator's subsequent instruction to leave the integration unchanged. They remain preserved with the unfinished provider work; no further provider source edits followed that instruction. The UI change compiled but its interactive behavior still needs acceptance. Provider source is not included in the documentation-only delivery commit.
