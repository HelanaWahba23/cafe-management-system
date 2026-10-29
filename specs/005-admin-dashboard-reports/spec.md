# Feature Specification: Admin Dashboard & Reports

**Feature Branch**:`master` | **Created**:2026-10-29 | **Status**:Clarified
**Input**:Approved basic sales/order statistics and management reporting, with consistent professional responsive customer, POS and Admin UI using the existing stack.

## Clarifications

### Session 2026-10-29

No unresolved business decisions for basic reports. Preserve existing management and data. Confirmed Paymob sandbox payments must not be presented as real sales. Explicit UTC date labels distinguish order creation dates from receipt recording dates; these are reporting conventions, not new payment/fulfillment rules.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Understand cafe activity (Priority: P1)

An authorized Admin chooses an inclusive date range and sees basic order and recorded-cash statistics.

**Why this priority**:Completes the approved management reporting workflow.
**Independent Test**:Use known owned orders/receipts inside and outside a date window; compare displayed totals/counts with saved records.
**Acceptance Scenarios**:

1. **Given** saved orders, **When** Admin loads the dashboard, **Then** total order count, current status breakdown, dine-in/pickup and customer/counter counts match orders created in the labelled UTC range.
2. **Given** recorded cash receipts, **When** the range is applied, **Then** cash revenue and receipt count/daily totals match cash recorded within that range. Unpaid orders and sandbox receipts never inflate cash revenue.
3. **Given** no matching records, **When** loaded, **Then** zero values and clear empty states appear without errors or fabricated data.
4. **Given** invalid dates/range or revoked/non-Admin access, **When** requested, **Then** clear validation or denial occurs without leaking contacts, private links or payment secrets.
5. **Given** catalog price/name changes, **When** reports refresh, **Then** financial sums still come from historical receipts, never current menu prices.

### User Story 2 - Use one consistent cafe interface (Priority: P2)

Customers, cashiers and Admin use readable, cohesive interfaces on phones/tablets/desktops without losing existing workflows.

**Why this priority**:Makes the completed portfolio usable and presentable.
**Independent Test**:Open menu/cart/private tracking, cashier queue/bill and Admin dashboard/management at320/768/desktop widths and use keyboard controls.
**Acceptance Scenarios**:

1. **Given** customer/POS/Admin screens, **When** opened, **Then** consistent colors, typography, cards, buttons, spacing and feedback remain readable with no horizontal page overflow at320/768/desktop widths.
2. **Given** staff sign-in, **When** successful, **Then** Admin lands on the dashboard and Cashier on POS, with existing menu/table management and public ordering reachable under unchanged role boundaries.
3. **Given** existing carts, private tracking and order controls, **When** styling/navigation is updated, **Then** no state, authorization or lifecycle behavior is replaced or silently discarded. Existing blocked native/phone checks stay pending.

### Edge Cases

- Midnight/date-boundary receipts, rejected/unpaid orders and orders paid on a later day must not confuse order-created and receipt-recorded measures.
- Long names/references and large exact monetary sums wrap/read correctly.
- Busy, load failure and reconnect states prevent misleading stale edits; reports themselves are read-only.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Dashboard and report data MUST require authenticated current Admin membership/security stamp; no Cashier or anonymous access.
- **FR-002**: Provide validated inclusive UTC date filtering with a clearly labelled default range, total orders and current status/type/origin counts for orders created in range.
- **FR-003**: Cash revenue/receipt count/daily cash totals MUST use immutable recorded Cash settlements in the selected receipt-recorded UTC range, never unpaid order totals or current prices.
- **FR-004**: Sandbox payment count/value MUST be separately identified as test data and excluded from real cash revenue; no invented transactions or sample success records.
- **FR-005**: Reports MUST expose aggregate data only, with no customer contacts, capabilities, provider secrets or raw persistence entities; invalid/empty/error states are explicit.
- **FR-006**: Admin navigation/login MUST lead to the dashboard while preserving categories/products/tables and existing Cashier/public access boundaries.
- **FR-007**: Preserve Blazor/Bootstrap and apply a consistent professional cafe theme, accessible labels/focus/touch controls and responsive layouts across customer, POS, Admin and login.
- **FR-008**: Preserve all existing records/accounts/QRs, unfinished changes and validated workflows; no unrelated analytics, export system, delivery or payment rules are added.

### Key Entities

- **Report window**:Inclusive start/end UTC dates with distinct creation/recording axes.
- **Order summary**:Counts grouped by current fulfillment status, fulfillment type and origin.
- **Receipt summary**:Exact historical cash totals/counts grouped by recorded day, with sandbox results separately labelled.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**:Known in/out-of-range order/receipt fixtures produce exact expected counts and EGP sums, including date boundaries and empty range.
- **SC-002**:Every anonymous/Cashier/revoked-Admin report request is denied and aggregate projections contain zero contact/private credential fields.
- **SC-003**:All specified screens remain usable by keyboard with no horizontal page overflow at320/768/desktop widths after theme changes.
- **SC-004**:Debug/Release/publish, targeted workflow/security and preservation checks pass; any unperformed phone/native/provider acceptance stays explicitly pending.

## Assumptions

- Basic read-only metrics only; no accounting ledger, tax calculation, forecasting, CSV/PDF exports, product analytics or new chart dependency.
- Default view is last seven UTC days; each request permits up to366 inclusive days as a bounded portfolio report query, with clear validation. Dates are labelled UTC rather than silently assuming a business timezone.
- Current order status is reported for orders created in the window; receipt revenue follows receipt time even when the order was created earlier.
- Sandbox receipt values may be shown separately but never presented as real revenue. No fake provider settlement is inserted for a demonstration.
- Existing phone/native/provider setup blockers do not prevent independent report/theme development; preserve their task markers.
