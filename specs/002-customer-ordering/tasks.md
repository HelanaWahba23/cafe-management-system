# Tasks: Customer Ordering

**Input**: Approved [spec.md](spec.md), [plan.md](plan.md), [research.md](research.md),
[data-model.md](data-model.md), [API contract](contracts/api.md), [UI contract](contracts/ui.md),
[quickstart.md](quickstart.md), requirements.md and Constitution v1.0.1.
**Created**: 2026-10-27. **Status**: T001–T008 validated; T009–T063 not started.
**Organization**: Setup → shared foundation → US1/US2/US3/US4 → cross-cutting validation.
**Validation policy**: Restore/build and manual checks; no generated automated tests by default.
Only an explicitly requested test or a concrete bug requiring diagnosis can justify an exception.

## Format and Path Conventions

Every task uses `- [ ] TNNN [P?] [USn?] Description`, explicit repository-relative paths,
prerequisites and FR/SC or Constitution mappings. Story labels apply only to story phases.
IDs restart for this feature; Feature 001's T051/T056 are different tasks and remain pending.

`[P]` means eligible to run alongside the named independent work **after all listed prerequisites
are complete**; it never overrides dependency gates. Never edit the same file in parallel.
Document outcomes under this file's Execution Evidence section when implementing later. Check
off a task only after its stated verification passes; a blocked/unperformed check stays unchecked.
Stop on missing prerequisites; preserve pre-existing uncommitted changes and do not stage them
implicitly. This file does not itself authorize implementation, commit, push or deployment.

Continue the existing ASP.NET Core 8 / Blazor Interactive Server / Bootstrap / SQL Server /
EF Core / Identity project. No Angular, extra projects, customer accounts, generic repository,
event bus, quote table, new package, payment scaffolding or staff POS UI. Existing QRCoder and
all issued table public codes remain unchanged. New persistence is additive only.

## Phase 1: Setup (T001–T003)

**Goal**: Establish an accurate baseline and safe implementation prerequisites without replacing
the existing app. This phase creates no replacement scaffolding.

- [X] T001 Review `specs/002-customer-ordering/spec.md`, all six planning artifacts, `requirements.md`, `.specify/memory/constitution.md`, `CafeManagement.Api/Program.cs` and current Git diff; confirm the 16/16 quality checklist and identify existing Feature 001 edits for preservation; record baseline and Feature 001 T051/T056 pending in `specs/002-customer-ordering/tasks.md` (no prerequisites; FR-012, Constitution II/VII).
- [X] T002 Verify .NET 8 SDK, existing dependencies in `CafeManagement.Api/CafeManagement.Api.csproj` and `.config/dotnet-tools.json`; run `dotnet tool restore`, `dotnet restore CafeManagement.sln` and `dotnet build CafeManagement.sln --no-restore` without framework/package upgrades; record results in `specs/002-customer-ordering/tasks.md` and stop on failure (after T001; FR-012, Constitution VI).
- [X] T003 Confirm the existing SQL target and backup/restore readiness using local configuration referenced by `CafeManagement.Api/Program.cs`; inventory the existing migration and capture non-secret preservation fingerprints for catalog/table/QR/Identity records; establish an explicitly owned isolated validation target before mutable/race/status checks and record safe target boundaries in `specs/002-customer-ordering/tasks.md`, never print credentials or mutate original accounts (after T002; FR-012, Constitution II/IV).

## Phase 2: Foundational (T004–T016)

**Goal**: Shared domain, persistence and HTTP/value contracts used by both order types.
**Gate**: All tasks in this phase must pass before story implementation. Migration application
requires confirmed target/review/backup readiness from T003, not an assumed disposable database.

