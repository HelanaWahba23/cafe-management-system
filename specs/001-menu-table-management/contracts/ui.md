# UI Contract: Menu and Table Management

Design only: .NET 8 Blazor Web App, Bootstrap responsive layouts and Interactive Server for
admin/menu components. Host in the existing API project. No separate frontend project, process
or client build pipeline. HTTP account pages are the intentional rendering exception.

## Staff session and render boundaries

- /account/login: ordinary Razor Page GET/POST with email/password, local returnUrl and framework
  antiforgery token. Generic credential failure, no registration link or client-selectable role.
  Submit as a full HTTP form; do not intercept into a circuit or call SignInManager from one.
- Successful login sets the Identity cookie and redirects with full navigation: Admin to
  /admin/categories or a validated permitted local target; Cashier to access denied for this
  feature, without inventing a cashier dashboard. Authenticated identity comes from the server.
- Admin layout shows Categories, Products, Tables, session identity and a POST sign-out form.
  /account/logout validates antiforgery, clears cookie and fully navigates to login. Fresh
  navigation establishes the new circuit identity; no browser token storage.
- Routable admin components require Admin and use AuthorizeRouteView/cascading authentication
  state; display gating supplements service authorization and current Identity checks.
  Obtain circuit identity from AuthenticationStateProvider, not a retained HttpContext.
- Lost/invalidated authorization blocks further protected operations and directs staff to a
  full sign-in navigation. Do not equate hidden buttons with access enforcement.

## Admin screens (Interactive Server)

| Route | Screen and actions | Validation and states |
| --- | --- | --- |
| /admin/categories | Bootstrap list/table; Add/Edit name form | Nonblank, max120; duplicate names allowed; loading/empty/save errors. |
| /admin/products | All products: Name, Category, Price EGP, Availability; Add/Edit form | Name nonblank max120, existing category required, positive price <=2 decimals, explicit availability. |
| /admin/tables | Labels, Add/Edit label form, QR preview with table label/menu link | Nonblank max50; unique normalized label; conflict shown at label; stable QR. |

Use EditForm/EditContext with validation summaries/field messages and Bootstrap form/grid styles.
Bind prices to C# decimal, validate excessive scale before persistence, format EGP to two places.
Services return projected values and typed validation outcomes; components do not call loopback
APIs. Avoid a third-party component suite, modal framework or global state store.

Disable duplicate saves during an operation, preserve input on validation errors, and reopen
saved values after success. Unavailable products stay visible/editable to admins. Category
selector can show ID alongside duplicate names to distinguish records. QR preview can bind
inline PNG bytes from the authorized service; label edits retain the same menu destination.
No deletion/deactivation, QR rotation, print/download template or bulk actions are added.

Use Bootstrap grid/spacing/card/table classes and small local CSS. Prefer Razor events for
controls over Bootstrap JS plugins manipulating Blazor-owned DOM. Local pinned styles/assets
and license are served from wwwroot; do not require an asset package manager or CDN at runtime.

## Public menu (Interactive Server)

- /menu/t/{publicCode}: anonymous routable component outside staff-only layout. No sign-in,
  account creation, authentication preflight or staff shell to view the menu.
- Read-only query service selects the shared cafe menu with correct visible table label.
  Show categories and available products only, name and EGP price with two decimal places.
  No internal management/Identity fields or hidden products in browser-visible projections.
- Responsive Bootstrap stacked cards/content and simple category navigation. Review at 360,
  390 and 1280px: readable text, usable controls, keyboard/focus access, no page horizontal scroll.
- Show clear loading/empty category/empty menu/read error states and a retry-read action.
  Invalid/unknown table is explicit, never silently mapped to a different table or login.
- Next open/reload reads current prices/availability; no application live-menu push or polling.
  The rendering circuit itself is not a new real-time menu feature.

## Circuit, navigation and recovery behavior

- Apply Interactive Server per admin/menu page; account HTTP pages remain outside interactive
  rendering. Do not adopt Auto or WebAssembly, or newer framework routing/static-asset APIs.
- Framework prerendering reads must be side-effect-free; do not create a record or issue a
  mutation during component initialization. Do not retain EF contexts in component state.
- On connection interruption show the framework reconnect state and disable writes. A lost
  response does not prove a failed save: do not replay writes automatically. Once a circuit is
  lost, reload current records before an explicit new save; unsaved edits may be lost on reload.
- Login/logout force a full navigation, not enhanced same-circuit routing. A separate tab's
  logout may leave another circuit until revalidation; protected service operations verify user,
  security stamp and role and must not rely only on old UI claims.
- Direct browser navigation/reload of /menu/t/{publicCode} and /admin/* must resolve correctly.
  Controllers, /api/*, /WeatherForecast and Swagger retain their routing and response behavior.
- Real phone QR requires configured reachable trusted HTTPS origin and circuit connectivity;
  desktop localhost and responsive emulation alone do not prove that end-to-end journey.

## Scope and acceptance mapping

No ordering/cart/checkout, payments, order tracking, pickup flow, reports, POS or customer profile
screens here. Menu maintenance -> SC-001/008; QR continuity -> SC-002; anonymous browsing ->
SC-003; mobile widths -> SC-004; access control -> SC-005; failure/recovery -> SC-006;
create/edit-only and visibility -> SC-007. Circuit recovery is technical failure handling, not
an added offline business requirement. Validate manually; no mandatory automated tests.
