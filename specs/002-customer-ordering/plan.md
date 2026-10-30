## Approved acceptance-mode revision —2026-10-30

The user explicitly approved automated mobile-browser emulation instead of a physical Android device for this portfolio's remaining acceptance. References to phone scan/mobile journeys in acceptance scenarios are fulfilled by decoding the actual generated QR PNGs and navigating their URLs in a fresh signed-out touch/mobile browser context over trusted loopback HTTPS. Verify two table contexts, saved QR after label rename, direct pickup and private tracking/revisit/privacy. Use only the isolated validation database copy and preserve all original/copy records after exact fixture cleanup. This does not claim Android hardware, camera, physical Chrome or real-phone network acceptance. No public deployment/tunnel/certificate/firewall change is needed or authorized.

# Implementation Plan: Customer Ordering

**Branch**: Existing `master` retained; Spec Kit context is `002-customer-ordering`.
**Date**: 2026-10-27 | **Spec**: [spec.md](spec.md)
**Input**: Approved spec/clarifications, requirements.md, Constitution v1.0.1 and inspected
Feature 001 implementation. Planning only; no implementation authorized.

## Summary

Extend the existing host with guest cart/checkout and private tracking. Preserve table menu
routes and QR identifiers; add a no-table pickup entry. Thin controllers and Blazor components
call one concrete OrderService through the existing context factory. Add Orders and OrderLines
through an additive migration. Revalidate accepted prices, require explicit reconfirmation
when prices change, and deduplicate interrupted submissions with one private capability
generated before the write. No payment or staff POS operations.

## Technical Context

**Language/Version**: C# / .NET 8; existing net8.0 project, no major upgrade.
**Primary Dependencies**: Existing ASP.NET Core 8, Interactive Server, local Bootstrap 5.3.8,
EF Core SQL Server/Identity 8.0.33, QRCoder 1.8.1, Swashbuckle 6.4.0. No new package needed.
**Storage**: Existing SQL Server database and migration retained; additive order schema.
Circuit-scoped cart; minimal browser session recovery metadata. No persisted cart or queue.
**Testing**: Restore/build/Release publish and focused manual checks in quickstart.md.
No automated tests by default. No runtime checks performed during planning.
**Target Platform**: Existing HTTPS host and mobile/tablet/desktop browsers; preserve launch
profiles and trusted PublicWebBaseUrl. Real-phone connectivity remains a separate gate.
**Project Type**: One layered web application/API in the existing solution.
**Performance Goals**: Small portfolio review dataset at 320/768/1280px; no invented traffic,
latency or production uptime commitments. Short transactions; no circuit-held DbContext.
**Constraints**: Preserve records, migrations, accounts, authorization, existing API shapes
and table links. Exact EGP arithmetic. No delivery, payments, full POS or reporting.
**Scale/Scope**: One cafe, two order types, one shared lifecycle, manual reload-based tracking.

### Existing implementation findings

- Program.cs already registers the context factory, Identity, Admin policy, anonymous
  antiforgery-token endpoint, secure cookies and server components in the same host.
- PublicMenuService requires a valid table code and projects available products only.
  Preserve its existing contract and add a separate pickup projection sharing selection
  logic. An invalid table must never silently become pickup.
- TableMenu.razor uses /menu/t/{PublicCode} and full-navigation category anchors. Add shared
  menu rendering and same-document anchors so ordinary category browsing retains the cart.
  Preserve the latest-navigation-wins load guard and all existing table URLs. Routes.razor is
  static with page-level interactive roots: embed checkout in the menu's interactive root rather
  than assuming a scoped cart survives navigation to another page. Do not change global routing.
- MenuService creates contexts per operation; automatic SQL retry is not configured.
  Products have decimal(18,2) prices and no rowversion. Leave existing entity schemas intact.
- ReconnectStatus locks content until reload; extend its submit guard to checkout and recover
  using an explicit read afterward, never automatic order replay.
- Pre-existing Feature 001 documentation edits are outside this change. T051 and T056 remain
  unchecked; this plan does not claim Feature 001 complete.

## Constitution Check

Pre-research and post-design gates pass; no deviation requested.

| Principle | Design evidence / post-design result |
| --- | --- |
| I Maintainability | Same project; concrete services/DTOs/components; one factory context per operation. No repository/CQRS/messaging framework. Pass. |
| II Preservation | Additive schema/routes; unchanged Identity/QR behavior; migration review and preservation checks. Pass. |
| III Security | Private 256-bit capability, hash-only storage, server pricing and HTTP write protection; no staff privileges or payments. Pass. |
| IV Consistency | Atomic order/lines, short locking transaction, unique capability hash, saved prices and order rowversion. Pass. |
| V Mobile | Shared menu/cart/checkout/tracking at approved widths; direct pickup and table-specific dine-in. Pass. |
| VI Validation | Small later increments; build/manual checks; no mandatory new automated tests; report pending checks honestly. Pass. |
| VII Scope | Approved ordering/lifecycle/guest tracking only; future integrations documented without operations. Pass. |