- [X] T004 Create `CafeManagement.Api/Domain/OrderType.cs` and `CafeManagement.Api/Domain/OrderStatus.cs` with exactly `DineIn`, `Pickup` and `Submitted`, `Preparing`, `Ready`, `Completed`, `Rejected`; no payment or delivery enums; compile these declarations against net8.0 (after T003; FR-008/012).
- [X] T005 Create shared identity/access fields in `CafeManagement.Api/Domain/Order.cs`: Id "bigint identity primary key, internal; no customer authorization value"; Reference "uniqueidentifier, generated server-side, required/unique, display-only"; CapabilityHash "binary(32), required/unique SHA-256 digest; raw 32-byte token never stored"; IntentHash "binary(32), required canonical submission-intent fingerprint"; CreatedAtUtc "datetime2, server-generated UTC instant"; RowVersion "SQL rowversion, EF concurrency token for later legitimate writers"; expose no plaintext credential or Identity-user FK (after T004; FR-007/009/012).
- [X] T006 Complete `CafeManagement.Api/Domain/Order.cs` with Type "nvarchar(16), DineIn or Pickup only"; TableId "nullable int FK to CafeTables, delete restricted"; TableLabel "nullable nvarchar(120), accepted table-label snapshot"; CustomerName "nvarchar(120), required, trimmed/nonblank"; CustomerPhone "nvarchar(40), required, trimmed/nonblank; preserve entered formatting"; Currency "char(3), required, EGP only"; Total "decimal(18,2), required, positive; sum of saved line amounts"; Status "nvarchar(16), Submitted initially; Preparing, Ready, Completed, Rejected allowed"; RejectionReason "nullable nvarchar(500); required/nonblank iff Rejected" and order-line relationship (after T005; FR-003/004/006/008/013).
- [X] T007 Create `CafeManagement.Api/Domain/OrderLine.cs` with Id "bigint identity primary key"; OrderId "required bigint FK to Orders, delete restricted"; ProductId "required int FK to Products, delete restricted"; ProductName "required nonblank nvarchar(120), accepted snapshot"; Quantity "int > 0, whole number"; UnitPrice "decimal(18,2) > 0, accepted authoritative price"; LineTotal "decimal(18,2) > 0; exact UnitPrice × Quantity"; compile the completed domain types together without changing existing entities (after T006; FR-002/005/006).
- [X] T008 [P] Configure `CafeManagement.Api/Infrastructure/Persistence/Configurations/OrderConfiguration.cs` exactly per T005/T006: unique Reference/CapabilityHash, binary lengths, field lengths/precision, rowversion, restrictive table FK, TableId and (Status, CreatedAtUtc) indexes; checks for valid Type/Status/Currency, positive Total, nonblank name/phone, DineIn table/label required versus Pickup both null, and nonblank reason iff Rejected (after T007; parallel with T009/T010; FR-003/004/005/007/008/013).
- [ ] T009 [P] Configure `CafeManagement.Api/Infrastructure/Persistence/Configurations/OrderLineConfiguration.cs` exactly per T007, including restrictive order/product FKs, unique (OrderId, ProductId), ProductId index, nonblank snapshot name, positive quantity/price/line total and widened-decimal SQL arithmetic check; do not claim cross-row sum/nonempty-line invariants are SQL CHECKs (after T007; parallel with T008/T010; FR-002/005/006).
- [ ] T010 [P] Create the internal `CafeManagement.Api/Application/OrderLifecycle.cs` rule allowing only Submitted→Preparing, Preparing→Ready, Ready→Completed and Submitted→Rejected with a trimmed nonblank reason of at most 500 characters; Completed/Rejected terminal; no public setter/endpoint/staff UI; verify rule table manually against every state pair using an in-memory diagnostic if needed (after T007; parallel with T008/T009; FR-008, SC-005, future POS boundary).
- [ ] T011 Add Orders/OrderLines to `CafeManagement.Api/Infrastructure/Persistence/CafeDbContext.cs` and apply the two new configurations; verify existing DbSets, Identity mapping, product/table schemas and context-factory registration remain intact; build the solution (after T008–T010; FR-005/012).
- [ ] T012 Generate one additive AddCustomerOrders migration in `CafeManagement.Api/Infrastructure/Persistence/Migrations/` after `20261027013745_InitialMenuAndIdentity`, including generated `*_AddCustomerOrders.cs`, designer and `CafeDbContextModelSnapshot.cs`; generate reviewed SQL to ignored `artifacts/customer-orders.sql`, record exact generated paths in this file, and stop on any unintended existing table/data/Identity alteration, reset or migration rewrite (after T011; FR-005/012, Constitution II/IV).
- [ ] T013 Apply the reviewed migration only to the confirmed authorized target(s) after T003 safety gates; verify new tables/indexes/checks/FKs and original catalog/table/QR/Identity fingerprints, and record outcomes in `specs/002-customer-ordering/tasks.md`; no EnsureCreated/EnsureDeleted, startup migration, account provisioning, reseeding, destructive downgrade or QR regeneration (after T012; FR-005/012, Constitution II/IV).
- [ ] T014 Define `CafeManagement.Api/Contracts/OrderContracts.cs` with OrderDraft, item, ReviewedPrice, Review, placement result and minimal Tracking shapes from `contracts/api.md`; enforce name/phone trimmed nonblank capacities 120/40, nonempty unique product IDs, positive whole int quantities, valid DineIn table UUID versus Pickup null/absent, exactly matching reviewed-price IDs and invariant positive two-place EGP strings; reject overflow/overprecision, prohibited/unknown write fields and truncation; no regional phone/account/quantity policy or contact fields in responses (after T013; FR-002–FR-010/013).
- [ ] T015 Configure only new order HTTP route protections in `CafeManagement.Api/Program.cs`: named configurable remote-IP fixed-window limiter initially 60 requests/minute, queue 0 and Retry-After; correct routing/middleware order; no-store/no-referrer for `/menu/t/*`, `/menu/pickup`, `/orders/track` and no-store order APIs; verify diagnostic configuration does not log bearer/body/contact/hash data; preserve Identity/CORS/antiforgery/original route behavior (after T014; FR-009/010/011/012/013).
- [ ] T016 Run restore/build and inspect registered schema/value contracts without introducing new dependencies; manually verify SQL field capacities, unique digests/references, FK/type/reason/price/quantity checks and unchanged existing records on the isolated target; record results and limitations in `specs/002-customer-ordering/tasks.md` (after T015; FR-005/006/007/008/012, Constitution VI).

## Phase 3: US1 — Review a Cart from the Current Menu (P1, T017–T023)

**Goal**: Available-product browsing and usable cart without order placement.
**Independent verification**: Signed out, add two available products (including distinct IDs
with duplicate names), adjust/remove, reject invalid quantities, and inspect exact EGP amounts;
no order/payment/staff workflow is required. Empty and load-error feedback remain clear.

- [ ] T017 [P] [US1] Create `CafeManagement.Api/Application/CartState.cs` as circuit-scoped data only: selected product IDs, positive whole int quantities, last displayed decimal prices, Type/TablePublicCode, one row per product ID, add/update/remove and checked exact line/total calculations; no static/shared tracked entities, customer credentials or durable-cart promise (after T016; parallel with T018; FR-001/002/006).
- [ ] T018 [P] [US1] Create `CafeManagement.Api/Components/Shared/MenuCatalog.razor` reusing public catalog projections with available products, EGP/two-place prices, empty categories/menu and add callbacks by product ID; use same-document category links and accessible focus without requiring sign-in or rewriting existing service results (after T016; parallel with T017; FR-001/002/011/012).
- [ ] T019 [US1] Create `CafeManagement.Api/Components/Shared/CartPanel.razor` using CartState for labeled quantity controls, explicit Remove, line amounts/total, empty state and field feedback; invalid zero/negative/fractional/overflow input is rejected and empty cart cannot request checkout (after T017/T018; FR-002/006/011).
- [ ] T020 [US1] Integrate MenuCatalog and CartPanel into `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor`, preserving `/menu/t/{PublicCode}`, correct visible table, invalid-link failure, latest-navigation-wins protection, available-only reads and existing QR identifiers; ordinary category browsing must not recreate the circuit or lose selections (after T019; FR-001/002/003/011/012).
- [ ] T021 [US1] Guard context-changing/full navigation in `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` and `CafeManagement.Api/Components/Shared/CartPanel.razor`: warn before clearing a nonempty unsubmitted cart and allow staying; keep displayed table/type accurate, no silent cross-table reassignment, and explain hard-reload cart loss without adding persistent cart storage (after T020; FR-002/003/004/011).
- [ ] T022 [US1] Register CartState scoped in `CafeManagement.Api/Program.cs`; verify page-local interactive roots retain one cart while rendering the catalog/cart, two tabs do not share state, and existing static `CafeManagement.Api/Components/Routes.razor`/Identity authorization remain unchanged (after T021; FR-002/010/012).
- [ ] T023 [US1] Run restore/build and manually validate all US1 scenarios, category focus/navigation, invalid quantities, duplicate names, empty/load failures and Admin regression using `specs/002-customer-ordering/quickstart.md`; record outcomes in `specs/002-customer-ordering/tasks.md`, confirming zero orders written (after T022; FR-001/002/006/011/012, SC-001/006/007).

