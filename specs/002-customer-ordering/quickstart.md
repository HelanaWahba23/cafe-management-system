# Quickstart and Manual Validation: Customer Ordering

**Status**: Future run/validation guide after authorized implementation. None of the commands,
migrations, database mutations or runtime checks below were executed by the planning command.
Do not treat this document as evidence of passed implementation acceptance.

## Prerequisites and preservation

- Existing Windows development environment with .NET 8 SDK, SQL Server, local EF tool and
  approved local configuration. Keep net8.0 and the current package baseline.
- Use the existing named database/Windows integrated authentication and user secrets already
  configured. Do not paste secret values, use password-bearing command arguments, run bootstrap
  account creation, reset the database, or change certificate/firewall/network settings.
- Before any later migration application, confirm the exact target and backup/restore readiness;
  record original category/product/table IDs, names/prices/availability and public QR codes,
  Identity account/role counts and preservation fingerprints without printing credential values.
- Prefer an explicitly owned isolated local validation database/copy for concurrent writes,
  lifecycle fixture preparation and failure injection. Preserve original domain/account data;
  never mutate original stamps/roles or rename original tables for an acceptance demonstration.
  If an isolated target cannot be safely prepared, stop and report affected checks pending.

## Feature selection

Existing active-feature metadata is intentionally preserved and still selects Feature 001.
Before later Spec Kit commands in this shell, select Feature 002 explicitly:

```powershell
$env:SPECIFY_FEATURE_DIRECTORY = 'specs/002-customer-ordering'
$env:SPECIFY_FEATURE_NO_PERSIST = '1'
```

This changes only process environment, not the approved spec, Git branch or feature.json.
Run all following project commands from the repository root. No tasks.md is created by planning.

## Build and additive migration review (later implementation only)

```powershell
dotnet --list-sdks
dotnet tool restore
dotnet restore CafeManagement.sln
dotnet build CafeManagement.sln --no-restore
dotnet ef migrations list --project CafeManagement.Api
```

After implementation has generated AddCustomerOrders, inspect its Up/Down/model snapshot and
review the SQL before application. Create ignored output directory if needed:

```powershell
New-Item -ItemType Directory -Path artifacts -Force | Out-Null
dotnet ef migrations script 20261027013745_InitialMenuAndIdentity AddCustomerOrders --project CafeManagement.Api --output artifacts/customer-orders.sql
```

Expected delta is two new order tables, their constraints/indexes/FKs and migration history entry.
Any unexpected existing data/table/Identity alteration, migration rewrite or destructive statement
blocks application. Confirm the selected local target through safe configuration inspection; do not
print connection credentials. Once separately authorized and reviewed:

```powershell
dotnet ef database update AddCustomerOrders --project CafeManagement.Api
dotnet run --project CafeManagement.Api --launch-profile https
```

Compare original preservation fingerprints after application. Do not invoke EnsureCreated,
EnsureDeleted, automatic startup migrations, bootstrap-user, downgrade or reseeding. Document
any migration failure and preserve database state; no automatic destructive repair.

## Manual workflow matrix

At the US2 checkpoint, verify browser helpers are loaded before first checkout review and open
each checkout-provided private link in a fresh signed-out browser, then reload the matching saved
order and check invalid/missing-link failure. This requires the minimal tracking page delivered
with US2, not later US4 refinements. Repeat with pickup at US3; US4 adds robustness, usability and
all-status/security validation. Reconnect handling remains a separate US2 integration check.

Use two existing tables and sample categories/products in the isolated validation target.
Include two distinct product IDs with the same name and at least one unavailable item.
Record actual outcomes, not only the intended checks.

| Check | Procedure and expected result | Spec coverage |
| --- | --- | --- |
| Catalog/cart | Signed-out table menu and /menu/pickup show available products only; add two, adjust quantities, remove one; same-name distinct products stay distinct; exact EGP/two-place totals | FR-001/002; SC-001 |
| Cart validation | Empty cart, zero/negative/fractional quantity, unknown/repeated IDs and extra reviewed prices rejected; no order or silent removal | FR-002/005; SC-001/003 |
| Dine-in | Submit two orders using distinct existing table codes; confirm correct visible table and saved association, Submitted status/private link; malformed/unknown code fails without pickup fallback | FR-003/007; SC-002/003 |
| Pickup | Fresh visit without QR, choose pickup and submit two orders; no table requested or persisted, same shared workflow | FR-004/008; SC-002 |
| Guest details | Both types require nonblank name/phone, give field feedback, accept formatted phone without registration/verification; overcapacity rejected without truncation; never exposed in catalog | FR-013; SC-008 |
| Price review | For both types change price upward and downward after cart review; show refreshed unit/line/total amounts, zero order until explicit confirmation; change price again before acceptance and repeat confirmation | FR-005/006; SC-004 |
| Authoritative amounts | Alter submitted price/total/status/type/table internals; wrong prices cause refresh/rejection, totals/status cannot be set, no underpriced order; accepted totals match DB prices and quantities | FR-005/006/010; SC-004 |
| Historical snapshots | After acceptance edit a sample product name/price and sample table label; saved order names/amounts/table snapshot remain, issued table code still resolves its existing table | FR-006/012; SC-004/007 |
| Availability | Make selected product unavailable before acceptance; reject entire submission, identify it, no partial order; public menu hides it on reload | FR-001/005; SC-001/003 |
| Recovery | Persist capability before placement; interrupt reply after commit; reload and explicitly check outcome to find exactly one saved order, even after catalog edits | FR-007; SC-003 |
| Private tracking | Copy link to a separate signed-out browser; view/reload order. Change only public reference, use table/name/phone alone or malformed bearer: no private details | FR-009/010/013; SC-005/008 |
| Responsive feedback | At 320/768/1280px verify cart/menu/category navigation, fields, confirmation/error/empty states, keyboard/touch access, tracking and copy/reload; no horizontal scrolling | FR-011; SC-006 |
| Preservation/scope | Verify original Admin/Cashier auth, categories/products/tables/QR, WeatherForecast, Swagger and unknown API JSON 404; no POS/payment/delivery/report/order-status mutation routes | FR-010/012; SC-007/008 |

