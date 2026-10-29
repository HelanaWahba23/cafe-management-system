# Independent review — 2026-10-29

**58/63 validated.** T022 accepted from existing registration/tab-isolation evidence under explicit authorization to continue independent tasks. Pending: T021, T023, T046, T059, T063. Preserve the four uncommitted source/configuration files described below. User now authorizes Features 003–005; these do not imply acceptance of native navigation or physical-phone checks. Confirmed POS: dine-in with existing table or takeaway without table, optional walk-in name/phone; online guest contacts remain required.

---

# Current checkpoint — 2026-10-29

**57/63 validated in the working tree; Feature 002 is not fully accepted.**
Pending: T021–T023, T046, T059 and T063. The supported in-app browser worked without
accessing unrelated Edge tabs. See [end-to-end-validation.md](end-to-end-validation.md)
for the exact completed checks, failures/retries, preservation result and requirement mapping.

The validated independent cart/checkout/session/tracking/reconnect code is committed separately.
Guard-dependent `TableMenu.razor`, `PickupMenu.razor` and `PublicLayout.razor` remain local,
including the prepared enhanced-navigation bypass fix. The initial non-secret development
URL setting in appsettings.Development.json also remains preserved and unstaged. Do not stage
these blindly or claim the native navigation acceptance passed.

The user's manual-check host remains on **https://localhost:7268**, loopback-only, with the
verified menu URL `/menu/t/691d9592-3b19-4466-ad62-314b0334af17`. It returned 200 with the correct
table/cart. Do not interrupt it during the requested test. Its owned launcher session is
49122; after the user finishes, its `stop` input stops only that host and performs the original
copy-preservation cleanup. Recheck concurrent changes before applying any fixture restoration.
Auxiliary ports 7269/7270/7271 and agent-created validation tabs were closed.

The four browser orders and the repeat SQL fixtures were removed; final original/copy
13-table fingerprints match, with Orders/OrderLines 0/0. No original records/schema/accounts
or issued QR identifiers changed. The copy's identity counters may have advanced. Debug,
Release and publish checks passed; the published build's interactive shell initialized with
zero client errors. All secrets remained in local configuration/in-memory diagnostic clients.

Next: collect the user's native Reload → Stay/Leave result, finish the navigation guard
acceptance and commit only the now-validated integration files. Real-phone T059 still needs
an approved phone-reachable trusted HTTPS origin; only loopback is available. Keep T063 pending
until all required checks pass. No background automation was created.

The earlier checkpoint below is retained as history, not current task status.

---

# Customer Ordering implementation handoff — 2026-10-29

## Validated and pushed

- `2db828c`: T024–T025, canonical guest capability/draft validation and read-only review.
- `b2f60fa`: T026–T029, atomic acceptance, price reconfirmation, duplicate/recovery handling
  and capability-only minimal tracking.
- `e3aa509`: T030–T033, protected JSON review/place/tracking APIs and scoped OrderService.
- `6839273`: T043–T044, independent public pickup catalog preserving the table-menu contract.
- T041's independent backend diagnostic matrix also passed; its evidence is recorded in the
  following documentation commit. No component/browser acceptance is implied.

The user explicitly authorized independent backend work while US1's browser acceptance is
deferred. These implementation/validation batches do not certify US1, US2, US3 or US4 acceptance.
Feature 001 still has its separate T051/T056 gaps. See tasks.md for individual evidence.

## Preserved unfinished work

All six initial modified files remain present. Initial source changes to CartState.Clear,
the cart notice/scoped registration and development public URL remain uncommitted. The
initial TableMenu navigation guard remains, extended by the new checkout integration.
Earlier navigation-validation evidence in tasks.md remains an unstaged addition.

Additional uncommitted code builds but has no full interactive acceptance:

- `wwwroot/js/order-session.js`, `Components/App.razor`: verified session metadata, secure
  capability generation, fragment handling/copy link and deterministic helper loading.
- `Components/Shared/CheckoutPanel.razor`, `OrderDetails.razor`: embedded guest review,
  explicit placement/reconfirmation, minimal confirmation, outcome reads and safe unchanged
  retry; known saved orders alone allow starting a new attempt.
