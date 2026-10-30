# Payment interfaces

Cashier:GET /api/pos/orders/{id}/payment returns method/state/amountEGP/time and staff-safe metadata, never hosted client secret/keys. POST /api/pos/orders/{id}/cash JSON {rowVersion} requires current Cashier, JSON/CSRF, exact saved total, no client amount/actor. Duplicate known cash result is idempotent; provider-paid/conflicting/pending/rejected/stale fail409. New payment returns receipt.

Customer:GET /api/orders/payment, POST /api/orders/payment/start and POST /api/orders/payment/reconcile use existing private Bearer capability only for Customer-origin order, rate-limited, no-store/no-referrer. POSTs JSON/CSRF. Start returns sandbox hosted URL only to this context and never replaces uncertain attempt. Status returns safe state/method/time/EGP, no identifiers/secrets. Customer cannot start online payment for counter origin.

Provider:POST /api/payments/paymob/webhook?hmac=... requires verified exact HMAC fields before any write, strict body-size limit, no browser CSRF (provider is not cookie-authenticated); no body/key/credential logging. Unknown/malformed/mismatched/live evidence cannot settle. Valid duplicate effects are no-ops. Redirection page only informs the customer to use their retained private link; query flags do not change payment state or disclose an order.

Staff billing and customer private tracking gain payment projections via shared PaymentPanel:full saved itemization/reference is the invoice/bill, payment state/method/time separate; sandbox explicitly labelled test. Payment actions have explicit confirmation and busy/error states, no auto charge/start on page load.

Configuration UserSecrets:Paymob:Enabled=false default; SecretKey,PublicKey,HmacSecret,ApiKey,IntegrationId,CallbackBaseUrl. Fixed provider origin, test-only key prefixes and false live response; callback base trusted publicHTTPS, no loopback setup pretending to be reachable. No configured values committed.
