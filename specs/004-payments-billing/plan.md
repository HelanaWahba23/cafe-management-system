# Implementation Plan: Payments & Billing

**Branch**:`master` | **Date**:2026-10-29 | **Spec**:[spec.md](spec.md)

## Summary

Add separate payment attempts and one-per-order settlements to the existing EF/SQL architecture. Implement authorized full cash recording/bills independently, then Paymob sandbox intention, signed callback and authenticated inquiry. Missing external setup blocks only actual provider acceptance, not cash/reporting.

## Technical Context

**Language/Version**:C#/.NET8.
**Primary Dependencies**:Existing ASP.NET Core controllers, Blazor/Bootstrap, Identity/EF Core8 SQL Server; built-in HttpClient, HMACSHA512 and Data Protection. No new packages.
**Storage**:Existing SQL Server, additive migration only. PaymentAttempt and PaymentSettlement tables, Order.PaymentRevision bigint touch column for rowversion concurrency.
**Testing**:Reviewed migration, targeted manual SQL/HTTP/browser diagnostics, builds/publish. Actual provider sandbox checks remain pending without keys/public callback; no simulated success or default test project.
**Target Platform**:Existing HTTPS web host.
**Project Type**:Single existing project.
**Performance Goals**:Short local transactions, bounded inquiry timeouts, no external calls within local transaction.
**Constraints**:One full receipt, exact decimal/checked integer piasters, current Cashier checks, private guest access, fixed Paymob Egypt origin, test-only key/method guards, no secret/raw callback logging.
**Scale/Scope**:One cafe, full cash or one sandbox provider payment, per-order invoice/bill; no refunds/partial/tax extensions.

## Constitution Check

PASS before and after design:single concrete services; preserve existing source/data; fresh server auth and private capability lookup; HMAC/authenticated inquiry only for payment success; unique/transactional payment effects and immutable totals; responsive interfaces; targeted honest validation; explicit approved provider/mode/safety rules. No deviations.

## Project Structure

- Domain/PaymentAttempt.cs, PaymentSettlement.cs and Order.PaymentRevision
- Infrastructure/Persistence/Configurations/PaymentConfiguration.cs and additive Migrations/
- Contracts/PaymentContracts.cs
- Application/PaymentService.cs (cash/status/shared settlement), PaymobOptions.cs, PaymobClient.cs, PaymobPaymentService.cs
- Controllers/PaymentsController.cs, PaymobWebhookController.cs
- Components/Shared/PaymentPanel.razor; integrate staff OrderDetail and private TrackOrder
- Program.cs registration and existing PosOrderService payment-aware rejection

## Transaction design

Order-first UPDLOCK,HOLDLOCK within short Serializable transactions for starting attempts, settling cash/provider and rejecting orders. Every payment-state mutation increments Order.PaymentRevision, forcing existing rowversion to change. Status rejection rechecks payment/attempt state within its transaction. Unique settlement OrderId prevents double settlement; unique provider transaction ID prevents assigning one payment twice. No external call inside a database transaction.

Persist Creating attempt/reference and commit before intention POST. An uncertain external response retains Uncertain and cannot automatically create another attempt. Persist returned provider order/id/encrypted client secret before exposing hosted link. Shared verified-settlement operation checks expected signed/authenticated fields and atomically records one receipt, increments order revision and marks attempt Succeeded. Verified failure is distinct from unknown; pending/unknown states continue to block cash/rejection. Duplicate accepted notifications return success without duplicate effects; conflicting evidence never overwrites receipts.

## Complexity Tracking

No violations. Data Protection is built-in and protects the intention client secret at rest; provider credentials remain UserSecrets/environment only. Do not add retries, provider packages or background reconciliation jobs.
