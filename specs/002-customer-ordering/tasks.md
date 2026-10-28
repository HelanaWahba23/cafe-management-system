# Tasks: Customer Ordering

**Input**: Approved [spec.md](spec.md), [plan.md](plan.md), [research.md](research.md),
[data-model.md](data-model.md), [API contract](contracts/api.md), [UI contract](contracts/ui.md),
[quickstart.md](quickstart.md), requirements.md and Constitution v1.0.1.
**Created**: 2026-10-27. **Status**: 57/63 validated in the working tree; T021–T023, T046, T059 and T063 remain pending.
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
- [X] T009 [P] Configure `CafeManagement.Api/Infrastructure/Persistence/Configurations/OrderLineConfiguration.cs` exactly per T007, including restrictive order/product FKs, unique (OrderId, ProductId), ProductId index, nonblank snapshot name, positive quantity/price/line total and widened-decimal SQL arithmetic check; do not claim cross-row sum/nonempty-line invariants are SQL CHECKs (after T007; parallel with T008/T010; FR-002/005/006).
- [X] T010 [P] Create the internal `CafeManagement.Api/Application/OrderLifecycle.cs` rule allowing only Submitted→Preparing, Preparing→Ready, Ready→Completed and Submitted→Rejected with a trimmed nonblank reason of at most 500 characters; Completed/Rejected terminal; no public setter/endpoint/staff UI; verify rule table manually against every state pair using an in-memory diagnostic if needed (after T007; parallel with T008/T009; FR-008, SC-005, future POS boundary).
- [X] T011 Add Orders/OrderLines to `CafeManagement.Api/Infrastructure/Persistence/CafeDbContext.cs` and apply the two new configurations; verify existing DbSets, Identity mapping, product/table schemas and context-factory registration remain intact; build the solution (after T008–T010; FR-005/012).
- [X] T012 Generate one additive AddCustomerOrders migration in `CafeManagement.Api/Infrastructure/Persistence/Migrations/` after `20261027013745_InitialMenuAndIdentity`, including generated `*_AddCustomerOrders.cs`, designer and `CafeDbContextModelSnapshot.cs`; generate reviewed SQL to ignored `artifacts/customer-orders.sql`, record exact generated paths in this file, and stop on any unintended existing table/data/Identity alteration, reset or migration rewrite (after T011; FR-005/012, Constitution II/IV).
- [X] T013 Apply the reviewed migration only to the confirmed authorized target(s) after T003 safety gates; verify new tables/indexes/checks/FKs and original catalog/table/QR/Identity fingerprints, and record outcomes in `specs/002-customer-ordering/tasks.md`; no EnsureCreated/EnsureDeleted, startup migration, account provisioning, reseeding, destructive downgrade or QR regeneration (after T012; FR-005/012, Constitution II/IV).
- [X] T014 Define `CafeManagement.Api/Contracts/OrderContracts.cs` with OrderDraft, item, ReviewedPrice, Review, placement result and minimal Tracking shapes from `contracts/api.md`; enforce name/phone trimmed nonblank capacities 120/40, nonempty unique product IDs, positive whole int quantities, valid DineIn table UUID versus Pickup null/absent, exactly matching reviewed-price IDs and invariant positive two-place EGP strings; reject overflow/overprecision, prohibited/unknown write fields and truncation; no regional phone/account/quantity policy or contact fields in responses (after T013; FR-002–FR-010/013).
- [X] T015 Configure only new order HTTP route protections in `CafeManagement.Api/Program.cs`: named configurable remote-IP fixed-window limiter initially 60 requests/minute, queue 0 and Retry-After; correct routing/middleware order; no-store/no-referrer for `/menu/t/*`, `/menu/pickup`, `/orders/track` and no-store order APIs; verify diagnostic configuration does not log bearer/body/contact/hash data; preserve Identity/CORS/antiforgery/original route behavior (after T014; FR-009/010/011/012/013).
- [X] T016 Run restore/build and inspect registered schema/value contracts without introducing new dependencies; manually verify SQL field capacities, unique digests/references, FK/type/reason/price/quantity checks and unchanged existing records on the isolated target; record results and limitations in `specs/002-customer-ordering/tasks.md` (after T015; FR-005/006/007/008/012, Constitution VI).

## Phase 3: US1 — Review a Cart from the Current Menu (P1, T017–T023)

**Goal**: Available-product browsing and usable cart without order placement.
**Independent verification**: Signed out, add two available products (including distinct IDs
with duplicate names), adjust/remove, reject invalid quantities, and inspect exact EGP amounts;
no order/payment/staff workflow is required. Empty and load-error feedback remain clear.

- [X] T017 [P] [US1] Create `CafeManagement.Api/Application/CartState.cs` as circuit-scoped data only: selected product IDs, positive whole int quantities, last displayed decimal prices, Type/TablePublicCode, one row per product ID, add/update/remove and checked exact line/total calculations; no static/shared tracked entities, customer credentials or durable-cart promise (after T016; parallel with T018; FR-001/002/006).
- [X] T018 [P] [US1] Create `CafeManagement.Api/Components/Shared/MenuCatalog.razor` reusing public catalog projections with available products, EGP/two-place prices, empty categories/menu and add callbacks by product ID; use same-document category links and accessible focus without requiring sign-in or rewriting existing service results (after T016; parallel with T017; FR-001/002/011/012).
- [X] T019 [US1] Create `CafeManagement.Api/Components/Shared/CartPanel.razor` using CartState for labeled quantity controls, explicit Remove, line amounts/total, empty state and field feedback; invalid zero/negative/fractional/overflow input is rejected and empty cart cannot request checkout (after T017/T018; FR-002/006/011).
- [X] T020 [US1] Integrate MenuCatalog and CartPanel into `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor`, preserving `/menu/t/{PublicCode}`, correct visible table, invalid-link failure, latest-navigation-wins protection, available-only reads and existing QR identifiers; ordinary category browsing must not recreate the circuit or lose selections (after T019; FR-001/002/003/011/012).
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

