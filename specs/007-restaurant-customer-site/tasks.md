# Tasks

- [x] T001 Record renewed full restaurant/customer/delivery scope and security/data contracts; preserve earlier scope history.
- [x] T002 Add customer/profile/order-address/visibility model and reviewed additive migration with backup/preservation.
- [x] T003 Implement secure signup/login/logout/profile and owner-only history/details.
- [x] T004 Extend authoritative checkout/intent/replay for delivery, pickup, table and authenticated ownership.
- [x] T005 Extend type-aware staff lifecycle and customer progress/address/cash presentation.
- [x] T006 Deliver coherent English restaurant home/menu/nav and curated persisted menu without deleting old records.
- [x] T007 Validate customer auth/profile/IDOR, delivery/address/pickup/QR/order tracking and staff/cash end-to-end on owned fixtures.
- [x] T008 Run builds/publish, preservation/security/diff checks; update setup/run instructions and deliver verified local changes. Latest user instruction explicitly removes any external deployment/push requirement.

Dependency order:T001→T002→T003→T004→T005→T006→T007→T008. No provider registration or public network change. No agents requested.

- [x] T009 Add14 requested restaurant menu items across the existing6 categories, preserve prior prices/records, verify visible menu and idempotency; update both optional seed/insert-only expansion scripts (user request: add items to menu).
