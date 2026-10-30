# Cafe Management implementation handoff — 29 October 2026

## Finalization checkpoint — 30 October 2026

Read the latest handoff, all remaining task markers and all seven local provider paths (three modified files plus four untracked files). No additional independent implementation task remains outside the recorded external gates. Preserve the provider changes locally until actual sandbox acceptance; task counts and unchecked acceptance markers remain unchanged.

Final full-working-tree Debug and Release builds both passed with zero warnings/errors using existing restored dependencies. Release publish with `--no-build --no-restore` succeeded to a new inactive directory, `artifacts/finalization-20261030-publish`. It includes prepared provider code; compilation does not establish provider acceptance. No hosts were stopped/replaced, database touched, successful functional validation repeated, credentials exposed or network configuration changed. README now contains the explicit final setup checklist.

The finalization commit changes documentation only. The validated application code milestone remains `18d6a67`; the previous documentation checkpoint is `49f43a4`. Initial remote verification failed because this execution environment could not connect to GitHub; verify push/remote state after the finalization commit rather than assuming it was published.

Latest code milestones on origin/master: customer menus/navigation `18d6a67`, dashboard/theme `8657b2e`, cash payments `0b9038c`, POS `3a30dea`. All validated code is pushed. Preserve the remaining local provider work.

| Feature | Accepted tasks | Remaining gate |
| --- | ---: | --- |
| 001 Menu & Tables | 54/56 | T051 physical-phone QR; T056 final acceptance. |
| 002 Customer Ordering | 61/63 | T059 physical-phone journeys; T063 final acceptance. |
| 003 Cashier & POS | 22/22 | None. |
| 004 Payments & Billing | 10/16 | T009–T014 provider protocol/workflow and complete provider billing acceptance. |
| 005 Admin Dashboard & Reports | 12/12 | None; provider/phone acceptance is not implied. |

## Newly resolved navigation acceptance

Standard Playwright 1.64.1 with installed Edge 156.0.4314.50 passed all four real beforeunload-dialog checks on the complete build at7128 and the independently published commit candidate at7274. Reload Stay retained cart/table; reload Leave cleared cart with the same table; pickup Stay retained cart/table; pickup Leave opened an empty pickup menu without table. Sandbox and normal TLS checks stayed enabled. Fresh isolated profiles were used; no existing user browser profile/tab was accessed. No order/payment was submitted. Do not repeat the old request for manual native-dialog checks.

Evidence is ignored under artifacts/native-navigation-proof. The exact candidate restored, built and published in Release with zero warnings/errors, passed the14-check HTTP role/privacy/date/style/public-route matrix, and passed final read-only original/copy fingerprints. Earlier full-source Debug/Release/publish, functional/security/responsive checks and dependency audit are recorded in the feature validation documents. No default automated test project was added.

## Remaining external prerequisites

1. Physical phone: provide an approved, trusted HTTPS origin reachable from a real phone. Current hosts are loopback-only. Acceptance requires actual QR entry/rescan, correct dine-in table, direct pickup without table, and private-link journeys. Desktop320/768/1280 checks are complete but cannot prove phone behavior. Do not alter firewall, certificates or network exposure without authorization. Keep001 T051/T056 and002 T059/T063 pending.
2. Paymob Egypt sandbox: configure actual test API/secret/public keys, test integration ID and HMAC secret through local User Secrets, plus an approved public HTTPS callback/return origin. Never put credentials or private checkout/tracking links in Git/chat. Confirm Hosted Unified Checkout with EGP/test methods, authentic success and failure, signed callbacks, authenticated inquiry/reconciliation, duplicate/out-of-order callbacks, interrupted initiation, and terminal/lost-link recovery. A failed transaction alone does not establish that an intention cannot later be paid. No successful provider transaction has been simulated or verified.

The approved safety rule remains: pending/uncertain attempts block cash recording and rejection until verified reconciliation; paid orders cannot be rejected. No live transactions or refunds. Redirects alone never settle an order.

## Local unfinished implementation — preserve

Untracked: Application/PaymobClient.cs, Application/PaymobPaymentService.cs, Controllers/PaymobWebhookController.cs, Components/Pages/Payments/Return.razor under CafeManagement.Api. Modified mixed files: Program.cs, Controllers/PaymentsController.cs and Components/Shared/PaymentPanel.razor contain additional online actions/registrations beyond their validated pushed cash versions. These compile and have local non-success HMAC/config checks, but remain uncommitted pending genuine sandbox acceptance. Do not stage all files or overwrite them with HEAD. See004 research/validation/contracts for protocol details and conservative recovery limitations.

## Data and running hosts

Original CafeManagement received only the reviewed additive AddCounterOrders/AddPaymentLedger migrations after a fresh COPY_ONLY/CHECKSUM backup, header verification and RESTORE VERIFYONLY WITH CHECKSUM. Prior records/accounts/QR fingerprints are preserved; original accounts2, QR identifiers5, orders0. Owned validation fixtures have been cleaned. Backup location and hash-only preservation evidence are ignored under artifacts/feature005-baseline; no restore/reset/seed occurred.

Main current build: https://localhost:7128/menu/t/691d9592-3b19-4466-ad62-314b0334af17, PID17112, artifacts/dashboard-review-publish, original database. Its Paymob feature is disabled/unconfigured. Older requested manual host7268 PID14556 remains running. QA7273 PID9024 shares the main publish directory; debug7269 PID6252 is older. Verify exact process command lines before stopping any host; never overwrite an active publish folder. Exact candidate7274 was stopped after verifying its recorded process command line. Launch helpers use hidden windows and keep secrets in memory.

Disk space is limited. Remove only verified obsolete generated output inside this workspace if needed; preserve all sources, evidence, backups and active-host output. No recurring/background automation is configured. Next useful work requires one of the external prerequisites above; independent cash, POS, reports and responsive UI work is complete.
