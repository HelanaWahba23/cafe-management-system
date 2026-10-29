# Tasks: Payments & Billing

**Input**:spec.md/plan.md/research.md/data-model.md/contracts/quickstart.md. Manual targeted diagnostics; no default automated test suite.

## Phase1 Setup

- [x] T001 Record approved provider/test mode/safety decisions and passing16-item requirement checklist in specs/004-payments-billing/spec.md and checklists/requirements.md (all FR).
- [x] T002 Complete official provider and local race research, plan/contracts/data model in specs/004-payments-billing/plan.md and research.md; analyze coverage before code (all FR/SC).

## Phase2 Foundation

- [x] T003 Add CafeManagement.Api/Domain/PaymentAttempt.cs, PaymentSettlement.cs and Order.PaymentRevision with exact money, restrictive FKs and unique receipt/provider/blocking-attempt constraints in Infrastructure/Persistence/Configurations/PaymentConfiguration.cs (FR002/003/006/009).
- [x] T004 Generate/review additive payment migration in CafeManagement.Api/Infrastructure/Persistence/Migrations/; apply only owned copy and verify original-column/account/QR preservation, record specs/004-payments-billing/validation.md (afterT003; FR009).

## Phase3 US1 Counter cash

Independent acceptance:full saved amount once, actor/time, stale/duplicate/access/rejected/pending conflict rules.

- [x] T005 [US1] Add Contracts/PaymentContracts.cs and Application/PaymentService.cs with fresh Cashier access, order-first update locks, rowversion check and atomic one-per-order cash receipt (afterT004; FR001/002/006/007).
- [x] T006 [US1] Add protected cash/status HTTP routes in Controllers/PaymentsController.cs, Program.cs registration and payment-aware rejection in Application/PosOrderService.cs using same order-first transaction discipline (afterT005; FR001/002/007).
- [x] T007 [US1] Add explicit cash confirmation/status to Components/Pages/Pos/OrderDetail.razor, busy/reconnect and stale reload behavior (afterT006; FR008/009).
- [x] T008 [US1] Validate cash once/repeat/concurrent, actor/amount, unauthorized/CSRF, rejected/paid/pending blocks and unchanged fulfillment edges; record specs/004-payments-billing/validation.md (afterT007; SC001/004).

## Phase4 US2 Online sandbox

Independent acceptance:actual hosted sandbox success/failure, verified callback/inquiry and no duplicate effects. Missing keys/reachable origin leaves actual provider checks pending; continue US3/Feature005.

- [ ] T009 [US2] Implement fail-closed sandbox config and official HTTP/HMAC protocol in Application/PaymobOptions.cs and PaymobClient.cs; built-in HttpClient/DataProtection only, no secrets/logged card data (afterT004; FR003/004/005/010).
- [ ] T010 [US2] Implement stable blocking attempt initiation/recovery/shared verified settlement in Application/PaymobPaymentService.cs; exact signed ID/amount/EGP/test integration, no blind uncertain POST replay or regression (afterT009; FR003–007).
- [ ] T011 [US2] Add capability-scoped start/status/reconcile and strictly verified provider callback in Controllers/PaymentsController.cs, PaymobWebhookController.cs and generic Components/Pages/Payments/Return.razor; no redirect settlement (afterT010; FR001/003–006).
- [ ] T012 [US2] Add customer private sandbox checkout/status actions in Components/Shared/PaymentPanel.razor and Pages/Orders/TrackOrder.razor; explicit initiation, no-key unavailable and test labels (afterT011; FR008/009/010).
- [ ] T013 [US2] Validate cryptographic/input/authorization/duplicate/mismatch handling separately from actual Paymob sandbox success/failure/inquiry/webhook; keep actual unperformed provider checks pending in specs/004-payments-billing/validation.md (afterT012; SC002/003/004).

## Phase5 US3 Billing and integration

Independent acceptance:historical itemization unchanged, truthful method/state/time and explicit sandbox separation.

- [ ] T014 [US3] Integrate payment projections with customer and staff bill views in Components/Shared/PaymentPanel.razor, Pages/Pos/OrderDetail.razor and Pages/Orders/TrackOrder.razor; verify historical prices, privacy, sandbox classification and320/768/desktop keyboard UI in specs/004-payments-billing/validation.md (afterT008; provider controls afterT012; FR008/009, SC004/005).
- [x] T015 Run restore/build/Release publish, security/diff and full preservation checks; update specs/004-payments-billing/quickstart.md and implementation-handoff.md with exact external setup/manual blockers (after independent implemented batches; FR009/010, SC005).
- [ ] T016 Commit/push only validated payment batches, preserving unfinished provider/native UI and original Feature002 files; record milestone in specs/004-payments-billing/implementation-handoff.md and continue Feature005 despite T013 external blockers (FR009/010).

## Dependencies / implementation strategy

Setup→foundation→US1, then US2, while US3 cash billing can proceed independently once US1 passes. Provider actual acceptance is an external gate, not a blocker for cash/reporting. US1 MVP continues automatically into all remaining authorized work. No parallel file edits; read-only research may run independently of code under plan skill. Unique filtered blocking slot and order-first update locks supplement rowversion; early callbacks without a saved provider-order binding produce no settlement and require retry/inquiry. No source/task completion from build alone.
