# POS API contract

All /api/pos routes require authenticated current Cashier role/stamp; anonymous=401, other/revoked=403, missing=404, invalid=400, stale/changed intent/price=409, safe unexpected=500. No-store responses. No capability hashes, intent hashes or persistence entities serialized.

- GET /api/pos/orders?status=Submitted&page=1: up to 50 summaries, total count, stable CreatedAtUtc/Id descending; optional exact named status filter, page 1..1000000.
- GET /api/pos/orders/{id}: saved details, optional contacts, origin/type/table label, reference/time/status/reason, EGP total, immutable lines and base64 RowVersion. Bill uses same historical details.
- POST /api/pos/orders/{id}/status: JSON {status,rowVersion,rejectionReason}; exact named status, base64 8-byte version, reason only for Submitted rejection. CSRF required; no automatic retry.
- GET /api/pos/tables: existing table public codes/labels for authorized counter selection.
- POST /api/pos/orders/review: JSON counter input {attemptKey,orderType,tablePublicCode,customerName?,customerPhone?,items:[{productId,quantity}],reviewedPrices?}; server review only, no write.
- POST /api/pos/orders: same input with exact complete reviewedPrices [{productId,unitPrice:"0.00"}]; CSRF required; 201 new, 200 recovered; result saved staff order id/reference (no guest link). Price conflict includes updated review and no save.

Unknown JSON fields refused. Origin, creator, status, totals and timestamps are never client accepted in creation. Creator comes only from server identity. Canonical attempt token is 43-character base64url encoding 32 random bytes. Retry unchanged attempt after uncertain response; never silently generate a replacement.
