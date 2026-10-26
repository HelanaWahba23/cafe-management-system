# Feature Specification: Menu and Table Management

**Feature Branch**: None created; feature directory is independent of the Git branch.

**Created**: 2026-10-25

**Status**: Clarified — ready for planning review

**Input**: User description: "Menu and Table Management: admin manages menu categories,
products, prices, availability, and cafe tables; each table has a unique QR code opening
the customer-facing menu associated with that table; customers browse on mobile without signing in."

## Clarifications

### Session 2026-10-25

- Q: Does management include removing categories, products, or tables? → A: Support creating
  and editing only. Deletion and deactivation are outside this feature; product availability
  remains a supported edit and is not record deactivation.
- Q: How are unavailable products presented to customers? → A: Hide them from the
  customer-facing menu; admins can still view and edit them.
- Q: Which currency, precision, price, and identifier rules apply? → A: EGP with two decimal
  places; product prices greater than zero; category names, product names, and table labels
  must not be blank; duplicate category/product names allowed; table labels unique;
  each table has its own unique QR code.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Maintain the Menu (Priority: P1)

An authorized admin creates and edits categories and products, sets product prices,
and changes product availability so customers can browse the current cafe offering.

**Why this priority**: An accurate menu is the prerequisite for useful customer browsing.

**Independent Test**: With authorized admin access, create a category and product, change
the product's price and availability, then reopen the records to verify the saved values.
No ordering or payment functionality is needed.

**Acceptance Scenarios**:

1. **Given** an authorized admin, **When** they create a named category and a product
   associated with it, **Then** both appear in the management view with the saved association.
2. **Given** an existing category and product, **When** the admin edits their names or
   the product price, **Then** reopening the records shows the saved changes.
3. **Given** a product, **When** the admin changes its availability and reopens it,
   **Then** the selected availability is retained; unavailable products remain viewable and
   editable by admins but are hidden from customers on the next menu open or reload.
4. **Given** a blank category/product name or a zero, negative, or more-than-two-decimal-place
   price, **When** the admin saves it,
   **Then** the system explains the invalid input and does not save a partial change.
5. **Given** a visitor without admin authorization, **When** they attempt any management
   change, **Then** the change is denied and existing records remain unchanged.
6. **Given** an existing category or product name, **When** the admin creates another
   category or product with the same name, **Then** the duplicate name is accepted.
7. **Given** a valid positive price such as EGP 25.50, **When** the admin saves and reopens
   the product, **Then** its price is retained and displayed in EGP with two decimal places.

---

### User Story 2 - Manage Tables and Their QR Codes (Priority: P1)

An authorized admin creates and edits cafe table records and obtains a unique QR code
for each table so customers can open a menu with the correct table context.

**Why this priority**: Table-specific access is a core confirmed entry point for dine-in customers.

**Independent Test**: Create two distinguishable table records and obtain each QR code.
Using an existing sample menu, scan both codes and verify their different table contexts.

**Acceptance Scenarios**:

1. **Given** an authorized admin, **When** they create and reopen a table record,
   **Then** its identifying label is retained and it is distinguishable from other tables.
2. **Given** an existing table, **When** the admin edits its displayed label,
   **Then** reopening the table shows the updated label.
3. **Given** two table records, **When** the admin generates and views their QR codes,
   **Then** each code identifies only its associated table and never the other table.
4. **Given** a table QR code, **When** a signed-out customer scans it,
   **Then** the cafe menu opens with the associated table visibly identified.
5. **Given** a visitor without admin authorization, **When** they attempt to change tables
   or generate management QR codes, **Then** the action is denied without changing records.
6. **Given** a blank table label or a label already assigned to another table, **When** an
   admin attempts to create or edit a table with that label, **Then** the change is rejected
   with a clear explanation and existing records remain unchanged.

---

### User Story 3 - Browse the Menu on a Phone (Priority: P1)

A customer scans a table QR code and browses categories, product names, and prices
on a phone without creating an account or signing in.

**Why this priority**: This completes the customer-facing value of the managed menu and table QR codes.

