# Implementation Plan: Admin Dashboard & Reports

**Branch**:`master` | **Date**:2026-10-29 | **Spec**:[spec.md](spec.md)

## Summary

Add one read-only current-Admin report service/endpoint and page-local Blazor dashboard, then refine the existing shared Bootstrap cafe theme. Use grouped historical order/receipt queries, separate cash revenue from test sandbox values, preserve all menu/POS/customer flows.

## Technical Context

**Language/Version**:C#/.NET8.
**Primary Dependencies**:Existing Blazor InteractiveServer, Bootstrap5, Identity, EF Core8 SQLServer; no additions.
**Storage**:Existing Orders and PaymentSettlements; no new table/migration.
**Testing**:Focused manual SQL/service/HTTP/browser checks, builds/publish and preservation; no default test suite.
**Target Platform**:Existing HTTPS site,320/768/desktop browsers.
**Project Type**:Single existing web project.
**Performance Goals**:One grouped order query (at most20 status/type/origin groups) and grouped receipt-day/method query, max366-day range, no whole entity/contact materialization.
**Constraints**:Current Admin check at service and HTTP policy; no-store aggregate DTOs, exact decimal history, no fake/sandbox revenue, preserve pending native/phone/provider work and reconnect protection.
**Scale/Scope**:Basic counts, cash totals/daily breakdown, separate sandbox summary and shared presentation theme.

## Constitution Check

PASS before/after design:I one concrete service in existing project;II preserve all existing code/data;III fresh current Admin role/stamp check and no private fields;IV read immutable receipt values with exact decimal;V responsive accessible Bootstrap;VI targeted documented checks/no new framework tests;VII only approved basic reporting/theme, no delivery/exports/advanced analytics. No deviation.

## Project Structure

- Application/AdminReportService.cs
- Contracts/AdminReportContracts.cs
- Controllers/AdminReportsController.cs
- Components/Pages/Admin/Dashboard.razor
- Components/Layout/AdminLayout.razor, Pages/Account/Login.cshtml.cs
- wwwroot/css/site.css, focused presentation-only layout/page adjustments if demonstrated necessary
- README.md setup/status instructions and specs/005-admin-dashboard-reports/validation.md / implementation-handoff.md

## Design

DateOnly optional inputs default last7 UTC days. Validate inclusive range1..366days and safe exclusive endpoint. SQL groups orders byStatus/Type/Origin onCreatedAtUtc; receipts separately byRecordedAtUtc.Date/Method. Derive exact cash totals only fromCash; sandbox shown separately as tests. Do not use order-total/current-price joins for revenue or order-created dates for receipt filtering. Return all enum labels withzero counts. Catch failures safely; no database write.

Theme extends existing site.css while retaining reconnect overlay and data-reload-required selectors. Warm espresso/cream palette, system/serif heading fonts, consistent cards/buttons/focus, minimum44px touch controls and wrapping. Preserve existing public link behavior and static routing. Admin default redirects dashboard; local ReturnUrl/Cashier defaults remain.

## Complexity Tracking

No new abstractions, charts, migrations or packages.
