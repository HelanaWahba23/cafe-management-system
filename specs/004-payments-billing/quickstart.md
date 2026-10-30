# Quickstart: Cash Payments & Billing

1. No Paymob account, test keys or callback is required. Preserve existing User Secrets and database; no migration is needed for online-payment removal.
2. Build Debug/Release and publish to a fresh inactive folder. Start an isolated HTTPS host with approved existing validation copy configuration; do not change current live hosts or expose publicly.
3. Confirm cash/status routes still require staff authority or private customer capability and cash POST still requires JSON/CSRF. Confirm removed online routes404 with their original HTTP methods, including POST webhook and GET return.
4. Customer bill must say pay cash at counter/on pickup and contain no checkout/reconciliation controls. OnlineAvailable must be false even when old Enabled/key settings exist. Never display a fake online paid result.
5. Reuse prior exact-amount/actor/time/duplicate/concurrency/rejection/history/responsive cash acceptance. Repeat changed dependency/panel routes and build validation; never replay old fixture cleanup scripts.
6. Verify archive manifest hashes and review scoped source/docs/secrets before push. Preserve all database migrations, records, accounts, QR identifiers and historical sandbox classification.
7. Physical Android QR/rescan/pickup/private tracking and final Features001/002 acceptance remain separate, unperformed checks in EXTERNAL-ACCEPTANCE.md.
