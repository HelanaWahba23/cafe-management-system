# Data model: Admin Dashboard & Reports

No persistence changes. ReportWindow:inclusive From/To dates inUTC, validated1..366days. OrderGroup:Status/Type/Origin +longCount filtered onCreatedAtUtc. ReceiptDayGroup:Day/Method +longCount +decimalAmount filtered onRecordedAtUtc. Derive aggregate report with totalorders, five statuscounts, two fulfillmentcounts, two origincounts, cashreceipt count/EGPsum, dailycashrows and separate sandbox test count/EGPsum. Monetary JSON values are invariant two-decimal strings; UI parses no client-authoritative amounts.

No customer names/phones, private credentials, creator IDs, provider IDs/secrets or EF entities. Existing paid/pending/rejection/fulfillment rules unchanged. Cash and sandbox are separate, not inferred fromCompleted or provider redirects.
