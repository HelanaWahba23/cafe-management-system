# Data model: Cash Payments & Billing

No changes to Orders, PaymentRevision, PaymentSettlements, PaymentAttempts, relationships, constraints, indexes or migrations. Cash remains the sole newly creatable method. The cashier/actor/time and exact EGP receipt semantics are unchanged.

Keep historical sandbox settlement display/revenue exclusion and pending/uncertain blocking behavior. Provider removal is not permission to delete attempts/receipts, reset accounts/QRs, rewrite migrations or unlock a payment. OnlineAvailable is alwaysfalse; it is not a database column.

Full historical provider model is archived under archive/paymob-20261030/feature004-before-scope-change/.
