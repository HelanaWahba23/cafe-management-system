# Tasks: Cashier & POS

**Input**: spec.md, plan.md, research.md, data-model.md, contracts/, quickstart.md.
**Tests**: Targeted manual diagnostics only; no automated test project requested.

## Phase 1: Setup

- [x] T001 Inspect/preserve Git baseline and unfinished UI; record scope and confirmed decisions in specs/003-cashier-pos/spec.md and research.md (FR-010/011).
- [x] T002 Complete requirement checklist, plan, contracts and dependency review in specs/003-cashier-pos/checklists/requirements.md and plan.md (all FR/SC).

## Phase 2: Foundation

- [x] T003 Extend CafeManagement.Api/Domain/Order.cs, OrderOrigin.cs and Infrastructure/Persistence/Configurations/OrderConfiguration.cs with safe customer/counter origin, optional counter contacts and restrictive creator attribution (FR-005/007/010).
- [x] T004 Generate/review additive migration in CafeManagement.Api/Infrastructure/Persistence/Migrations/; validate on owned copy with original field/account/QR preservation, record specs/003-cashier-pos/validation.md (after T003; FR-010, SC-006).
- [x] T005 Add current Cashier checks in CafeManagement.Api/Application/StaffAccess.cs and policy/service registration in Program.cs, preserving Admin checks (FR-001).

## Phase 3: US1 incoming order management

**Independent acceptance**: guest dine-in/pickup share queue; all valid/invalid lifecycle edges, stale writes and authorization outcomes verified.

- [x] T006 [US1] Define bounded queue, saved detail and status DTOs in CafeManagement.Api/Contracts/PosOrderContracts.cs (after T004/T005; FR-002/003/004).
- [x] T007 [US1] Implement guarded queue/detail/status operations in CafeManagement.Api/Application/PosOrderService.cs, reuse OrderLifecycle and rowversion conflict handling (after T006; FR-001–004).
- [x] T008 [US1] Expose protected JSON/CSRF endpoints in CafeManagement.Api/Controllers/PosOrdersController.cs and no-store middleware in Program.cs (after T007; FR-001–004/010).
- [x] T009 [US1] Add CafeManagement.Api/Components/Layout/PosLayout.razor and Pages/Pos/Orders.razor, OrderDetail.razor with filter/refresh/paging, error/busy/reconnect and status controls (after T008; FR-002/003/004/009).
- [x] T010 [US1] Route Cashier login to POS in CafeManagement.Api/Pages/Account/Login.cshtml.cs while preserving Admin defaults/local-return validation (after T009; FR-001/010).
- [x] T011 [US1] Validate queue, all status pairs/reason boundaries, stale updates, guest tracking reflection and anonymous/Admin/revoked/CSRF denial; record specs/003-cashier-pos/validation.md (after T010; SC-001/002/004).

## Phase 4: US2 counter creation

**Independent acceptance**: dine-in and takeaway, optional contacts, price reconfirmation and duplicate recovery, no guest boundary regression.

- [x] T012 [US2] Add separate counter DTOs in CafeManagement.Api/Contracts/PosOrderContracts.cs; extend OrderContracts.cs only with internal optional-contact validation paths preserving guest defaults (after T011; FR-005/006/010).
- [x] T013 [US2] Add internal creator-bound review/placement entry points in CafeManagement.Api/Application/OrderService.cs using shared atomic acceptance/recovery; guest lookup filters Customer origin (after T012; FR-006/007/010).
- [x] T014 [US2] Add authorized counter catalog/tables/review/place operations to CafeManagement.Api/Application/PosOrderService.cs and Controllers/PosOrdersController.cs (after T013; FR-001/005–007).
- [x] T015 [US2] Add CafeManagement.Api/Components/Pages/Pos/NewOrder.razor with mode/table, optional contacts, quantity editor, review/explicit reconfirm, retained unchanged retry attempt and leave warning (after T014; FR-005–007/009).
- [x] T016 [US2] Validate both modes, missing contacts, price/availability changes, replay/changed intent, cross-cashier/guest isolation, exact totals and rollback; record specs/003-cashier-pos/validation.md (after T015; SC-001/003/004).

## Phase 5: US3 bills

**Independent acceptance**: bill remains identical after catalog edits and never claims settlement.

- [x] T017 [US3] Render saved staff bill in CafeManagement.Api/Components/Pages/Pos/OrderDetail.razor using existing historical projection only (after T011; FR-008/011).
- [x] T018 [US3] Validate immutable bill and no-settlement claim in specs/003-cashier-pos/validation.md (after T017; SC-003).

## Phase 6: Integration and polish

- [x] T019 Validate keyboard, empty/error/busy states and actual 320/768/desktop POS layouts; refine CafeManagement.Api/Components/Pages/Pos/ and Layout/PosLayout.razor; record any blocked native checks in specs/003-cashier-pos/validation.md (after T016/T018; FR-009, SC-005).
- [x] T020 Run restore/build, Release build/publish, security/diff review and final database/account/QR preservation; record specs/003-cashier-pos/validation.md (after T019; FR-001/010, SC-006).
- [x] T021 Update setup and accurate implementation handoff in specs/003-cashier-pos/quickstart.md and implementation-handoff.md, reconcile every FR/SC and leave unverified tasks pending (after T020; all FR/SC).
- [ ] T022 Commit/push only validated Feature 003 paths, preserving Feature 002 unfinished files; record commit in specs/003-cashier-pos/implementation-handoff.md (after validated batches; SC-006).

## Dependencies and parallel opportunities

Setup → foundation → US1 → US2; US3 depends only on US1. Final integration requires all three stories. Schema/auth work is reviewed before API work. Tasks sharing files execute sequentially. After US1, independent bill acceptance T017–T018 can run alongside counter design T012; no parallel edit of OrderDetail. MVP is US1, but user authorizes continuing through all stories and subsequent Features 004–005 automatically.

## Implementation strategy

Validate each coherent batch against its acceptance before marking complete or committing. Backend independent work may continue when browser-only acceptance is blocked, under explicit user authorization, while affected task stays pending. Do not claim any feature complete from source/build alone. Retain existing Feature 001/002 manual gaps separately.
