# Café Frontend Refresh

**Updated**:2026-10-30 | **Status**: Clarified | **Brand/language**: Café / English.

## Scope clarification

The user explicitly narrowed the request to frontend appearance with simple photos. Earlier customer registration/profile/address/order-history expansion is cancelled and its draft preserved in archive/customer-portal-draft-20261030. No new account workflow, database/entity/migration, delivery or payment behavior is authorized by this feature. Preserve current guest QR/pickup/tracking, staff login, cash and reports.

## User stories and acceptance

### US1 — Café homepage (P1)

Opening the root or Run displays a finished café-style page with a simple photographic hero, Café identity, a clear menu/order call to action and a short about section. Home/Menu/About navigation must work on desktop and mobile. Do not fabricate address, hours, customer reviews or business history.

### US2 — Consistent customer screens (P1)

The existing pickup/table menu and private tracking receive consistent header/footer/spacing/color treatment. Current menu categories/products/prices, cart, checkout and privacy behavior remain intact. Navigation away from a nonempty cart keeps the existing confirmation behavior. Staff areas keep their existing layout.

## Functional requirements

- FR-001: Root and local Run open the Café homepage rather than Swagger or the raw pickup form.
- FR-002: Provide locally served café photo, English Café branding, functional Home/Menu/About/order navigation and short neutral introduction.
- FR-003: Apply responsive visual polish to the public layout/menu without changing order/price/payment/authentication/storage behavior.
- FR-004: All navigation/CTAs work with keyboard and touch; no horizontal overflow at390/768/1280; images have appropriate alt text and load without third-party requests.
- FR-005: Preserve all existing records/accounts/QRs, guest and staff functionality; no new registration/profile fields or database changes.
- FR-006: Keep real catalog data authoritative and payment cash-only; never imply nonexistent customer accounts, delivery or verified real-business details.

## Success criteria

- SC-001: Root visibly presents café photo/brand/heading/menu CTA and about content; Run opens root.
- SC-002: Menu/about/logo navigation works at review widths; screenshot review confirms legibility/contrast and no overflow.
- SC-003: Existing available products render and Add to cart still works; public private-tracking and staff authorization routes retain their behavior.
- SC-004: Build passes; local image serves200; no new credentials/migrations/packages or database writes; scoped diff is reviewed before commit/push.

## Entities and assumptions

No new entities. One decorative generated café photograph, existing public pages and CSS only. No claim that the decorative image is an actual named business. Existing customer account feature was not implemented and is not part of this delivery.
