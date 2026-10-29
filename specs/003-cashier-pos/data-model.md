# Data model: Cashier & POS

Order adds Origin (Customer default / Counter) and CreatedByUserId (nullable existing Identity user FK, restrictive delete). Customer requires null creator and nonblank contacts; Counter requires creator and contacts may be null or nonblank. Contact limits remain 120/40. No historical reference/hash/price/table changes.

Type remains DineIn/Pickup; Counter Pickup is displayed as Takeaway. DineIn requires table FK + label; Pickup forbids both. Existing order lines and exact money checks unchanged.

CapabilityHash remains unique binary32. Counter hash is domain-separated and bound to trusted cashier identity and a canonical random 32-byte attempt token. IntentHash retains normalized contacts/type/table/sorted quantities, excludes reviewed prices. Guest lookups restrict Origin=Customer. Counter recovery restricts creator.

RowVersion remains SQL rowversion, projected only to staff as base64. All state transitions use OrderLifecycle: Submitted→Preparing→Ready→Completed or Submitted→Rejected with nonblank reason <=500. No payment column/state introduced in Feature 003.

Migration changes existing columns only to permit null, adds default origin and nullable creator; replaces contact checks with explicit origin/creator/contact checks and restrictive FK. Compare original field projections rather than changed whole-row shape when preserving existing orders.
