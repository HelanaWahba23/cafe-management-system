# Feature Specification: Customer Ordering

**Feature Branch**: None created; feature directory is independent of the Git branch.

**Created**: 2026-10-27

**Status**: Clarified — ready for planning review.

**Input**: Customer Ordering: browse available products, manage a cart, place dine-in
orders through valid table QR links or online pickup orders without a QR code, share one
order workflow for future Cashier/POS use, and track order status. No payment processing.

## Clarifications

### Session 2026-10-27

- Q: Which fulfillment lifecycle applies to dine-in and pickup? → A: Both use Submitted →
  Preparing → Ready → Completed. Rejection is allowed from Submitted only, with a required
  rejection reason.
- Q: Which checkout details and tracking access are required? → A: Guest checkout without
  registration; customer name and phone number are required for both order types, and the
  customer receives a private order-tracking link.
- Q: What happens if prices change before submission? → A: Show the updated prices and
  totals and require explicit customer confirmation before placing the order.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Review a Cart from the Current Menu (Priority: P1)

A customer browses the existing menu, adds available products, changes quantities, removes
items, and reviews quantities and EGP totals before deciding to place an order.

**Why this priority**: Both order types need a clear, reviewable selection of products.

**Independent Test**: Browse sample products and manage a cart without submitting an order
or needing a cashier or payment workflow.

**Acceptance Scenarios**:

1. **Given** a menu with available and unavailable products, **When** a customer browses,
   **Then** only available products appear, with categories and EGP prices to two decimal places,
   without requiring sign-in to browse.
2. **Given** two available products, **When** the customer adds them, increases and decreases
   quantities, and removes one, **Then** the cart shows the selected products, positive whole-number
   quantities, line amounts, and a total matching its displayed unit prices and quantities.
3. **Given** an empty cart or an invalid quantity such as zero, a negative number, or a fraction,
   **When** submission or the invalid quantity change is attempted, **Then** the action is rejected
   with a clear explanation; removing an item is a separate supported action.
4. **Given** two distinct products with the same name, **When** both are selected,
   **Then** they remain distinct selections with their own prices and quantities.

---

### User Story 2 - Place a Dine-in Order for the Correct Table (Priority: P1)

A customer follows an existing table QR link, reviews the identified table and cart, and
submits a dine-in order associated with that table.

**Why this priority**: This extends the existing table menu into the confirmed dine-in journey.

**Independent Test**: Using an existing table and available products, submit one dine-in
order and reopen its confirmation without a cashier screen or payment integration.
Use guest checkout with customer name and phone number; the initial status is Submitted.

**Acceptance Scenarios**:

1. **Given** a valid existing table link and a valid nonempty cart, **When** the customer
   reviews and submits as a guest with a customer name and phone number, **Then** exactly one order is
   recorded as dine-in for that table, and a confirmation shows its reference, items,
   quantities, accepted prices, total, table context, status Submitted, and a private tracking link.
2. **Given** a malformed or unknown table link, **When** dine-in submission is attempted,
   **Then** no order is created and the customer receives a clear table-context error;
   the system does not substitute a different table or silently convert the order to pickup.
3. **Given** a table whose label was edited, **When** its previously issued QR link is used,
   **Then** it still identifies the same table and the order uses that table's current label.
4. **Given** a product became unavailable after selection, **When** the customer submits,
   **Then** no partial order is created, the affected selection is identified, and the customer
   can correct the cart before submitting again.
5. **Given** an interrupted submission or repeated activation of the same submission,
   **When** the customer checks its outcome or retries that submission,
   **Then** it resolves to at most one order and does not silently replay a new order.
6. **Given** a missing or whitespace-only customer name or phone number, **When** submission
   is attempted, **Then** the missing detail is identified and no order is placed. Supplying
   both details permits submission without registering or signing in.
7. **Given** a selected product's price changed since the last cart review, **When** the
   customer attempts submission, **Then** updated unit prices, line amounts and the total
   are shown and no order is placed until the customer explicitly confirms those amounts.
8. **Given** refreshed prices awaiting confirmation, **When** the customer declines or leaves
   without confirming, **Then** no order is placed. If prices change again before acceptance,
   the newly changed amounts require another explicit confirmation.

---

### User Story 3 - Place an Online Pickup Order (Priority: P1)

A customer enters the cafe website directly, browses the same available menu, reviews a
cart explicitly identified as pickup, and places an order without a table QR code.

**Why this priority**: Pickup is a separately confirmed entry point, not a fallback for invalid tables.