- [X] T024 [US2] Create `CafeManagement.Api/Application/OrderService.cs` capability/draft validation using the existing context factory: validate exactly 32 random bytes / 43-character base64url, SHA-256 binary(32) digest, and deterministic versioned length-delimited intent hashing of trimmed name/phone, Type, stable table code and sorted product IDs/quantities; exclude reviewed prices/server fields and never log credentials or return hashes (after T023; FR-005/007/009/010/013).
- [X] T025 [US2] Implement read-only review in `CafeManagement.Api/Application/OrderService.cs`: validate guest fields/nonempty unique selections, resolve DineIn table via existing PublicCode or require Pickup no table, load available products, calculate authoritative checked decimal line/total amounts within decimal(18,2), and return Review or safe field/not-found/unavailable errors; no reservation or order write (after T024; FR-003/004/005/006/013).
- [X] T026 [US2] Implement atomic new placement in `CafeManagement.Api/Application/OrderService.cs`: short RepeatableRead, stable-ID product/table reads, revalidate availability and complete reviewed-price vector, calculate from DB prices only, persist name/price/table-label snapshots plus name/phone, server reference/time, Submitted order and all lines, then commit; enforce nonempty lines/summed total and exact representability; no locks while waiting for user confirmation (after T025; FR-003/004/005/006/007/008/013).
- [X] T027 [US2] Add accepted-outcome lookup before current catalog validation and unique-digest race recovery in `CafeManagement.Api/Application/OrderService.cs`: same capability/intent returns saved result, changed intent conflicts; rollback/dispose failed insert and fresh-context lookup only for the relevant duplicate index; ambiguous commit/deadlock/cancellation yields safe recovery guidance, never automatic write retry/new credential (after T026; FR-005/007/009).
- [X] T028 [US2] Add price_changed/product_unavailable/submission_conflict results to `CafeManagement.Api/Application/OrderService.cs` and `CafeManagement.Api/Contracts/OrderContracts.cs`; any price increase/decrease returns refreshed Review with zero inserts, later confirmation rechecks again, unknown/extra price entries fail validation; retain existing `CafeManagement.Api/Contracts/ApiErrors.cs` behavior and add only the safe endpoint-specific review extension needed by the contract (after T027; FR-005/006/007/011).
- [X] T029 [US2] Add minimal capability-only tracking/outcome read in `CafeManagement.Api/Application/OrderService.cs`, projecting saved reference/type/table snapshot/line snapshots/EGP amounts/status/reason/time with no name/phone/internal IDs/hashes/rowversion/Identity data; missing/invalid/unknown capability is the same safe failure; never use public reference/contact/table code as credentials (after T028; FR-007/009/010/013).
- [X] T030 [US2] Add anonymous JSON POST `/api/orders/review` in `CafeManagement.Api/Controllers/PublicOrdersController.cs`, extracting opaque guest bearer explicitly and requiring existing antiforgery header/cookie, common validation and safe 200/400/404/409 ProblemDetails behavior; no JWT/Identity-scheme replacement, staff-cookie ownership, write or private-order lookup through review (after T029; FR-005/006/010/013).
- [X] T031 [US2] Add JSON POST `/api/orders` to `CafeManagement.Api/Controllers/PublicOrdersController.cs` using the same protections: 201 new, 200 saved replay, safe 400/404/409 review/conflict results, token-free `/orders/track` relative path and no bearer Location header; reject prohibited/unknown fields and preserve global error handling (after T030; FR-003/005/006/007/010/013).
- [X] T032 [US2] Add anonymous GET `/api/orders/tracking` to `CafeManagement.Api/Controllers/PublicOrdersController.cs` requiring capability-only lookup, generic 404 for missing/invalid/unknown context and minimal DTO; use it for read-only uncertain-outcome recovery, no public lookup/list/status mutation endpoint (after T031; FR-007/009/010/013).
- [X] T033 [US2] Register OrderService scoped and apply the named guest limiter only to new review/place/tracking HTTP endpoints in `CafeManagement.Api/Program.cs` and `CafeManagement.Api/Controllers/PublicOrdersController.cs`; verify no-store, JSON 415, 429/Retry-After, safe failures and unchanged original Identity/Admin/API behavior (after T032; FR-009/010/011/012).
- [X] T034 [P] [US2] Create `CafeManagement.Api/wwwroot/js/order-session.js` helpers for Web Crypto 32-byte capability generation/base64url, sessionStorage read/write verification, started flag/reference metadata and trusted-origin private-link construction, and wire their deterministic loading in `CafeManagement.Api/Components/App.razor`; verify helper availability after interactive render before any T035 use, retain credential before first review/write, reserve separate pending-attempt and active-tracking storage keys, persist no contact data/cart, block on storage failure, and never replace an uncertain attempt or log its secret (after T023; can run beside T024–T033 on distinct files; FR-007/009/013).
- [X] T035 [US2] Create `CafeManagement.Api/Components/Shared/CheckoutPanel.razor` within the menu root: labeled required name/phone fields, technical capacities, type/table/cart review and server-direct read-only review using the registered OrderService and T034's loaded helpers; retain capability through JS first, invalidate review on any draft edit, display old/new unit prices/line totals/total and no registration/payment/address controls (after T025/T033/T034; FR-003/005/006/011/013).
- [X] T036 [US2] Add explicit placement and reconfirmation states to `CafeManagement.Api/Components/Shared/CheckoutPanel.razor`: send the unchanged reviewed-price vector, disable duplicate clicks, require Confirm updated prices and place order after any change, repeat on a further change, correct unavailable selections explicitly, and render saved reference/details/Submitted status with private-link access; no automatic resubmit or false success (after T028/T033/T035; FR-005/006/007/008/011).
- [X] T037 [US2] Embed CheckoutPanel beside CartPanel in `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` and add explicit read-only Check order outcome recovery in `CafeManagement.Api/Components/Shared/CheckoutPanel.razor` using T029 and retained session metadata; keep review/back in the same root, validate visible table, preserve `Components/Routes.razor`, and build private links with trusted PublicWebBaseUrl; found results use saved prices, not-found during an in-flight save remains inconclusive, and only unchanged-intent/capability explicit retry is allowed, never changed-cart replay/new token after reload or credential prerender (after T036/T029; FR-003/007/009/010/011/012).
- [X] T038 [US2] Create a minimal functional `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` at `/orders/track` with PublicLayout/Interactive Server and extend the already-loaded `CafeManagement.Api/wwwroot/js/order-session.js` for after-render fragment validation, separate active-tracking storage and fragment removal; use T029 to show saved reference/type/table snapshot/items/EGP total/current status and rejection reason when applicable, same-tab reload and safe missing/invalid/unknown/load-error outcomes without registration, prerendered private data, contact fields or fallback lookup; preserve any pending attempt and verify a freshly opened checkout link resolves its matching order before US2 completion (after T037/T029/T033/T034; FR-007/008/009/010/011/013).
- [X] T039 [US2] Extend `CafeManagement.Api/wwwroot/js/reconnect-status.js` submit protection to order write forms while preserving Admin guards/inert/reload behavior; mark checkout forms in `CafeManagement.Api/Components/Shared/CheckoutPanel.razor`, verify the T034 bootstrap remains unchanged and server busy guards/service checks authoritative, exercise T037's explicit read-only recovery after reload, and never auto-replay writes (after T038; FR-007/010/011/012).
- [X] T040 [US2] Manually validate review/place/recovery HTTP contracts and equivalent direct component checks using `specs/002-customer-ordering/contracts/api.md`: missing/invalid/mispaired CSRF rejection, correct pair, malformed input/bearer, unknown fields, price/total/status/role/table-ID tampering, generic failures and unavailable filtering; record safe outcomes in `specs/002-customer-ordering/tasks.md` (after T039; FR-003/005/006/009/010/013, SC-003/004/005).
- [X] T041 [US2] On the owned validation target exercise `CafeManagement.Api/Application/OrderService.cs` with parallel same-capability submissions, changed-intent conflicts, price/availability edits versus transaction locks, rollback/no partial lines, numeric overflow, lost-success-response recovery and restart/historical snapshot checks; record actual row counts and preservation results in `specs/002-customer-ordering/tasks.md`, never alter original records (after T040; FR-005/006/007/012, SC-003/004/007).
- [X] T042 [US2] Run restore/build and manually complete US2 scenarios for two distinct existing table links, required guest details, price increase/decrease/rechange with explicit confirmation and correct saved associations; open each checkout-provided private link in a fresh signed-out browser to T038's working page, verify matching saved details/Submitted status, reload and negative missing/invalid-link privacy outcomes without any US4 work; use `specs/002-customer-ordering/quickstart.md` and record evidence here, with physical scans pending if HTTPS reachability is unavailable (after T041/T038/T039; FR-003/005/006/007/008/009/010/011/013, SC-002/003/004/005/006/008).