## Phase 4: US2 — Place a Dine-in Order for the Correct Table (P1, T024–T042)

**Goal**: Safe guest checkout/submission for a valid existing table; shared service also
supports the approved Pickup variant so US3 adds its entry/UI without duplicating persistence.
**Independent verification**: Valid table + cart + guest name/phone → exactly one Submitted
order/lines and confirmation/private link, no cashier/payment flow. Changed prices require a
new confirmation; interrupted submission can recover via capability and minimal tracking read.
The capability-only recovery read and a minimal functional `/orders/track` page are delivered
here, so a customer can open the private link immediately after checkout. US4 extends that
existing page with tracking refinements and complete status/security validation.

- [ ] T024 [US2] Create `CafeManagement.Api/Application/OrderService.cs` capability/draft validation using the existing context factory: validate exactly 32 random bytes / 43-character base64url, SHA-256 binary(32) digest, and deterministic versioned length-delimited intent hashing of trimmed name/phone, Type, stable table code and sorted product IDs/quantities; exclude reviewed prices/server fields and never log credentials or return hashes (after T023; FR-005/007/009/010/013).
- [ ] T025 [US2] Implement read-only review in `CafeManagement.Api/Application/OrderService.cs`: validate guest fields/nonempty unique selections, resolve DineIn table via existing PublicCode or require Pickup no table, load available products, calculate authoritative checked decimal line/total amounts within decimal(18,2), and return Review or safe field/not-found/unavailable errors; no reservation or order write (after T024; FR-003/004/005/006/013).
- [ ] T026 [US2] Implement atomic new placement in `CafeManagement.Api/Application/OrderService.cs`: short RepeatableRead, stable-ID product/table reads, revalidate availability and complete reviewed-price vector, calculate from DB prices only, persist name/price/table-label snapshots plus name/phone, server reference/time, Submitted order and all lines, then commit; enforce nonempty lines/summed total and exact representability; no locks while waiting for user confirmation (after T025; FR-003/004/005/006/007/008/013).
- [ ] T027 [US2] Add accepted-outcome lookup before current catalog validation and unique-digest race recovery in `CafeManagement.Api/Application/OrderService.cs`: same capability/intent returns saved result, changed intent conflicts; rollback/dispose failed insert and fresh-context lookup only for the relevant duplicate index; ambiguous commit/deadlock/cancellation yields safe recovery guidance, never automatic write retry/new credential (after T026; FR-005/007/009).
- [ ] T028 [US2] Add price_changed/product_unavailable/submission_conflict results to `CafeManagement.Api/Application/OrderService.cs` and `CafeManagement.Api/Contracts/OrderContracts.cs`; any price increase/decrease returns refreshed Review with zero inserts, later confirmation rechecks again, unknown/extra price entries fail validation; retain existing `CafeManagement.Api/Contracts/ApiErrors.cs` behavior and add only the safe endpoint-specific review extension needed by the contract (after T027; FR-005/006/007/011).
- [ ] T029 [US2] Add minimal capability-only tracking/outcome read in `CafeManagement.Api/Application/OrderService.cs`, projecting saved reference/type/table snapshot/line snapshots/EGP amounts/status/reason/time with no name/phone/internal IDs/hashes/rowversion/Identity data; missing/invalid/unknown capability is the same safe failure; never use public reference/contact/table code as credentials (after T028; FR-007/009/010/013).
- [ ] T030 [US2] Add anonymous JSON POST `/api/orders/review` in `CafeManagement.Api/Controllers/PublicOrdersController.cs`, extracting opaque guest bearer explicitly and requiring existing antiforgery header/cookie, common validation and safe 200/400/404/409 ProblemDetails behavior; no JWT/Identity-scheme replacement, staff-cookie ownership, write or private-order lookup through review (after T029; FR-005/006/010/013).
- [ ] T031 [US2] Add JSON POST `/api/orders` to `CafeManagement.Api/Controllers/PublicOrdersController.cs` using the same protections: 201 new, 200 saved replay, safe 400/404/409 review/conflict results, token-free `/orders/track` relative path and no bearer Location header; reject prohibited/unknown fields and preserve global error handling (after T030; FR-003/005/006/007/010/013).
- [ ] T032 [US2] Add anonymous GET `/api/orders/tracking` to `CafeManagement.Api/Controllers/PublicOrdersController.cs` requiring capability-only lookup, generic 404 for missing/invalid/unknown context and minimal DTO; use it for read-only uncertain-outcome recovery, no public lookup/list/status mutation endpoint (after T031; FR-007/009/010/013).
- [ ] T033 [US2] Register OrderService scoped and apply the named guest limiter only to new review/place/tracking HTTP endpoints in `CafeManagement.Api/Program.cs` and `CafeManagement.Api/Controllers/PublicOrdersController.cs`; verify no-store, JSON 415, 429/Retry-After, safe failures and unchanged original Identity/Admin/API behavior (after T032; FR-009/010/011/012).
- [ ] T034 [P] [US2] Create `CafeManagement.Api/wwwroot/js/order-session.js` helpers for Web Crypto 32-byte capability generation/base64url, sessionStorage read/write verification, started flag/reference metadata and trusted-origin private-link construction, and wire their deterministic loading in `CafeManagement.Api/Components/App.razor`; verify helper availability after interactive render before any T035 use, retain credential before first review/write, reserve separate pending-attempt and active-tracking storage keys, persist no contact data/cart, block on storage failure, and never replace an uncertain attempt or log its secret (after T023; can run beside T024–T033 on distinct files; FR-007/009/013).
- [ ] T035 [US2] Create `CafeManagement.Api/Components/Shared/CheckoutPanel.razor` within the menu root: labeled required name/phone fields, technical capacities, type/table/cart review and server-direct read-only review using the registered OrderService and T034's loaded helpers; retain capability through JS first, invalidate review on any draft edit, display old/new unit prices/line totals/total and no registration/payment/address controls (after T025/T033/T034; FR-003/005/006/011/013).
- [ ] T036 [US2] Add explicit placement and reconfirmation states to `CafeManagement.Api/Components/Shared/CheckoutPanel.razor`: send the unchanged reviewed-price vector, disable duplicate clicks, require Confirm updated prices and place order after any change, repeat on a further change, correct unavailable selections explicitly, and render saved reference/details/Submitted status with private-link access; no automatic resubmit or false success (after T028/T033/T035; FR-005/006/007/008/011).
- [ ] T037 [US2] Embed CheckoutPanel beside CartPanel in `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` and add explicit read-only Check order outcome recovery in `CafeManagement.Api/Components/Shared/CheckoutPanel.razor` using T029 and retained session metadata; keep review/back in the same root, validate visible table, preserve `Components/Routes.razor`, and build private links with trusted PublicWebBaseUrl; found results use saved prices, not-found during an in-flight save remains inconclusive, and only unchanged-intent/capability explicit retry is allowed, never changed-cart replay/new token after reload or credential prerender (after T036/T029; FR-003/007/009/010/011/012).
- [ ] T038 [US2] Create a minimal functional `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` at `/orders/track` with PublicLayout/Interactive Server and extend the already-loaded `CafeManagement.Api/wwwroot/js/order-session.js` for after-render fragment validation, separate active-tracking storage and fragment removal; use T029 to show saved reference/type/table snapshot/items/EGP total/current status and rejection reason when applicable, same-tab reload and safe missing/invalid/unknown/load-error outcomes without registration, prerendered private data, contact fields or fallback lookup; preserve any pending attempt and verify a freshly opened checkout link resolves its matching order before US2 completion (after T037/T029/T033/T034; FR-007/008/009/010/011/013).
- [ ] T039 [US2] Extend `CafeManagement.Api/wwwroot/js/reconnect-status.js` submit protection to order write forms while preserving Admin guards/inert/reload behavior; mark checkout forms in `CafeManagement.Api/Components/Shared/CheckoutPanel.razor`, verify the T034 bootstrap remains unchanged and server busy guards/service checks authoritative, exercise T037's explicit read-only recovery after reload, and never auto-replay writes (after T038; FR-007/010/011/012).
- [ ] T040 [US2] Manually validate review/place/recovery HTTP contracts and equivalent direct component checks using `specs/002-customer-ordering/contracts/api.md`: missing/invalid/mispaired CSRF rejection, correct pair, malformed input/bearer, unknown fields, price/total/status/role/table-ID tampering, generic failures and unavailable filtering; record safe outcomes in `specs/002-customer-ordering/tasks.md` (after T039; FR-003/005/006/009/010/013, SC-003/004/005).
- [ ] T041 [US2] On the owned validation target exercise `CafeManagement.Api/Application/OrderService.cs` with parallel same-capability submissions, changed-intent conflicts, price/availability edits versus transaction locks, rollback/no partial lines, numeric overflow, lost-success-response recovery and restart/historical snapshot checks; record actual row counts and preservation results in `specs/002-customer-ordering/tasks.md`, never alter original records (after T040; FR-005/006/007/012, SC-003/004/007).
- [ ] T042 [US2] Run restore/build and manually complete US2 scenarios for two distinct existing table links, required guest details, price increase/decrease/rechange with explicit confirmation and correct saved associations; open each checkout-provided private link in a fresh signed-out browser to T038's working page, verify matching saved details/Submitted status, reload and negative missing/invalid-link privacy outcomes without any US4 work; use `specs/002-customer-ordering/quickstart.md` and record evidence here, with physical scans pending if HTTPS reachability is unavailable (after T041/T038/T039; FR-003/005/006/007/008/009/010/011/013, SC-002/003/004/005/006/008).

