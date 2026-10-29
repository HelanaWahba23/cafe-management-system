# Feature Specification: Cashier & POS

**Feature Branch**: `master` (existing checkout; feature directory is independent)
**Created**: 2026-10-29
**Status**: Clarified
**Input**: Approved cashier order management, walk-in orders, status updates and bills; preserve existing application.

## Clarifications

### Session 2026-10-29

- Q: Walk-in fulfillment? A: Dine-in with an existing table, or takeaway without a table.
- Q: Walk-in contacts? A: Name and phone optional. Online customer contacts remain required.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Manage incoming orders (Priority: P1)

A cashier sees customer dine-in and pickup orders together, opens details and updates fulfillment.

**Why this priority**: Connects customer ordering to cafe operations.
**Independent Test**: Submit both customer types; sign in as cashier, refresh, filter, open details and advance status.

**Acceptance Scenarios**:

1. **Given** saved orders, **When** a cashier refreshes, **Then** both types appear with reference, time, context, status and total; status filtering returns matching orders.
2. **Given** Submitted, **When** preparation, Ready and Completed are selected in order, **Then** each persists and private tracking reflects it.
3. **Given** Submitted, **When** rejected with a trimmed nonblank reason up to 500 characters, **Then** tracking shows Rejected and reason. Rejection after preparation and terminal changes fail.
4. **Given** two cashiers viewing the same version, **When** both change status, **Then** the stale write conflicts and requires reload.
5. **Given** anonymous, non-cashier or revoked access, **When** reads or writes are requested, **Then** access is denied without order data.

### User Story 2 - Create walk-in orders (Priority: P2)

A cashier selects available products, chooses table dine-in or takeaway, optionally adds contacts, reviews current prices and explicitly submits.

**Why this priority**: Counter and customer orders share the operational queue.
**Independent Test**: Create both modes without contacts; verify snapshots, creator, totals and retry recovery.

**Acceptance Scenarios**:

1. **Given** available items, **When** a reviewed dine-in order uses an existing table, **Then** one Submitted order saves with table-label snapshot and cashier creator.
2. **Given** takeaway with empty contacts, **When** submitted, **Then** no table or fabricated contacts are saved.
3. **Given** changed prices after review, **When** submitted, **Then** no order saves until explicitly reconfirmed; unavailable items or missing tables require correction.
4. **Given** a lost reply, **When** the unchanged attempt is retried, **Then** one saved order is recovered; changed-intent reuse is refused.
5. **Given** guest checkout, **When** either contact is absent, **Then** it stays invalid. Counter orders receive no anonymous tracking link.

### User Story 3 - View itemized bills (Priority: P3)

A cashier views the saved order itemization.

**Why this priority**: Accurate bills precede Feature 004 payments.
**Independent Test**: Change catalog prices/names after saving; the bill must remain unchanged.

**Acceptance Scenarios**:

1. **Given** a saved order, **When** its bill is viewed, **Then** saved names, quantities, unit prices, line totals, total EGP, reference and fulfillment context appear consistently.
2. **Given** no payment integration yet, **When** viewed, **Then** the bill makes no payment-settled or paid-receipt claim.

### Edge Cases

- Empty queue, invalid page/filter, missing order, unavailable products, duplicate product IDs, invalid quantities and excessive totals produce clear errors without writes.
- Repeated clicks and uncertain outcomes recover the same attempt; another cashier cannot recover it.
- Disconnect disables writes; stale details never silently overwrite newer status.
- Long names and rejection reasons wrap on narrow screens.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Only authenticated current Cashier identities may read or mutate POS data. Preserve existing Admin privileges.
- **FR-002**: Provide a bounded shared queue of customer and counter orders, status filter, refresh and details.
- **FR-003**: Permit only Submitted → Preparing → Ready → Completed or Submitted → Rejected with trimmed nonblank reason up to 500 characters.
- **FR-004**: Detect stale status writes and require reload; preserve saved lines, contacts, context and totals.
- **FR-005**: Counter creation offers dine-in with an existing table or takeaway without one; name and phone optional only for counter orders.
- **FR-006**: Validate available items, quantities, table context and exact EGP totals on the server; save immutable snapshots atomically; require explicit price reconfirmation.
- **FR-007**: Retain authenticated counter creator; identical retries save once and recover the result; changed-intent reuse fails.
- **FR-008**: Show a per-order itemized saved bill without claiming settlement.
- **FR-009**: Provide keyboard operation, clear busy/error/empty states and usable responsive controls at 320px, 768px and desktop widths.
- **FR-010**: Preserve existing records, accounts, QR identifiers, menu administration, customer ordering and private tracking. No anonymous access to counter orders.
- **FR-011**: Payments/invoices and reports belong to Features 004–005. No delivery, invented taxes, discounts, split bills or refunds.

### Key Entities

- **Order**: Existing saved intent/lines, fulfillment state and context, extended with customer/counter origin and cashier creator.
- **Counter attempt**: Private identifier bound to cashier and draft for duplicate recovery.
- **Bill**: Read-only saved-order itemization; payment state is separate.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Both customer types and both walk-in choices appear correctly in all four queue journeys.
- **SC-002**: Every permitted status edge succeeds; other edges, stale writes and invalid rejection reasons fail without data loss.
- **SC-003**: Identical retries yield exactly one order; changed prices require reconfirmation; later catalog edits never alter bills.
- **SC-004**: All anonymous/non-cashier/revoked-access checks deny access; guest tracking gains no staff contacts or counter access.
- **SC-005**: Queue, details, bills and creation are keyboard usable without horizontal page overflow at 320px, 768px and desktop widths.
- **SC-006**: Preservation checks show unchanged prior records/accounts/QRs; build, security and workflow evidence is recorded honestly.

## Assumptions

- Reuse existing Cashier accounts and lifecycle. Admin role does not implicitly grant POS access.
- Takeaway uses existing no-table pickup semantics, labelled Takeaway for counter origin.
- Bills are per-order itemizations; no fiscal invoice or settlement claim here.
- Manual refresh and bounded pages suffice; no notification system is required.
- Feature 002 native navigation and physical-phone gaps remain pending independently.
