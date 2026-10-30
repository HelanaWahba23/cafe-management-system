# Cash payment interfaces —2026-10-30

Cashier: GET /api/pos/orders/{id}/payment and POST /api/pos/orders/{id}/cash {rowVersion}; current Cashier authority, JSON/CSRF, saved amount, actor/time, concurrency and idempotency unchanged.

Customer: GET /api/orders/payment with private Bearer capability, rate-limited and no-store. OnlineAvailable is alwaysfalse regardless of old configuration. Status cannot mutate payment. Customer UI offers cash-at-counter/pickup instructions only.

Removed routes: POST /api/orders/payment/start, POST /api/orders/payment/reconcile, POST /api/pos/orders/{id}/payment/reconcile, POST /api/payments/paymob/webhook and GET /payments/return. None can initiate or settle a payment. Old Paymob settings are ignored; no credentials are read/removed by this scope change.

Retained historical Pending/Uncertain/PaidSandbox views protect old data; they do not create online actions or count sandbox values as cash revenue. No database/model migration. Existing itemized bills and order tracking remain.
