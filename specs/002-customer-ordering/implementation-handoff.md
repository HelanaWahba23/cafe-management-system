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
