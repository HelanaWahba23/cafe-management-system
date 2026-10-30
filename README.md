# Cafe Management System

A portfolio cafe application built with ASP.NET Core8, Blazor Server, Bootstrap, EF Core, SQL Server and Identity.

Customers order dine-in through table QR or place pickup orders directly. Cashiers manage dine-in, pickup and walk-in orders, update fulfillment, view saved bills and record full cash receipts. Admin manages menu/tables and views order and cash reports.

## Current scope and acceptance —30 October2026

**Payment is cash at the cafe counter or on pickup. Online ordering remains available; online payment is removed. No Paymob account or keys are required.** All unfinished provider source/design is recoverably preserved in archive/paymob-20261030, outside compilation.

| Feature | Completed active tasks | Remaining |
| --- | ---: | --- |
| Menu & Tables |54/56|Real Android QR and final acceptance|
| Customer Ordering |61/63|Real Android journeys and final acceptance|
| Cashier & POS |22/22|None|
| Cash Payments & Billing |11/11|None; five original provider tasks removed, not passed|
| Dashboard & Reports |12/12|None|

**160/164 active tasks complete**, plus5 removed from the original169. Four real-phone/final-review tasks remain unperformed. This is not169/169 or a claim of phone acceptance. The approved decision is in requirements.md and specs/004-payments-billing; historical scope/documents remain archived.

## Run locally

Prerequisites: .NET8 SDK, SQL Server and a valid local HTTPS certificate. Preserve existing credentials, accounts and database. Existing workspace needs no migration or bootstrap for this scope change.

For a fresh explicitly chosen environment only:

```powershell
dotnet tool restore
dotnet restore CafeManagement.sln
dotnet user-secrets set "ConnectionStrings:CafeDatabase" "<your SQL Server connection string>" --project CafeManagement.Api
dotnet user-secrets set "PublicWebBaseUrl" "https://localhost:7128" --project CafeManagement.Api
```

Keep actual credentials outside Git/chat and prefer protected interactive/private configuration over command history. Before applying reviewed migrations to an existing selected database, take and verify a current backup and confirm the target. Never delete/reset the database.

```powershell
dotnet ef migrations list --project CafeManagement.Api
dotnet ef database update --project CafeManagement.Api
dotnet run --project CafeManagement.Api --launch-profile https
```

HTTPS profile listens on https://localhost:7128. PublicWebBaseUrl must match the actual trusted origin. Do not bypass browser certificate warnings or install trust automatically.

## Entry points

- `/menu/pickup`: direct customer pickup.
- `/menu/t/{publicCode}`: existing table QR dine-in menu.
- `/orders/track#key=…`: private tracking/bill link from checkout; keep it private.
- `/account/login`: staff sign-in.
- `/pos/orders`: Cashier queue, walk-ins, fulfillment and cash bills.
- `/admin/dashboard`: Admin order/cash reports.
- `/admin/categories`, `/admin/products`, `/admin/tables`: menu and table management.

Existing Admin/Cashier accounts are preserved. On a fresh selected database only, configure Bootstrap:Email, Bootstrap:Password and Bootstrap:Role (Admin or Cashier) privately, run `dotnet run --project CafeManagement.Api --launch-profile https -- --bootstrap-user`, then remove Bootstrap:Password. The command neither promotes an existing account nor replaces its password; ordinary startup does not provision users.

## Workflow rules

The server validates availability, quantities and current prices. Price changes require explicit reconfirmation; saved item names/prices/table labels remain historical. Online customer name/phone are required; Cashier walk-in contacts are optional.

Customer access is a private capability; table QR is not staff authority. Fulfillment is Submitted→Preparing→Ready→Completed, or Submitted→Rejected with a reason. Cash records one full saved EGP amount, cashier and UTC time. Paid orders cannot be rejected. Any preserved pending/uncertain historical attempt continues blocking cash/rejection; scope removal never force-unlocks data.

Reports separate creation-date order counts and receipt-date cash revenue, labelled UTC. Historical sandbox values remain excluded from cash revenue. There are no delivery, refunds, partial/split payments or invented taxes.

## Build, evidence and deployment

Debug/Release builds and cash-only Release publish passed with zero warnings/errors. Twelve scoped HTTPS route/privacy/authorization checks passed; prior cash/customer/POS/reporting and responsive checks are retained. See [deployment and validation](DEPLOYMENT.md), [payment evidence](specs/004-payments-billing/validation.md) and [handoff](IMPLEMENTATION-HANDOFF.md).

The current source exposes no online initiation/reconciliation, Paymob webhook or payment-return route. Old provider settings cannot enable these features. No payment-related schema/migration/data changes were made.

[Remaining Android acceptance](EXTERNAL-ACCEPTANCE.md) requires an approved reachable HTTPS test origin and a real phone operator. Public staging/tunnels/firewall/trust changes require approval; none was created. Prefer an isolated staging database and separate test staff accounts.
