# Research: Cash-only scope revision —2026-10-30

No provider research or account is needed to remove online payments. User explicitly approved the reduced scope. Preserve the original implementation, including uncommitted fixes, in archive/paymob-20261030 with SHA-256 evidence. Archive is reference-only text outside the application; no online route is compiled.

Retain existing payment schema/migrations and sandbox-history classification to avoid data loss or unsafe changes. Cash and rejection guards must continue to recognize legacy attempts. PaymentStatus.OnlineAvailable remains for backwards-compatible serialization but is alwaysfalse, independently of any old secret values.

Reuse the validated cash-only controller/panel baseline, then adjust customer copy and remove provider registration/options dependency. No package changes, database operations, forced unlock or new payment method. Historical provider design/research survives in the archived Feature004 documents.
