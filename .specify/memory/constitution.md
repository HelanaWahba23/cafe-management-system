# Cafe Management Constitution

## Core Principles

### I. Maintainable Architecture

Development MUST start from the existing ASP.NET Core .NET 8 controller-based API.
Code MUST use clear names and separate HTTP handling, business rules, and persistence
responsibilities where those responsibilities exist. Simple classes and folders within the
existing project MUST be preferred unless a concrete approved requirement justifies more.
Additional projects, frameworks, infrastructure, or abstraction layers MUST have a documented
need. Microservices, messaging systems, and generic frameworks MUST NOT be introduced merely
for anticipated growth. The architecture must remain understandable within a three-day project.

### II. Preserve Existing Code

Existing source, configuration, dependencies, and observable behavior MUST be preserved unless
a change is necessary for an approved requirement or a demonstrated defect. Every deletion,
replacement, dependency change, or refactor MUST have an explicit reason tied to that work.
Unrelated cleanup and wholesale scaffold replacement MUST NOT accompany feature changes.
The Git baseline MUST remain available for comparison and recovery.

### III. Backend Authorization and Payment Verification

Staff-only operations MUST enforce backend authorization; hiding controls in a user interface
is insufficient. Customer requests MUST NOT gain staff privileges through table QR codes or
client-provided roles. Customer order access MUST be restricted to the intended order context.
The backend MUST validate inputs and calculate authoritative order prices and totals.

Online payment success MUST be established through authenticated provider evidence verified
by the backend, never through a browser redirect or client-provided success flag alone.
Verification MUST match the intended order, amount, currency, and payment result. Duplicate
provider notifications MUST NOT produce duplicate payment effects. Provider secrets MUST
remain outside committed source and public responses; raw card credentials MUST NOT be stored.
Any later approved cashier-recording operation MUST require an authorized staff identity and
retain who recorded the payment and when. These rules do not select a provider or authorize
implementing an unconfirmed payment-recording workflow.

### IV. Order and Payment Consistency

When persistence is introduced, orders and payment records MUST use database constraints and
transactions for related local changes that must succeed or fail together. Monetary values
MUST use a representation suitable for exact currency calculations. Order lines MUST retain
the price used at purchase so later price changes cannot alter historical totals.
Payment state and order fulfillment state MUST be distinct and changed only through valid
backend operations. A pending or failed payment MUST NOT be treated as settled. External
provider calls MUST NOT be assumed to participate in a local database transaction; interrupted
and repeated callbacks MUST leave a recoverable, consistent payment state. Database and ORM
selection remain future design decisions.

### V. Mobile-Friendly Customer Interfaces

Customer ordering interfaces MUST support narrow mobile screens, readable content, usable
touch controls, and clear validation and payment feedback. Dine-in ordering MUST obtain table
context through a unique QR code for each table. Online pickup MUST be accessible directly
through the cafe website without a QR code. Both flows MUST remain usable at desktop widths
without introducing a separate mobile application as an assumed requirement.

### VI. Small Changes and Proportionate Validation

Changes MUST be small enough to review independently and confined to the approved purpose.
Application changes MUST be validated with the solution build and focused manual checks for
the affected behavior. Documentation-only changes MUST receive content and scope review;
they do not require a build. Validation results MUST identify what was checked, any failures,
and any checks that could not be performed. A failed or unperformed check MUST NOT be reported
as successful.

New automated tests MUST NOT be added by default. They may be added only when the user
explicitly requests them or when necessary to diagnose a specific issue; the reason MUST be
stated. This exception does not remove build validation or focused manual verification.

### VII. Confirmed Scope and Portfolio Simplicity

Requirements MUST come from confirmed user instructions and the requirements document.
Missing behavior MUST be reported for confirmation rather than silently added. Delivery is
out of scope. Implementation MUST remain realistic for a three-day portfolio project;
unnecessary features, speculative flexibility, and architectural overengineering MUST be
excluded. A necessary security or consistency control MUST be kept even when scope is small;
reduce feature scope rather than bypassing the control.

