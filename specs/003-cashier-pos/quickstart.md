# Quickstart validation: Cashier & POS

Use existing local User Secrets and a confirmed owned SQL validation copy. Never print connection strings/passwords or reset original database. Read implementation-handoff.md for current host/fixture ownership before changing a running host.

1. dotnet restore CafeManagement.sln; dotnet build CafeManagement.sln --no-restore. If user host locks ordinary output, reuse ignored artifacts/customer-ordering-next OutputPath. Build Release and publish to artifacts/cafe after code validation.
2. Generate/review additive EF migration SQL. Capture pre-existing row/account/QR fingerprints; apply only to owned copy, verify original-field projections unchanged and existing guest rows default Customer.
3. Using existing Cashier/Admin accounts, verify anonymous401, Admin403 for POS, validCashier200, CSRF missing/rejected, unknown JSON400, fresh role/stamp revocation403. Do not alter original accounts for tests.
4. Submit existing guest dine-in/pickup flows and counter dine-in/takeaway without contacts. Verify shared queue, filters/pages, snapshots/creator, guest required contacts, no guest counter tracking.
5. Verify price-change reconfirmation, unavailable items, invalid quantities/context, retry once/recovery, changed-intent conflict, atomic rollback and stale-version conflict. Record only diagnostic-owned rows and clean only those after checks.
6. Exercise every lifecycle edge and invalid pair, rejection reason bounds, tracking reflection and immutable bills after catalog edit.
7. Browser check POS keyboard/busy/error/empty states and actual 320/768/desktop dimensions. Keep restricted native/physical-phone checks pending; no privacy bypass.
8. Preserve database fingerprints; review diff/security, mark only passing tasks, commit/push validated paths. Feature 002 unfinished UI remains separate.