## Phase 5: US3 — Place an Online Pickup Order (P1, T043–T048)

**Goal**: Direct no-QR browsing/checkout using the same service, tables and lifecycle.
**Independent verification**: Fresh signed-out visit without a table → pickup cart, required
guest details, exactly one order per submission with null table fields and a private link.
US2 supplies shared acceptance/recovery; no dine-in interaction is needed to use pickup.

- [ ] T043 [US3] Add a separate pickup catalog read to `CafeManagement.Api/Application/PublicMenuService.cs` and additive response shape in `CafeManagement.Api/Contracts/PublicMenuContracts.cs`, reusing available category/product projection with EGP/two-place prices and no table/contact fields while leaving the existing table response/validation unchanged (after T042; FR-001/004/012).
- [ ] T044 [US3] Add anonymous no-store GET `/api/menu/pickup` in `CafeManagement.Api/Controllers/PublicMenuController.cs`, including empty/load-error behavior and no table requirement; verify invalid table endpoints still fail rather than becoming pickup (after T043; FR-001/004/011/012).
- [ ] T045 [US3] Create `CafeManagement.Api/Components/Pages/Menu/PickupMenu.razor` at `/menu/pickup` using PublicLayout, Interactive Server, shared MenuCatalog/CartPanel/CheckoutPanel and circuit CartState, explicit Pickup label and null table context; reuse guest validation, price reconfirmation and capability recovery without a second order service (after T044; FR-001/002/004/005/006/007/011/013).
- [ ] T046 [US3] Add direct pickup discovery in `CafeManagement.Api/Components/Layout/PublicLayout.razor` and integrate context-leaving warnings with `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` / `PickupMenu.razor`; verify leave clears unsubmitted cart, stay retains it, no hidden table/type transfer or forced QR step, and existing Admin/login navigation remains intact (after T045; FR-003/004/011/012).
- [ ] T047 [US3] Manually place two pickup orders without QR/table, reject any supplied table code, validate missing/blank/overlong name/phone, unavailable products and both price directions/rechanges, then inspect shared Orders/OrderLines/status/guest recovery and open/reload each private link in a fresh signed-out browser through the existing US2 tracking page with no table association; record outcomes in `specs/002-customer-ordering/tasks.md` using `quickstart.md`, without staff/payment features (after T046/T038; FR-004/005/006/007/008/009/013, SC-002/003/004/005/008).
- [ ] T048 [US3] Run restore/build and pickup independent acceptance plus table-menu/API-shape regression from `specs/002-customer-ordering/quickstart.md`; verify signed-out direct entry, same-name distinct products, no table association, no exposure of private fields, and original preservation fingerprints; record evidence in `specs/002-customer-ordering/tasks.md` (after T047; FR-001/004/010/011/012, SC-002/006/007).

