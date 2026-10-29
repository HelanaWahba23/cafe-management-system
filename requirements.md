# Cafe Management Requirements

## Confirmed scope

- Support dine-in ordering using a unique QR code for each table.
- Support online pickup ordering directly through the cafe website, without requiring a QR code.
- Both dine-in and online pickup orders must appear in the same Cashier/POS system.
- Customers can pay online or pay at the cashier/pickup counter.

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

## Project constraint

- Keep the implementation simple and realistic for a three-day portfolio project.
- Keep the confirmed scope achievable within three days of AI-assisted development; do not add unrelated features or expand basic sales reporting beyond basic order and sales statistics.

## Confirmed implementation decisions — 2026-10-29

- Complete Features001–005 in dependency order using Spec Kit and preserve ASP.NET Core, Blazor, Bootstrap, EF Core, SQL Server and Identity.
- Counter orders: dine-in with existing table, or takeaway without table; name/phone optional. Online customer name/phone remain required.
- Payments: Paymob Egypt sandbox/test only, EGP, Hosted Unified Checkout. No live transactions or simulated success. Backend initiation, authenticated evidence/HMAC verification, reconciliation and duplicate protection are required; browser redirects never prove payment.
- Counter cash recording and rejection are blocked while an online attempt is pending or uncertain; paid orders cannot be rejected. Reconcile first; refunds are outside this scope.
- Continue independent cash/billing/reporting work when Paymob credentials or sandbox access are unavailable; leave actual provider acceptance pending.
- Polish consistent responsive customer/POS/Admin UI; basic order/sales reports only. No delivery.
