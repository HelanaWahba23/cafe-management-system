# Research: Admin Dashboard & Reports

Decision:aggregate-only SQL groups for orders and receipt day/method, separate timestamp axes and exact historical receipt amounts. Rationale:bounded data transfer, no contacts/entities and no catalog-price drift. Alternative:loading whole orders/lines or treating order totals as sales would expose unnecessary data and inflate unpaid/test sales.

Decision:inclusive UTC DateOnly window, max366days, exclusive end midnight construction with explicit MaxValue guard. Rationale:existing timestamps are UTC and labels make boundary meaning visible; no invented local business timezone or new scheduling subsystem.

Decision:fresh StaffAccess.CheckAdminAsync on every service read plus endpoint policy; no-store response and read-only Blazor page. Rationale:current role/stamp lookup rejects stale claims/circuits; UI hiding alone is insufficient.

Decision:extend existing site.css Bootstrap component/root variables, no external fonts/images/chart packages. Preserve reconnect selectors, z-index2000, static routing, interactive roots and existing PublicLayout enhanced-navigation opt-out. Rationale:professional consistency without discarding unfinished work or behavior. Cards and responsive tables fit320/768/desktop.

Read-only plan research inspected actual PaymentSettlement, StaffAccess and theme paths. All technical unknowns resolved locally; no new business question or dependency required.
