# Cafe Management Requirements

## Confirmed scope

- Support dine-in ordering using a unique QR code for each table.
- Support online pickup ordering directly through the cafe website, without requiring a QR code.
- Both dine-in and online pickup orders must appear in the same Cashier/POS system.
- Customers pay cash at the cashier/pickup counter. Online ordering remains supported; online payment is outside the current delivery scope.

### Cashier/POS

- Cashiers can view dine-in and online pickup orders, create walk-in orders, update order status, and view customer bills.

### Menu Management

- Admin can create, edit, and manage menu categories, products, prices, and product availability.

### Table Management

- Admin can manage cafe tables and generate a unique QR code for each table.

### Order Tracking

- Customers can view their order status, and cashiers can update it.

### Cashier Payments

- Cashiers can record payments made at the counter.

### Sales Reporting

- Admin can view basic order and sales statistics.

## Out of scope

- Delivery.
- Online payment, Paymob registration/credentials, hosted checkout and provider callbacks (removed by user approval on2026-10-30).

## Project constraint

- Keep the implementation simple and realistic for a three-day portfolio project.
- Keep the confirmed scope achievable within three days of AI-assisted development; do not add unrelated features or expand basic sales reporting beyond basic order and sales statistics.

## Historical implementation decisions — 2026-10-29 (online payment superseded)

- Complete Features001–005 in dependency order using Spec Kit and preserve ASP.NET Core, Blazor, Bootstrap, EF Core, SQL Server and Identity.
- Counter orders: dine-in with existing table, or takeaway without table; name/phone optional. Online customer name/phone remain required.
- Payments: Paymob Egypt sandbox/test only, EGP, Hosted Unified Checkout. No live transactions or simulated success. Backend initiation, authenticated evidence/HMAC verification, reconciliation and duplicate protection are required; browser redirects never prove payment.
- Counter cash recording and rejection are blocked while an online attempt is pending or uncertain; paid orders cannot be rejected. Reconcile first; refunds are outside this scope.
- Continue independent cash/billing/reporting work when Paymob credentials or sandbox access are unavailable; leave actual provider acceptance pending.
- Polish consistent responsive customer/POS/Admin UI; basic order/sales reports only. No delivery.

## Approved scope revision —2026-10-30

The user approved removing Paymob through Spec Kit because account setup is unavailable. Current payment scope is cash only. Provider source and prior planning are archived recoverably; no credentials, data, accounts, QR identifiers or migrations are deleted. Original T009–T013 are cancelled/out of scope, not passed. T014 retains cash-only billing acceptance. The four real-phone/final-acceptance tasks in Features001/002 remain pending; they were not waived by this payment scope change.

## Approved acceptance revision —2026-10-30

The user explicitly approved mobile browser emulation and corresponding Spec Kit acceptance updates in place of physical-device testing. Remaining T051/T056 (Feature001) and T059/T063 (Feature002) can complete only after actual automated QR/rename/dine-in/pickup/private-tracking checks and final coverage review. Do not claim real Android/camera/network testing. Preserve existing records and avoid public exposure.

## Approved frontend-only refresh —2026-10-30

The user confirmed English Café branding and then explicitly narrowed the request to frontend appearance with simple photos. Add a café homepage, Home/Menu/About navigation and public menu visual polish only. No customer registration/profile/address/account history, new database entities, delivery or online payment is part of this change. Preserve all current guest/staff/order/cash behavior. Superseded broader draft is archived; Feature006 governs this six-task frontend addition.

## Renewed complete restaurant scope —2026-10-30

User now explicitly authorizes customer signup/login/profile/address/history, restaurant UI and categorized menu, delivery(cash-on-delivery)/pickup/table-QR ordering, staff-driven accepted/preparing/ready/delivery/completed tracking. This supersedes prior frontend-only/no-delivery exclusions; no online payment or courier integration. Feature007 is authoritative. Preserve records/accounts/QRs and use additive reviewed migration with backup.