**Independent Test**: In a fresh visit without a table link, submit a pickup order and reopen
its confirmation and private tracking link as a guest with name and phone number.
No dine-in journey is required.

**Acceptance Scenarios**:

1. **Given** a customer entering the website without a QR link, **When** they select pickup,
   **Then** the available menu and cart are accessible without supplying a table identifier.
2. **Given** a valid pickup cart and a customer name and phone number, **When** it is submitted,
   **Then** exactly one pickup order is recorded with no table association, and its confirmation
   shows the order reference, items, quantities, accepted prices, total, status Submitted,
   and a private tracking link, without requiring registration or sign-in.
3. **Given** one dine-in and one pickup order, **When** their saved details are reviewed,
   **Then** both use Submitted → Preparing → Ready → Completed, with rejection from Submitted
   only and a required rejection reason; their type and
   table association distinguish them. This requires no Cashier/POS screen in this feature.
4. **Given** a pickup order, **When** the customer completes submission,
   **Then** no delivery address or payment processing is required by this feature.
5. **Given** a pickup submission with missing name/phone or changed product prices,
   **When** it is reviewed, **Then** the same required-detail validation and explicit refreshed-price
   confirmation rules in Story 2 scenarios 6–8 apply, without requiring any table context.

---

### User Story 4 - View the Intended Order's Status (Priority: P2)

A customer reopens an order's confirmation/tracking view and sees its saved details and
current fulfillment status, without gaining access to other customers' orders or staff actions.

**Why this priority**: Customers need confirmation and visibility after submission; staff
status management belongs to the future Cashier/POS feature.

**Independent Test**: With a private tracking link and prepared orders in Submitted, Preparing,
Ready, Completed and Rejected, open and reload tracking. No staff status-update interface is needed.

**Acceptance Scenarios**:

1. **Given** an order's private tracking link, **When** the customer opens tracking,
   **Then** its reference, order type, applicable table, items, quantities, saved total and
   current fulfillment status are visible without registering or signing in.
2. **Given** an order's saved fulfillment status has changed through an authorized workflow,
   **When** its tracking view is reloaded, **Then** the current status is shown. For this feature's
   manual review, prepared status examples may be used; no new staff mutation screen is required.
3. **Given** no authorization for another customer's order, **When** the visitor changes an
   order reference or uses only a shared table QR link, **Then** that order's details are not
   exposed and no order or staff action is permitted.
4. **Given** an unavailable tracking service or invalid access context, **When** tracking is
   opened, **Then** a safe error is shown without revealing private order data or claiming
   an order was lost, cancelled, or paid.
5. **Given** an order rejected from Submitted with a recorded rejection reason, **When** its
   private tracking link is opened, **Then** status Rejected and that reason are displayed.
6. **Given** either order type, **When** its fulfillment progression is reviewed, **Then** the
   only allowed transitions are Submitted → Preparing, Preparing → Ready, Ready → Completed,
   and Submitted → Rejected with a nonblank reason. Rejection without a reason, rejection
   after Submitted, skipped/backward steps and transitions out of Completed or Rejected
   are invalid. Review uses prepared examples; implementing staff transition operations
   remains outside this feature.
7. **Given** someone knows only an order reference, customer name, phone number or table QR
   link, **When** they attempt tracking without its private access link, **Then** those details
   alone do not reveal the order, customer details, or private tracking link.

### Edge Cases

- Empty menus and categories retain clear empty states; an empty cart cannot be submitted.
- Unknown products, unavailable products, invalid quantities, and invalid dine-in table
  context are rejected before an order is accepted; no partial order or silent item removal.
- Any product price change between cart review and submission requires refreshed prices
  and totals and explicit customer confirmation, whether the price increased or decreased.
  No confirmation means no order; a further change requires fresh confirmation.
- A client-altered price or total cannot determine the saved amount. Saved prices remain
  unchanged when the menu is edited later.
- A dropped connection or uncertain save must show an uncertain outcome and a safe way to
  check the same submission, rather than false success or uncontrolled resubmission.
- Opening a different table link or changing order type must not silently submit the prior
  table/type context; the final review must show the actual context being submitted. Cart
  retention across those navigations is not a confirmed requirement.
- Sharing a table QR code establishes table context only, not identity, physical presence,
  permission to list all orders for that table, or staff privileges.
- Missing or whitespace-only customer name/phone blocks both order types with field-specific
  feedback. No country-specific phone format, phone ownership verification, or account
  registration policy is added by these clarifications.
- Private tracking links grant access to the corresponding order only; guessing or changing
  a public order reference must not expose private details. Possession of a shared private
  tracking link grants its holder access; the table QR link is not such a credential.