- `Components/Pages/Menu/TableMenu.razor`, `PickupMenu.razor`, `Layout/PublicLayout.razor`:
  shared cart/checkout, explicit pickup entry, navigation guards and busy state.
- `Components/Pages/Orders/TrackOrder.razor`: after-render private access, minimal status,
  latest-navigation-wins reads, safe errors, focus, explicit reload/copy and sharing notice.
- `wwwroot/js/reconnect-status.js`: preserves Admin guard and includes order write forms.

T021–T023, T034–T040, T042, T045 onward remain pending. T041 is a validated independent backend
task; its original T040 component-validation gate remains deferred by explicit user direction.
Do not infer completion from source presence.

## Validation boundaries and preservation

Debug/Release builds and Release publish succeeded with zero application warnings/errors.
The ordinary output is locked by an existing app, so isolated ignored OutputPath folders
were used without terminating that user process. NuGet restore used approved access to its
existing local configuration; no dependency/framework changes.

Node VM diagnostics passed helper crypto/storage/fragment/trusted-origin behavior. HTTP checks
passed normal localhost HTTPS, CSRF/JSON/unknown-field protections, status codes, replay,
rate limiting, private headers/projections, pickup filtering/empty/error behavior and safe
prerender shells. These checks do not represent interactive browser or mobile acceptance.

Only the previously owned `CafeManagement_F002_Validation_20261027_134110_c57ad692` copy was
mutated for diagnostics. Restoration/removal was restricted to copied fixture fields and
recorded diagnostic order references. All-table row fingerprints matched before/after the
backend diagnostic, with Orders/OrderLines restored to zero. Identity counters on the copy
can advance. No original database, account, role, migration, QR, certificate or firewall write.
Temporary diagnostic programs/build outputs are ignored under artifacts/ and are not a
committed test project/framework. Diagnostic-owned hosts are stopped in finally; any older
user/earlier-session host is left alone.

Extended diagnostics passed product price and availability locks, failure after the SQL order
header insert with no partial rows, post-commit reply loss with same-capability recovery,
historical table/product snapshots, all five status/reason projections and rowversion stale
update detection. The published host was stopped/restarted between acceptance and replay;
the saved outcome survived. Expanded HTTP tampering checks rejected totals, roles, internal
IDs, timestamps, rejection reason and hashes. Each run cleaned only its recorded own rows.
T040/T052/T056/T060 still include deferred component/dependency checks and remain pending.

## Next safe steps

1. Read current Git status/diff and preserve all tracked and untracked work above.
2. Follow [pending-browser-validation.md](pending-browser-validation.md) only when browser
   checks are resumed. Do not inspect unrelated tabs or retry the rejected unscoped inventory.
3. Validate the existing UI increment before staging it; review any demonstrated fixes,
   then commit/push only its validated scope. Do not stage all files automatically.
4. Finish remaining component, signed-in regression, responsive and physical-phone checks;
   retain every unperformed marker and reconcile final acceptance only after those pass.

The user deferred browser checks and asked not to be prompted repeatedly. No background run
or recurring automation is active for this handoff.

## Current superseding handoff — 2026-10-29

Feature002 is61/63 after all four isolated Playwright native navigation cases passed. The three preserved UI files PublicLayout/TableMenu/PickupMenu are now validated for delivery; existing checkout/price/recovery/private tracking/responsive evidence is in end-to-end-validation.md. Do not repeat the old blocked native/manual request. Only actual-phone T059 and final T063 remain pending. Feature001 retains54/56 (T051/T056). Feature003 is22/22, Feature004 cash10/16 with actual Paymob sandbox acceptance pending, and Feature005 report/theme milestone8657b2e is pushed. Prepared Paymob source/online hunks remain local and unverified. Main original-DB host7128 is healthy; manual7268 stays running. See Feature005 handoff and README for current migrations/backup/security evidence. Preserve all provider files and do not fabricate receipts or release uncertain attempts without verified reconciliation.
