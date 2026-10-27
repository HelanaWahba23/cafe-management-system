# UI Contract: Customer Ordering

**Rendering**: Existing Blazor Interactive Server, PublicLayout and local Bootstrap.
No customer registration and no new frontend/runtime. Existing Admin/login/QR screens retained.

## Routes and navigation

| Route | Behavior |
| --- | --- |
| Existing /menu/t/{PublicCode} | Same QR target and identified table; extend with add-to-cart actions and cart panel |
| New /menu/pickup | Direct public no-table menu; label Pickup clearly |
| Menu-local checkout panel | Within either menu's interactive root: cart review, name/phone, explicit context and final confirmation; no separate checkout route |
| New /orders/track | Token-free static shell; credential comes from URL fragment or current-tab session metadata after interactive render |

Add a pickup link to PublicLayout so customers can find the no-QR entry directly. Do not reinterpret
an invalid table route as pickup. Keep category links same-document (fragment navigation) rather
than the current full-page navigation which would discard a circuit cart; preserve focus/readability.
Menu and checkout are panels within one interactive root, so reviewing/back-to-menu does not
navigate away or discard the circuit cart. Existing Routes.razor remains static; do not depend
on scoped state surviving a different page/root. Hard reload is not promised to restore a cart;
private pending-outcome metadata is deliberately separate and retained first.

Delivery checkpoints: US2 includes deterministic browser-helper loading before checkout review
and a working private tracking route before order-link acceptance. Its minimum page opens the
matching order in a fresh signed-out browser, shows saved details/current status and applicable
rejection reason, supports same-tab reload, and fails safely without contact exposure or signup.
US3 pickup uses that same page. US4 refines storage/fragment robustness, replaced-link isolation,
accessible loading/retry/copy-link interactions and complete status/security validation; it does
not create the initial route. Reconnect integration stays separate from helper bootstrap.

## Shared menu and CartState

- Reuse existing available-only categories/products, EGP/two-decimal display, empty/error/reload
  states and latest-navigation-wins guard; shared MenuCatalog renders both entry modes.
- Add a product by its ID, never its potentially duplicated name. Repeated addition adjusts
  that row's positive whole quantity; separate IDs remain separate even with identical names.
- CartPanel supports quantity changes and explicit Remove, displays unit prices, line amounts
  and total. Invalid values have field feedback; no empty-cart checkout submission.
- Use scoped circuit CartState with no DbContext/Identity credential. Never register cart state
  as singleton/static. Mode/table changes visibly update context; if a nonempty cart would move
  contexts, warn that navigation clears the unsubmitted cart and offer Leave or Stay. No automatic
  context reassignment, cross-context transfer or durable/cart-sharing requirement is introduced.
- Circuit loss/full navigation may clear unsubmitted selections; explain that limitation. Existing
  table codes/menu records remain unchanged. No reservation, stock counter or pickup scheduling.

## Checkout states

1. **Editing**: Show type/table and cart. Collect name/phone for both types; trim, require nonblank,
   enforce documented storage capacity. No country-specific mask, registration, verification code,
   delivery address, payment form or arbitrary quantity/minimum-order policy.
2. **Reviewing**: Allocate/retain a capability before the first review request and reuse it for
   this checkout; block on storage failure. Call read-only authoritative review with fresh service context. Display each
   current price/amount and total. If changed from cart display, show old/new prices and updated
   total; final action explicitly reads Confirm updated prices and place order. For unchanged
   prices use Place order after the visible final review.
3. **Ready**: Any edit to name/phone/type/table/selection/quantity invalidates the displayed review;
   refresh before allowing a placement action. The last user-reviewed vector is sent unchanged.
4. **Submitting**: Before write, verify the existing crypto-random capability remains successfully
   retained in sessionStorage; storage failure blocks placement with feedback. Disable duplicate interaction;
   extend reconnect submit guard to a generic write-form marker while retaining existing Admin
   coverage. Server deduplication is still mandatory.
5. **Changed price**: A 409 shows new review with no accepted order. Require a fresh explicit
   action; never silently resubmit or treat the previous click as confirmation. Further changes
   repeat this state, including decreases. Unavailable products require deliberate cart correction.
6. **Saved**: Show returned persisted reference, items/quantities/amounts, type/table, Submitted
   status and private tracking link. Start a new order only on an explicit action allocating a new
   capability after a known successful result; never reassign the old pending credential silently.
7. **Uncertain / disconnected**: Do not show definitive success/failure. Retain capability and
   lock writes until existing reconnect flow/full reload. Offer Check order outcome, a read-only
   service call. Found result shows saved confirmation even after menu edits; not-found remains
   inconclusive while a request may still be in flight. Only explicit unchanged retry may use the
   same capability; no automatic POST, new credential or replay of changed cart contents.

The recovery metadata contains capability, started flag and public reference when known, not
customer name/phone or durable cart contents. If cart contents are lost, recovery can still read
the accepted outcome; do not reconstruct/retry a different intent under its capability. Explain
that closing/clearing the tab can lose unsaved cart/private access unless the link was copied.

## Private tracking

- After interactive render, JS reads `#key=...`, stores it in current-tab session storage, removes
  the fragment with replaceState, then passes it explicitly to OrderService. No private data is
  loaded or credential serialized during prerender. Same-tab reload may use retained capability.
- The full link can be copied to another browser/phone to grant access intentionally. Show a
  short warning that anyone with the private link can view the order. Use configured trusted
  origin, never incoming Host; copied value is browser-constructed, not logged/server-rendered.
- Show minimal tracking DTO: reference/type/applicable table snapshot, lines/total/EGP, status,
  created time, and rejection reason when Rejected. Omit stored customer contact fields.
- Refresh status on explicit reload, no new polling, notifications or status-changing controls.
  Prepared examples cover all five states. Rejected always has the recorded nonblank reason.
- Missing/invalid link and unknown capability use the same safe unavailable-order outcome.
  No public search/list, recovery-by-phone/name/reference/table or staff navigation is offered.
- Set no-store/no-referrer response headers on /menu/t/*, /menu/pickup and /orders/track before
  rendering through middleware/endpoint handling, since checkout is embedded in the menu document;
  do not change HTTP headers from a live circuit. Keep local assets and encoded text.

## Responsive/accessibility acceptance

At 320, 768 and 1280px, menu/cart/checkout/tracking fit without horizontal page scrolling.
Label name/phone and quantity inputs; use appropriate phone keyboard without inventing phone
syntax. Make add/remove/confirm/reload/copy controls reachable by touch and keyboard, announce
loading/errors/status with accessible regions, move focus to changed-price summary/errors,
and retain the existing reconnect overlay's inert background and reload warning.
Use manual mobile/tablet/desktop review; actual phone access remains subject to secure reachability.

## Future boundaries

No Cashier/POS order board, status/rejection form, payment buttons, bill, receipt, checkout-paid
claim, delivery or reports. Internal lifecycle rules and prepared display samples do not authorize
staff mutation UI. Preserve existing protected Admin screens and authentication unchanged.