## Phase 5: US3 — Place an Online Pickup Order (P1, T043–T048)

**Goal**: Direct no-QR browsing/checkout using the same service, tables and lifecycle.
**Independent verification**: Fresh signed-out visit without a table → pickup cart, required
guest details, exactly one order per submission with null table fields and a private link.
US2 supplies shared acceptance/recovery; no dine-in interaction is needed to use pickup.

- [X] T043 [US3] Add a separate pickup catalog read to `CafeManagement.Api/Application/PublicMenuService.cs` and additive response shape in `CafeManagement.Api/Contracts/PublicMenuContracts.cs`, reusing available category/product projection with EGP/two-place prices and no table/contact fields while leaving the existing table response/validation unchanged (after T042; FR-001/004/012).
- [X] T044 [US3] Add anonymous no-store GET `/api/menu/pickup` in `CafeManagement.Api/Controllers/PublicMenuController.cs`, including empty/load-error behavior and no table requirement; verify invalid table endpoints still fail rather than becoming pickup (after T043; FR-001/004/011/012).
- [X] T045 [US3] Create `CafeManagement.Api/Components/Pages/Menu/PickupMenu.razor` at `/menu/pickup` using PublicLayout, Interactive Server, shared MenuCatalog/CartPanel/CheckoutPanel and circuit CartState, explicit Pickup label and null table context; reuse guest validation, price reconfirmation and capability recovery without a second order service (after T044; FR-001/002/004/005/006/007/011/013).
- [ ] T046 [US3] Add direct pickup discovery in `CafeManagement.Api/Components/Layout/PublicLayout.razor` and integrate context-leaving warnings with `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor` / `PickupMenu.razor`; verify leave clears unsubmitted cart, stay retains it, no hidden table/type transfer or forced QR step, and existing Admin/login navigation remains intact (after T045; FR-003/004/011/012).
- [X] T047 [US3] Manually place two pickup orders without QR/table, reject any supplied table code, validate missing/blank/overlong name/phone, unavailable products and both price directions/rechanges, then inspect shared Orders/OrderLines/status/guest recovery and open/reload each private link in a fresh signed-out browser through the existing US2 tracking page with no table association; record outcomes in `specs/002-customer-ordering/tasks.md` using `quickstart.md`, without staff/payment features (after T046/T038; FR-004/005/006/007/008/009/013, SC-002/003/004/005/008).
- [X] T048 [US3] Run restore/build and pickup independent acceptance plus table-menu/API-shape regression from `specs/002-customer-ordering/quickstart.md`; verify signed-out direct entry, same-name distinct products, no table association, no exposure of private fields, and original preservation fingerprints; record evidence in `specs/002-customer-ordering/tasks.md` (after T047; FR-001/004/010/011/012, SC-002/006/007).

## Phase 6: US4 — View the Intended Order's Status (P2, T049–T055)

**Goal**: Refine the functional private tracking delivered in US2 and reused by US3; complete
robustness, usability and all-status validation without registration or staff actions.
**Independent verification**: Use already-accepted or isolated prepared orders and a private
link in a second signed-out browser; read all five statuses/rejection reason, reject unrelated
access contexts, and reload. No staff screen/status-update endpoint is needed.

- [X] T049 [US4] Refine the T034/T038 helper in `CafeManagement.Api/wwwroot/js/order-session.js` for malformed/replaced fragments, blocked/cleared storage, repeated link navigation and trusted-origin copy-link support; preserve functional initial parsing, separate pending-attempt/active-tracking keys, after-render-only credential access, fragment removal and same-tab reload, ensuring a new tracking link cannot overwrite a pending submission or silently fall back to a stale credential on an explicitly invalid fragment (after T048/T038; FR-007/009/010/013).
- [X] T050 [US4] Refine the existing T038 page `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` with accessible loading/error/retry focus and latest-navigation-wins handling for slow reads or a replaced link; preserve the functioning `/orders/track` route, token-free prerender, minimal DTO and identical invalid/missing/unknown feedback, never displaying a previous order during a new-context load/error or adding contact/reference/table fallback or staff UI (after T049/T038; FR-009/010/011/013).
- [X] T051 [US4] Add explicit reload/copy-private-link actions and bearer-sharing warning in `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` and `CafeManagement.Api/Components/Shared/CheckoutPanel.razor`; verify links contain secrets only in fragments at runtime, use trusted PublicWebBaseUrl not Host, survive same-tab reload after fragment removal, and expose no raw credential in prerendered HTML/server logs (after T050; FR-007/009/010/011/013).
- [X] T052 [US4] Validate/refine the existing Submitted/Preparing/Ready/Completed/Rejected and required-reason display in `CafeManagement.Api/Components/Pages/Orders/TrackOrder.razor` using owned isolated prepared examples; verify `CafeManagement.Api/Application/OrderLifecycle.cs` allows exactly four approved transitions, rejects every other pair/reason omission, and leaves terminal states terminal; no duplicate tracking page, committed automated test suite or production mutation endpoint (after T051; FR-008/009, SC-005).
- [X] T053 [US4] Manually check `CafeManagement.Api/Controllers/PublicOrdersController.cs` and `Components/Pages/Orders/TrackOrder.razor` against missing/invalid/unknown/wrong-order capability, public reference/name/phone/table-only attempts, staff-cookie-only requests, response projection, no-store/no-referrer and sanitized failures; confirm zero private contact/hash/Identity exposure and zero customer status mutation, recording results in `specs/002-customer-ordering/tasks.md` (after T052; FR-009/010/013, SC-005/008).
- [X] T054 [US4] Open copied dine-in/pickup links in a separate signed-out browser, reload current states after isolated prepared changes, recover confirmation after lost reply/restart and simulate unavailable service/storage loss; verify sharing grants only the matching order and missing access never becomes a contact lookup; record actual outcomes in `specs/002-customer-ordering/tasks.md` (after T053; FR-007/009/011/013, SC-003/005/008).
- [X] T055 [US4] Run restore/build and all US4 acceptance scenarios using `specs/002-customer-ordering/quickstart.md`; verify no signup requirement, five statuses/reason, exact immutable amounts, privacy boundaries and no automatic write replay; record results and any blocked physical-phone check in `specs/002-customer-ordering/tasks.md` (after T054; FR-008/009/010/011, SC-004/005/006).

## Phase 7: Polish and Cross-Cutting Validation (T056–T063)

**Goal**: Prove integration and preservation; no unrelated refactor, optimization or new feature.
These are manual/build validations and narrowly necessary fixes, not automated-test generation.

