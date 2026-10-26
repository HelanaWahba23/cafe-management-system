# Research: Menu and Table Management

**Date**: 2026-10-25. Revised for the approved .NET 8 Blazor Web App and Bootstrap stack.
Only planning documents changed; no dependencies, source, templates, accounts or databases created.

## Single-host Blazor and API

**Decision**: Add Blazor Web App services/components to the existing net8.0 Web SDK project;
retain MapControllers, Swagger and the sample API. Interactive Server on admin screens and the
customer menu; Bootstrap 5.3.8 CSS served locally, pinned with license. A second project/process,
client runtime, frontend package toolchain and cross-origin cookie forwarding are unnecessary.
**Rationale**: Reuses existing hosting and .NET build, reducing three-day integration work.
**Alternatives considered**: A separate Blazor host needs deployment/auth/service coordination;
WebAssembly adds client assets/auth/HTTP boundaries without a confirmed offline requirement.
Static rendering alone is unsuitable for the requested interactive admin screens.
**Evidence**: [Microsoft render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0)
describes Interactive Server registration and per-component modes.
[Bootstrap introduction](https://getbootstrap.com/docs/5.3/getting-started/introduction/)
documents responsive styles and direct assets without a package build pipeline. Do not add its
JS bundle unless an actual component requires it; prefer small Razor interactions.

## Shared application boundary

**Decision**: Controllers and interactive components call the same concrete application services.
Controllers map HTTP DTOs/results; components bind C# form/view models and map service errors
into validation summaries. Management services accept only a trusted server principal: HTTP User
or AuthenticationStateProvider's principal. They authorize every protected read/write. A small
staff-access helper checks current Identity user/security stamp/Admin membership in a short scope.
No authorization trusts a principal/role submitted in a request body.
**Rationale**: Components run on the server; an internal HTTP call would introduce duplicate
serialization and fail to automatically carry the browser's authenticated cookies.
**Alternatives considered**: Cookie forwarding and a browser API wrapper add needless lifecycle
work. Component visibility/route authorization alone does not protect shared service entry points.
**Evidence**: [Blazor security](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-8.0)
explains server authentication state and the distinction between display gating and secure access.

## Identity and HTTP/circuit distinction

**Decision**: ASP.NET Core Identity UserManager/SignInManager, standard roles Admin/Cashier,
HttpOnly Secure SameSite=Lax session cookie. Only Admin can manage menu/tables or retrieve QR
management data. Cashier can log in but has no feature privileges. One-time operator bootstrap
creates missing roles and supplied account; no public registration or role-management feature.

Use ordinary Razor Pages at /account/login and /account/logout, protected by form antiforgery.
Login/logout perform HTTP POST and then full browser navigation so cookie changes take effect
in a new circuit. No SignInManager calls from interactive component event handlers. Provide
IdentityRevalidatingAuthenticationStateProvider-style circuit checks as well as per-operation
staff validation; do not rely on interactive-time HttpContext or stale role display claims.
**Rationale**: Cookies require a live HTTP response before headers are sent. Circuit events
are not new HTTP login requests. Long-lived circuit state needs explicit revalidation.
**Alternatives considered**: Full interactive login cookie mutation is unreliable; mapping the
entire default Identity API adds unrequested registration/recovery flows. No bearer-token storage.
**Evidence**: [Microsoft Blazor/Identity security](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-8.0)
and [.NET 8 Identity overview](https://devblogs.microsoft.com/dotnet/whats-new-with-identity-in-dotnet-8/).
Use Razor Pages for the narrow HTTP account boundary, rather than scaffolding every account page.

**Decision**: HTTP account forms include server-generated antiforgery tokens. Unsafe
cookie-authenticated controller calls require the configured RequestVerificationToken header,
paired with the framework cookie. GET /api/auth/antiforgery supplies the request token for
manual/API consumers, no-store. The Blazor UI does not need to fetch this endpoint for ordinary
service calls. Interactive events use framework circuit protections plus explicit server-side
service validation, not fictional per-event controller CSRF tokens. Reject untrusted cross-origin
access; no permissive CORS. Authentication/authorization precede UseAntiforgery.
**Evidence**: [Microsoft antiforgery](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-8.0).
Do not assume a newer framework's automatic header-CSRF middleware exists in .NET 8.

## EF Core lifetime and SQL Server

**Decision**: CafeDbContext remains IdentityDbContext in one SQL Server database. Register
IDbContextFactory consistently; the registration also permits HTTP-scoped Identity stores.
Every menu/table service operation creates and disposes a fresh context. Components retain DTOs,
not contexts or tracked entities. Circuit Identity validation creates its own short service scope.
Avoid conflicting scoped options/singleton factory registrations and test DI construction early.
**Rationale**: Blazor scopes can last an entire circuit; DbContext is not thread-safe and cannot
be shared across overlapping component actions. Disable duplicate form submits as well.
**Alternatives considered**: A circuit-scoped DbContext/UserManager retains tracked state too long;
generic repositories or separate databases do not solve lifecycle and add complexity.
**Evidence**: [Microsoft Blazor EF Core](https://learn.microsoft.com/en-us/aspnet/core/blazor/blazor-ef-core?view=aspnetcore-8.0)
recommends factory-created contexts per operation. [EF providers](https://learn.microsoft.com/en-us/ef/core/providers/)
requires compatible provider/EF majors. Use compatible serviced 8.0.x EF/Identity packages/tooling.

## QR navigation and exact currency

**Decision**: Keep QRCoder 1.8.1 PngByteQRCode; generate images on demand from immutable unique
GUID PublicCode plus trusted HTTPS PublicWebBaseUrl and /menu/t/{publicCode}. No image storage,
external QR service, per-table menu copies or link rotation. Label edits preserve old QR links.
**Rationale**: Minimal local rendering without System.Drawing or image-lifecycle storage.
**Alternatives considered**: Label-based URLs break on edit; expiring signed links do not fit
stable table navigation; possession is not proof of presence or authorization.
**Evidence**: [QRCoder package](https://www.nuget.org/packages/QRCoder/1.8.1),
[maintainer repository](https://github.com/Shane32/QRCoder).

**Decision**: Keep EGP decimal(18,2), price >0 and no silent excessive-scale rounding.
Blazor uses decimal form values and two-place formatting. Preserve invariant string price
representation in HTTP DTOs; direct service calls use decimal, no unnecessary serialization.
**Rationale**: Exact internal arithmetic and an explicit existing HTTP contract, independent of UI.
**Alternatives considered**: Binary floating point or silently rounded inputs violate approved precision.

## Engineering assumptions retained from approved planning review

- Trim input; category/product names 1–120 chars, table labels 1–50; reject whitespace-only.
- Duplicate category/product names allowed. Table label uniqueness uses trimmed invariant-uppercase
  key with binary SQL comparison; A1/a1 conflict. Display casing retained.
- Product belongs to one required existing category. Explicit availability input, no default guessed.
- Last successful edit wins for competing edits, with SQL uniqueness/FK/check constraints enforced.
  No collaboration UI, pagination/search, auditing subsystem or concurrency framework added.
- Public read includes all categories and only available products. Empty categories have explicit
  empty states. Next load/reload reflects changes; no menu push/caching layer.
- Interactive Server prerendering may read twice; reads must be side-effect-free. Writes occur
  only on explicit authorized actions, never during initialization/prerender or reconnection.

## Risks, dependencies and validation implications

| Risk/dependency | Handling |
| --- | --- |
| Persistent connection | Interactive Server requires server connectivity and circuit transport. Show reconnect state; block uncertain resubmissions and reload records after an interrupted save. No offline promise. |
| Circuit resources | One host is appropriate for portfolio scale; proxy/WebSocket support and actual capacity must be verified before deployment. Do not add scale-out infrastructure now. |
| Identity staleness | Full reload on cookie changes, revalidating provider, per-operation user/stamp/role checks. Logout of another tab does not imply instant circuit termination. |
| Database lifetime | Factory per operation, short Identity scopes, DI validation and two-tab/manual repeated-action checks. |
| Existing old runtime | SDK 8.0.400/runtime 8.0.8 previously observed; use supported servicing patches before claiming operational readiness. |
| SQL and phone access | Named service observed but connectivity/version/permissions unverified. Dedicated database, supplied secrets and a phone-reachable trusted HTTPS URL required. |
| Asset/package compatibility | Pin actual compatible EF/Identity/QR/Bootstrap versions at implementation, retain static-asset license; no package restores proven here. |
| Schedule | Minimize UI and integration; establish one end-to-end slice early. Three-day overall scope is a constraint, not a delivery guarantee. |
| Source wording | Constitution v1.0.1 reconciles the confirmed scope inventory; principles and governance rules remain unchanged. |

[Official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)
lists .NET 8 end of support as 2026-11-10. Preserve requested net8.0; make a separate decision
before operating beyond support. Some live documentation mixes newer framework examples into
older views: use .NET 8 APIs, not MapStaticAssets, RendererInfo or newer routing attributes.

No new business decision or alternative rendering mode is needed. Static HTTP account pages are
a technical exception to interactive screens, not a change to the feature's customer/admin scope.