## Concurrency, failure and lifecycle checks

1. Send two parallel placements with the same capability and immutable intent: one saved order,
   one new response and one replay/recovered outcome. Reuse accepted capability with changed
   quantity/customer/type/table: submission_conflict and unchanged original. Different credentials
   represent different explicit intentions, not global cart deduplication.
2. Hold a selected product row in an isolated Admin edit versus placement; confirm acceptance
   either uses the last confirmed authoritative amounts under lock or returns price_changed.
   No stale acceptance after a price edit slips into the read/save window. Repeat with availability.
3. Inject save failure/rollback in the isolated target: no order without its lines; no falsely
   successful response. Test numeric representability at line/total boundaries, rejecting overflow.
4. Stop/restart the local host with a pending save and with saved orders: reconnect gate locks
   writes, session capability survives same-tab reload, explicit outcome read returns saved data;
   missing result while a request may still commit stays inconclusive. No automatic POST/new token.
5. Two tabs have independent circuit carts; neither shares a tracked DbContext. Category anchors
   and opening/closing the embedded checkout panel retain the cart without full reload. New
   table/mode navigation warns before clearing the cart; no silent context reassignment.
6. On isolated prepared orders display Submitted, Preparing, Ready, Completed and Rejected with
   reason. Manually exercise the internal lifecycle rule through a disposable diagnostic harness
   if needed: allow only Submitted→Preparing, Preparing→Ready, Ready→Completed and
   Submitted→Rejected with nonblank reason; reject every other pair/reason omission. No committed
   test suite, production status setter, original-record mutation or new staff interface.
7. Verify rowversion detection for two isolated future-writer-style updates and SQL current-state/
   rejection/type/positive-money constraints. SQL checks do not prove transition history or summed
   totals; inspect service/domain enforcement and transaction alongside manual evidence.

## HTTP/security checks

- Use Swagger or a temporary in-memory/manual HTTP client; load existing anonymous antiforgery
  token/cookie first. New POSTs reject missing, invalid and mismatched pairs; a correct pair works.
  Ensure status/roles/internal order or table IDs/unknown placement fields are not honored;
  approved public product/category identifiers remain valid in their contracted inputs/outputs. Staff boundaries
  remain 401/403, and guest capabilities cannot call any Admin method.
- New API GET tracking with missing/invalid/unknown bearer has equivalent generic failure; public
  reference/phone/name/table code is not a lookup key. Responses show minimal snapshots and no
  customer contact fields, hashes, rowversions, Identity data or server exceptions.
- Inspect page/API headers for no-store and private-page no-referrer. Browser network URLs and
  server access logs contain no fragment credential; bearer/request-body logging is disabled or
  redacted. Inspect rendered/prerendered HTML for credential absence; never save real tokens to Git.
- Verify copied link uses trusted PublicWebBaseUrl, not spoofed Host. A complete shared private
  link grants intended access; partial links do not. Confirm reload after fragment removal works
  within the same tab, and storage failure blocks placement before an unrecoverable write.
- Exercise technical rate limiting and Retry-After without enabling public exposure or trusting
  arbitrary forwarded IP headers. Shared-NAT usability remains a tuning check, not a business quota.

## Build/publish and reporting

```powershell
dotnet build CafeManagement.sln -c Release
dotnet publish CafeManagement.Api -c Release --no-restore -o artifacts/cafe
```

Inspect ignored output for required local Bootstrap/JS and compiled pages; scan staged changes
and publish output for credentials, raw private links, certificates, DB files and generated tests.
Run the published build locally with existing approved configuration; no deployment implied.
Compare original data/account/QR fingerprints, stop owned validation processes, and report actual
results and any skipped/blocked checks. Clean up only explicitly owned disposable validation data.

Real-phone ordering/QR checks require securely reachable HTTPS and a trusted certificate. Until
that prerequisite is approved and available, record them pending; do not change firewall/certs or
mark Feature 001 T051/T056 complete. Desktop simulation cannot substitute for physical scanning.

See [API contract](contracts/api.md), [UI contract](contracts/ui.md) and
[data model](data-model.md) for exact interfaces and consistency rules. No automated tests are
required by default; a specific diagnosed bug or explicit user request is needed to add them.
