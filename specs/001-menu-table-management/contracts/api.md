# API Contract: Menu and Table Management

Design contract for the existing ASP.NET Core 8 controllers plus the co-hosted Blazor Web App.
No endpoints or application code generated. Keep existing /WeatherForecast and Development
Swagger. Document /api routes and DTOs in Swagger; HTML account pages are documented here,
not misrepresented as JSON APIs. API routes remain usable independently of Blazor components.

## Shared server application boundary

Controllers and Interactive Server components call the same MenuService/TableService/QR behavior.
Components use C# view models and typed results directly; they do not issue loopback HTTP or copy
browser cookies to a server HttpClient. Every management read/write authorizes Admin server-side.
The trusted principal comes from controller User or circuit AuthenticationStateProvider; neither
client-supplied roles nor table PublicCode authenticates an administrator. A small staff-access
helper checks current Identity user/security stamp/Admin membership in a short scope, using no
long-lived circuit DbContext. Controller policies supplement, not replace, service checks.

Service validation/forbidden/not-found/conflict outcomes map to field messages in components and
HTTP statuses below in controllers. Never let UI guards be the only authorization boundary.

## Identity account HTTP forms

Identity application cookie: HttpOnly, Secure, SameSite=Lax, session lifetime (no remember-me
feature). Roles Admin and Cashier. Cashier may authenticate but cannot manage this feature.
Only operator-provisioned accounts; no public registration, role assignment or recovery flow.

| Method | Route | Access/input | Success or failure |
| --- | --- | --- | --- |
| GET | /account/login | Anonymous; optional local returnUrl | 200 HTML Razor Page with email/password and antiforgery form field. |
| POST | /account/login | Form email/password/returnUrl plus valid antiforgery token/cookie | Set Identity cookie, local 302 redirect causing full navigation; invalid credentials -> 200 HTML generic failure, no authenticated session. |
| POST | /account/logout | Authenticated staff; valid form antiforgery token/cookie | Clear Identity session; local 302 redirect to login. No GET sign-out. |
| GET | /api/auth/me | Authenticated staff | 200 Session; otherwise 401. |
| GET | /api/auth/antiforgery | Anonymous or staff | 200 { requestToken: string }; sets framework antiforgery cookie, no-store. |

Session = { email: string, roles: string[] }. Request-token endpoint permits manual/HTTP clients
to call protected cookie-authenticated APIs; ordinary Blazor service calls do not use it.
After login/logout obtain a fresh HTTP token before another API write because identity changed.
ReturnUrl must be local, otherwise use a safe local default. Admin default is /admin/categories;
Cashier receives an authenticated access-denied view, not a POS/dashboard feature.

Login/logout use Razor Pages HTTP handlers and Identity SignInManager, not interactive events
or a component calling SignInManager after response headers have started. Disable enhanced
form interception for those account forms; navigate fully after cookie changes to rebuild the
circuit. Invalid antiforgery returns 400; credential/lockout failure remains generic and must
not reveal account existence. Retain framework password hashing and documented lockout defaults.

## HTTP antiforgery, circuit security and failures

- Validate antiforgery tokens on account POSTs and every POST/PUT cookie-authenticated /api/admin
  request. Account forms carry the hidden field; JSON consumers send RequestVerificationToken
  header paired with the framework cookie. Do not expose a readable authentication cookie.
- Interactive component events are circuit messages, not calls to the controller endpoints.
  Use built-in Blazor circuit security, same-origin HTTPS, and explicit service authorization;
  do not claim controller antiforgery attributes automatically protect direct service calls.
- Blazor routes use server authorization and a revalidating authentication-state provider.
  Protected service operations use the current trusted identity checks described above; API
  endpoints also require Admin policy. Do not rely on circuit-time IHttpContextAccessor.
- Unauthenticated /api calls -> 401, wrong role -> 403, no HTML redirects. HTTP page navigation
  may direct anonymous staff to /account/login. Sign-out in another tab does not guarantee
  immediate termination of already-connected circuits; document/review revalidation behavior.
- Errors for /api routes are application/problem+json; validation includes errors dictionary.
  No stack traces, SQL, connection strings, passwords or raw internal exceptions in responses.
- No permissive cross-origin configuration. Framework circuit/static assets share the host.
  No-store for auth and dynamic menu responses. No secret-bearing diagnostics in browser output.

## Admin category endpoints

All admin routes below require authenticated Admin; unsafe methods also require antiforgery.

| Method | Route | Body | Success |
| --- | --- | --- | --- |
| GET | /api/admin/categories | None | 200 Category[] |
| GET | /api/admin/categories/{id} | None | 200 Category |
| POST | /api/admin/categories | CategoryInput | 201 Category, Location to item |
| PUT | /api/admin/categories/{id} | CategoryInput | 200 Category |

