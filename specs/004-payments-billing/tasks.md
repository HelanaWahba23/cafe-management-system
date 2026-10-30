# Current scope disposition —2026-10-30

Cash-only delivery approved by the user. T009–T013 are removed, never passed; T014 is limited to cash billing plus safe runtime removal. Historical checkpoint notes below are superseded by this scope decision. Existing task IDs are preserved; original task descriptions/artifacts are archived. Active payment tasks11; historical provider tasks removed5. Physical-phone acceptance in Features001/002 remains pending.

# Tasks: Payments & Billing

**Input**:spec.md/plan.md/research.md/data-model.md/contracts/quickstart.md. Manual targeted diagnostics; no default automated test suite.

## Phase1 Setup

- [x] T001 Record approved provider/test mode/safety decisions and passing16-item requirement checklist in specs/004-payments-billing/spec.md and checklists/requirements.md (all FR).
- [x] T002 Complete official provider and local race research, plan/contracts/data model in specs/004-payments-billing/plan.md and research.md; analyze coverage before code (all FR/SC).

## Phase2 Foundation

- [x] T003 Add CafeManagement.Api/Domain/PaymentAttempt.cs, PaymentSettlement.cs and Order.PaymentRevision with exact money, restrictive FKs and unique receipt/provider/blocking-attempt constraints in Infrastructure/Persistence/Configurations/PaymentConfiguration.cs (FR-002/003/006/009).
- [x] T004 Generate/review additive payment migration in CafeManagement.Api/Infrastructure/Persistence/Migrations/; apply only owned copy and verify original-column/account/QR preservation, record specs/004-payments-billing/validation.md (afterT003; FR-009).

## Phase3 US1 Counter cash

Independent acceptance:full saved amount once, actor/time, stale/duplicate/access/rejected/pending conflict rules.

- [x] T005 [US1] Add Contracts/PaymentContracts.cs and Application/PaymentService.cs with fresh Cashier access, order-first update locks, rowversion check and atomic one-per-order cash receipt (afterT004; FR-001/002/006/007).
- [x] T006 [US1] Add protected cash/status HTTP routes in Controllers/PaymentsController.cs, Program.cs registration and payment-aware rejection in Application/PosOrderService.cs using same order-first transaction discipline (afterT005; FR-001/002/007).
- [x] T007 [US1] Add explicit cash confirmation/status to Components/Pages/Pos/OrderDetail.razor, busy/reconnect and stale reload behavior (afterT006; FR-008/009).
- [x] T008 [US1] Validate cash once/repeat/concurrent, actor/amount, unauthorized/CSRF, rejected/paid/pending blocks and unchanged fulfillment edges; record specs/004-payments-billing/validation.md (afterT007; SC-001/004).

## Phase4 US2 Online sandbox — removed from scope

Historical provider acceptance is cancelled by the approved cash-only scope; none of these tasks is counted as passed.

- ~~T009 [US2] Implement fail-closed sandbox config and official HTTP/HMAC protocol in Application/PaymobOptions.cs and PaymobClient.cs; built-in HttpClient/DataProtection only, no secrets/logged card data (afterT004; FR-003/004/005/010).~~ **REMOVED FROM SCOPE 2026-10-30, not passed.** Original requirement retained in archive/paymob-20261030/feature004-before-scope-change/tasks.md.
- ~~T010 [US2] Implement stable blocking attempt initiation/recovery/shared verified settlement in Application/PaymobPaymentService.cs; exact signed ID/amount/EGP/test integration, no blind uncertain POST replay or regression (afterT009; FR-003–007).~~ **REMOVED FROM SCOPE 2026-10-30, not passed.** Original requirement retained in archive/paymob-20261030/feature004-before-scope-change/tasks.md.
- ~~T011 [US2] Add capability-scoped start/status/reconcile and strictly verified provider callback in Controllers/PaymentsController.cs, PaymobWebhookController.cs and generic Components/Pages/Payments/Return.razor; no redirect settlement (afterT010; FR-001/003–006).~~ **REMOVED FROM SCOPE 2026-10-30, not passed.** Original requirement retained in archive/paymob-20261030/feature004-before-scope-change/tasks.md.
- ~~T012 [US2] Add customer private sandbox checkout/status actions in Components/Shared/PaymentPanel.razor and Pages/Orders/TrackOrder.razor; explicit initiation, no-key unavailable and test labels (afterT011; FR-008/009/010).~~ **REMOVED FROM SCOPE 2026-10-30, not passed.** Original requirement retained in archive/paymob-20261030/feature004-before-scope-change/tasks.md.
- ~~T013 [US2] Validate cryptographic/input/authorization/duplicate/mismatch handling separately from actual Paymob sandbox success/failure/inquiry/webhook; keep actual unperformed provider checks pending in specs/004-payments-billing/validation.md (afterT012; SC-002/003/004).~~ **REMOVED FROM SCOPE 2026-10-30, not passed.** Original requirement retained in archive/paymob-20261030/feature004-before-scope-change/tasks.md.