- Rejected orders show their required reason. Customer cancellation, reopening terminal orders,
  and staff status-update interfaces remain outside this feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Customers MUST browse the same managed categories and available products
  anonymously, retaining EGP/two-decimal display and existing table-menu behavior.
  Acceptance: Story 1 scenarios 1 and 4; existing menu preservation in SC-007.
- **FR-002**: Customers MUST add products, edit positive whole-number quantities, remove
  selections, and review line amounts and a cart total. Empty carts and invalid quantities
  MUST NOT be accepted as orders. Acceptance: Story 1 scenarios 2–3.
- **FR-003**: Dine-in orders MUST resolve a valid existing table QR context, display the
  table before submission, and retain the correct table association. Invalid context MUST
  fail without substituting a table or order type. Acceptance: Story 2 scenarios 1–3.
- **FR-004**: Online pickup MUST be directly accessible without a QR code or table, clearly
  identified before submission, and saved without a table association.
  Acceptance: Story 3 scenarios 1–2 and the context-change edge case.
- **FR-005**: Acceptance MUST revalidate products, availability, quantities and applicable
  table context, calculate authoritative amounts, and save the whole order consistently
  or reject it without partial data. Acceptance: Story 2 scenario 4 and invalid-input edges.
- **FR-006**: Prices and totals MUST use exact EGP amounts to two decimal places; order
  lines MUST retain their accepted unit prices so subsequent menu edits cannot change the
  saved total. If prices changed before submission, updated prices and totals MUST be shown
  and explicitly confirmed before acceptance; further changes require fresh confirmation.
  Acceptance: Story 2 scenarios 7–8, Story 3 scenario 5, SC-004 and price/tampering edge cases.
- **FR-007**: A successful submission MUST return a durable order reference and confirmation
  of saved contents, type, applicable table, amounts and current status. Repeated handling
  of the same submission MUST NOT create duplicate orders; an uncertain outcome MUST have
  a check/recovery path. Confirmation MUST provide a private order-tracking link.
  Acceptance: Story 2 scenarios 1 and 5; Story 3 scenario 2.
- **FR-008**: Dine-in and pickup MUST form one shared order workflow suitable for future
  Cashier/POS use, distinguishable by order type and table association, with the same status
  vocabulary: initial Submitted, followed only by Preparing → Ready → Completed, or Rejected
  directly from Submitted with a required nonblank rejection reason. Completed and Rejected
  have no outgoing transitions in this lifecycle. This feature MUST NOT add staff
  order-management operations. Acceptance: Story 3 scenario 3; Story 4 scenarios 5–6.
- **FR-009**: Customers MUST view their intended order's current status and saved details,
  including after reload; they MUST NOT view others' private orders or mutate fulfillment
  status. Access MUST use the private tracking link, not an order reference, contact detail or
  table QR link alone. Rejected status MUST show its recorded reason.
  Acceptance: Story 4 scenarios 1–5 and 7.
- **FR-010**: Table links and customer submissions MUST NOT grant Admin/Cashier privileges.
  Existing authentication and management permissions MUST remain intact. Acceptance:
  Story 4 scenario 3 and SC-007.
- **FR-011**: Cart, submission, and tracking MUST support mobile, tablet, and desktop use
  with readable amounts, usable controls, and clear validation, failure and uncertain-outcome
  feedback. Acceptance: SC-006 and failure edge cases.
- **FR-012**: This feature MUST preserve existing menu/table records, issued QR identifiers,
  Identity accounts, and existing functionality. It MUST NOT implement payment processing,
  cashier payment recording, POS, staff walk-in ordering, reports, or delivery. Acceptance:
  SC-007 and scope review in SC-008.
- **FR-013**: Both order types MUST support guest checkout without registration or sign-in,
  require a nonblank customer name and phone number, and retain them with the order.
  Missing details MUST prevent acceptance with clear feedback. These details and private
  tracking links MUST NOT be exposed by public menus or general order lookup.
  Acceptance: Story 2 scenario 6; Story 3 scenarios 2 and 5; Story 4 scenarios 3 and 7.

### Key Entities *(include if feature involves data)*

- **Cart / Cart Selection**: A customer's unsubmitted product choices and positive whole-number
  quantities, displayed prices and amounts, and currently reviewed order type/table context.
  A cart is not an accepted order or a payment record.
- **Order**: A saved customer submission with a reference, dine-in/pickup type, table association
  for dine-in only, saved lines/total/currency, current fulfillment status, and protected
  customer name and phone number, private tracking access, and a rejection reason when
  Rejected. Fulfillment states are Submitted, Preparing, Ready, Completed and Rejected.