- [X] T056 Repeat cross-flow SQL/concurrency checks for `CafeManagement.Api/Application/OrderService.cs` and `Infrastructure/Persistence/Configurations/OrderConfiguration.cs` / `OrderLineConfiguration.cs`: parallel duplicate placement, Admin price/availability edits, lost commit response, rollback atomicity, context/tab isolation, order restart persistence and rowversion stale-update detection using isolated fixtures only; record evidence in `specs/002-customer-ordering/tasks.md` (after T055; FR-005/006/007/008/012, SC-003/004/007).
- [X] T057 Review `CafeManagement.Api/Program.cs`, `Controllers/PublicOrdersController.cs`, `Application/OrderService.cs`, browser modules and existing StaffAccess/Identity handlers; validate CSRF, JSON/unknown-field rejection, rate limiter/Retry-After, direct-service checks, cookie/staff boundaries, safe logging/errors/headers and no bearer in requests' URLs/prerender/source; record results in `specs/002-customer-ordering/tasks.md`, without changing auth policies or networking (after T056; FR-009/010/012/013, SC-005/007/008).
- [X] T058 Validate menu/cart/embedded checkout/tracking at 320/768/1280px in `CafeManagement.Api/Components/Pages/Menu/`, `Components/Shared/CheckoutPanel.razor`, `Components/Pages/Orders/TrackOrder.razor` and `wwwroot/css/site.css`; check no horizontal scrolling, touch/keyboard labels/focus, category/cart retention, empty/validation/error/reconnect/price-change feedback; make only demonstrated scoped fixes and record manual evidence here (after T057; FR-001/002/003/004/011, SC-001/002/006).
- [ ] T059 Verify real-phone QR dine-in entry/rescan and pickup/private-link journeys using `CafeManagement.Api/Components/Pages/Menu/TableMenu.razor`, `PickupMenu.razor`, `Components/Pages/Orders/TrackOrder.razor` and trusted `Application/PublicMenuOptions.cs` only if an approved securely reachable HTTPS origin exists; otherwise record exact blocker and leave this task pending in `specs/002-customer-ordering/tasks.md`; no firewall/certificate/network changes or automatic completion of Feature 001 T051/T056 (after T058; FR-003/004/009/011/012, SC-002/005/006/007).
- [X] T060 Run `dotnet restore CafeManagement.sln`, `dotnet build CafeManagement.sln --no-restore`, `dotnet build CafeManagement.sln -c Release` and `dotnet publish CafeManagement.Api -c Release --no-restore -o artifacts/cafe`; inspect/pilot the published build locally with approved configuration, compiled pages/local Bootstrap/browser JS and no secrets/DB/certificate files; record results in `specs/002-customer-ordering/tasks.md` (after T058; independent of blocked T059; FR-011/012, Constitution VI).
- [X] T061 Verify original catalog/table/QR/Identity fingerprints, existing Admin/Cashier authentication, menu APIs, `/WeatherForecast`, Swagger and unknown API JSON 404 using `specs/002-customer-ordering/quickstart.md`; stop only owned validation hosts and clean only explicitly owned disposable data, preserving original records and local configuration; record outcomes here (after T060; independent of blocked T059; FR-010/012, SC-007).
- [X] T062 Review final Git diff and actual routes/DTOs against `specs/002-customer-ordering/spec.md` and contracts; confirm no payment/delivery/POS/report/status-mutation/contact-lookup features, dependency/framework replacement, source removal or mandatory tests, and no generated outputs, credentials or private links to stage; record scope/security review in `specs/002-customer-ordering/tasks.md`, preserving unrelated working edits (after T061; FR-008/009/010/012/013, SC-007/008, Constitution II/VII).
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

### Order-line mapping, lifecycle and additive migration evidence (T009–T013, 2026-10-27)

- Started from clean master at c9514564df7102a60f98abc71ac576c8b69616e7 with T001–T008
  complete. Reviewed exact selected dependencies and approved artifacts/Constitution; the
  quality checklist remains 16/16. Feature 001 remains byte-identical with 54 tasks complete
  and only T051/T056 pending. No extension hooks or additional repository instructions.
- T009: Added OrderLineConfiguration with decimal(18,2) UnitPrice/LineTotal, required
  120-character snapshot name, positive quantity/amount checks, widened decimal(29,2)
  arithmetic comparison, unique (OrderId, ProductId), ProductId index and restrictive
  order/product foreign keys. EF model inspection now emits zero decimal precision warnings.
  Nonempty lines and summed Order.Total remain future transactional service invariants;
  this task does not claim SQL CHECK enforcement of cross-row rules.
- T010: Added internal OrderLifecycle.TryTransition with only Submitted → Preparing,
  Preparing → Ready, Ready → Completed and Submitted → Rejected. Rejection requires a
  trimmed nonblank reason of at most 500 characters. Completed/Rejected are terminal;
  invalid transitions leave the order unchanged. No public status setter, endpoint or staff UI.
  In-memory reflection verified all 25 state pairs, six rejection boundaries (including
  Unicode whitespace, 500/501 characters and trimming), inappropriate non-rejection reasons
  and unknown status rejection. No test project/framework or diagnostic file was added.
- T011: Added Orders/OrderLines DbSets only; existing assembly-based configuration discovery
  applies both mappings. Existing DbSets, Identity configuration, context factory registration,
  Program.cs, authentication and routes are unchanged. All ten prior EF table definitions
  and ten prior indexes matched the before-model SQL exactly.
- T012: Generated and fully reviewed the additive migration:
  - CafeManagement.Api/Infrastructure/Persistence/Migrations/20261027141549_AddCustomerOrders.cs
  - CafeManagement.Api/Infrastructure/Persistence/Migrations/20261027141549_AddCustomerOrders.Designer.cs
  - CafeManagement.Api/Infrastructure/Persistence/Migrations/CafeDbContextModelSnapshot.cs
  Generated SQL is retained only in ignored artifacts/customer-orders.sql. Up creates only
  Orders/OrderLines, 13 checks, three NO ACTION foreign keys, two primary keys and six secondary
  indexes, then appends migration history. Existing tables/data and initial migration are
  untouched. Designer and snapshot model bodies match; EF reports no pending model changes.
  The generated Down method drops only the new tables and was never executed.
- T013: Rechecked all original/copy row fingerprints and the owned-copy marker. Revalidated
  the T003 COPY_ONLY/CHECKSUM backup header and RESTORE VERIFYONLY WITH CHECKSUM; its original
  data fingerprints still matched. Applied the reviewed migration first to the owned
  CafeManagement_F002_Validation_20261027_134110_c57ad692 database using a process-only
  connection override, then to the confirmed CafeManagement database after isolated success.
  Local user secrets/default connection and backup remain unchanged outside Git.
- All 32 temporary SQL probes on the owned validation copy passed and rolled back: valid
  pickup/dine-in, maximum decimal/int values, invalid names/quantities/amounts/arithmetic,
  widened-product rejection without arithmetic overflow, decimal storage overflow, duplicate
  products/reference/digest, missing order/product FKs, restrictive parent deletion, and
  header type/status/currency/contact/table/reason constraints. No probe records remain.
  These are scoped manual diagnostics, not a generated or committed automated test suite.
- Physical verification passed on both targets: 13 enabled/trusted checks, three enabled/
  trusted NO ACTION FKs, six expected secondary indexes/uniqueness, decimal(18,2) money and
  required rowversion. Both migration histories contain the unchanged initial EF 8.0.33 row
  plus 20261027141549_AddCustomerOrders. Both new tables are empty on both targets.
