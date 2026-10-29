# POS validation — 2026-10-29

## Baseline and preservation

Initial tracked diff and untracked PickupMenu backed up under ignored artifacts/feature003-baseline. Four pre-existing source/config changes remain excluded from POS commits. Feature 002 independently reviewed to 58/63; its native navigation and real-phone acceptance remain pending. Feature 001 T051/T056 remain pending.

Reviewed AddCounterOrders migration adds origin/creator, defaults existing orders to Customer, permits optional counter contacts with explicit nonnull customer constraints and restrictive creator FK. Applied only to the existing owned validation copy. All 12 application/Identity table original-column fingerprints matched before/after; original database unchanged. Migration history is intentionally excluded from that comparison. Existing order count was zero. No reset or original database/schema/account/QR write.

## Backend and HTTP

58 focused diagnostic checks passed against the owned copy: guest dine-in/pickup acceptance, shared queue, strict named status/page validation, anonymous/Admin/missing-stamp denial, all 25 lifecycle pairs, stale rowversion conflict, rejection lengths 0/501, immutable saved bills after product edits, both counter modes, null optional contacts, creator attribution, table/no-table context, exact line sums, explicit changed-price reconfirmation, replay before mutable availability/catalog lookup, changed-intent conflict, counter token refusal by guest tracking and guest contacts still required. Diagnostic rows removed by recorded IDs only and copied product fields restored.

Normal trusted localhost HTTPS checks passed: anonymous401, Admin403, Cashier200, missing antiforgery400, unknown JSON400, numeric status400, missing order404, no-store queue and Cashier document200. Existing credentials read only in memory. No payment credentials or private tracking values printed.

## Interactive browser

Supported dedicated in-app localhost tab; no unrelated Edge inventory or security bypass. Existing Cashier login → queue → takeaway review/save → historical bill → Preparing → Ready → Completed passed. Dine-in existing table → quantity two → review51.20EGP → save → rejection reason → Rejected passed. Both omitted phone; dine-in omitted both contacts. Queue status filter and empty states passed.

Actual viewport checks for queue, detail/bill and composer at320/768/1280 showed document scroll width no greater than client width. Real keyboard quantity0 disabled review; correction2 produced server-reviewed quantity2. DOM fill alone did not fire Blazor onchange reliably; it was not treated as acceptance and was repeated using actual keyboard events.

Confirmed defect: enhanced navigation bypassed NavigationLock on page-local roots. POS now intercepts enhanced links before navigation, presents an in-app Stay/Leave confirmation and separately retains ordinary native beforeunload protection for actual reloads. In-app Stay kept composer controls present; Leave returned to queue. Native Reload→Leave acceptance is not claimed and remains in the project manual checklist alongside Feature002. A separate multi-item bug was fixed and checked: editing a valid second quantity does not clear the first item's invalid state.

Restore, Debug build, Release build and Release publish passed with zero application warnings/errors before the final navigation refinement; repeat Release verification is required for the final candidate. Full final task acceptance and Git milestone reconciliation remain pending until the remaining checks below pass.

## Outstanding acceptance

- Final candidate Release build/publish and preservation after browser fixtures cleanup.
- Explicit current stamp mismatch/current-role-forgery denial, cross-creator attempt-hash isolation and guest tracking reflection after staff update.
- Confirm in-app Stay retains quantity value and Leave/new composer starts empty; native reload warning acceptance remains separately manual.
- No Paymob keys configured (key count0); actual provider sandbox confirmation is blocked until local credentials and an approved reachable HTTPS webhook origin exist. No provider payment has been simulated.

## Final candidate checkpoint

Final POS diagnostic matrix:63/63 focused checks passed (includes mismatched current security stamp, forged Cashier role versus current database role, cross-creator hash isolation and guest private tracking reflecting cashier preparation). Browser fixture cleanup and original/copy12-table fingerprints passed. Debug, Release and Release publish passed on final source. Published queue/composer initialized and in-app Stay was focused and activated by keyboard; quantity1 remained. Leave by keyboard returned to queue. Fresh composer showed empty cart and zero quantity inputs. A clean Release navigation reproduction produced zero browser console errors.

An interrupted background-launch attempt used a PowerShell dictionary/property assignment incorrectly and read the original order schema, producing missing-column errors before any POS writes. The owned launcher was corrected with the explicit connection-string setter and the corrected published queue returned the empty copied data. No schema migration was applied to the original. This diagnostic failure was not counted as acceptance.

Existing atomic acceptance/rollback and precise unique-index recovery are reused without changing transaction boundaries; counter-origin acceptance and recovery passed the new matrix. In-app navigation acceptance is complete; native full Reload→Stay/Leave and real-phone project checks remain pending under Feature002. No security restrictions were bypassed.

All Feature003 FR001–011 and SC001–006 map to completed tasks and evidence above. The original database has not yet received AddCounterOrders; deployment must apply reviewed migrations explicitly with preservation/backup readiness. Feature004 must add payment-aware rejection rules using the user's confirmed decision, without changing the permitted fulfillment edges.