**Independent Test**: With sample categories, products, and tables, scan a table QR code
in a signed-out mobile browser and browse the menu without any order or payment flow.

**Acceptance Scenarios**:

1. **Given** a valid table QR code and populated menu, **When** a signed-out customer opens
   it on a phone, **Then** they can browse categories and the associated products and prices
   without encountering a mandatory sign-in or account-creation step.
2. **Given** a saved name or price update, **When** the customer next opens or reloads
   the menu, **Then** the updated value is displayed; live updates without reload are not required.
3. **Given** an unavailable product, **When** a customer loads the menu,
   **Then** that product is absent; after an admin makes it available, it appears on the
   next open or reload with its price displayed in EGP with two decimal places.
4. **Given** a menu viewed at the review widths in SC-004, **When** the customer browses,
   **Then** names and prices are readable and category controls usable without horizontal scrolling.
5. **Given** an invalid table link, **When** the customer opens it,
   **Then** they see that the table cannot be identified and are not assigned to another table.

### Edge Cases

- An empty menu or a category with no products must show a clear empty state rather than
  a failure or fabricated products.
- If a menu cannot be loaded or a management change cannot be saved, the user must receive
  a clear failure message; the interface must not claim a successful save.
- An invalid or unknown table reference must not silently become another table's context.
- A table label edit must not cause its existing QR code to open a different table.
- Deletion and deactivation of categories, products, and tables are outside this feature;
  removing records or invalidating issued QR codes is not part of its management journeys.
- If all products are unavailable, the customer menu shows a clear empty state; admins
  can still view and edit the products and restore their availability.
- Blank names/labels, duplicate table labels, prices at or below zero, and prices exceeding
  two decimal places must be rejected without partial changes. Category/product name
  duplicates must be accepted. Renaming a table must retain a unique label.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Only authorized admins MUST be permitted to change menu categories, products,
  prices, availability, tables, and table QR-code management data. A QR code MUST NOT grant
  management privileges. Acceptance: Story 1 scenario 5 and Story 2 scenario 5.
- **FR-002**: Admins MUST be able to create, view, and edit menu category names and associate
  products with categories. Acceptance: Story 1 scenarios 1–2.
- **FR-003**: Admins MUST be able to create, view, and edit products including their names,
  category association, prices, and availability. Acceptance: Story 1 scenarios 1–3.
- **FR-004**: Successful management changes MUST remain available when the affected record
  is reopened. Failed changes MUST NOT be presented as saved. Acceptance: Story 1 scenarios
  2–4, Story 2 scenarios 1–2, and the save-failure edge case.
- **FR-005**: Admins MUST be able to create, view, and edit distinguishable cafe table
  records and generate and view each table's unique QR code. Acceptance: Story 2 scenarios 1–3.
- **FR-006**: Each table QR code MUST open the customer menu with the correct table
  visibly identified and MUST NOT identify a different table, including after a label edit.
  Acceptance: Story 2 scenarios 3–4 and the table-label edge case.
- **FR-007**: Customers MUST be able to browse categories, products, and prices on mobile
  without signing in. Saved menu changes MUST be reflected on the next open or reload.
  Acceptance: Story 3 scenarios 1–2 and 4.
- **FR-008**: Empty menus, load/save failures, and invalid table links MUST have clear
  user-facing outcomes without invented menu data, false success, or incorrect table context.
  Acceptance: Story 3 scenario 5 and the empty-menu and failure edge cases.
- **FR-009**: Category, product, and table management MUST be limited to creating, viewing,
  and editing records, plus table QR-code generation. Deletion and record deactivation
  MUST remain outside this feature. Product availability editing remains included.
  Acceptance: management journeys provide no deletion or record-deactivation actions.
- **FR-010**: The customer-facing menu MUST exclude unavailable products. Admins MUST
  still be able to view and edit them. Availability changes MUST appear on the next
  customer menu open or reload. Acceptance: Story 1 scenario 3, Story 3 scenario 3,
  and the all-products-unavailable edge case.
