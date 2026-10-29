# Research: Cashier & POS

## Origin and optional contacts

Decision: Customer/Counter origin, nullable creator FK restricted to existing Identity users, optional persisted contacts for counter rows only. Existing rows default Customer. Explicit IS NOT NULL checks for required customer fields avoid SQL CHECK UNKNOWN loopholes.
Rationale: preserves fulfillment enum and guest required-contact contract, without fabricated contacts.
Alternatives: new order table duplicates lifecycle; new fulfillment enum confuses origin with delivery mode; placeholder contacts violate user decision.

## Shared acceptance and recovery

Decision: reuse OrderService acceptance via internal counter-only methods, authorized by PosOrderService before entry. Domain-separate attempt hash with creator and random 32-byte token; retain immutable intent, repeatable-read catalog locks, exact reviewed price vector, precise unique-index recovery. Guest lookup filters Customer origin.
Rationale: avoids divergent totals and duplicate recovery rules.
Alternatives: copy acceptance logic risks drift; public optional-contact switch weakens guest security.

## Access and concurrency

Decision: factor role-specific StaffAccess checks while preserving Admin semantics; check current Cashier role and security stamp on every service operation. Status accepts an opaque 8-byte rowversion and exact allowed transition through OrderLifecycle, stale writes return conflict. API POSTs require antiforgery and JSON. Blazor calls same guarded service using trusted circuit principal.
Rationale: cookies/circuit role claims can become stale; hiding UI is insufficient.
Alternatives: policy-only access or blind status overwrite fails existing security requirements.

## Bills and interface

Decision: saved-order projections only, manual refresh and 50-row pages, optional status filter. Dedicated page-local InteractiveServer POS routes and layout; no global routing change. Existing Cashier login lands on /pos/orders; Admin default unchanged.
Rationale: minimal reusable portfolio design; no unpaid/paid inference until Feature 004.

Read-only research agents inspected current implementation under the plan skill's explicit research instruction. All technical unknowns resolved from local source. No dependencies changed.
