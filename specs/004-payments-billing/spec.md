# Feature Specification: Cash Payments & Billing

**Feature Branch**: `master` | **Updated**: 2026-10-30 | **Status**: Scope revised with user approval

## Scope decision

The user approved removing Paymob after explaining that account/merchant setup is unavailable. Current delivery accepts cash at the cafe counter or pickup only. Online ordering remains supported; online payment is excluded. No real Android acceptance was waived by this instruction. Prior provider specification and all unfinished source are preserved in `archive/paymob-20261030/`.

## User Scenarios & Testing

### User Story 1 — Record counter cash (P1)

A current Cashier explicitly records the full saved EGP amount once, retaining actor and UTC time. Repeated/concurrent recording must not duplicate receipts. Unauthorized, rejected, stale or previously pending/uncertain payment contexts must be refused safely.

### User Story 2 — View historical bill (P2)

A customer uses their private order context to see saved items, amounts and payment status. Cashier sees the same saved bill with authorized payment controls. Customers are told to pay cash at the counter or on pickup. Catalog edits must not change old bills. Legacy sandbox/pending records, if present, remain truthful and protected; they do not enable online actions or count as cash revenue.

## Requirements

- **FR-001**: Cash recording/staff details require current Cashier authorization; customer payment access remains private and order-scoped.
- **FR-002**: One full cash receipt per order, saved total/EGP, actor/time, transaction/concurrency and duplicate protection.
- **FR-003**: Current application MUST expose no online checkout initiation or reconciliation action.
- **FR-004**: Browser redirects/client payment claims MUST never settle an order; no provider callback or return route is part of current delivery.
- **FR-005**: Remove provider runtime registrations/configuration dependencies; preserve unfinished provider source outside compilation and database schema/data intact.
- **FR-006**: Duplicate cash submissions remain idempotent; no online retry, test transaction or fictitious paid state.
- **FR-007**: Paid orders cannot be rejected; any preserved pending/uncertain attempts continue to block cash and rejection. No automatic force-unlock.
- **FR-008**: Private/staff bills retain historical EGP itemization and truthful method/state/time, with clear cash instructions and sandbox-history separation.
- **FR-009**: Preserve records/accounts/QRs, existing menu/order/POS/reporting, responsive keyboard-accessible controls and immutable migration history.
- **FR-010**: No provider account/keys/public callbacks are required. No refunds, partial/split payments, taxes or delivery added. Unperformed Android tests remain pending.

## Key Entities

Existing Orders, PaymentSettlements and PaymentAttempts remain unchanged. Attempts are retained only for historical safety/compatibility. There is no schema migration for this scope change.

## Success Criteria

- **SC-001**: Existing cash repeat/concurrency acceptance still produces one exact receipt and actor/time.
- **SC-002**: Removed provider routes are absent; no configuration can enable online checkout or change OnlineAvailable from false.
- **SC-003**: All provider work is recoverably archived; no provider acceptance is reported as passed. T009–T013 are explicitly removed from scope.
- **SC-004**: Existing access/payment guards and historical amounts remain unchanged.
- **SC-005**: Cash billing remains usable at320/768/desktop and with keyboard; Debug/Release/publish and focused removal checks pass.

## Assumptions

Cash receipt represents the cashier's explicit confirmation, not fiscal invoice certification. Existing history is preserved even when a feature is removed. Public HTTPS/real-phone journeys are separate acceptance tasks in Features001/002.
