# Quickstart: Payments & Billing

1. Preserve current Git files/DB row/account/QR baseline. Generate/review additive payment migration; apply only owned validation copy first.
2. Build Debug/Release and publish using existing artifacts paths. Verify current Cashier-only cash JSON/CSRF routes and capability-only customer payment status.
3. On owned orders verify full cash once, repeat, stale/concurrent/refused role/rejected cases, recorded actor/time, exact EGP, immutable bill and paid-order rejection refusal. Remove only recorded diagnostic rows in child→parent order; compare fingerprints.
4. Without Paymob configuration verify explicit unavailable, no fake checkout/success, cash still works where no active attempt. Test local HMAC handling only as cryptographic/input validation, not provider acceptance.
5. Operator setup:configure Paymob Egypt test secrets and known test card integration in local UserSecrets, plus approved publicly reachable trusted HTTPS callback base. Do not paste secrets into chat, commit them, create a tunnel or weaken certificate checks. Actual endpoint /api/payments/paymob/webhook and generic return page /payments/return.
6. With real sandbox setup verify hosted success/failure/cancel, exact evidence match, duplicate/reordered webhook, uncertain initiation/inquiry and cash/rejection locking. No live transactions. Keep this step pending without actual provider evidence.
7. Check bill/status/actions at320/768/desktop and keyboard, error/disconnect states, no sensitive logs/public projections. Record builds/security/preservation and exact blocked acceptance. Push only validated batches; continue Feature005 independent of provider setup.
