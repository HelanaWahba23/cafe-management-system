# Research: Customer Ordering

**Date**: 2026-10-27 | **Status**: Design questions resolved; not runtime verification.

## R1 — Same host and context factory

**Decision**: Retain installed dependencies and current layers. Thin controllers and interactive
components use concrete application services, one factory-created context per operation, and
component busy guards. Share catalog selection while preserving existing table responses.
**Rationale**: Fits Program.cs, MenuService and PublicMenuService; circuit-shared contexts
can receive concurrent component operations and are not thread safe.
**Alternatives**: Separate frontend/deployments, repository framework and circuit-held tracked
contexts add work or risk without an approved requirement.
Routes.razor is currently static with page-level interactive roots. Keep checkout as a shared
panel within each menu root, not a separate page expecting circuit state to persist. Change
category links to same-document navigation; leave the existing global router/authorization intact.
Full navigation/context changes may clear unsubmitted carts, as allowed by the specification.
**Source**: [Microsoft Blazor/EF guidance](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-ef-core?view=aspnetcore-8.0).

## R2 — Short locked acceptance and price reconfirmation

**Decision**: Read-only review returns server prices. Placement supplies a reviewed product/price
vector for comparison only. Within a short RepeatableRead transaction, resolve table/products,
check availability and compare every price, including decreases. Mismatch returns refreshed
amounts with zero writes. Another explicit action confirms them; acceptance rechecks again.
Compute saved totals exclusively from database prices and validated quantities.
**Rationale**: Existing products lack rowversion; holding read locks until commit closes the
price-check/save race without changing Admin entities. Never hold a transaction during human review.
**Alternatives**: Unrelated read/save operations allow races. Product rowversion changes existing
schema. A quote table or signed quote token is unnecessary: submitted reviewed prices are never
authoritative or authorization. Tampering can only match current prices or cause rejection;
it cannot choose a different saved amount. No quote expiry or new key-management policy needed.
**Sources**: [EF concurrency/isolation](https://learn.microsoft.com/en-us/ef/core/saving/concurrency),
[EF transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions).

## R3 — One capability for submission identity and tracking

**Decision**: Before placement, generate 32 cryptographically random bytes in the HTTPS browser
and retain base64url form in sessionStorage. Send as a guest bearer header or explicit circuit
service parameter. Persist SHA-256(token) under a unique binary(32) index, never the token itself.
This same capability deduplicates submission and authorizes recovery/tracking. Display a separate
Guid reference that is never an access credential.

Persist a canonical immutable-intent hash: trimmed customer name/phone, order type, stable table
public code (or none), sorted product IDs/quantities. Same capability/same intent returns the
existing saved outcome; different intent conflicts. Reviewed prices are excluded from this hash:
after acceptance, recovery returns historical amounts regardless of later menu edits. Before
acceptance, corrected inputs and refreshed prices may reuse the capability because no row exists.

**Rationale**: No attempt table or recoverable plaintext server secret is needed. A lost success
response does not lose the pre-existing tracking credential.
**Alternatives**: Reference, phone or table code cannot authorize private access. A secret generated
only after commit cannot be reconstructed from its hash after response loss. Registration conflicts
with guest checkout; cookie-only ownership does not provide the approved shareable private link.
**Sources**: [Web Crypto random values](https://www.w3.org/TR/WebCryptoAPI/#Crypto-method-getRandomValues),
[.NET cryptographic randomness](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator.getbytes?view=net-8.0).

## R4 — Fragment link and minimal browser bridge

**Decision**: Browser builds `<trusted PublicWebBaseUrl>/orders/track#key=<capability>`.
Static route contains no private order data. After interactive render, read fragment through
small JS, retain capability in sessionStorage, then remove the fragment using replaceState.
Copy-link action reconstructs the full private URL. Never serialize credentials into prerendered
HTML or history state. Use no-store/no-referrer on checkout/tracking and local assets only.
**Rationale**: Fragments are not sent in HTTP requests; bearer still reaches the backend through
a protected circuit/request. Fragment use alone does not protect against XSS, screenshots or sharing.
Encode text; redact Authorization, contact fields, bodies and capability hashes from diagnostics.
**Alternatives**: Query/path secrets leak into access logs. No SMS/email delivery, contact lookup,
link rotation or expiration business policy is added.
**Source**: [RFC 3986 fragment semantics](https://www.rfc-editor.org/rfc/rfc3986#section-3.5).

## R5 — HTTP and interactive security boundaries

**Decision**: Reuse GET /api/auth/antiforgery, RequestVerificationToken header/cookie, and
ValidateAntiForgeryToken for both guest HTTP POSTs; require JSON and no permissive CORS.
Tracking GET requires the capability, never staff cookies. Blazor calls services directly with
equivalent validation and explicit capability; no circuit HttpContext cookie mutation.
Extend the existing reconnect write guard to checkout. After reload offer a read-only outcome
check, never automatic POST replay. Named built-in rate limiting protects guest HTTP endpoints,
configurable as technical abuse protection rather than an order quota; actual remote IP is used
unless a trusted proxy is deliberately configured. Circuit busy guards are not an abuse boundary:
service validation/capability checks apply on every call and no client status/role is accepted.
**Rationale**: Anonymous access must preserve current staff security and protect submissions.
**Alternatives**: Bypassing service checks for components, disabling CSRF or using a staff cookie
as guest ownership weakens the current application.
**Sources**: [ASP.NET Core antiforgery](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-8.0),
[built-in rate limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0).

## R6 — Additive persistence and bounded lifecycle

**Decision**: Orders/OrderLines with restrictive FKs, immutable name/price snapshots, unique
reference/capability digest, required customer fields, money/type/status checks and order rowversion.
Small internal OrderLifecycle helper defines the four approved transitions. SQL enforces valid
current state/reason, not transition history or aggregate sums; application rules/transactions
enforce those. No transition/list-all endpoint or staff order screen now.
**Rationale**: One future POS data source; new-order rowversion supports later authorized writers
without altering products. Future payment records reference order ID; no payment placeholders.
**Alternatives**: Separate order tables duplicate logic; event sourcing, triggers and generic
state-machine/payment frameworks add unnecessary scope.
**Sources**: [SQL checks](https://learn.microsoft.com/en-us/sql/relational-databases/tables/unique-constraints-and-check-constraints),
[EF migration application](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying).

## R7 — Migration and unknown-outcome safety

**Decision**: One additive migration after InitialMenuAndIdentity; review SQL and confirmed target
before applying. No EnsureCreated, DB reset, migration rewrite, automatic startup migration,
account provisioning, QR regeneration or retry-strategy change. A missing tracking result during
an uncertain save is not proof that an in-flight transaction cannot commit. Explicit retry uses
the same capability/intent; unique persistence prevents duplication.
**Rationale**: Preserves existing data and allows recovery if commit succeeds but response is lost.
**Alternatives**: Automatic POST replay or new credentials on timeout can duplicate orders.
**Source**: [EF commit ambiguity](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency).

## Closure

All planning choices resolved. Browser storage failure, SQL contention, migration review and phone
reachability remain validation risks, not missing business policy. No mandatory new tests or
additional feature requirements inferred. Research and code inspection were read-only.
