# Data model: Payments & Billing

Order gains PaymentRevision:bigint default0. Increment on every local payment-state mutation to update SQL RowVersion and invalidate stale staff changes. Historical fields/totals/hashes unchanged.

PaymentSettlement:OrderId primary key/restrictive one-to-one Order FK; Amount decimal18,2>0; Currency char3 EGP; Method Cash/PaymobSandbox; RecordedByUserId existing Identity restrictive FK required only for cash; ProviderTransactionId bigint unique filtered nonnull required only for sandbox; RecordedAtUtc. One receipt per order and provider transaction. No raw card data.

PaymentAttempt:Id Guid stable special_reference; OrderId restrictive FK/index; Amount decimal18,2>0; EGP; IntegrationId positive configured test method; State Creating/Pending/Uncertain/Failed/Succeeded; ProviderOrderId nullable bigint unique filtered; ProviderIntentionId nullable120; ProtectedClientSecret nullable4096 using DataProtection; CreatedAtUtc/UpdatedAtUtc. One active nonterminal attempt per order enforced by IsBlocking/state checks and a unique filtered OrderId index where IsBlocking=1, plus order-first UPDLOCK,HOLDLOCK transactions. Unknown provider creation is never blindly retried. Terminal Failed requires authenticated evidence; inquiry missing/notfound alone does not cancel an existing payable intention.

Payment state projection:existing receipt→PaidCash/PaidSandbox; active Creating/Pending→Pending; unknown→Uncertain; otherwiseUnpaid. Fulfillment remains existing OrderStatus. Rejected order cannot initiate/settle new payment; pending/unknown/paid blocks rejection. No refunds or partial settlement.

Migration:add columns/tables/indexes/checks only. Validate on owned copy against original-field fingerprints before any original schema deployment.
