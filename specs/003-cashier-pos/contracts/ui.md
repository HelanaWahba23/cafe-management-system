# POS UI contract

/pos/orders: Cashier-only page-local InteractiveServer root, refresh, status filter, bounded paging, cards/table with both customer modes and counter origins. /pos/orders/new: available catalog, quantities, optional contacts, mode/table selector, review and explicit confirm; retain attempt/draft during uncertain reply, unchanged retry, clear only after known save. Warn on leaving unsaved counter cart. /pos/orders/{id}: saved details and bill, contacts staff-only, lifecycle controls and rejection reason, refresh conflict. Writes disabled while busy or disconnected (existing reconnect data-order-write guard).

PosLayout exposes Orders / New walk-in / sign-out. Existing Admin layout and public menu preserved. Login directs Cashier to POS, Admin to existing management; local return URLs only. Responsive wrapping, accessible labels/focus/status messages; verify 320/768/desktop and keyboard. No payment claim before Feature 004.
