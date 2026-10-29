# Cafe Management System

A single ASP.NET Core 8 application with Blazor Server interfaces, Bootstrap, EF Core, SQL Server and Identity. Customers order by table QR or pickup, cashiers manage fulfillment and record cash, and Admin reviews order activity and recorded cash.

## Current status — 29 October 2026

| Feature | Validated tasks | Status |
| --- | ---: | --- |
| Menu & Table Management | 54/56 | Implemented; physical-phone QR and final acceptance remain pending. |
| Customer Ordering | 61/63 | Dine-in, pickup, checkout, reconfirmation, duplicate recovery, private tracking and native navigation validated. Physical-phone and final acceptance remain pending. |
| Cashier & POS | 22/22 | Validated and pushed as `3a30dea`. |
| Payments & Billing | 10/16 | Validated cash ledger/UI pushed as `0b9038c`. Paymob sandbox implementation is prepared locally and unverified. |
| Admin Dashboard & Reports | 12/12 | Reports, responsive theme, builds, authorization and preservation validated; pushed as `8657b2e`. |

This is an implementation checkpoint, not a claim that all five features are accepted. The report/theme milestone is `8657b2e`; the customer UI continuation is recorded in its handoff. Read the implementation handoffs before resuming work or changing a running host.

## Run locally

Prerequisites: .NET 8 SDK and SQL Server with an existing or explicitly chosen development database. Use HTTPS; Identity and antiforgery cookies are secure cookies.

```powershell
dotnet tool restore
dotnet restore CafeManagement.sln

dotnet user-secrets set "ConnectionStrings:CafeDatabase" "<your SQL Server connection string>" --project CafeManagement.Api
dotnet user-secrets set "PublicWebBaseUrl" "https://localhost:7128" --project CafeManagement.Api
```

Keep credentials outside Git. Preserve the existing connection and accounts when resuming this workspace. Before applying migrations to an existing database, take and verify a current SQL backup and confirm the target. Never delete or reset the database.

```powershell
dotnet ef migrations list --project CafeManagement.Api
dotnet ef database update --project CafeManagement.Api
dotnet build CafeManagement.sln
dotnet run --project CafeManagement.Api --launch-profile https
```

The HTTPS profile listens on `https://localhost:7128`. `PublicWebBaseUrl` must match the actual trusted HTTPS origin. Check the local development certificate with `dotnet dev-certs https --check`; handle any certificate trust setup explicitly rather than bypassing a browser warning.

Entry points in the complete working tree:

- `/menu/pickup`: customer pickup menu, without a QR code.
- `/menu/t/{publicCode}`: dine-in menu from an existing table QR.
- `/orders/track#key=…`: private link generated after checkout. Keep the link private; its credential is never an order reference or table identifier.
- `/account/login`: staff sign-in.
- `/pos/orders`: Cashier queue, walk-ins, fulfillment and bills.
- `/admin/dashboard`: Admin order and cash reports.
- `/admin/categories`, `/admin/products`, `/admin/tables`: existing management.

Customer UI native navigation is now accepted and the dashboard is pushed. The pushed cash milestone deliberately excludes unverified Paymob endpoints. The existing `CafeManagement` development database received the reviewed `AddCounterOrders` and `AddPaymentLedger` migrations after a fresh COPY_ONLY/CHECKSUM backup and RESTORE VERIFYONLY. Prior row/account/QR fingerprints were preserved. For another environment, confirm the target and repeat backup/preservation before applying migrations.

## Staff accounts

Existing Admin and Cashier accounts are retained. No password, default account or secret is committed. On a fresh, explicitly chosen database only, the existing operator command can create a role-specific account:

```powershell
dotnet user-secrets set "Bootstrap:Email" "<staff email>" --project CafeManagement.Api
dotnet user-secrets set "Bootstrap:Password" "<staff password>" --project CafeManagement.Api
dotnet user-secrets set "Bootstrap:Role" "Admin" --project CafeManagement.Api
dotnet run --project CafeManagement.Api --launch-profile https -- --bootstrap-user
dotnet user-secrets remove "Bootstrap:Password" --project CafeManagement.Api
```

Use role `Cashier` for the cashier account. The command does not promote an existing account or replace its password. It does not run during ordinary startup.

## Workflows and boundaries