## Phase 6: US4 — View the Intended Order's Status (P2, T049–T055)

**Goal**: Refine the functional private tracking delivered in US2 and reused by US3; complete
robustness, usability and all-status validation without registration or staff actions.
**Independent verification**: Use already-accepted or isolated prepared orders and a private
link in a second signed-out browser; read all five statuses/rejection reason, reject unrelated
access contexts, and reload. No staff screen/status-update endpoint is needed.

- [ ] T049 [US4] Refine the T034/T038 helper in `CafeManagement.Api/wwwroot/js/order-session.js` for malformed/replaced fragments, blocked/cleared storage, repeated link navigation and trusted-origin copy-link support; preserve functional initial parsing, separate pending-attempt/active-tracking keys, after-render-only credential access, fragment removal and same-tab reload, ensuring a new tracking link cannot overwrite a pending submission or silently fall back to a stale credential on an explicitly invalid fragment (after T048/T038; FR-007/009/010/013).
- [ ] T050 [US4] Refine the existing T038 page `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` with accessible loading/error/retry focus and latest-navigation-wins handling for slow reads or a replaced link; preserve the functioning `/orders/track` route, token-free prerender, minimal DTO and identical invalid/missing/unknown feedback, never displaying a previous order during a new-context load/error or adding contact/reference/table fallback or staff UI (after T049/T038; FR-009/010/011/013).
- [ ] T051 [US4] Add explicit reload/copy-private-link actions and bearer-sharing warning in `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` and `CafeManagement.Api/Components/Shared/CheckoutPanel.razor`; verify links contain secrets only in fragments at runtime, use trusted PublicWebBaseUrl not Host, survive same-tab reload after fragment removal, and expose no raw credential in prerendered HTML/server logs (after T050; FR-007/009/010/011/013).
- [ ] T052 [US4] Validate/refine the existing Submitted/Preparing/Ready/Completed/Rejected and required-reason display in `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` using owned isolated prepared examples; verify `CafeManagement.Api/Application/OrderLifecycle.cs` allows exactly four approved transitions, rejects every other pair/reason omission, and leaves terminal states terminal; no duplicate tracking page, committed automated test suite or production mutation endpoint (after T051; FR-008/009, SC-005).
- [ ] T053 [US4] Manually check `CafeManagement.Api/Controllers/PublicOrdersController.cs` and `Components/Pages/Orders/TrackOrder.razor` against missing/invalid/unknown/wrong-order capability, public reference/name/phone/table-only attempts, staff-cookie-only requests, response projection, no-store/no-referrer and sanitized failures; confirm zero private contact/hash/Identity exposure and zero customer status mutation, recording results in `specs/002-customer-ordering/tasks.md` (after T052; FR-009/010/013, SC-005/008).
- [ ] T054 [US4] Open copied dine-in/pickup links in a separate signed-out browser, reload current states after isolated prepared changes, recover confirmation after lost reply/restart and simulate unavailable service/storage loss; verify sharing grants only the matching order and missing access never becomes a contact lookup; record actual outcomes in `specs/002-customer-ordering/tasks.md` (after T053; FR-007/009/011/013, SC-003/005/008).
- [ ] T055 [US4] Run restore/build and all US4 acceptance scenarios using `specs/002-customer-ordering/quickstart.md`; verify no signup requirement, five statuses/reason, exact immutable amounts, privacy boundaries and no automatic write replay; record results and any blocked physical-phone check in `specs/002-customer-ordering/tasks.md` (after T054; FR-008/009/010/011, SC-004/005/006).

## Phase 7: Polish and Cross-Cutting Validation (T056–T063)

**Goal**: Prove integration and preservation; no unrelated refactor, optimization or new feature.
These are manual/build validations and narrowly necessary fixes, not automated-test generation.