- **Order Line**: A distinct selected product, quantity, accepted unit price and line amount;
  duplicate product names do not merge distinct products.
- **Existing Product / Category / Cafe Table**: The managed menu and seating records already
  in use; product availability and table identity remain authoritative.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In a manual sample of two available products, all add/change/remove actions
  show the expected quantities and amounts; zero unavailable products appear for selection,
  and all empty/zero/negative/fractional-quantity submissions are rejected.
- **SC-002**: Two dine-in submissions for two distinct existing table links and two pickup
  submissions without QR links produce exactly four corresponding orders with 100% correct
  type/table associations. Reopening each confirmation retains its contents and total.
- **SC-003**: Repeating one submission and recovering one interrupted submission produce
  at most one saved order per intended submission; no invalid-table or unavailable-product
  attempt produces a partial order or success message.
- **SC-004**: Altered customer-supplied prices cannot change authoritative totals; editing
  a menu price after acceptance changes zero saved order totals. All order money displays
  use EGP/two decimal places. In both order types, an increase and a decrease before submission
  each show updated amounts and create zero orders until explicitly confirmed; a further price
  change invalidates the previous confirmation. Accepted totals match the confirmed amounts.
- **SC-005**: For Submitted, Preparing, Ready, Completed and Rejected, private tracking shows its current value on
  reload. Attempts with another order's reference or a table link alone expose zero private
  order details and allow zero customer status changes. Rejected examples show a nonblank
  reason; lifecycle review accepts all four permitted transitions and rejects every other
  transition, including rejection without a reason. Missing-link attempts with name or phone
  alone also reveal zero private order details.
- **SC-006**: At 320px, 768px and 1280px review widths, customers can browse, edit a cart,
  review/submit each order type and read tracking without horizontal page scrolling; every
  invalid-input and unavailable-service example shows readable, actionable feedback.
- **SC-007**: Before/after review finds zero unintended changes to existing menu/table
  data, QR identifiers and staff accounts; existing Admin access and anonymous table-menu
  journeys still work, and customer requests permit zero staff operations.
- **SC-008**: Scope review finds zero payment-processing, delivery, POS, reporting or staff
  order-management journeys added by this feature. Both order types can be submitted without
  registration; missing/blank name or phone examples create zero orders, and successful
  confirmations provide a private tracking link that reopens the corresponding order.

## Assumptions

- Authoritative sources are requirements.md, Constitution v1.0.1, the approved Feature 001
  business rules, this feature request, and the approved clarifications recorded above.
  Fulfillment follows the approved lifecycle; no new payment policy is approved.
- Existing anonymous menu browsing is implemented for table links; direct no-table pickup
  browsing is an extension in this feature, reusing the same menu rather than a separate catalog.
  Existing records cover menu and tables; no cart or order workflow is currently implemented.
- Feature 001 T051 (physical-phone QR verification) and T056 (final acceptance) remain pending.
  Drafting this specification neither completes them nor claims Feature 001 fully accepted.
  Accessible phone hosting remains a validation dependency, not authorization for network changes.
- Session-local cart interaction is the minimum review scope. Cross-device cart synchronization,
  long-term cart recovery, and a guaranteed retention duration are not confirmed requirements.
- Quantity positivity/integrality, authoritative totals, all-or-nothing acceptance, private
  order access and duplicate-submission protection are minimal validation/consistency controls.
  No stock counts, arbitrary quantity limits, minimum spend, pickup slots, reservation/occupancy
  checks, delivery, discounts, customizations, notifications or customer cancellation/editing
  after acceptance are introduced as requirements.
- The future Cashier/POS feature owns staff order viewing and status changes. This feature
  provides shared orders and customer status viewing, not a staff interface or public status setter.
  Reload-based tracking is sufficient; live updates are not required. Defining the approved
  lifecycle and displaying prepared status examples does not authorize staff mutation screens.
- Online and counter payments remain confirmed overall scope for later work. This feature
  neither processes/records payments nor defines payment selection, paid states, fees, tax rules,
  provider behavior or a payment prerequisite for fulfillment.
- Sample counts and viewport widths are manual acceptance conditions, not traffic commitments.
  No automated tests, technical architecture, implementation plan or tasks are prescribed.

- All three previously unresolved business decisions are resolved by the 2026-10-27
  clarification session. Private-link protection and submission/recovery mechanisms are
  planning details; no new contact-format, retention, payment or fulfillment policy is assumed.
