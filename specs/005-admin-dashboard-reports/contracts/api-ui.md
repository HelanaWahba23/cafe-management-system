# Report contracts

GET /api/admin/reports/overview?from=yyyy-MM-dd&to=yyyy-MM-dd requires Admin policy and current DB membership/security stamp. Defaults to last7UTCdays. Invalid format/window400, anonymous401, wrong/revoked role403, safe unexpected500. No-store; read-only, no CSRF needed forGET.

Response:from,to,currencyEGP,totalOrders,statuses[{label,count}],fulfillment[{label,count}],origins[{label,count}],cashReceiptCount,cashRevenue(two-decimal string),sandboxReceiptCount,sandboxValue(two-decimal string),dailyCash[{date,count,total}]. Only aggregates; never contact/private/provider fields.

/admin/dashboard:page-local InteractiveServer/AdminLayout, date selectors labelledUTC, Apply/Refresh, clear loading/empty/error states, accessible card metrics, breakdowns and daily table. Distinguishorders created fromcash recorded; label sandbox values test-only/not revenue. Admin navigation gainsDashboard, defaultAdmin login lands there; preserve localReturnUrl andCashier landing.

Shared site.css updates presentation only; keep reconnect overlay/inert handling, native navigation guard work and all authorization/state intact. Verifycustomer menu/cart/private tracking,POSqueue/bill,Adminmanagement/dashboard/login at320/768/desktop andkeyboard. No external fonts/scripts/packages.