## Scope and Existing Baseline

The existing application contains one .NET 8 Web API project, a sample WeatherForecast
controller and model, Swagger/OpenAPI through Swashbuckle, logging configuration, and local
launch profiles. It does not yet contain cafe workflows, a customer interface, database
persistence, authentication handlers, or payment integration. This constitution governs
future work; it does not claim these capabilities are implemented.

The confirmed product scope in `requirements.md` is:

- Dine-in ordering through a unique QR code for each table.
- Online pickup ordering directly through the cafe website without a QR code.
- Both order types appearing in the same Cashier/POS system.
- Cashiers viewing dine-in and online pickup orders, creating walk-in orders, updating order
  status, and viewing customer bills.
- Admin creation, editing, and management of menu categories, products, prices, and availability.
- Admin management of cafe tables and generation of a unique QR code for each table.
- Customers viewing their order status, with cashiers able to update it.
- Cash payment at the cashier/pickup counter; online payment is excluded from current delivery by the user's2026-10-30 scope revision.
- Cashiers recording payments made at the counter.
- Admin viewing basic order and sales statistics.
- No delivery and a simple, realistic three-day portfolio scope.

These requirements have been explicitly confirmed and recorded in `requirements.md`.
Their inclusion describes product scope, not implemented functionality or authorization to
implement every feature at once. Any unconfirmed scope MUST remain unresolved until explicitly
confirmed; its mention MUST NOT be interpreted as approval to add it to requirements or
implement it. Feature 001 is limited to its approved Menu and Table Management specification.

## Development Workflow

Before feature work, unresolved scope MUST be presented to the user and approved requirements
recorded explicitly. Specifications, plans, and tasks MUST be created only when authorized.
Each approved change MUST identify its requirement or defect, the existing behavior to
preserve, and the smallest adequate implementation. Reviews MUST check scope, code
preservation, authorization, payment handling, consistency, mobile usability where applicable,
and validation evidence. Checks MUST be proportionate to the actual change.

This constitution authorizes no feature implementation, dependency installation, database
selection, deployment, or further Spec Kit artifact generation by itself.

## Governance

This constitution governs subsequent project specifications, plans, tasks, and code reviews.
Explicit user instructions take precedence; any resulting conflict MUST be identified and
reconciled in the governance document rather than hidden.

Amendments MUST state the proposed rule change and rationale, receive user approval, and
update this document's version and amendment date. Use semantic versioning: a major increment
for incompatible removal or redefinition of a principle, a minor increment for a new or
materially expanded rule, and a patch increment for wording-only clarification. Review
affected downstream artifacts after an amendment, without changing them outside the
authorized scope. Every change review MUST identify any applicable constitutional deviation
and obtain explicit approval before proceeding with it.

## Approved amendment —2026-10-30

User explicitly approved removing Paymob/online payment using Spec Kit. Cash billing, online ordering and all existing records remain supported; provider source is archived. This narrows the confirmed product scope without weakening authorization, consistency, preservation or honest acceptance rules. Physical-device acceptance remains unperformed. Requirements and Feature004 artifacts must reflect this revision.

**Version**: 1.3.0 | **Ratified**: 2026-10-25 | **Last Amended**: 2026-10-30


## Approved acceptance amendment —2026-10-30

The user explicitly authorized automated mobile-browser emulation instead of physical Android acceptance for the portfolio. Product mobile behavior remains required, and actual generated QR images, signed-out contexts, correct table after rename, pickup and private tracking must be tested. Evidence must state the emulation scope; never claim hardware/camera/physical-network verification. Constitution VI honesty and preservation rules remain unchanged.

## Explicit scope extension —2026-10-30

User authorized Feature007 restaurant accounts/profile and cash-on-delivery in addition to pickup/dine-in. Earlier no-delivery/frontend-only scope is superseded for this feature. Preservation/security/verified payment/honest acceptance principles still apply; no online provider, courier/GPS or public hosting is required. Version1.3.0 scope amendment; prior accepted features remain historical baselines.
