# Implementation Plan: Cash Payments & Billing

**Updated**:2026-10-30. User-authorized in-place scope revision; no feature restart.

## Technical context

Existing ASP.NET Core8 controllers, Blazor/Bootstrap, EF Core/SQL Server and Identity. No packages, new infrastructure or migrations. Current API supports authorized cash and capability-scoped payment status only.

## Constitution check

PASS: preserve code in verified archive before runtime removal; retain immutable schema, accounts and records; keep server authorization, exact money, locking, historical bills, privacy and responsive controls. The user's explicit cash-only decision supersedes the historical online-payment scope; governing principles remain applicable if online payments are ever separately reauthorized.

## Design and sequence

1. Archive exact ten affected source files, unfinished tracked diff and prior Feature004 artifacts under archive/paymob-20261030; verify SHA-256 manifest before removal.
2. Restore previously validated cash-only controller/panel/Program portions while retaining current application features. Remove Paymob client/service/webhook/return/options from compiled sources after archiving. Remove provider-only action contracts and PaymentService options dependency.
3. Keep PaymentStatus.OnlineAvailable for API compatibility but always return false. Show cash-at-counter/pickup instructions. Preserve historical sandbox projection/revenue exclusion and pending guards; never alter database records or constraints.
4. Revise specifications, contracts, research, task dispositions, setup and handoff. Preserve task IDs. T009–T013 become removed, never passed. T014 is cash-only billing and can complete based on prior cash-bill evidence plus scoped validation.
5. Build Debug/Release and publish fresh inactive output. On an isolated loopback validation host verify removed routes404, current public/status/cash authorization and no checkout UI/provider dependencies. Reuse existing successful cash/ordering/report evidence. No provider, phone, firewall, certificate or public-exposure action.
6. Review staged contents, secret patterns and archive hashes; commit/push verified scope change to origin/master.

## Delivery status

Original total169 contains five removed provider tasks. Active original tasks164; after cash-only T014 acceptance160 complete and four real-phone/final-acceptance tasks remain pending. Do not claim original169/169 or physical acceptance.
