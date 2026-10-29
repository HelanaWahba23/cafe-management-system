# Implementation Plan: Cashier & POS

**Branch**: `master` | **Date**: 2026-10-29 | **Spec**: [spec.md](spec.md)

## Summary

Extend existing orders with origin/creator, reuse authoritative order acceptance through internal counter entry points, and add a concrete authorized PosOrderService, protected JSON endpoints and page-local Blazor cashier interfaces.

## Technical Context

**Language/Version**: C# / .NET 8.
**Primary Dependencies**: Existing ASP.NET Core controllers, Blazor Interactive Server, Bootstrap, Identity and EF Core 8 SQL Server; no new packages.
**Storage**: Existing SQL Server. Additive migration only, default old rows to Customer; make contacts nullable with origin-dependent checks.
**Testing**: Builds, reviewed migration SQL, targeted local SQL/HTTP/browser diagnostics; no new automated test project.
**Target Platform**: Existing HTTPS web host, modern desktop and mobile browsers.
**Project Type**: Single existing web project.
**Performance Goals**: Bounded 50-order pages with stable newest-first ordering; no speculative throughput SLA.
**Constraints**: Preserve all records/accounts/QRs and four unfinished source files; fresh backend role/stamp checks; atomic exact EGP totals; no payment claim.
**Scale/Scope**: One cafe portfolio, queue/details, walk-in composer and per-order bill.

## Constitution Check

Pre-research and post-design gates PASS: I single-project concrete services; II no replacement/reset; III trusted-principal current Cashier checks plus CSRF JSON mutations; IV immutable snapshots, repeatable-read acceptance, rowversion writes and separate payment state; V responsive Bootstrap; VI targeted recorded validation without default automated tests; VII user-approved modes/contact choices, no delivery or invented fiscal rules. Existing Feature 002 phone/native checks remain pending.

## Project Structure

Documentation: spec.md, plan.md, research.md, data-model.md, contracts/api.md, contracts/ui.md, quickstart.md, tasks.md.

Source additions/changes:
- CafeManagement.Api/Domain/Order.cs and OrderOrigin.cs
- CafeManagement.Api/Infrastructure/Persistence/Configurations/OrderConfiguration.cs and Migrations/
- CafeManagement.Api/Application/StaffAccess.cs, OrderService.cs, PosOrderService.cs
- CafeManagement.Api/Contracts/OrderContracts.cs and PosOrderContracts.cs
- CafeManagement.Api/Controllers/PosOrdersController.cs
- CafeManagement.Api/Components/Layout/PosLayout.razor
- CafeManagement.Api/Components/Pages/Pos/Orders.razor, OrderDetail.razor, NewOrder.razor
- CafeManagement.Api/Program.cs and Pages/Account/Login.cshtml.cs

Reuse internal acceptance methods with explicit server-only creator context, never a deserialized origin/role. Guest DTO validation stays required by default. No generic repository or new architecture.

## Complexity Tracking

No constitutional deviations. Migration reviewed and validated on owned copy before any original schema application. Report blockers explicitly.