- All ten pre-existing data-table row counts and full-row SHA-256 fingerprints matched
  before/after, including five table/QR records, eight categories, eight products and two
  Identity users/roles/memberships. No original account or record was changed. No reset,
  reseeding, startup migration, account provisioning, QR regeneration or downgrade occurred.
- Solution restore passed; final dotnet build CafeManagement.sln --no-restore passed with
  0 warnings/0 errors (5.12 seconds). EF has-pending-model-changes passed. Before updating
  this evidence, 106 pre-existing source/configuration/planning files were byte-identical
  except the approved context and model snapshot edits. Reviewed source/migration/task diff
  and ignore/secret boundaries; build outputs and SQL artifacts stay excluded.
- T009–T013 are complete. T014–T063 remain unchecked and were not implemented. No cart,
  ordering API/UI, POS, payments, delivery, network, certificate or package changes.

### Value contracts and HTTP security foundation evidence (T014–T016, 2026-10-27)

- Started from clean master at 7ba0d7f70e8e111c46b5a0ea2eb1d18ed4b87d93 with exactly
  T001–T013 complete. Reviewed the selected task definitions/dependencies, approved spec,
  planning/contracts, requirements and Constitution. Specification checklist remains 16/16.
  No additional repository instructions or implementation extension hooks were present.
- T014: Added Contracts/OrderContracts.cs with OrderDraft, OrderDraftItem, ReviewedPrice,
  OrderPlacementInput, review/table/item projections, minimal tracking/item projections and
  placement result. Guest fields are trimmed, nonblank and limited to 120/40 characters
  without truncation or regional formatting rules. DineIn requires a nonempty UUID and
  Pickup rejects non-null table context. Items require distinct positive IDs and positive
  int quantities; JSON numbers must be whole and within int range.
- Reviewed prices require positive invariant two-place strings within decimal(18,2), with
  exactly matching product IDs and no duplicates/extras. Write DTOs reject unmapped JSON
  properties locally, preserving all existing API serializer behavior. Direct draft/placement
  validation shares the same rules for later service/component calls. Database existence,
  availability and authoritative calculations remain the later OrderService tasks.
- All 51 in-memory manual JSON/value examples passed: valid pickup/dine-in, required fields,
  trim/Unicode whitespace, 120/121 and 40/41 boundaries, null/empty/duplicate items, positive/
  fractional/overflow/string quantities, UUID/context rules, matching reviewed IDs and
  exact price formatting/capacity, and unknown/prohibited root/nested fields. Inspected all
  six public response shapes: no contact fields, internal order/table IDs, digest, rowversion,
  Identity data or credential. Product IDs remain permitted in review; trackingPath is fixed
  to the token-free /orders/track. A PowerShell assembly-resolution issue in the initial
  diagnostic was corrected in memory; no application defect or test file was involved.
- T015: Added named opt-in GuestOrders fixed-window HTTP policy in Program.cs. Initial
  setting is 60 permits per minute per actual remote IP, queue zero; GuestOrders:PermitLimit
  is configurable and must be positive. Rejection callback supplies 429 ProblemDetails and
  invariant whole-second Retry-After. No global limiter, forwarded-IP trust, new auth scheme,
  permissive CORS or change to existing Identity/antiforgery policy.
