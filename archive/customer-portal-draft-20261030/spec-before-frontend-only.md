# Feature Specification: Customer Café Portal

**Created**:2026-10-30 | **Status**: Clarified | **Language/brand**: English, Café (confirmed by user).

## Approved scope

The user requests a real café-style website as the first Run page: Home, Menu, About, customer registration/login, saved profile/contact/address, My Orders and order progress. This extends the delivered cash-only portfolio; it does not restart or waive earlier accepted requirements. Existing guest QR/pickup, staff accounts, cash and reports remain supported.

## User Scenarios & Testing

### US1 — Explore the café (P1)

The root opens a styled café homepage with authentic-looking decorative café imagery, a clear brand/navigation and visible menu/order actions. Menu displays existing available products; About describes the offering without inventing business history, location, opening hours or testimonials. Mobile navigation remains usable.

### US2 — Create and use a customer account (P1)

A visitor registers with name/email/phone/password, receives validation and can sign in/out. Credentials are securely handled; duplicate email and invalid input cannot create partial accounts or staff access. Customer login is separate from staff sign-in; existing staff roles/accounts are preserved.

### US3 — Maintain a profile (P2)

Authenticated customers view/edit their own display name, phone and optional saved address. Refresh/relogin retains saved values. Other users cannot read or edit the profile. The address is profile information only, not a delivery workflow.

### US4 — Find and follow orders (P1)

An authenticated customer places dine-in or pickup orders and finds only their own orders under My Orders, including saved bill, current fulfillment status/progress and rejection reason. New orders can use saved profile contacts. Guest orders/private links remain valid; ownership cannot be inferred from names/phone/reference or claimed without private order access. Cashier status updates appear on refresh. No delivery/shipping progress or online payment is introduced.

## Functional requirements

- FR-001: Root and local Run open the café homepage, with responsive Home/Menu/About navigation and clear order actions.
- FR-002: Public homepage/about imagery and copy must not claim unverified real restaurant facts; existing menu data stays authoritative.
- FR-003: Customers can register/login/logout without acquiring Admin/Cashier privileges or changing existing accounts.
- FR-004: Validate identity/contact/password input server-side; safe errors and protected sessions/forms; no secret leakage.
- FR-005: Profile/name/phone/address persist and are readable/editable only by the current authenticated owner.
- FR-006: Logged-in customers can see their own new orders and saved details/progress; no cross-account access.
- FR-007: Order ownership requires authenticated identity plus actual private order access; never infer ownership from contact/reference/table data.
- FR-008: Preserve guest QR/pickup/private tracking, authoritative price/reconfirmation/deduplication and staff fulfillment/cash/report behavior.
- FR-009: Historical order prices/contact snapshots remain immutable when profile/menu values change.
- FR-010: Preserve all data/accounts/QRs/unfinished archived work; additive storage only with reviewed backup/migration and preservation checks.
- FR-011: English Café branding, readable keyboard/touch navigation and no overflow at390/768/1280; root visibly resembles a café site rather than a developer/order form.
- FR-012: Payment remains cash, order modes remain dine-in/pickup. No fictitious reviews, delivery, social login, provider payments, public deployment or outbound email requirement.

## Success criteria

- SC-001: Root/Run display a complete café homepage with working navigation/menu/order actions at review widths.
- SC-002: Two independent customer accounts register/login; invalid registration and privilege injection fail; existing staff login remains functional.
- SC-003: Profile changes survive logout/login; cross-account profile/order requests disclose no private data.
- SC-004: Authenticated dine-in/pickup orders appear only in their owner's history; guest private tracking and cashier-driven progress still work.
- SC-005: Original row/account/QR fingerprints match before/after additive changes; builds/publish and focused browser/security checks pass.

## Entities and assumptions

Customer account; saved profile (name/phone/optional address); private account-order association; existing order/bill/progress. Guest checkout stays optional. Account recovery/email verification and public hosting are not requested; no fake email delivery is claimed. The user confirmed English and temporary Café branding, and requests the homepage first.