The Constitution's starter-baseline description is historical; inspected code is the actual
implementation baseline. Its principles and requirements remain authoritative and unchanged.

## Project Structure

### Documentation (this feature)

```text
specs/002-customer-ordering/
  spec.md                         # approved, unchanged
  checklists/requirements.md      # unchanged, 16/16
  plan.md
  research.md
  data-model.md
  contracts/api.md
  contracts/ui.md
  quickstart.md
```

No tasks.md is generated. Preserve .specify/feature.json; select this feature with
SPECIFY_FEATURE_DIRECTORY for subsequent commands until a metadata update is authorized.

### Source Code (proposed paths; not created by planning)

```text
CafeManagement.Api/
  Domain/Order.cs, OrderLine.cs, OrderType.cs, OrderStatus.cs
  Application/OrderService.cs, CartState.cs, OrderLifecycle.cs
  Application/PublicMenuService.cs
  Contracts/OrderContracts.cs, PublicMenuContracts.cs
  Controllers/PublicOrdersController.cs, PublicMenuController.cs
  Infrastructure/Persistence/CafeDbContext.cs
  Infrastructure/Persistence/Configurations/OrderConfiguration.cs
  Infrastructure/Persistence/Configurations/OrderLineConfiguration.cs
  Infrastructure/Persistence/Migrations/<timestamp>_AddCustomerOrders.cs
  Components/Pages/Menu/TableMenu.razor, PickupMenu.razor
  Components/Pages/Orders/TrackOrder.razor
  Components/Shared/MenuCatalog.razor, CartPanel.razor, CheckoutPanel.razor
  Components/Layout/PublicLayout.razor
  Components/App.razor, wwwroot/js/order-session.js, wwwroot/js/reconnect-status.js
  wwwroot/css/site.css, Program.cs
```

**Structure Decision**: Continue existing layers. Components invoke services directly, not
self-HTTP calls forwarding staff cookies. Add no frontend/project/framework abstraction.

## Design and Execution Boundaries

1. Before later schema work, inventory the target DB and preservation fingerprints. Review one
   additive migration and its SQL; do not reset or rewrite existing records/migrations.
2. Define shared validation, review, atomic submission/recovery and minimal tracking. Controllers
   protect HTTP boundaries; components use identical service rules.
3. Add circuit cart, mobile menu/pickup, checkout/reconfirmation and private tracking. Limit JS
   to secure random capability/session storage, fragment handling and same-document navigation.
   Load browser helpers before checkout first uses them. Deliver the minimal functional tracking
   page with US2 so checkout links already open saved orders; US3 reuses it and US4 refines
   robustness, usability and complete status/security validation. Reconnect integration remains
   its own later US2 step; it is not responsible for initial helper loading.
4. Validate affected behavior/preservation and Release publish. These are design stages, not
   generated tasks or permission to implement now.

### Concurrency and recovery

Review is read-only. Final placement uses a short SQL RepeatableRead transaction: check existing
capability outcome first; resolve table/products; validate availability and reviewed prices;
calculate amounts; insert order/lines; commit. Read locks prevent Admin edits between final
validation and commit. Never wait for customer confirmation inside a transaction.
Price mismatch returns 409 with a refreshed review and zero inserts. Require another explicit
confirmation. Unique capability-hash index handles concurrent duplicates; after rollback use a
fresh context to recover the winner. No unconditional automatic write retry on uncertain commit.

### Future integrations

- POS later reads the single Orders set and uses OrderLifecycle plus rowversion for authorized
  transitions. Define rules now, but expose no mutation/list-all endpoint or staff order UI.
  Prepared isolated examples validate status displays and lifecycle rules.
- Payments later reference immutable order ID, EGP total and saved lines in separate records.
  No payment fields/provider/state placeholders or paid-success responses now. Fulfillment
  status never implies payment status.

## Risks and Readiness

- Sharing a private link grants access; a lost link cannot be recovered using name/phone.
  Protect headers/bodies/browser storage from logging and same-origin script compromise.
- SQL read locks can contend/deadlock with Admin edits. Keep transactions short, product reads
  ordered, and feedback safe; validate races without unconditional write retry.
- Storage can fail/be cleared. Block submission if the capability cannot be retained first;
  never replace it automatically after an uncertain outcome. Durable cart recovery is not promised.
- Validate exact-money overflow and finite field capacity, never round/truncate or add cafe limits.
  Proposed technical bounds are in data-model.md.
- Migration application needs the confirmed existing target, review and backup readiness during
  implementation. Planning performs no DB, certificate, firewall or network operations.
- Physical-phone checks need reachable trusted HTTPS. Existing T051/T056 remain pending.

**Blocking decisions**: None for planning; business clarifications are approved. Migration
safety, browser recovery and phone reachability are implementation/validation gates. Review
this plan before generating tasks.

## Complexity Tracking

No violations. Two new persisted entities, one concrete order service, circuit cart, small
lifecycle helper and browser credential bridge. No quote table, event bus, durable cart,
background worker, customer Identity account or payment abstraction.