- Explicit routing precedes authentication/rate limiting/authorization/antiforgery. Middleware
  sets no-store/no-referrer for /menu/t/*, /menu/pickup and /orders/track, and no-store for
  /api/orders responses, including safe failures. OnStarting preserves these headers if
  downstream error handling clears the response. Prefix boundaries do not capture unrelated
  paths. Existing JSON error behavior and global router are unchanged.
- Started and stopped one owned loopback-only HTTPS host at https://localhost:7246 against
  the existing owned validation database using process-only overrides. Existing certificate
  validation succeeded normally; no certificate/firewall/network/configuration file changes.
  Verified anonymous table API/document browsing, header behavior on existing/future paths
  and failures, case/trailing slash matching, unrelated prefix paths, existing Swagger,
  antiforgery token plus Secure/HttpOnly cookie, /api/auth/me 401, and all three Admin API
  collections 401 when signed out. Sixty-five WeatherForecast reads all returned 200,
  confirming existing endpoints were not globally rate-limited.
- Inspected diagnostic configuration: no HTTP body/header or EF sensitive-data logging
  enabled. Synthetic non-secret bearer/contact/body markers sent to the unimplemented order
  path did not appear in host logs. Log files are ignored artifacts only.
- Limits of this foundation check: no order endpoints, service, cart or UI exist yet. Applying
  GuestOrders to those routes and real endpoint-level 429/Retry-After, guest CSRF, availability,
  price-reconfirmation and placement/tracking checks remain T024–T042 (especially T033/T040).
  The prepared policy/callback was reviewed; no claim of an exercised order HTTP limiter.
  Signed-in full workflow regression remains later story validation; no original login/account
  state was changed merely to demonstrate access.
- T016: Inspected the isolated installed schema: 13 enabled/trusted checks, three restrictive
  NO ACTION FKs, six secondary indexes with expected uniqueness, required rowversion and all
  three decimal(18,2) money columns. Verified exact type/length/nullability for 12 selected
  name/phone/table/reason/type/status/currency/hash/reference/quantity fields. Existing physical
  checks/constraints match the reviewed migration validated in T013; no migration was added.
- Before/after counts and full-row SHA-256 fingerprints matched all 13 tables on both
  CafeManagement and CafeManagement_F002_Validation_20261027_134110_c57ad692, including the
  existing catalog, table/QR records, Identity accounts/roles and both migration entries.
  Orders/OrderLines remain empty. No database mutation/reset, provisioning or QR regeneration.
- Final solution restore and build passed: 0 warnings/0 errors (1.55 seconds).
  EF has-pending-model-changes passed. No package changes, test projects, automated tests,
  POS, payments, delivery, production settings or unrelated features were added.
- Only OrderContracts.cs, Program.cs and this task evidence change. Existing entities,
  migrations, configuration and Feature 001 task history remain preserved. T014–T016 complete;
  T017–T063 remain unchecked and unimplemented. Feature 001 still has 54 complete tasks with
  only T051/T056 pending; it is not claimed fully accepted.

### Shared menu and cart evidence (T017–T020, 2026-10-27)

- Started from clean master at c20bf16ffd61239f39b618d6abe8a1beee5442c0, with exactly
  T001–T016 complete and specification checklist 16/16. Reviewed the selected task definitions,
  dependencies, approved specification/plan/contracts and Constitution. No implementation hooks
  or additional repository instructions were found. Feature 001 history remains unchanged.
- T017: Added Application/CartState.cs with immutable display-line values, one row per product
  ID, positive int quantities, last displayed decimal prices/names, order type/table code,
  add/update/remove and exact checked line/total calculations within decimal(18,2). Invalid
  input or arithmetic/capacity failure leaves the accepted cart unchanged. No EF entities,
  contacts, credentials, static/shared data, persistence or arbitrary cafe quantity cap.
- In-memory manual checks passed for repeated additions, separate same-name product IDs,
  exact 25.10 total, quantity edits/removal/empty state, maximum int quantity and add overflow,
  exact maximum monetary value, line/total overflow atomicity, nonpositive/overprecision prices,
  stable versus changed table context, independent instances and last-displayed-price refresh.
  An initial PowerShell decimal-literal conversion rounded the diagnostic's maximum value;
  corrected the diagnostic to invariant decimal string parsing. Application code was unchanged.
  No test project/framework, automated test files or committed diagnostics were introduced.
- T018: Added Components/Shared/MenuCatalog.razor using existing public category/product
  projections, category/menu empty-state branches, EGP/two-place prices and add callbacks by
  product ID. Category headings remain focusable. Links target the exact current document
  plus category fragment, retaining query/path context and avoiding the root base-href trap.
  Existing available-only SQL projection and public API contracts remain unchanged.
- T019: Added Components/Shared/CartPanel.razor with associated quantity labels, explicit Remove,
  displayed unit/line/total amounts, empty feedback, live amounts and field-specific accessible
  errors. Zero, negative, fractional, empty/out-of-int-range text is rejected without changing
  accepted values. The optional future review callback is hidden when unwired and guarded
  against empty carts or unresolved quantity errors; no checkout flow is implemented.
- T020: Integrated both components within the existing anonymous Interactive Server table page,
  keeping /menu/t/{PublicCode}, PublicLayout, visible table, error/reload states and unchanged
  latest-navigation-wins version guard. Page-owned CartState is shared within this root;
  scoped DI registration remains T022. Same-table menu reload preserves selections; a different
  context never silently transfers them. Navigation warning/full-reload explanation remain T021.
  Program.cs, Routes.razor, authentication, table/QR services and database mappings are untouched.
- Manual signed-out browser checks on an owned localhost-only HTTPS validation host passed:
  four available products, existing empty categories, repeated additions giving quantity 2
  and combined EGP 85.60, increase to 3 giving EGP 136.80, decrease/removal and empty feedback.
  Actual keyboard edits of 0, -1, 1.5 and 2147483648 showed field feedback and retained accepted
  amounts; corrected input cleared the error. Programmatic fill without a real change event
  was replaced by real keyboard edits for this manual verification.
- Category navigation retained quantity 2 and focused heading category-3 with no full reload;
  subsequent interactive quantity controls worked. Reload menu preserved the same table/cart.
  Invalid table link showed the existing safe error with no catalog/cart or pickup fallback;
  a second existing table link displayed its correct label and an empty cart.
- Browser inspection found and corrected two new-markup issues before completion: a literal
  string component currency parameter and root-relative fragment resolution under Blazor's
  base tag. Confirmed EGP display and same-document/focus/cart behavior after rebuilding.
  Expected negotiation warnings occurred while the owned host was deliberately stopped for
  that rebuild; a fresh signed-out session subsequently added a product with zero browser
  warnings/errors. No certificate validation bypass or network changes were used.
- Responsive browser screenshots and DOM measurements passed at 320/768/1280px. Document
  widths were 305/753/1265px respectively (vertical scrollbar excluded), with no horizontal
  page overflow. Menu/cart stack at narrow/tablet widths and use two columns at desktop;
  labels, long existing names, prices and controls wrap within their cards. Temporary viewport
  overrides were reset and all agent-created browser tabs/owned hosts were closed.
- Existing API checks passed: four available public products, anonymous Admin category/product/
  table collections and /api/auth/me return 401, malformed table API returns safe JSON 400,
  menu no-store/no-referrer retained, Swagger and WeatherForecast return 200. Fully empty
  catalog branch was code-reviewed/compiled; original catalog data was not altered to stage it.
  Full US1 regression, navigation guards and DI/circuit lifetime acceptance remain T021–T023.
- Before/after full-row SHA-256 fingerprints and row counts matched all 13 tables on both
  CafeManagement and the owned CafeManagement_F002_Validation_20261027_134110_c57ad692 copy.
  Existing records, accounts, roles, table public QR identifiers and both migrations are intact;
  Orders/OrderLines stay empty. No database mutation/migration, account provisioning or reset.
- Final restore/build passed with 0 warnings/0 errors (1.58 seconds); EF pending-model check
  found no changes. Final scope/secret/artifact review includes only CartState.cs, MenuCatalog.razor,
  CartPanel.razor, TableMenu.razor and this evidence. No dependency, automated test, checkout,
  POS, payment, delivery, firewall/certificate or unrelated change.
- T017–T020 complete. T021–T063 remain unchecked and unimplemented. Feature 001 remains at
  54 completed tasks with only T051/T056 pending; no complete-feature acceptance is claimed.

### Navigation validation continuation (T021–T023, 2026-10-28)

- Preserved all four interrupted source edits: CartState.Clear, NavigationLock and scoped
  injection in TableMenu, the temporary-cart explanation in CartPanel, and scoped CartState
  registration in Program.cs. No additional application edits were made during this
  continuation. T001–T020 remain complete; T024 and later were not started.
- Quality checklist remains 16/16 and no implementation extension hooks are present.
  Reviewed exact T021–T023 criteria against the approved UI/plan/Constitution. Routes.razor,
  Identity handlers, public menu filtering and database migrations remain unchanged.
- Interrupted validation had left all products unavailable on the explicitly owned copy.
  Restored only its temporary IsAvailable values from the unchanged original baseline after
  confirming ownership and identical IDs/names/prices/categories. All 13 copy-table row
  counts/full-row SHA-256 fingerprints then matched the original baseline exactly.
- Confirmed Leave logic without browser-security workarounds: invoked the actual compiled
  TableMenu.BeforeNavigationAsync in a disposable in-memory manual diagnostic, supplying a
  confirmation answer through IJSRuntime. Same-document category navigation made zero
  confirmation calls and retained the item. Stay called PreventNavigation and retained it.
  Leave cleared the cart and left navigation unprevented; an empty cart did not prompt.
  This proves the component branch, not an actual native before-unload dialog acceptance.
  A diagnostic-only System.Runtime 8/10 reference warning was handled in the PowerShell
  host; no application framework, build setting, security validation or source changed.
  No automated test files/project/framework or committed diagnostic was created.
- Supported in-app browser checks passed again: two additions merged to quantity 2 / EGP
  51.20; a category fragment retained the cart and focused its heading; an independent second
  tab had an empty cart. Real keyboard edits of 0/-1/1.5/2147483648 produced field feedback
  without changing accepted amounts; correction to 3 yielded EGP 76.80; Remove emptied it.
- Earlier interrupted browser observations remain valid for the unchanged implementation:
  two existing same-name products, temporarily available only on the owned copy, stayed
  distinct at EGP 25.60 and 25.50 with EGP 51.10 total; full empty-menu/category/cart feedback
  rendered after a temporary availability fixture; an intentionally unavailable-DB host
  rendered the generic load error and Retry loading menu. All fixtures were subsequently
  restored; no original catalog/account data was changed.
- Existing staff regression now passed over normally validated loopback HTTPS using only
  operator-local secrets in memory: both Admin and Cashier signed in and /api/auth/me returned
  their existing roles. Admin category/product/table APIs and screens returned 200, QR PNG
  read returned 200, and a no-CSRF write was rejected with 400. Cashier category/product/table
  Admin APIs returned 403. Credentials/cookies were never printed, stored in Git or changed.
- Original and owned-copy fingerprints matched for all 13 tables after those logins,
  including unchanged Identity hashes/stamps/roles, catalog, table/QR values and migration
  history. Orders/OrderLines remain empty. No database schema changes/reset/provisioning.
- Restore and final build passed with 0 warnings/0 errors (2.12 seconds). EF reported no
  pending model changes. Diff review found only the four intended source files plus this
  evidence; no unrelated feature, secret, generated build output or new dependency.
- Pending browser gates: the supported browser API cannot accept a native before-unload
  confirmation. Native Windows automation previously halted because Edge's URL could not be
  identified reliably; it was not retried or bypassed. The operator was asked to verify Stay
  and Leave/Reload manually; no result has been supplied, so native acceptance is unverified.
- A current responsive rerun also remains pending: the viewport capability accepted 320/768
  requests but measured/rendered at 1280 throughout. Desktop rendered within its page width;
  do not report a new mobile/tablet pass from these unsuccessful overrides. Earlier T017–T020
  viewport results do not substitute for this fresh check of the cart notice/guard increment.
  Temporary viewport overrides were reset and agent-created verification tabs were closed.
- T021 remains partially verified; T022's scoped-registration/tab-isolation criteria passed
  but its dependency gate remains T021; T023 has the verified checks above and the remaining
  browser gates. All three completion markers intentionally remain unchecked. Nothing was
  staged, committed or pushed because the selected task set has not fully passed.
- An owned HTTPS host at https://localhost:7246 remains loopback-only for the requested manual
  confirmation check. Existing user secrets, production settings, certificates and firewall
  settings are unchanged. Stop that owned host after manual verification or at operator request.

### Independent backend continuation (T024–T025, 2026-10-29)

- User explicitly authorized independent backend development while browser-only T021–T023
  remain pending, overriding that validation-only execution gate without claiming US1 accepted.
  T024 then T025 were implemented in dependency order. No browser/UI integration was advanced.
- Preserved the six initial uncommitted files. An ignored binary patch at
  artifacts/resume-20261029-preserved.patch has SHA-256
  A00DCDC46CDAFC82627A0E9720C4FA3B4A981006E3ABB41BEF18735A5C2C7743.
  Initial application edits and earlier navigation evidence are intentionally excluded from
  this commit. Added pending-browser-validation.md for deferred checks; no privacy bypass.
- Requirements checklist still passes 16/16; no AGENTS.md or implementation extension hooks.
- T024: strict canonical 43-character base64url decoding to 32 bytes; SHA-256 on decoded bytes;
  validated/snapshotted drafts; versioned length-prefixed UTF-8 intent with sorted IDs/quantities,
  type, stable public table code and trimmed name/phone. Hashes stay private and are not logged.
- Focused disposable diagnostic passed malformed/padded/noncanonical capabilities, hash lengths
  and independently computed capability digest, sorted-item equivalence, credential-independent
  intent, each changed immutable field, delimiter collision resistance, null/blank/overlong
  contacts, invalid types/table modes, missing/null/duplicate items and invalid quantities.
- T025: fresh untracked context per read; authoritative product prices, whole quantities,
  checked decimal arithmetic, decimal(18,2) bounds and two-place EGP output. No reservation/write.
  Read-only owned-copy SQL checks passed Pickup/no-table, correct DineIn table code/label,
  missing table/product, unavailable product and required capability. Orders 0 -> 0 and
  OrderLines 0 -> 0; no fixtures, original catalog, accounts or QR records were mutated.
  Boundary diagnostic also passed invalid money/maximum amount, safe DB failure and cancellation.
- Local restore needed approved access to the existing NuGet configuration. Default build was
  blocked by a running app locking its executable; solution build with an ignored isolated
  OutputPath passed with 0 warnings/0 errors. No running user process was terminated.
  The ignored console diagnostic initially loaded the portable SQL stub, then lacked sandbox
  SQL encryption support; using the existing Windows driver and approved local SQL execution
  passed. No encryption/certificate restriction, dependency or application setting was changed.
  One nullable warning was confined to the disposable diagnostic, not the solution build.
- Only OrderService.cs, the two task markers/new evidence and pending-browser-validation.md
  belong to this validated batch. No endpoint/DI registration, placement, payment or POS added.

### Atomic placement and private tracking (T026–T029, 2026-10-29)

- Continued independent backend work in dependency order under the user's browser deferral.
  Reused the validated review core inside a short RepeatableRead acceptance transaction;
  table/products are authoritative, totals exact, and all lines/snapshots commit together.
- T026: owned-copy diagnostics saved one Pickup and one DineIn order with correct Submitted
  status, contact fields, table association/snapshot and exact current product prices/totals.
- T027: same capability/intent returned the saved reference without another insert; changed
  customer intent conflicted. Historical replay succeeded even after owned-copy name/price/
  availability edits. Parallel same-capability calls yielded exactly one created order; an
  explicit subsequent call recovered that same winner. Code inspection confirms recovery
  only for SQL 2601/2627 naming IX_Orders_CapabilityHash, after failed context/transaction
  disposal; no automatic write retry. Injected save failure returned uncertain-outcome
  guidance and left zero partial order/lines. Broader locking/commit-loss exercises remain T041.
- T028: both upward/downward reviewed-price mismatches returned price_changed/current Review
  without insertion; safe ProblemDetails includes review only for that error. Extra product
  prices rejected. Unavailable products identify public product IDs and reject the whole order;
  decimal(18,2) line overflow rejected before save. Existing ApiErrors behavior is unchanged.
- T029: capability-only read returned minimal saved snapshots; missing/malformed/unknown keys
  returned identical order_not_found errors. Serialized tracking omitted contact fields,
  internal IDs, digests and rowversion. Lost response can be recovered through this read;
  no public reference/contact/table lookup or staff/status endpoint was added.
- Owned copy baseline was Orders=0/OrderLines=0; three diagnostic orders/three lines were
  created in total (two sequential and one concurrent winner). Finally restored only the
  edited copy fixture values and removed only the recorded diagnostic references/lines.
  Counts returned to 0/0 and combined full-row fingerprints of all primary-key tables matched
  exactly before/after, including Identity, QR/catalog and migration history. No original DB
  writes, migration, account provisioning or reset occurred; copy identity counters may advance.
- Final solution build in the ignored isolated output passed with 0 warnings/0 errors.
  Manual diagnostic artifacts are ignored, not a committed test project/framework. Existing
  six unfinished files/evidence remain unstaged; T021–T023 and T030 onward remain pending.

### Guest HTTP boundary (T030–T033, 2026-10-29)

- Added only anonymous JSON review/place and capability-only tracking endpoints, explicit
  opaque Bearer extraction, existing antiforgery validation on POSTs and named GuestOrders
  limiter on this controller. OrderService registered scoped; Identity schemes unchanged.
- Owned loopback HTTPS host on port 7258 passed startup with normal certificate validation.
  Temporary HTTP client checks passed missing/invalid/mismatched CSRF rejection, a valid pair,
  malformed bearer, non-JSON 415, unknown status and fractional quantity rejection, 409 with
  current safe review, 201 new placement/no Location bearer and 200 saved replay.
- Tracking returned only minimal saved DTO/no-store; missing, malformed and unknown bearer
  returned generic 404. Review no-store and private-document no-store/no-referrer passed.
  Rate-limit exercise returned 429 with Retry-After/no-store; WeatherForecast still returned
  200 after guest exhaustion. Existing anonymous Identity/Admin requests remained 401 and
  Swagger returned 200. Full signed-in regression remains a later check.
- HTTP checks wrote exactly one owned-copy order/line, then removed only the recorded
  diagnostic reference; counts returned to their baseline. The diagnostic-owned host was
  stopped in finally. No original records, security settings, accounts or schema changed.
- Solution build passed 0 warnings/0 errors before HTTP validation. No automated test suite
  or new package added. Only new PublicOrdersController, OrderService registration and this
  evidence/task markers are staged. The pre-existing CartState registration and five other
  unfinished file changes remain unstaged. Browser-only gates and T034 onward stay pending.

### Independent pickup catalog (T043–T044, 2026-10-29)

- User's explicit direction to continue independent work permits the additive read-only pickup
  catalog while T042's browser acceptance remains pending. This backend read has no runtime
  dependency on checkout/private-link acceptance; no US2/US3 completion is claimed.
- T043/T044: added no-table PickupMenuDetails/Response, reused the existing available-only
  category query, and exposed anonymous no-store GET /api/menu/pickup. The existing table
  response and table-validation route retain their prior behavior.
- Owned-copy HTTPS checks passed EGP/no-table shape, available product IDs matching SQL,
  byte-equivalent category projection versus the existing valid-table response, and malformed
  table 400 without pickup fallback. Temporarily unavailable copy fixtures yielded empty
  product arrays with categories retained; availability was restored in finally. A throwing
  context-factory diagnostic returned the safe load error without exception details.
- Repeated affected order HTTP checks still passed, with one temporary order/line removed by
  its recorded reference and owned host stopped; no original DB/schema/account changes.
  Final solution build passed 0 warnings/0 errors. No new dependencies or test suite.
- Only the three backend catalog/controller/DTO files and this evidence/task markers are staged.
  T034–T039 and T045–T046 have new uncommitted implementation that builds, but interactive,
  navigation, reconnect and responsive acceptance remain unverified and unchecked. JS runtime
  diagnostics passed storage failure/canonical capability/metadata separation/fragment/link
  checks; HTTP confirmed helper delivery/order and credential-free prerender shells. These
  are not substituted for actual interactive browser checks.

### Independent deep backend validation and UI handoff (T041; partial later gates, 2026-10-29)

- Under the user's explicit independent-work instruction, T041's backend-only diagnostic
  matrix was exercised without claiming T040's component checks passed. Price and availability
  edits on the owned copy held placement reads until commit; acceptance then returned the
  current price_changed/product_unavailable result without stale acceptance or partial rows.
- Failure injection after the actual SQL order-header INSERT rolled back the header/lines.
  A separate interceptor threw only after commit: placement reported uncertain, capability-only
  tracking found the committed order, and explicit unchanged retry replayed that saved reference.
  Parallel duplicate attempts still produced one order, changed intent conflicted, actual
  line overflow was rejected, and historical table/product snapshots survived copy catalog edits.
- All five saved status/reason projections passed. Two isolated future-writer-style contexts
  proved rowversion rejects the stale update. Each complete backend run created four diagnostic
  orders/lines (Pickup, DineIn, concurrent winner and lost-response order), restored all edited
  copy fixture values and removed only recorded diagnostic references. Orders/OrderLines returned
  to 0/0; combined full-row fingerprints of all 13 tables matched before/after. No original writes.
- Published Release host on owned loopback HTTPS was stopped/restarted between placement and
  replay/tracking, which recovered the same persisted reference. Expanded HTTP inputs for totals,
  roles, internal table/order IDs, timestamps, rejection reason and digests all returned 400.
  Earlier CSRF/JSON/429/header/privacy/original-route checks passed again. HTTP runs created one
  owned-copy order/line and removed it afterward; both owned host instances were stopped.
- T041 is complete as an independent backend diagnostic; T040 and full story acceptance remain
  pending. T052/T056 have backend status/rowversion/concurrency evidence, not UI acceptance.
  T060 has successful Debug/Release builds, final Release publish, asset/config inspection and
  published-host pilot evidence, but its deferred responsive dependency remains unchecked.
- Preserved new uncommitted implementation for T034–T039/T045–T046 and tracking refinements
  T049–T051: session helper/bootstrap, CheckoutPanel, OrderDetails, PickupMenu/PublicLayout,
  TrackOrder, TableMenu integration and reconnect selector. Solution builds with 0 warnings/errors;
  no interactive/clipboard/reconnect/responsive acceptance is claimed. JS VM checks passed
  secure random bytes, storage write/read verification, invalid/blocked storage, pending/active
  separation, immutable uncertain attempt, trusted-origin fragment links and stale-access denial.
  HTTP served the helper before Blazor and prerendered no private credential/order data.
- Final Debug build and Release publish passed after the scoped tracking-navigation guard fix.
  Publish includes local Bootstrap/order helper, no credential/DB/certificate files, and no
  ConnectionStrings or Bootstrap secrets in published appsettings. Full integration, signed-in
  regression, actual browser/mobile and real-phone gates remain pending. No extra package,
  framework, payment/POS/report/delivery/status-write/contact-lookup route was introduced.
- Reviewed pending-browser-validation.md and implementation-handoff.md. Only this validated
  evidence, T041 marker and handoff/checklist documentation are committed in this batch; all
  unfinished source and original navigation evidence remain unstaged. Initial preservation
  patch remains ignored under artifacts/. No recurring/background execution was created.


### End-to-end browser continuation (2026-10-29)

- Verified actual repository baseline at f7c23c9: 33/63 checked, nine modified tracked files and five untracked UI source groups. Preserved the complete initial diff and untracked sources before edits.
- Supported directly targeted in-app localhost browsing succeeded without reading unrelated Edge tabs or bypassing privacy/security restrictions. Requirements checklist remains 16/16; no implementation hooks.
- See [end-to-end-validation.md](end-to-end-validation.md) for precise functional, responsive, security, failure/recovery, database and publish evidence, including diagnostic failures/retries and remaining gates.
- T034–T040/T042/T045/T047–T058/T060–T062 criteria now have the recorded independent evidence. This does not override pending native navigation acceptance or claim final Feature 002/Feature 001 completion.
- Confirmed scoped fixes: invalid cart text now blocks checkout; review/error focus is accessible; tracking clears old state before asynchronous loads and uses identical invalid-access feedback. Prepared native-navigation pickup-link fix; native Leave acceptance is still unverified.
- The four browser orders and owned SQL fixtures were cleaned; final 13-table original/copy fingerprints matched and order/line counts are 0/0. Repeated concurrency semantics passed; a final read timeout was diagnosed and preservation was verified on retry.
- Restore/Debug/Release/publish and published-host pilot passed; credentials remained local, packages/framework unchanged. Disk-full duplicate diagnostic output was recovered without deleting source/data. No production mutation endpoint or unapproved feature was added.
- T021–T023 and T046 remain pending for the operator's native reload result; their guard-dependent menu/layout changes remain uncommitted. T059 has no approved phone-reachable HTTPS origin; T063 remains pending. Host 7268 stays running at the user's explicit request; auxiliary hosts were stopped.