- [ ] T056 Repeat cross-flow SQL/concurrency checks for `CafeManagement.Api/Application/OrderService.cs` and `Infrastructure/Persistence/Configurations/OrderConfiguration.cs` / `OrderLineConfiguration.cs`: parallel duplicate placement, Admin price/availability edits, lost commit response, rollback atomicity, context/tab isolation, order restart persistence and rowversion stale-update detection using isolated fixtures only; record evidence in `specs/002-customer-ordering/tasks.md` (after T055; FR-005/006/007/008/012, SC-003/004/007).
- [ ] T057 Review `CafeManagement.Api/Program.cs`, `Controllers/PublicOrdersController.cs`, `Application/OrderService.cs`, browser modules and existing StaffAccess/Identity handlers; validate CSRF, JSON/unknown-field rejection, rate limiter/Retry-After, direct-service checks, cookie/staff boundaries, safe logging/errors/headers and no bearer in requests' URLs/prerender/source; record results in `specs/002-customer-ordering/tasks.md`, without changing auth policies or networking (after T056; FR-009/010/012/013, SC-005/007/008).
- [ ] T058 Validate menu/cart/embedded checkout/tracking at 320/768/1280px in `CafeManagement.Api/Components/Pages/Menu/`, `Components/Shared/CheckoutPanel.razor`, `Components/Pages/Orders/TrackOrder.razor` and `wwwroot/css/site.css`; check no horizontal scrolling, touch/keyboard labels/focus, category/cart retention, empty/validation/error/reconnect/price-change feedback; make only demonstrated scoped fixes and record manual evidence here (after T057; FR-001/002/003/004/011, SC-001/002/006).
- [ ] T059 Verify real-phone QR dine-in entry/rescan and pickup/private-link journeys using `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor`, `PickupMenu.razor`, `Components/Pages/Orders/TrackOrder.razor` and trusted `Application/PublicMenuOptions.cs` only if an approved securely reachable HTTPS origin exists; otherwise record exact blocker and leave this task pending in `specs/002-customer-ordering/tasks.md`; no firewall/certificate/network changes or automatic completion of Feature 001 T051/T056 (after T058; FR-003/004/009/011/012, SC-002/005/006/007).
- [ ] T060 Run `dotnet restore CafeManagement.sln`, `dotnet build CafeManagement.sln --no-restore`, `dotnet build CafeManagement.sln -c Release` and `dotnet publish CafeManagement.Api -c Release --no-restore -o artifacts/cafe`; inspect/pilot the published build locally with approved configuration, compiled pages/local Bootstrap/browser JS and no secrets/DB/certificate files; record results in `specs/002-customer-ordering/tasks.md` (after T058; independent of blocked T059; FR-011/012, Constitution VI).
- [ ] T061 Verify original catalog/table/QR/Identity fingerprints, existing Admin/Cashier authentication, menu APIs, `/WeatherForecast`, Swagger and unknown API JSON 404 using `specs/002-customer-ordering/quickstart.md`; stop only owned validation hosts and clean only explicitly owned disposable data, preserving original records and local configuration; record outcomes here (after T060; independent of blocked T059; FR-010/012, SC-007).
- [ ] T062 Review final Git diff and actual routes/DTOs against `specs/002-customer-ordering/spec.md` and contracts; confirm no payment/delivery/POS/report/status-mutation/contact-lookup features, dependency/framework replacement, source removal or mandatory tests, and no generated outputs, credentials or private links to stage; record scope/security review in `specs/002-customer-ordering/tasks.md`, preserving unrelated working edits (after T061; FR-008/009/010/012/013, SC-007/008, Constitution II/VII).
- [ ] T063 Reconcile every acceptance scenario/FR-001–FR-013/SC-001–SC-008 against recorded evidence in `specs/002-customer-ordering/tasks.md` and `quickstart.md`; document completed/blocked/skipped checks and future POS boundary, keeping T059 or other unperformed tasks unchecked; complete final acceptance only after required checks pass, never claim Feature 001 or Feature 002 fully accepted while their verification remains pending (after T062 and T059 for full acceptance; FR-001–FR-013, SC-001–SC-008, Constitution VI/VII).

## Dependencies and Execution Order

### Phase gates and story graph

```text
Setup T001–T003
  → Foundation T004–T016
    → US1 T017–T023 (cart-only MVP)
      → US2 T024–T042 (shared ordering/recovery + dine-in)
        → US3 T043–T048 (pickup entry, same order service)
          → US4 T049–T055 (private tracking UI)
            → T056 → T057 → T058
              ├→ T059 (physical phone; may be blocked)
              └→ T060 → T061 → T062
                both branches → T063 full acceptance
```

Story numbering follows the approved spec: US1/US2/US3 P1; US4 P2. Shared models serve both
order types and belong in Foundation; shared submission/recovery service and minimal tracking
read belong in the earliest ordering story, US2. Do not duplicate these in US3/US4. Each story
is independently demonstrable after its named shared prerequisites; they are not falsely
claimed to be implementation-independent. T038 supplies the functional tracking page for US2;
US3 reuses it. US4 extends that existing page, so no US2/US3 acceptance depends on later US4 work.

T034's only prerequisite is T023, so it may run alongside backend US2 work despite its later
display position; it now includes App.razor bootstrap and must verify helpers are available before
T035. T035 needs T025/T033/T034; placement integration waits for T036. T037 supplies checkout
recovery, T038 supplies the working private-link destination, and T039 retains reconnect integration.
T042 verifies actual signed-out link navigation/reload before the US2 checkpoint. Keep
shared-file changes to OrderService, OrderContracts, PublicOrdersController, Program, CheckoutPanel
and browser modules sequential. T060–T062 may finish while phone T059 is blocked; T063 can record
a partial review then but remains unchecked until full acceptance is actually established.

### Parallel examples

- Foundation: after T007, T008 (OrderConfiguration), T009 (OrderLineConfiguration) and T010
  (OrderLifecycle) touch distinct files. T011 waits for all three.
- US1: after T016, T017 (CartState) and T018 (MenuCatalog callbacks/projection) are independent;
  T019 integration waits for both. No parallel edits to TableMenu/Program.
- US2: after T023, T034 (browser helper and App.razor bootstrap) can run alongside T024–T033
  backend work on distinct files; T035 waits for helper availability and OrderService registration.
  T037–T039 integration/recovery/minimal tracking/reconnect work remains serial.
- US3: no `[P]` tasks; service/DTO → endpoint → menu → navigation → validation is deliberately
  serial for this small project. Do not overlap it with US2 edits to shared files.
- US4: no `[P]` tasks; helper robustness → existing-page usability/context isolation → copy/reload
  → lifecycle/security → acceptance is serial. The initial tracking route is already working.
  No parallel edits to the browser helper or CheckoutPanel.

## Requirement Coverage

