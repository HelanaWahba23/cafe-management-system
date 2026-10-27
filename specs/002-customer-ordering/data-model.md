# Data Model: Customer Ordering

**Status**: Proposed additive design; no entities, migrations or data changed by planning.

## Relationships

Existing CafeTable 1 → many Orders (optional table on Order); existing Product 1 → many
OrderLines; Order 1 → many OrderLines. Use restrictive deletes everywhere. Existing catalog,
table, Identity and migration schemas remain intact. Guest orders have no Identity-user FK.
Names/prices are snapshots, not joins used to recalculate history.

## Order — new Orders table

| Field | Storage / rule |
| --- | --- |
| Id | bigint identity primary key, internal; no customer authorization value |
| Reference | uniqueidentifier, generated server-side, required/unique, display-only |
| CapabilityHash | binary(32), required/unique SHA-256 digest; raw 32-byte token never stored |
| IntentHash | binary(32), required canonical submission-intent fingerprint |
| Type | nvarchar(16), DineIn or Pickup only |
| TableId | nullable int FK to CafeTables, delete restricted |
| TableLabel | nullable nvarchar(120), accepted table-label snapshot |
| CustomerName | nvarchar(120), required, trimmed/nonblank |
| CustomerPhone | nvarchar(40), required, trimmed/nonblank; preserve entered formatting |
| Currency | char(3), required, EGP only |
| Total | decimal(18,2), required, positive; sum of saved line amounts |
| Status | nvarchar(16), Submitted initially; Preparing, Ready, Completed, Rejected allowed |
| RejectionReason | nullable nvarchar(500); required/nonblank iff Rejected |
| CreatedAtUtc | datetime2, server-generated UTC instant |
| RowVersion | SQL rowversion, EF concurrency token for later legitimate writers |

Index unique Reference and CapabilityHash; index TableId FK and (Status, CreatedAtUtc) for
the known future shared workflow. No general customer/contact lookup index or public listing.

Checks: valid Type/Status/Currency; DineIn requires TableId and nonblank TableLabel; Pickup
requires both null; nonblank name/phone; Total > 0; Rejected requires nonblank reason, all
other statuses require null reason. Service uses Unicode-aware whitespace validation; SQL
trim/LEN checks are defense in depth, not a replacement for it.

## OrderLine — new OrderLines table

| Field | Storage / rule |
| --- | --- |
| Id | bigint identity primary key |
| OrderId | required bigint FK to Orders, delete restricted |
| ProductId | required int FK to Products, delete restricted |
| ProductName | required nonblank nvarchar(120), accepted snapshot |
| Quantity | int > 0, whole number |
| UnitPrice | decimal(18,2) > 0, accepted authoritative price |
| LineTotal | decimal(18,2) > 0; exact UnitPrice × Quantity |

Unique (OrderId, ProductId): one cart row per actual product identity. Adding the same product
updates that quantity; different products sharing a name remain distinct. Index ProductId FK.
Use a SQL arithmetic check with widened decimal intermediates for LineTotal = UnitPrice × Quantity;
application also validates representability before insert. No silent SQL numeric truncation.

Order.Total = sum(lines) and at least one line are cross-row invariants enforced by OrderService
inside the transaction, not falsely claimed as simple CHECK constraints. No payment totals/state.

## Validation and technical capacity

- Name/phone/reason bounds are technical storage capacities, not a phone-country rule, identity
  verification, commercial quantity cap or new cafe policy. Reject overlong input explicitly;
  never truncate. Preserve phone formatting; require only nonblank content, not SMS verification.
- Order/product IDs must resolve; quantities are positive whole integers representable as int.
  No smaller business maximum is invented. Duplicate ProductId rows in an HTTP request fail
  validation rather than ambiguously merging; UI maintains one row per product.
