# Research: Payments & Billing —2026-10-29

## Provider protocol

Decision:Fixed Egypt origin https://accept.paymob.com. POST /v1/intention/ with Authorization:Token secret key, exact integer amount, EGP, configured test integration payment_methods, unique saved special_reference, notification_url/redirection_url and real guest name/phone. Returned id/intention_order_id/client_secret are saved before launch. Hosted URL /unifiedcheckout/?publicKey=...&clientSecret=... . Test keys may have sk_test_/pk_test_ or egy_sk_test_/egy_pk_test_ prefixes; also require response payment method live=false. Never call live mode.
Rationale:Official current protocol; local stable attempt is necessary because no documented intention Idempotency-Key guarantee exists. special_reference is correlation, not a duplicate-prevention guarantee.
Alternatives:Legacy iframe flow or simulated success would violate selected hosted checkout/verified success.
Sources:[Create Intention](https://developers.paymob.com/paymob-docs/intention-apis/create-intention), [official Paymob intention reference](https://raw.githubusercontent.com/PaymobAccept/Paymob-AI-Integration-Skill/main/skills/paymob-integration/references/intention-api.md).

## HMAC and evidence

Decision:POST TRANSACTION obj fields concatenated without separators in this exact order:amount_cents,created_at,currency,error_occured,has_parent_transaction,id,integration_id,is_3d_secure,is_auth,is_capture,is_refunded,is_standalone_payment,is_voided,order.id,owner,pending,source_data.pan,source_data.sub_type,source_data.type,success. Preserve raw strings/timestamp and lowercase booleans, strict integer numbers; UTF8 HMAC-SHA512, exact128hex query hmac, constant-time decoded-byte comparison. Reject unsupported missing/null fields rather than guessing serialization. Match signed order.id/amount/currency/integration against saved attempt; successful nonpending standalone/capture, no auth-only/error/refund/void. is_live and merchant_order_id are not signed and cannot independently authorize/select a settlement.
Rationale:Only verified provider evidence may settle, not redirects or arbitrary callback metadata.
Sources:[official HMAC transaction callback](https://developers.paymob.com/paymob-docs/developers/webhook-callbacks-and-hmac/hmac/hmac-transaction-callback), [callback semantics](https://developers.paymob.com/paymob-docs/developers/transaction-callbacks), [official implementation notes](https://raw.githubusercontent.com/PaymobAccept/Paymob-AI-Integration-Skill/main/skills/paymob-integration/references/hmac-verification.md).

## Authenticated reconciliation

Decision:POST /api/auth/tokens with API key, then Bearer token POST /api/ecommerce/orders/transaction_inquiry with stored order_id or merchant_order_id stable attempt reference; GET /api/acceptance/transactions/{transaction_id} is a secondary supported inquiry. Validate response exact order/amount/EGP/test integration and is_live=false; shared settlement checks. No automatic retry of uncertain intention creation.
Rationale:Official linked Postman collection gives full endpoints where current guide renders incomplete examples; API key may cover both modes, so verify each result's mode/integration. Inquiry by reference returns latest transaction; unknown/no transaction is not proof an existing payable intention is cancelled.
Sources:[official Transaction Inquiry collection](https://raw.githubusercontent.com/PaymobAccept/API-Postman-Collections/refs/heads/main/Transaction%20Inquiry%20API.postman_collection.json), [order/reference guide](https://developers.paymob.com/paymob-docs/developers/transaction-inquiry-apis/by-order-id-or-reference).

## Integration boundary and setup blocker

Current docs disagree about notification_url overrides for non-card methods. Configure a confirmed test card integration initially; do not silently promise unsupported methods. Real merchant sandbox confirmation resolves the shape and billing-data requirements. No local Paymob keys are configured; no approved publicly reachable HTTPS callback exists. Never create a tunnel, bypass certificates or claim provider verification from local HMAC vectors. Actual sandbox acceptance remains pending.

## Local consistency

Decision:Separate one-per-order settlement and recoverable attempt tables, Serializable local transactions, order revision touches, unique provider transaction/order IDs, shared validated settlement logic; external calls outside transaction. Cash actor from trusted current Cashier principal. Existing fulfillment edges remain, plus user-approved paid/pending rejection protection.
Rationale:Prevents cash/provider/status races without conflating fulfillment with settlement. No generic infrastructure or automatic background jobs required.

Post-research refinement:use Order.PaymentRevision bigint and an IsBlocking bit/state-consistency check with unique filtered OrderId where IsBlocking=1. Order-first UPDLOCK,HOLDLOCK loading in every payment/status writer avoids read-lock upgrade races. Callback resolution is by already-saved signed provider order ID only; unsigned merchant reference never binds an unknown callback to an order. Early unbound callbacks require provider retry/authenticated inquiry. Unknown-intention reconciliation may bind provider ID only from authenticated inquiry whose merchant reference matches the stable saved attempt. No notfound response is proof of cancellation.

## Provider recovery clarification from official documentation

Further official research found intention retrieval at GET /v1/intention/element/{public_key}/{client_secret}/ and authenticated update at PUT /v1/intention/{client_secret}/, but no documented terminal-state guarantee or atomic cancellation that proves an in-flight/failed context can never later be paid. Inquiry returns a latest transaction, not permanent closure. Reopening an old hosted link is not documented as exactly-once charging. Do not add automatic timeout/failure unlock or blind checkout replay. The existing conservative block is intentional pending provider-confirmed semantics and real sandbox cases.
Sources: [official Intention collection](https://raw.githubusercontent.com/PaymobAccept/API-Postman-Collections/main/Intention%20APIs.postman_collection.json), [Update Intention](https://developers.paymob.com/paymob-docs/developers/intention-apis/update-intention).