- Customer dine-in uses an existing table QR; pickup needs no QR. Online customer name and phone remain required.
- Cashier walk-ins support an existing dine-in table or takeaway without a table. Contacts are optional.
- The server validates availability, quantities and current prices. Price changes require explicit reconfirmation; saved line names/prices and table labels remain historical snapshots.
- Private customer access uses a random capability whose hash is stored on the server. Staff authority comes from current Identity membership and security-stamp checks.
- Fulfillment is Submitted → Preparing → Ready → Completed, or Submitted → Rejected with a reason. Paid orders cannot be rejected; pending/uncertain online attempts block rejection and cash until reconciliation.
- Full cash receipt records the saved EGP amount, cashier and UTC time once. Order/payment state changes share an order lock and concurrency version.
- Reports use order creation dates for counts and receipt recording dates for cash. Both are labelled UTC. Sandbox values are separated from real recorded-cash revenue.
- No delivery, live online payments, refunds, split/partial payments, invented taxes or advanced analytics are implemented.

## Paymob sandbox setup and remaining acceptance

Approved provider: Paymob Egypt, EGP, Hosted Unified Checkout, test/sandbox only. No successful Paymob transaction has been simulated or verified. The prepared online implementation remains local until actual sandbox acceptance.

Required local User Secrets keys:

- `Paymob:Enabled` (default false; do not enable before the integration is ready for sandbox validation).
- `Paymob:SecretKey`, `Paymob:PublicKey`, `Paymob:HmacSecret`, `Paymob:ApiKey`.
- `Paymob:IntegrationId`: a confirmed test card integration.
- `Paymob:CallbackBaseUrl`: an approved, publicly reachable, trusted HTTPS origin.

There is no separate Egypt sandbox hostname: test keys and test integration selection determine mode. The local implementation rejects live key prefixes and live/mismatched evidence. Paymob cannot call a loopback URL. No account, tunnel, firewall change or certificate bypass has been created automatically.

The proposed callback is `/api/payments/paymob/webhook`; `/payments/return` is informational and never settles a payment. Only backend-verified HMAC/authenticated inquiry matching the saved order, amount, EGP and test integration may settle. Duplicate notifications must not create another receipt.

Actual sandbox success, failure, interrupted initiation, reconciliation and duplicate callbacks remain pending. An unsuccessful transaction alone does not prove the hosted intention can no longer be paid; the implementation conservatively keeps such outcomes blocking until verified. Lost checkout-link and terminal-failure recovery usability must be confirmed with the real merchant sandbox before acceptance.

Official references: [Create Intention](https://developers.paymob.com/paymob-docs/intention-apis/create-intention), [HMAC verification](https://developers.paymob.com/paymob-docs/developers/webhook-callbacks-and-hmac/hmac/hmac-transaction-callback), [transaction inquiry](https://developers.paymob.com/paymob-docs/developers/transaction-inquiry-apis/by-order-id-or-reference).

## Validation and remaining manual checks

Build and publish:

```powershell
dotnet build CafeManagement.sln -c Release --no-restore
dotnet publish CafeManagement.Api -c Release --no-restore -o artifacts/cafe
```

No automated test framework was added. Focused temporary diagnostics used an owned database copy and removed only their recorded fixture rows. Evidence records exact successes and blockers:

- [Customer ordering evidence](specs/002-customer-ordering/end-to-end-validation.md).
- [Pending phone acceptance](specs/002-customer-ordering/pending-browser-validation.md).
- [POS evidence](specs/003-cashier-pos/validation.md).
- [Cash/payment evidence](specs/004-payments-billing/validation.md).
- [Dashboard handoff](specs/005-admin-dashboard-reports/implementation-handoff.md).

Still required: real-phone QR/rescan/pickup/private-link checks on an approved reachable HTTPS origin and actual Paymob sandbox setup/confirmation. Native Reload/Pickup Stay/Leave passed in isolated Edge. Local responsive emulation and cryptographic vectors do not replace the remaining external checks.

## Resume safely

Follow each feature's `implementation-handoff.md`, `tasks.md` and validation evidence. Preserve all local work. Never stage every file blindly: `Program.cs`, `PaymentsController.cs` and `PaymentPanel.razor` contain additional unverified Paymob changes beyond the pushed cash versions. Git history preserves the validated milestones; no force-push or database reset is needed.
