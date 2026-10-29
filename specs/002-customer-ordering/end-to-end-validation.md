# Customer Ordering validation — 2026-10-29

## Current result

57/63 task criteria have supporting evidence in the current working tree. This is **not**
full Feature 002 acceptance. T021–T023, T046, T059 and T063 remain unchecked. Guard-dependent
menu/layout integration stays uncommitted while the native navigation gate is pending;
validated independent cart/checkout/tracking components are committed separately.

The initial tracked patch and all untracked source were copied into ignored
`artifacts/resume-end-to-end/` before edits. Initial patch SHA-256:
`D2ACEC55FAE004C60F04897AC9158CA806C42DFDA1B7231DA63135F74B68B9F1`.
The requirements checklist remains 16/16; no implementation hooks or AGENTS.md were present.

## Functional evidence

- Two different existing table links produced two DineIn orders with their correct saved
  table labels. Two independent pickup journeys produced two Pickup orders with null table
  association. All initially displayed Submitted and immutable exact EGP snapshots.
- Real interactive guest review required no sign-in. Whitespace-only details were rejected;
  rendered capacities are 120/40 and existing direct-service checks reject overcapacity.
  Editing a guest detail invalidated the confirmation. Existing service checks still reject
  supplied pickup table codes, invalid/duplicate IDs, quantities and reviewed-price vectors.
- For **both** entry types, increasing a price and then decreasing it again returned new
  unit/line/total amounts and required another explicit confirmation. SQL counts stayed
  unchanged before the final click. DineIn accepted EGP 49.20; the changed-price Pickup
  accepted EGP 24.60. A second table and pickup accepted EGP 25.60 and EGP 30.00 respectively.
- An unavailable selection was rejected without partial acceptance. Same-name products at
  EGP 25.60/25.50 remained product IDs 1/2, separate cart rows, total EGP 51.10.
- Confirmed and fixed a defect where cart field errors did not reach checkout: zero,
  negative, fractional and overflowing text now blocks review/placement while retaining
  accepted amounts. Correction restores review. Cart removal/clearing prunes stale errors.
- Fresh signed-out tracking tabs opened the checkout links and matched the saved order;
  fragments disappeared and same-tab reload retained access. Replaced links changed the
  displayed order. Invalid and unknown context used the same generic failure; invalid
  fragments plus reload could not fall back to earlier private access.
- Prepared owned-order examples displayed Submitted, Preparing, Ready, Completed, and
  Rejected with the recorded reason. This was fixture preparation, not a customer/staff
  status mutation API. The unchanged T010 lifecycle matrix still allows exactly four edges.
- A rollback-only order-row lock delayed a tracking read. Once the loading state appeared,
  previous order data was absent. A newer link loaded first and remained displayed after
  the older read finished. The page clears state before awaits and discards old completions.
- Copy actions reported successful completion; the runtime private-link builder used the
  configured origin and fragment-only credential. The automation clipboard read API was
  empty despite the browser's successful clipboard promise; no native paste/readback claim
  is made. Helper construction/storage cases were checked separately with the existing VM
  diagnostic, including denied/lost/malformed storage and preservation of uncertain attempts.
- Stopping only the secondary host made the app inert, focused Reload saved data and kept
  editing locked after connection recovery. Reload offered Check order outcome; that explicit
  read recovered the same saved reference with no new order. No automatic write replay.
- An independent intentionally unavailable-database host showed safe menu error/Retry and
  private-tracking load error without old order details or SQL/contact/credential exposure.

## Responsive evidence

Menu/cart/checkout were measured at actual 320/768/1280px widths, document widths
305/753/1265px respectively; tracking measured 320/768/1280 without overflow. Visual review
covered wrapping names, amounts, required fields, review/confirmation and actions. The 320px
reconnect view measured document width 305, retained inert editing and focused the reload
control. An override that targeted another tab was excluded until a fresh tab measured 320.
Temporary overrides were reset. Screenshots are ignored artifacts, not acceptance substitutes
for a physical phone: `checkout-320.png` and `reconnect-320.png` under the session artifact folder.

## Backend, security and preservation

- Repeated post-integration diagnostics on new owned product/table fixtures passed parallel
  deduplication, changed-intent conflicts, locked price/availability edits, overflow, rollback
  after the SQL header insert, post-commit reply-loss recovery, historical snapshots and
  rowversion stale-update rejection. Existing manual-test tables/products were not edited by
  this repeat. The first final fingerprint read timed out; no blocking request remained on
  inspection. Cleanup then removed only the named owned fixtures and a fresh full comparison
  passed. The timeout is not reported as a successful first run.
- Existing Admin and Cashier accounts signed in using local configuration held in memory.
  Admin APIs/screens/QR returned 200; Cashier Admin APIs returned 403. Both cookie-only private
  tracking requests returned 404. A no-CSRF Admin write returned 400 with no inserted category.
- Existing HTTP evidence covers CSRF pairing, JSON/unknown-field/tampering rejection,
  201/200/409 behavior, minimal tracking, no-store/no-referrer, 429/Retry-After and unchanged
  original routes. Components invoke the same validated service directly; no staff-cookie
  forwarding or authorization-scheme change. Source/logging review found no credential/body
  logging, payment/POS/delivery/report feature, status writer or contact/reference lookup.