| Requirement | Implementation / focused validation |
| --- | --- |
| FR-001 available anonymous catalog | T017–T023, T043–T048, T058 |
| FR-002 cart and quantities | T007/T009/T014, T017–T023, T045/T048 |
| FR-003 correct table dine-in | T006/T008/T014, T020/T021, T025/T026, T037/T040–T042 |
| FR-004 direct no-table pickup | T006/T008/T014/T025/T026, T043–T048 |
| FR-005 atomic server validation | T007–T016, T024–T028/T040/T041/T047/T056 |
| FR-006 exact accepted prices/reconfirmation | T006–T009/T014, T017/T019, T025–T028/T035/T036/T041/T042/T047/T056 |
| FR-007 durable confirmation/dedup/recovery/link | T005/T008, T024/T026–T034/T036–T042, T049/T051/T054/T056 |
| FR-008 shared lifecycle/future POS boundary | T004/T006/T008/T010, T026/T042/T047, T052/T055/T062 |
| FR-009 private status access | T005/T024/T029/T032/T037/T038/T042, T049–T055/T057 |
| FR-010 staff/security preservation | T015/T022/T030–T033/T040, T049–T055/T057/T061/T062 |
| FR-011 responsive/clear outcomes | T018–T023, T028/T035–T039/T042/T045–T048, T050–T055/T058/T059/T060 |
| FR-012 preservation and feature exclusions | T001–T003/T011–T016, T020/T022/T033/T039/T043/T046/T048, T056–T063 |
| FR-013 guest name/phone/private details | T006/T008/T014, T024–T026/T029–T032/T034/T035/T040/T042/T047, T049–T054/T057/T062 |

SC-001: T023/T048; SC-002: T042/T047/T048/T059; SC-003: T040–T042/T054/T056;
SC-004: T041/T042/T047/T055/T056; SC-005: T038/T042/T052–T055/T057; SC-006: T058/T059;
SC-007: T003/T013/T016/T061/T062; SC-008: T042/T047/T052–T055/T062/T063.

## Implementation Strategy

1. Complete Setup/Foundation with successful builds and reviewed additive migration. If target,
   backup readiness or isolation is missing, report the gate instead of improvising destructive work.
2. Deliver US1 as the smallest independently useful menu/cart increment; review before checkout.
3. Deliver US2 safe shared order acceptance/recovery, dine-in and a functional private tracking
   page; validate fresh signed-out link opening/reload without waiting for US4.
4. Deliver US3 no-QR pickup using the same working tracking page, then US4 robustness/usability
   refinements and complete prepared-status/security validation.
5. Run cross-cutting checks, Release publish and preservation review. Record genuine blocked
   checks; no new network access, production deployment or business feature is implied.

Keep changes small and reviewable; run build/manual validation for affected increments. Do not
commit/push automatically from this task-generation command. Any later commit includes only the
authorized validated scope and never pre-existing unrelated edits or secrets.

## Blockers and Pending Prerequisites

- No unresolved business decisions block task generation; the three approved clarifications
  are incorporated. Environment/migration/validation readiness must be established by T002/T003.
- Existing Feature 001 T051/T056 remain pending and unchanged. They do not block local task
  design or independent implementation but remain separate acceptance gaps.
- Real-phone T059 is conditional on an approved reachable trusted HTTPS origin; absent that,
  it and full final acceptance T063 remain pending. No firewall/certificate changes are authorized.
- If a concrete bug prevents a prerequisite, stop and report it. Do not expand scope, create
  mandatory tests or silently substitute desktop emulation for physical-phone evidence.

## Execution Evidence

### Setup prerequisite evidence (T001–T003, 2026-10-27)

- T001: Started on clean master at a947581f22ab14a00b2dd61e6ffdcb44ecf25956. Reviewed the
  approved specification, six planning artifacts, complete task/dependency list, requirements,
  Constitution and current Program.cs. The specification-quality checklist passed 16/16 with
  zero unchecked items. No AGENTS.md or implementation extension hooks were present. Selected
  Feature 002 using process-only environment variables; feature.json was preserved.
- Confirmed Feature 001 has 56 tasks, 54 completed and only T051/T056 pending. There were no
  remaining unrelated working edits; the earlier index issue was already resolved. Feature 001
  history and all application/source/configuration files were preserved without modification.
- T002: Active SDK 8.0.427; installed SDKs 8.0.400/8.0.427 and ASP.NET Core/.NET runtimes
  8.0.8/8.0.33. Existing net8.0, EF/Identity 8.0.33, QRCoder 1.8.1 and Swashbuckle 6.4.0
  remain unchanged. Local dotnet-ef 8.0.33 tool restore and solution restore succeeded;
  `dotnet build CafeManagement.sln --no-restore` passed with 0 warnings/0 errors (10.49 seconds).
  C: had 2.53 GiB free; no installation, framework upgrade or package change was needed.
  Existing ignore rules cover build outputs, artifacts, dotenv, user files and database files;
  no ignore-file edit was necessary. No automated tests were added or executed.
- T003: Safely read existing local configuration without printing credentials. Confirmed
  localhost\MSSQLSERVER01 / CafeManagement with Windows integrated authentication, SQL Server
  Developer Edition and local backup/restore permissions. Original allocated size was 16 MiB;
  SQL data/backup volume D: had 63.19 GiB free. Existing migration inventory contains only
  20261027013745_InitialMenuAndIdentity (EF 8.0.33). No new schema/migration was generated/applied.
- Created a new COPY_ONLY backup with CHECKSUM in the SQL instance's existing backup directory:
  `D:\programming applications\MSSQL16.MSSQLSERVER01\MSSQL\Backup\CafeManagement_F002_Validation_20261027_134110_c57ad692.bak`.
  Verified header source/copy-only/checksum flags and RESTORE VERIFYONLY WITH CHECKSUM. No
  existing backup file was overwritten. Retain this local backup outside Git; it contains
  database/account data and must never be added to the repository or shared as documentation.
- Successfully restored that backup as the new isolated database
  `CafeManagement_F002_Validation_20261027_134110_c57ad692`, using new uniquely named data/log
  files in the instance's existing DATA directory. No REPLACE, source restore/reset, migration
  rewrite or account provisioning occurred. Marked the copy with database extended properties:
  CodexValidationOwner = its exact database name; CodexFeature = 002-customer-ordering:T003.
  Re-read the ownership marker and compared all 11 table fingerprints after the actual restore.
