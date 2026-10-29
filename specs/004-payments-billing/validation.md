# Payments validation —2026-10-29

## Reviewed planning gates

Specification quality16/16. Cross-artifact analysis:10 functional requirements,5 success criteria,16 tasks;100% requirement coverage, no critical/high conflicts after refining bigint revision, unique blocking slot and order-first locks. Existing Constitution gates pass; no packages/framework/tests added.

## Preserved migration

Reviewed AddPaymentLedger:only Order.PaymentRevision default0 and two new constrained payment tables/indexes/FKs. Applied only to owned CafeManagement_F002_Validation_20261027_134110_c57ad692. All prior12 application/Identity table row projections matched before/after; original DB unchanged. Migration history is intentionally expected to change on copy. Final exact browser fixture/child cleanup passed; new payment tables empty and previous row/account/QR fingerprints preserved.

## Passing cash and local checks

26 focused cash/local safety checks passed:unpaid/unconfigured state, no-key online initiation creates no attempt/payment, invalid private context denied, anonymous/Admin denial, invalid/stale version, concurrent cash exactly once, exact EGP saved amount/actor/time, minimal customer payment projection, private capability boundaries, order rowversion update, paid rejection blocked while Preparing/Ready/Completed remain permitted, unpaid rejection allowed, rejected cash refused, pending/uncertain guard states and cash-versus-rejection race safety. Pending fixtures were explicitly local guard diagnostics, never simulated successful Paymob payments. All diagnostic-owned rows removed.

13 local cryptographic/config checks passed using a non-success/pending vector only:HMAC ordering, short/nonhex/wrong-key signatures, amount alteration, missing/null fields, no configuration, live key prefixes, loopback/insecure/query-bearing callback rejection. No provider calls, credentials or successful provider settlement simulation.

HTTP checks passed:anonymous cash401, Admin payment403, client amount tampering400, missing cash CSRF400, known receipt recovery200 and staff cookie alone cannot grant private payment access404. Earlier no-CSRF diagnostic accidentally retained a header in PowerShell WebSession; confirmed retained=true, explicitly cleared it and repeated successfully. Not counted as an application defect.

Browser:Cashier→new takeaway→saved25.60EGP bill→explicit checkbox→Record cash→Paid Cash/time; refresh persisted receipt. Actual320/768/1280 widths had no horizontal overflow. Existing console negotiation errors corresponded to the deliberate debug-host restart, before the successful flow; no provider result claimed. Confirmed pre-interactive click loss was prevented by keeping composer/cash controls disabled until OnAfterRender initialization.

## Pending provider acceptance

Paymob protocol/client/orchestration/routes/online controls are prepared locally and build, but real sandbox success/failure/reconciliation/duplicate webhook and actual billing/response shapes remain UNVERIFIED. No credentials configured and no approved publicly reachable callback origin. Do not mark T009–T014 complete merely from source/build. Local HMAC vectors are not provider acceptance.

Safety limitations to verify with real merchant setup:no blind repeat of an existing intention; failed transaction evidence retains a blocking uncertain intention because failure alone does not prove checkout cannot later be paid. Missing inquiry result does not unlock cash. Unknown early callbacks cannot bind via unsigned merchant reference. If a hosted URL is lost, retain/reconcile the stable attempt; no unsafe replacement attempt or force-unlock is offered. Actual terminal-failure/recovery usability requires sandbox verification before provider completion.

Only validated cash/schema/status and cash UI versions are eligible for the next commit. Preserve additional Paymob implementations locally. Complete Release build/publish of the exact staged cash snapshot before push.

## Exact staged cash milestone

Exported Git index to ignored artifacts/cash-index-review without changing the working tree. Its Release restore/build/publish passed with zero warnings/errors, independently of prepared Paymob files. The staged publish on7272 passed12 focused HTTP checks, including protected cash/private status and confirmed404 for unverified online initiation/webhook routes. Cash-only versions of PaymentPanel/PaymentsController/Program are staged; online additions remain local and preserved. This validates the actual commit contents rather than relying on a working tree with extra files.
