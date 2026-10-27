# API Contract: Customer Ordering

**Status**: Proposed interface; no endpoints implemented by planning.
Base: same HTTPS application; JSON, camelCase, explicit public DTO projections, Swagger/OpenAPI.
Existing Admin/auth/menu routes and response shapes remain unchanged.

## Security and common conventions

- Anonymous here means no Identity registration/login required; it does not remove validation.
  A signed-in staff cookie grants no extra customer-order access.
- New order endpoints accept a 32-byte random capability encoded as 43 base64url characters in
  `Authorization: Bearer <capability>`. This is an opaque guest credential, not a JWT and not
  a replacement Identity scheme. Controllers extract it explicitly; OrderService verifies it.
  Never accept it in query/path, return its hash, or treat a public reference as authorization.
- Both POST endpoints require the existing antiforgery cookie and RequestVerificationToken
  obtained from GET /api/auth/antiforgery. Missing/invalid pairs return 400 with no accepted order.
  Require application/json; unsupported content type 415; no wildcard CORS or JSONP.
- Apply no-store on order responses and no-referrer on checkout/tracking documents. Never log
  Authorization, private fragments, contact values, payload bodies or capability hashes.
  Private response HTML/API results must not enter shared caches or prerendered state.
- Invalid/missing/unknown capability on tracking returns the same generic 404 without confirming
  existence. Malformed capability on POST is 400. Generic 500/503 errors never expose SQL or PII.
- Guest HTTP policy uses the built-in configurable per-remote-IP fixed window, initial technical
  setting 60 requests/minute, queue 0, on new review/place/tracking routes only. 429 includes
  Retry-After. This is abuse protection, not a commercial order quota; shared-NAT effects require
  manual review. No forwarded-IP trust or staff-policy change is introduced implicitly.
- All monetary output and reviewed input uses invariant EGP decimal strings such as "25.50";
  reject overprecision/overflow, no silent rounding. Internally use decimal, not binary floats.

## Shared request/value shapes

| Shape | Fields / validation |
| --- | --- |
| OrderDraft | orderType: DineIn or Pickup; tablePublicCode: valid existing UUID for DineIn, null/absent for Pickup; customerName/Phone: trimmed nonblank, capacities 120/40; items: nonempty array |
| DraftItem | productId: positive existing int; quantity: positive whole int; no repeated productId entries |
| ReviewedPrice | productId and unitPrice: two-place positive EGP string; exactly one for every draft product, no extras/duplicates |
| Review | orderType, table: {publicCode,label} for dine-in or null, currency: EGP, items: {productId,productName,quantity,unitPrice,lineTotal}, total |
| Tracking | reference: public UUID, orderType, tableLabel: snapshot or null, currency, items: {productName,quantity,unitPrice,lineTotal}, total, status, rejectionReason: nonblank iff Rejected, createdAtUtc |

Customer phone/name are stored but omitted from public catalog and minimal tracking responses.
Internal order and table IDs, capability digests, intent hash, rowversion and staff/Identity
records are never part of public responses. Approved public catalog product/category identifiers
remain permitted, including productId in Review and existing catalog shapes. The public order
reference and table public QR code are distinct from internal IDs and are included only where
specified by these DTOs; neither grants order access. Request does not accept status, reason,
totals, server timestamps or roles.
Reject prohibited/unknown placement fields rather than giving the impression they were honored.

## Menu routes

| Method / path | Outcome |
| --- | --- |
| Existing GET /api/menu/tables/{publicCode} | Preserve response/table validation, anonymous access and unavailable filtering |
| New GET /api/menu/pickup | 200 {currency: EGP, categories: existing public category/product shapes}, no table or contact data; empty arrays/available-only behavior retained |

Invalid table references do not redirect to pickup. Both catalog reads are no-store to retain
the existing next-load availability behavior. GETs never create a cart/order/payment.

## POST /api/orders/review

**Input**: OrderDraft, capability and antiforgery headers. No database write.
**200**: Authoritative Review with available products/amounts; does not reserve prices, inventory,
tables or pickup time, create an order, or authorize staff actions. UI compares old/new prices
and explicitly displays changes before final action.
**400**: Invalid draft/context/fields; **404**: unknown table/product; **409**: unavailable selected
product; **429/500/503**: safe failure. No silent item removal or partial acceptance.
Review endpoint returns no order details for a reused credential; recovery uses tracking GET.

## POST /api/orders

**Input**: {draft: OrderDraft, reviewedPrices: ReviewedPrice[]}, capability and antiforgery headers.
The final explicit UI action sends the reviewed vector; it is a comparison snapshot, never an
authoritative price/total. Service recomputes prices/amounts and revalidates everything.

| Status | Meaning |
| --- | --- |
| 201 | New order committed as Submitted; response {order: Tracking, trackingPath: /orders/track}. Client already holds credential and builds fragment link locally |
| 200 | Same capability and immutable intent already accepted; return the saved order, including historical prices/status; never a second order |
| 400 | Invalid fields/quantity/context shape/review vector or unrepresentable money; no order |
| 404 | Unknown table/product on a new intent; no order |
| 409 price_changed | No order written; ProblemDetails includes current Review in a review extension; requires explicit confirmation and revalidation again |
| 409 product_unavailable | No order written; identify affected product IDs without private data; correct cart explicitly |
| 409 submission_conflict | Capability already accepted with different immutable intent; original order unchanged |
| 429 / 500 / 503 or transport failure | Safe feedback; possibly uncertain commit. Check same capability before any explicit retry; never replace it automatically |

Use existing ProblemDetails/ValidationProblemDetails conventions with stable errorCode and field
keys. Add endpoint-specific safe review extension for price_changed; do not embed customer inputs,
raw exceptions or secrets. A new review invalidates old displayed confirmation in the UI; a second
price change repeats 409, including decreases. Successful insert uses the transaction in data-model.md.

Duplicate resolution precedes current catalog validation. Canonical intent includes customer
name/phone, type/table and IDs/quantities, excluding reviewed prices. Repeating a successful
intent returns saved prices even after catalog edits; it cannot revise an accepted order.
Parallel inserts rely on the unique digest, rollback and fresh-context winner lookup.

## GET /api/orders/tracking

**Input**: Capability header only. No reference, phone, name or table lookup parameter.
**200**: Minimal Tracking; no registration required. **404**: generic unavailable/invalid private
context; **429/500/503**: safe error. A 404 while a submission is in flight is inconclusive:
offer explicit read retry or unchanged resubmission with the same credential, never automatic POST.
This read also recovers confirmation after response loss or circuit reload.

Shareable link is `<trusted PublicWebBaseUrl>/orders/track#key=<capability>`; server responses
contain only the token-free relative trackingPath. The browser parses fragments after render.
Do not put bearer links in Location headers, query strings, logs or Swagger examples with real values.

## Deliberately absent interfaces

No list-all-orders, public reference/contact lookup, status mutation, rejection command, customer
order edit/cancellation, staff walk-in creation, bills, payment selection/recording/provider callback,
reports or delivery endpoint. Preparing/Ready/Completed/Rejected views use isolated fixtures until
the separately authorized staff workflow exists. WeatherForecast and existing Swagger remain intact.

## In-process boundary

OrderService exposes review, place and track methods with explicit capability and validated DTOs.
Blazor invokes them directly with fresh contexts per operation; it does not send staff cookies
through self-HTTP. UI busy guards/disconnect protection never replace these service checks.
No browser-controlled role, price, total, status or table internal ID is trusted.