- Require decimal input in invariant EGP two-place form for reviewed prices. Compute amounts
  using checked decimal operations and reject amounts beyond decimal(18,2), including summed
  overflow; do not round. Customer totals, status, reasons, timestamps and internal IDs are not inputs.
- DineIn resolves TablePublicCode using existing uniqueness; request cannot assign an internal
  TableId. Pickup rejects any supplied table code. Resolve the label at acceptance, retain stable FK.
- Price/availability/table validation uses authoritative current rows; menu images/modifiers,
  stock counts, discounts, tax/fee components and delivery data are absent.

## Non-persisted structures

- CartState: circuit-scoped selected product IDs, quantities and last displayed unit prices,
  Type/TablePublicCode; no tracked EF entities or static/singleton shared state.
- CheckoutDraft: Type, TablePublicCode?, trimmed customer name/phone, selected IDs/quantities.
- OrderReview: authoritative available product names/prices, quantities, line totals, Total,
  currency, resolved table context. Used by an explicit confirmation action, not a saved quote.
- ReviewedPrices: one ProductId/UnitPrice entry per draft item; compare only, never authoritative.
- Browser session metadata: capability, whether submission started, and saved Reference when known.
  No name/phone or durable cart serialization. Copy link carries capability only in fragment.

## Lifecycle

| Current | Next allowed | Condition |
| --- | --- | --- |
| Submitted | Preparing | Future authorized staff workflow |
| Submitted | Rejected | Future authorized staff workflow and nonblank rejection reason |
| Preparing | Ready | Future authorized staff workflow |
| Ready | Completed | Future authorized staff workflow |
| Completed / Rejected | None | Terminal |

An internal OrderLifecycle helper validates all four transitions; invalid/skip/backward steps
fail. No feature-002 API/UI invokes status mutation. SQL allowlist/reason checks cannot enforce
transition history; future writers must use this helper and rowversion conflict detection.
Validate current displays and lifecycle rules using isolated prepared examples, not original records.

## Atomic placement and duplicate handling

1. Validate capability encoding/length and canonicalize the draft. Hash the immutable intent
   (versioned deterministic field ordering, length-delimited values, sorted product IDs/quantities,
   stable table code, trimmed name/phone). Exclude reviewed price vector and server-computed fields.
2. Look up capability hash before consulting current catalog. If accepted, same intent returns
   saved order; different intent returns submission_conflict. No mutation or price revalidation.
3. For a new capability, begin short RepeatableRead, resolve table/selected products in stable
   ID order, check availability and compare complete reviewed prices to current database prices.
4. Invalid/unavailable data returns a safe error and no writes. Price mismatch rolls back and
   returns a current review. UI must explicitly confirm it, then repeat validation on placement.
5. Compute exact snapshots, insert Submitted order and all lines in one transaction, commit.
   No locks survive waiting for a browser/user. No external calls inside the transaction.
6. SQL duplicate index violation: roll back/dispose context, look up capability in a fresh context;
   return matching winner or conflict. Unrelated unique failures remain safe errors, not success.
7. Deadlock/timeouts/cancellation/connection loss may be uncertain: no new token or automatic write
   retry. Read by the same capability. Not-found does not certify that another in-flight write
   cannot commit; explicit retry keeps the same intent/capability and remains deduplicated.

## Migration and future boundaries

Add one migration after 20261027013745_InitialMenuAndIdentity; update model snapshot only for
new entities/relationships. Review generated SQL for order-table creation/checks/indexes/FKs;
unexpected alterations to existing tables/data/Identity require stopping and investigating.
No EnsureDeleted/EnsureCreated, migration replacement, automatic startup migration, reseeding,
QR regeneration or destructive downgrade. Back up and compare original records before/after
later application to the confirmed target. Planning creates no migration.

POS later uses this single Orders set with authorized writers. Payments later add separate
records referencing Id and immutable amount/currency; no PaymentStatus, provider IDs, paid flags,
counter receipts or fake billing fields are introduced now. No extra event/history table is needed.
