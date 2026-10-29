# Feature Specification: Payments & Billing

**Feature Branch**: `master` | **Created**:2026-10-29 | **Status**:Clarified
**Input**: Approved Feature004, Paymob Egypt sandbox EGP Hosted Unified Checkout, cashier-recorded cash, itemized billing and verified payment status.

## Clarifications

### Session2026-10-29

- Provider/mode: Paymob Egypt, test/sandbox only, EGP, Hosted Unified Checkout; never live or simulated successful payments. Secrets stay in local User Secrets. Missing credentials leave real provider validation pending while independent cash/reporting work continues.
- Safety: pending/unknown online attempts block rejection and counter cash recording until reconciliation; paid orders cannot be rejected. Refunds are excluded.

## User Scenarios & Testing *(mandatory)*

### User Story1 - Record counter cash (Priority:P1)

A current cashier records receipt of the full saved order amount once and sees the receipt/bill.

**Why this priority**: Delivers the approved counter-payment path independently of external credentials.
**Independent Test**: Record cash for an unpaid order, repeat, then attempt unauthorized/rejected/online-pending cases.
**Acceptance Scenarios**:

1. **Given** an unpaid non-rejected order with no active/unknown online attempt, **When** cashier explicitly confirms full cash receipt, **Then** exact saved EGP total, recorder and UTC time persist once.
2. **Given** an already paid order, **When** repeated or concurrent recording occurs, **Then** no second payment is created or total changed.
3. **Given** unauthorized access, rejected order or pending/uncertain online attempt, **When** recording is requested, **Then** it fails without settlement.

### User Story2 - Pay online in sandbox (Priority:P2)

A customer uses their private order context to open Paymob-hosted sandbox checkout and refresh verified status afterwards.

**Why this priority**: Adds the approved online path without handling card credentials.
**Independent Test**: With actual test credentials/reachable HTTPS callback, complete hosted sandbox success/failure and reconcile interrupted results.
**Acceptance Scenarios**:

1. **Given** a private unpaid non-rejected customer order and configured sandbox, **When** payment starts, **Then** an authoritative exact-amount EGP attempt is saved before external initiation and only test hosted checkout is offered.
2. **Given** missing credentials or invalid test configuration, **When** online payment is requested, **Then** clear unavailable feedback appears and no paid state is simulated.
3. **Given** authenticated provider evidence, **When** signature/order/amount/currency/integration/result checks pass, **Then** one sandbox payment is recorded; redirects, forged/mismatched/pending/failed/live/authorization-only results cannot settle it.
4. **Given** duplicate/reordered notifications or interrupted initiation, **When** processing/reconciliation is repeated, **Then** no duplicate settlement or blind new external attempt occurs; unknown outcomes remain blocked until verified.
5. **Given** counter orders or another private context, **When** anonymous payment access is requested, **Then** no unrelated order/payment information is disclosed.

### User Story3 - View bill and payment status (Priority:P3)

Customers privately and cashiers securely view itemized saved bills with payment state distinct from fulfillment.

**Why this priority**: Makes receipt status truthful and understandable.
**Independent Test**: Compare unpaid, cash paid, pending and sandbox-paid bills and change catalog prices.
**Acceptance Scenarios**:

1. **Given** a saved order, **When** its bill/status is viewed, **Then** saved itemization/EGP total remain unchanged and Unpaid/Pending/Uncertain/Paid cash/Paid sandbox are distinguished.
2. **Given** sandbox settlement, **When** displayed, **Then** it is explicitly labelled test payment, never real revenue or a live receipt.
3. **Given** paid or pending/uncertain payment, **When** rejection is attempted, **Then** it fails while other approved fulfillment edges remain independent.

### Edge Cases

- Amount overflow, duplicate transaction IDs, concurrent cash/online/status changes, callback before initiation persistence completes, unknown provider order and malformed/null signature fields fail closed without data loss.
- Lost replies retain a stable attempt for recovery. Failed initiation is not retried blindly.
- No provider keys, raw card data or intention secrets appear in logs, public status responses or Git.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Only current authorized Cashier identities may record cash or read staff payment details; private customer access remains capability scoped to the intended customer-origin order.
- **FR-002**: Full-order cash recording MUST retain exact saved total, EGP, recorder and time atomically, reject rejected/paid/pending/uncertain conflicts and prevent duplicate effects.
- **FR-003**: Online initiation MUST use Paymob Egypt sandbox Hosted Unified Checkout, authoritative integer piasters, stable saved attempt and configured test integration. No live processing or card storage.
- **FR-004**: Backend MUST verify provider evidence and match intended order, exact amount, EGP, configured test integration and actual successful nonpending captured/standalone result; browser redirects never settle.
- **FR-005**: Verify webhook HMAC in constant time before mutations; reconciliation uses authenticated provider inquiry and shares the same settlement checks.
- **FR-006**: Duplicate notifications, concurrent recording and uncertain external calls MUST remain recoverable without duplicate payments or blind retries.
- **FR-007**: Pending/unknown attempts block cash and rejection; paid orders cannot be rejected. Fulfillment/payment states otherwise remain separate.
- **FR-008**: Saved itemized bill/invoice views MUST truthfully show unpaid/pending/uncertain/paid method/time and sandbox labels, preserving historical amounts and privacy.
- **FR-009**: Existing data/accounts/QRs and Features001–003 MUST be preserved; responsive keyboard-accessible payment/error/busy UI uses the established visual stack.
- **FR-010**: Missing provider credentials/reachable HTTPS callback or unperformed sandbox confirmation MUST remain explicit pending acceptance. No fake success, refunds, partial/split payments, taxes or delivery are introduced.

### Key Entities

- **Payment attempt**: Stable reference, intended order/amount/currency, test integration, external identifiers, recoverable state and protected hosted client secret.
- **Payment settlement**: One full saved-order receipt, cash recorder or verified provider transaction, method/time and explicit sandbox classification.
- **Bill**: Historical order itemization joined with truthful payment projection, not recalculated catalog prices.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Counter cash acceptance and repeated/concurrent recording produce exactly one receipt with the exact saved amount and actor/time.
- **SC-002**: All invalid signature/order/amount/currency/integration/result and redirect-only scenarios produce zero paid effects.
- **SC-003**: Actual Paymob sandbox success/failure/reconciliation and duplicate delivery are demonstrated before claiming provider integration verified.
- **SC-004**: All forbidden access and payment/rejection conflicts fail; historical itemization remains unchanged.
- **SC-005**: Payment/bill screens have usable keyboard controls and no page overflow at320/768/desktop widths; builds/preservation/security checks are recorded.

## Assumptions

- Counter recording represents the cashier's explicit confirmation of the full saved amount, no tender/change calculator or partial payments.
- Invoice means a reference-labelled itemized order bill/receipt, not a jurisdiction-certified fiscal invoice; no invented tax rules.
- Provider test credentials and an approved externally reachable trusted HTTPS callback origin are operator setup dependencies. Do not create accounts/tunnels, weaken certificates or change firewall rules.
- Sandbox payments are excluded from actual cash revenue reporting and identified separately in Feature005.