CategoryInput: { name: string }. Category: { id: integer, name: string }.
Trimmed name length 1–120; blank rejected; duplicates allowed. No DELETE/deactivation routes.

## Admin product endpoints

| Method | Route | Body | Success |
| --- | --- | --- | --- |
| GET | /api/admin/products | None | 200 Product[], including unavailable |
| GET | /api/admin/products/{id} | None | 200 Product |
| POST | /api/admin/products | ProductInput | 201 Product, Location to item |
| PUT | /api/admin/products/{id} | ProductInput | 200 Product |

ProductInput: { name: string, categoryId: integer, price: string, isAvailable: boolean }.
Product: same fields plus id: integer and currency: "EGP". isAvailable is required.
price uses invariant digits and at most two fractional digits, positive and within decimal(18,2).
Output always two fractional digits, e.g. "25.50"; currency is read-only. Blazor binds decimal
and calls services directly; this HTTP wire shape remains unchanged by the frontend revision.
Do not bind client currency, id, role or unrelated entity properties as editable data. Category
must exist. Name length 1–120 after trim; duplicates allowed. PUT replaces editable fields only.

## Admin table and QR endpoints

| Method | Route | Body | Success |
| --- | --- | --- | --- |
| GET | /api/admin/tables | None | 200 Table[] |
| GET | /api/admin/tables/{id} | None | 200 Table |
| POST | /api/admin/tables | { label: string } | 201 Table, Location to item |
| PUT | /api/admin/tables/{id} | { label: string } | 200 Table |
| GET | /api/admin/tables/{id}/qr | None | 200 image/png |

Table: { id: integer, label: string, publicCode: UUID, menuUrl: absolute HTTPS URL }.
Label nonblank, trimmed length 1–50; uniqueness uses invariant-uppercase normalized value as
specified in data-model.md. Display casing retained. PUT cannot change PublicCode. QR rendering
is an idempotent read of the stable table link; repeated rendering does not create new links.

Blazor may display the generated PNG bytes returned by the authorized QR service as an inline
image. The controller route remains for HTTP consumers; no forced loopback is required. Generate
menuUrl from trusted configured PublicWebBaseUrl, not unchecked Host headers. No image storage,
QR rotation, printing workflow, deletion or deactivation added.

## Anonymous customer menu

GET /api/menu/tables/{publicCode}, no authentication or customer account.
The Blazor /menu/t/{publicCode} component calls the same anonymous menu-query service directly.
Both paths enforce the same server-side visibility rules and table context.

200 response:

```json
{
  "table": { "label": "Table 1", "publicCode": "f47ac10b-58cc-4372-a567-0e02b2c3d479" },
  "currency": "EGP",
  "categories": [
    { "id": 1, "name": "Coffee", "products": [
      { "id": 2, "name": "Espresso", "price": "25.50" }
    ] }
  ]
}
```

Only available products are returned/projected; do not send hidden data to the browser to filter.
Categories with no visible products use products: []; no categories uses categories: []. Shared
menu per cafe, requested table context. Omit internal table IDs, Identity data, normalization
fields and unavailable-product details from the public projection. Malformed publicCode -> 400;
well-formed unknown code -> 404; never choose a different table. Component equivalents are clear
invalid/unknown-table states without a login prompt. Next open/reload shows current values.

## Common validation and failure behavior

- Positive integer route IDs, invalid body/IDs -> 400; missing addressed resource -> 404.
- Field validation -> ValidationProblemDetails with errors; component uses the same field errors.
- Missing CategoryId on save -> 400 errors.categoryId; missing addressed product -> 404.
- Duplicate table label, including races -> 409 ProblemDetails with errorCode
  "duplicate_table_label" and field "label". Failed write rolls back.
- No hidden removal routes. Unknown /api routes stay API 404, never Blazor HTML fallbacks.
  Unsupported methods return 405 where routed; sensitive routes never rely on hiding controls.
- Small nonpaginated lists ordered by ID. All backend validation applies to HTTP and circuit
  entry points even when a form is bypassed; never bind persistence or Identity entities.
- Unhandled /api errors -> generic 500 ProblemDetails; component failures -> safe error message.
  Log diagnostics server-side without secrets. Do not automatically retry a mutation after
  a lost circuit: it may have committed. Reload before another explicit save.

## Traceability

FR-001 -> Admin policy, Identity and service checks; FR-002/003/004 -> category/product services,
API contracts and save errors; FR-005/006 -> table CRUD and stable QR/public menu;
FR-007/008 -> anonymous menu, reload and failure states; FR-009 -> no deletion/deactivation;
FR-010 -> public projection; FR-011 -> EGP/precision/nonblank/name-duplicate/table-code constraints.