- **FR-011**: Prices MUST be in EGP, use two decimal places, and be greater than zero.
  Prices with more than two decimal places MUST be rejected rather than silently rounded.
  Category names, product names, and table labels MUST not be blank. Duplicate category
  and product names MUST be allowed; table labels MUST be unique. Each table MUST have its
  own unique QR code. Acceptance: Story 1 scenarios 4, 6–7, Story 2 scenarios 3 and 6,
  and Story 3 scenario 3.

### Key Entities *(include if feature involves data)*

- **Menu Category**: A grouping of menu products with a nonblank name; duplicate names allowed.
- **Product**: A menu item with a nonblank name (duplicates allowed), category association,
  positive EGP price with two decimal places, and availability. Unavailable products are
  hidden from customers but remain viewable and editable by admins.
- **Cafe Table**: A distinguishable physical table represented by a nonblank, unique label
  and its association with a unique QR code.
- **Table QR Code**: A scannable entry point to the customer menu for one table; it does
  not authenticate a customer or prove their physical presence.
- **Admin**: A person authorized to maintain menu and table information; customer browsing
  does not require this authorization.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: In a manual review, one admin can create two categories and two products,
  edit a category and product name, change a product's price and availability, and reopen
  all affected records with 100% of successful saved changes retained.
- **SC-002**: For two distinct tables, 100% of their QR scans open the correct table's
  menu; neither code assigns the other table, including after one table's label is edited.
- **SC-003**: A signed-out customer completes the scan-and-browse journey for both tables
  with zero mandatory sign-in or account-creation steps and can identify product prices
  and the current table without staff assistance.
- **SC-004**: At review widths of 360, 390, and 1280 pixels, the customer can browse the
  sample menu with readable names and prices, usable category controls, and zero horizontal
  page scrolling. These are review conditions, not a claim of support for every device.
- **SC-005**: Every attempted management change in the signed-out/non-admin review is
  denied and leaves menu and table data unchanged.
- **SC-006**: The empty-menu, failed-save/load, and invalid-table review cases all produce
  the outcomes in FR-008 with zero false save confirmations or incorrect table assignments.
- **SC-007**: Manual review confirms zero deletion or record-deactivation actions in the
  feature's management journeys, zero unavailable products in the customer menu, and admin
  access to every unavailable sample product. Making a product available restores it to
  the customer menu on the next load.
- **SC-008**: Manual review accepts duplicate category/product names and positive prices
  such as EGP 25.50, and rejects blank names/labels, a duplicate table label, prices of zero
  or less, and a price with more than two decimal places. Every rejection leaves existing
  records unchanged; all displayed product prices use EGP and two decimal places.

## Assumptions

- Authoritative sources are `requirements.md`, Constitution v1.0.1, and the approved feature
  request and clarification answers. Constitution v1.0.1 reconciles the confirmed scope
  inventory without changing the v1.0.0 principles or governance rules. This source-reference
  correction does not change the approved feature's business behavior or acceptance criteria.
- Categories, products, and tables describe one cafe's menu and seating; separate menus per
  table are not required by a menu "associated with" a table. Table context is the confirmed
  distinction. This interpretation is recorded for review, not as a new product requirement.
- Minimal records use category/product names and a table label. Images, descriptions,
  modifiers, translations, sorting controls, QR styling, downloads, printing, and bulk
  operations are not added as requirements.
- Admin authorization is required but does not currently exist in the starter application.
  The feature depends on authorized admin access; account enrollment, credential recovery,
  and the method of authorization remain outside this business specification.
- QR browsing assumes a customer's phone can scan a code and access the cafe website.
  QR possession does not establish identity or occupancy; reservations and occupancy
  management are not required.
- The mobile widths in SC-004 and the small sample dataset are proposed manual acceptance
  conditions, not production traffic or performance commitments. No automated tests are
  mandated by this specification.
- Customer order placement, pickup ordering, cashier order workflows, bills, order tracking,
  payments, and sales statistics remain confirmed overall requirements for later features;
  they are outside this first feature. Delivery remains out of scope for the whole project.
- Preserve the existing application and configuration. This specification authorizes no
  replacement, implementation, dependency choice, plan, or task generation. Keep the feature
  bounded to these management and browsing journeys within the overall three-day project.