- Preservation method: per-table row counts and SHA-256 of stable-primary-key-ordered full-row
  SQL JSON, including nulls, with an explicit empty-array representation for empty tables.
  Password hashes/security stamps stayed inside SQL; only aggregate digests were returned.
  Original before/after and restored-copy results matched for every table: five cafe tables/QR
  codes, eight categories, eight products, two users, two roles, two user-role memberships,
  one migration and four empty Identity support tables.

  | Original table | Rows | Before/after/copy SHA-256 |
  | --- | --- | --- |
  | CafeTables | 5 | 0DA7E8CAB47B28BEA6CC822E38EF309D022CA75C09DBF79D5DFCCFCE835318D3 |
  | MenuCategories | 8 | FE37F409DD7C7872008B135F18E62D7D36A2576DD9DE528601F4E156093165FD |
  | Products | 8 | 4988206FBFECB7205C8E3CC7D72047B290E937B1E7AB761699523110B32B2EA5 |
  | AspNetUsers | 2 | AAF70956B919344A9A52A800697749BB4A1F631345FB5A6C63AC56F8F0117163 |
  | AspNetRoles | 2 | 6586ED98851F4859A5B17F7ADBD0BC8917972C7B790C74110E694A21B031BF66 |
  | AspNetUserRoles | 2 | 4BC01AA4DB803E0CCBCB8173CF97B06E8171AFD7E83E7A015E5CDF602055D839 |
  | __EFMigrationsHistory | 1 | 1886095F52DA074825BCE2D7A2137BDD50D1125E093C3A623F1D351830F02248 |

- Later mutable/race/status checks must explicitly target the owned validation copy through
  process-only connection overrides, after verifying its marker. The application's existing
  user secrets/default connection still target CafeManagement and were not changed. No new
  host was started. Do not run mutable acceptance checks against the original, recreate it,
  overwrite unrelated databases, or delete this owned copy/backup without scoped authorization.
- Corrected in-memory diagnostic helper issues (PowerShell property access and a reserved SQL
  alias) before preparation; these were not application defects and required no source/test files.
  All readiness/restore/preservation checks subsequently passed. Only this task document changes.
- T001–T003 are complete. T004–T063 remain unchecked and were not started. No ordering, POS,
  payments, delivery, new entities, code, migrations, certificates or network changes were made.

### Order domain and header mapping evidence (T004–T008, 2026-10-27)

- Started from clean master at eb256c0d4f1fd35635b0570d52ab70a437cd45cb; T001–T003 complete,
  specification checklist 16/16, no hooks or additional repository instructions. Reviewed exact
  T004–T008 dependencies, approved model/spec/plan/contracts and Constitution. Feature 001's
  54 completed tasks and pending T051/T056 are preserved. No later tasks were implemented.
- T004: Added OrderType (DineIn/Pickup) and OrderStatus (Submitted/Preparing/Ready/Completed/
  Rejected) only. T005–T006: Added Order with long internal ID, distinct backend-generated
  nonempty Guid reference, required digest fields, optional table/navigation/label snapshot,
  required guest fields, EGP, decimal total, Submitted default, internal fulfillment/reason
  setters, UTC creation default, database rowversion and initialized Lines collection.
  No plaintext capability, guest Identity FK, payment fields or lifecycle mutation workflow.
- T007: Added OrderLine with long IDs/order FK, existing int product FK/navigation, required
  product-name snapshot, int quantity and decimal unit/line amounts. Existing entities,
  DbContext, Program.cs and application routes were not edited. Explicit line mapping/checks
  remain T009; no service/DTO/checkout behavior is claimed complete.
- T008: Added OrderConfiguration with identity key, unique reference/digest, binary(32)
  digests, enum-to-string 16-character Type/Status, table label/name length 120, phone 40,
  char(3) EGP, decimal(18,2) positive total, reason 500, datetime2, required rowversion,
  restrictive optional table FK, TableId and (Status, CreatedAtUtc) indexes. Eight SQL checks
  cover type/status/currency, name/phone, positive total, correct table shape and reason iff
  Rejected. Nullable label/reason branches explicitly test IS NOT NULL to avoid SQL UNKNOWN
  accidentally accepting a missing dine-in label or rejection reason.
- Restore passed; Debug build passed with 0 warnings/0 errors (9.68 seconds), on net8.0 and
  unchanged dependencies. In-memory reflection confirmed exact enums, distinct references,
  EGP/Submitted/UTC defaults, initialized lines and no public status/reason setter. No test
  project, test framework, committed diagnostic, host process or automated test suite was added.
- Inspected EF-generated create SQL in memory only, never executed it. All ten existing
  EF-mapped table definitions and their indexes matched the before snapshot. Orders columns,
  eight checks, four indexes, rowversion and ON DELETE NO ACTION table FK matched this task.
  Existing ApplyConfigurationsFromAssembly discovers OrderConfiguration and its related
  OrderLine by convention; this is not a DbContext edit or execution of T011.
- EF model inspection emitted precision warnings for OrderLine.UnitPrice and LineTotal:
  explicit OrderLine configuration is intentionally pending T009. Its temporary conventional
  table name is OrderLine. Do not apply this intermediate model as database schema; T009 and
  the later reviewed migration will supply the complete approved OrderLines mapping.
- Evaluated all eight generated Order CHECK predicates through read-only SQL SELECTs on the
  owned T003 validation target. All 21 cases passed: valid pickup/dine-in and five statuses;
  invalid type/status/currency, zero/negative total, blank name/phone, missing/blank dine-in
  table context, pickup table data, rejected null/blank reason and inappropriate non-rejected
  reason. This verifies predicate semantics, not installed database constraints; actual schema
  application remains T012/T013 and was not performed.
- Read-only before/after fingerprints matched all 11 original tables, including five table
  records/QR identifiers, eight categories, eight products, two accounts/roles/memberships
  and the existing single migration. The owned validation database still has no order tables.
  No original or validation records, migration files, account credentials or configuration changed.
- Reviewed the final source/task diff and secret/ignore boundaries. T004–T008 are complete;
  T009–T063 remain unchecked. No POS, payment, delivery, API/UI, migration or unrelated change.