- The four browser orders/lines were removed only by recorded references plus diagnostic
  customer prefix. Catalog fixtures were restored. All **13** original and owned-copy table
  fingerprints matched the original preservation baseline; Orders/OrderLines returned to 0/0.
  No original account, role, QR, table, catalog or migration was written. Copy identity
  counters may advance. One disposable private link appeared in an initial accessibility
  snapshot; later snapshots were redacted and its owned order has been removed.

## Build and remaining gates

Restore, isolated Debug build, normal Release build and final Release publish passed with
zero application warnings/errors. The ordinary Debug app output remains owned by a running
process, so isolated output was used. The published build passed normal HTTPS page/API/static
asset checks and interactive checkout initialization with zero client errors. Published
configuration contains no connection strings/bootstrap secrets, certificate or database files.
No package/framework change or committed automated test project was introduced.

A temporary disk-full error occurred while duplicating diagnostic dependencies. Only newly
generated duplicate output was removed/deduplicated, and the tiny launcher was changed to use
shared framework references. Builds/publish subsequently passed. Source and data were preserved.

- **T021–T023/T046:** native Reload → Stay/Leave is still awaiting the user's result. The tool
  cannot accept a native before-unload dialog. The enhanced-navigation pickup bypass was
  confirmed and a scoped `data-enhance-nav="false"` fix prepared; the secondary browser then
  retained the nonempty cart on the blocked navigation attempt. Native warning/Leave acceptance
  is not inferred from that observation. Related menu/layout files remain uncommitted.
- **T059:** only loopback HTTPS is available; no approved trusted phone-reachable origin or
  physical-phone evidence exists. No certificate, firewall or network change was attempted.
- **T063:** final acceptance remains pending on the above gates. Feature 001 remains 54/56,
  with its original T051/T056 gaps unchanged.

The user's verified manual-check URL remains running on localhost port 7268:
`/menu/t/691d9592-3b19-4466-ad62-314b0334af17`. Auxiliary hosts and temporary browser tabs were
closed. Do not stop the requested manual host or treat an unanswered check as a pass.

## Acceptance reconciliation

| Requirement | Evidence / remaining limitation |
| --- | --- |
| FR-001 | Anonymous available-only catalog, empty/error feedback and direct pickup read. |
| FR-002 | Distinct IDs/quantities, checked amounts, invalid-input checkout lock; native navigation gate pending. |
| FR-003 | Two correct saved table contexts; native context-leaving acceptance pending. |
| FR-004 | Two direct Pickup orders with null table context and no QR/address requirement. |
| FR-005 | Authoritative review, atomic save, unavailable/invalid rejection and rollback diagnostics. |
| FR-006 | Exact EGP, both price directions/rechanges, explicit confirmation and historical snapshots. |
| FR-007 | Same-capability deduplication, uncertain outcome recovery, restart and private-link retention. |
| FR-008 | Five prepared status displays and unchanged validated four-edge lifecycle. |
| FR-009 | Capability-only tracking, generic missing/invalid/unknown failures and context isolation. |
| FR-010 | Existing role boundaries, guest CSRF/JSON/limiter controls and shared service validation. |
| FR-011 | Measured 320/768/1280 layouts and focus/reconnect feedback; physical phone pending. |
| FR-012 | Original 13-table fingerprints, issued QR/auth/code preservation and approved scope only. |
| FR-013 | Required bounded trimmed guest fields, formatted phone accepted, no contacts in public tracking. |

SC-001/003/004/005/007/008 have the corresponding cart, deduplication, price, lifecycle,
preservation and guest/privacy evidence above. SC-002 and SC-006 have browser journey/layout
evidence but retain the actual-phone QR/private-link gate. Native navigation acceptance and
all source integration must also be settled before T063 can certify full completion.

## Native navigation gate resolved — 2026-10-29

All four native beforeunload cases passed using standard Playwright1.64.1 with installed Edge156.0.4314.50 in a fresh isolated headless profile. Browser sandbox and normal certificate checks stayed enabled. The HTTPS7128 table route loaded normally; warning dialogs were observed, not replaced or disabled. Stay preserved cart/table for reload and pickup; reload Leave cleared cart with correct table; pickup Leave cleared cart and opened pickup without table. No order/payment was submitted and no existing user browser profile was accessed. The first diagnostic click preceded Blazor interactivity and a canceled reload correctly caused a navigation timeout; the final complete run waited for interactive rendering and verified each resulting DOM state. Four of four cases passed. T021/T023/T046 are accepted with the earlier full US1/responsive/backend evidence. Only phone T059 and final T063 remain pending.

The same four cases also passed against the independently built/published cash-only commit candidate on owned port7274, with unverified Paymob source excluded. Its14-check HTTP role/privacy/date/style/public-route matrix passed. A final read-only original/copy fingerprint comparison passed after these checks. The main7128 host remains running unchanged for local review.
