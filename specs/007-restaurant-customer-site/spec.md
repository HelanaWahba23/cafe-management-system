# Restaurant Customer Website

**Approved**:2026-10-30, supersedes frontend-only/no-delivery scope. English Café branding; existing .NET/Blazor/SQL/Identity architecture.

## Stories and acceptance

US1(P1): Root is a coherent restaurant homepage with food imagery, Menu/About/Experience and top-left Login/Sign up. Categorized persisted restaurant menu supports actual cart quantities/totals. No fabricated reviews/history/location.
US2(P1): Customer signs up/logs in/out, edits saved name/phone/address, revisits securely. No self-assigned staff role; current identity/stamp is checked for protected operations.
US3(P1): Customer chooses delivery or pickup; delivery requires reviewed/confirmed address and cash-on-delivery, pickup no address. Existing QR resolves a valid table and the same site displays table instead of delivery address. Prices and duplicate handling stay authoritative.
US4(P1): Account history and private tracking persist exact bill/current progress. Staff receive all modes and advance Pending→Accepted→Preparing→Ready→Out for delivery(delivery only)→Delivered/Completed; rejection has reason. Existing cash/rejection guards remain.

## Requirements

FR-001 English restaurant UI/food imagery/working navigation at390/768/1280.
FR-002 Actual categorized restaurant catalog and cart; preserve existing catalog records when adding curated portfolio menu.
FR-003 Secure persisted customer accounts/profile/address; server validation, antiforgery and no staff privilege escalation.
FR-004 Owner-only history/details; no contact/reference-based access; guest private links preserved.
FR-005 Delivery address snapshot required/confirmed, cash-on-delivery only; pickup/dine-in do not retain delivery address.
FR-006 Unique existing table QR context validated, retained through restaurant navigation and bound to order.
FR-007 Persist accepted/preparing/ready/delivery/completed progress and expose cashier updates to intended customer.
FR-008 Preserve existing accounts/QRs/records/cash; reviewed additive migrations with verified backup; no live/provider payment or public hosting.

## Success criteria

SC-001 Two accounts register/login/profile persistence and cross-account/anonymous denial verified.
SC-002 Delivery/pickup/two-table QR checkout, totals/address/type association and duplicate safeguards verified.
SC-003 Cashier progress appears in customer tracking; non-delivery cannot enter delivery stage; cash receipt behavior retained.
SC-004 Debug/Release/publish and responsive/navigation tests pass; original-record preservation verified with intentional additive catalog changes documented.

Assumptions: no email sending/recovery provider is required or falsely claimed; account is local Identity password/cookie. Guest checkout remains. Delivery means address capture and staff-driven fulfillment, not courier/GPS integration. No invented delivery fees/taxes; saved item total is payable in cash.