## Phase5 US3 Billing and integration

Independent acceptance:historical itemization unchanged, truthful method/state/time and explicit sandbox separation.

- [x] T014 [US3] Verify cash-only historical customer/staff bills, privacy, method/time, responsive keyboard UI and online route/control removal; preserve archived provider source and database history. Reuse accepted cash evidence and record scoped validation in specs/004-payments-billing/validation.md (FR-001–010, SC-001–005).
- [x] T015 Run restore/build/Release publish, security/diff and full preservation checks; update specs/004-payments-billing/quickstart.md and implementation-handoff.md with exact external setup/manual blockers (after independent implemented batches; FR-009/010, SC-005).
- [x] T016 Commit/push only validated payment batches, preserving unfinished provider/native UI and original Feature002 files; record milestone in specs/004-payments-billing/implementation-handoff.md and continue Feature005 despite T013 external blockers (FR-009/010).

## Dependencies / implementation strategy

Setup→foundation→US1, then US2, while US3 cash billing can proceed independently once US1 passes. Provider actual acceptance is an external gate, not a blocker for cash/reporting. US1 MVP continues automatically into all remaining authorized work. No parallel file edits; read-only research may run independently of code under plan skill. Unique filtered blocking slot and order-first update locks supplement rowversion; early callbacks without a saved provider-order binding produce no settlement and require retry/inquiry. No source/task completion from build alone.

## Resume verification — 30 October 2026

Verified local HEAD and remote master at `d94c845`. Re-read exact remaining requirements and superseding handoff. Project-specific User Secrets presence-only check confirmed all seven Paymob keys absent; no values were exposed. Official provider protocol review found and corrected inquiry authentication to include JSON `auth_token` for both saved-order and merchant-reference branches. Four intercepted HTTP requests passed; no network/provider/database operation occurred. Complete working-tree Release build passed with zero warnings/errors after the correction.

See [external acceptance setup](../../EXTERNAL-ACCEPTANCE.md) for credential entry, approval-gated temporary HTTPS, Android steps, provider failure guidance and per-task evidence. All existing unchecked markers remain pending: no phone or genuine Paymob acceptance was performed. Preserve the seven unfinished provider paths uncommitted, including the locally validated inquiry correction, until provider acceptance. No hosts, database records, credentials, firewall rules or certificate trust were changed.

Portfolio resume: counts and unchecked acceptance markers remain unchanged. See DEPLOYMENT.md for completed Debug/Release/publish, advisory audit and15 read-only HTTP checks, PAYMENTS-ACCESS.md for unresolved individual sandbox eligibility, and EXTERNAL-ACCEPTANCE.md for the real Android operator steps. No actual phone or provider acceptance has been performed. Continue from these specific gates rather than restarting completed features.

Final portfolio acceptance2026-10-30: approved mobile emulation and final review passed; total164/164 active original tasks complete,5 provider tasks removed. No Paymob/hardware test success is implied.
