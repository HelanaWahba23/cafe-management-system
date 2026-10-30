## Current frontend scope —2026-10-30

Latest user request is FRONTEND APPEARANCE AND SIMPLE PHOTOS ONLY. The preceding account/profile/address expansion is cancelled; draft preserved in archive/customer-portal-draft-20261030. Do not implement those backend features from older chat messages. Brand/language:English,Café. Active feature specs/006-customer-cafe-portal, six tasks. Homepage/menu public visual refresh complete and22 checks plus Debug/Release builds passed; see its validation/assets records.

Preview host:https://localhost:7281/, PID796 at time of launch, exact CafeManagement.Api/bin/Release/net8.0/CafeManagement.Api.dll; verify actual PID command line before stopping. This is loopback only, original database with unchanged existing configuration, no record writes. Normal7128 port was freed by stopping only my matching diagnostic host; user can Run Debug to see the new root page. Existing other hosts retained. No new credentials/migrations/packages; local generated photo copied to wwwroot/images/cafe-hero.png. Only source/assets/spec/documentation for frontend refresh should be staged. Earlier164-task acceptance remains historical/valid; new frontend delivery tracked separately.

## Final approved emulated acceptance —2026-10-30

The user explicitly approved mobile-browser emulation in place of physical Android acceptance. The four remaining tasks passed under that revised scope: actual QR PNG decoding for two tables, correct anonymous touch/mobile contexts, Admin rename/saved-QR continuity, dine-in/pickup checkout and cashier association, private-link reload/second-context revisit and invalid/missing access denial, status update and responsive/focus checks.55 checks passed on isolated Edge156 with sandbox and normal TLS, using the cash-only publish from70fb29f. No hardware/camera/public-phone-network acceptance is claimed.

Exact run-owned fixture cleanup passed. All15-table row fingerprints match in both original and validation-copy databases; prior accounts, QR identifiers and migrations preserved. The owned7278 host was stopped after matching PID/DLL/port; existing hosts retained. No application source changed or public exposure/certificate/firewall change occurred.

Final coverage review reconciles Feature00111FR/8SC and Feature00213FR/8SC with recorded prior evidence and the new result. Later authorized POS/cash/reports are not scope violations of earlier features. See FINAL-ACCEPTANCE.md and specs/mobile-emulation-result.json. All active original tasks are complete164/164;5 Paymob tasks are removed, not passed. Earlier physical-phone blockers below are historical and superseded only by the explicit user-approved emulation revision.

## Current cash-only scope —2026-10-30

User approved removing Paymob through Spec Kit. All ten original affected source files and pre-change Feature004 design/tasks are preserved under archive/paymob-20261030; SHA-256 hashes verified. Archive text is excluded from compilation and marked -text to preserve exact bytes across Git checkout. No credentials, database records, Identity accounts, QR identifiers, constraints or migrations were changed.

Compiled Paymob client/service/options/webhook/return and online actions/registrations were removed after archiving. Previously accepted cash controller/panel baseline was restored; PaymentService no longer accepts provider options and always projects OnlineAvailable=false. Cash instructions replace unavailable-provider copy. Historical sandbox/rejection/uncertain guards remain to protect existing data. No old secret setting can turn online payments back on.

Spec Kit requirements, constitution1.1.0, Feature004 spec/plan/research/data model/contracts/quickstart/tasks and current setup were aligned. T009–T013 are explicitly REMOVED FROM SCOPE, not passed. T014 cash-only bill acceptance combines prior26 cash/security checks and320/768/1280/keyboard evidence with the unchanged cash controller/panel behavior, scoped dependency review and fresh route/build checks. The prior interactive Paymob UI is no longer in scope.

Debug/Release builds passed zero warnings/errors. Release publish passed to inactive artifacts/cash-scope-20261030-publish. Twelve read-only HTTPS checks on7277 passed: existing pickup/QR/login; missing customer capability denied; staff payment/cash/report access denied anonymously; removed initiation, both reconciliation routes, webhook and return all404. This ran against the existing validation copy with legacy Paymob Enabled=true deliberately set only for that process, proving configuration cannot restore removed routes. No payment/order/catalog writes, migration or cleanup ran. The temporary host was stopped only after verifying PID/DLL/port; all prior hosts remain untouched.

Active original tasks164:160 completed,4 pending;5 original provider tasks removed. Feature00411/11 active; Feature00154/56, Feature00261/63, Feature00322/22, Feature00512/12. Real Android acceptance was not performed or waived. No provider account/credentials are required for current delivery. Earlier handoff/provider setup below is historical and superseded by this section.

# Historical handoff checkpoints

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

## Resume checkpoint — 30 October 2026 (supersedes setup uncertainty)

Local HEAD and GitHub master were both verified at d94c845 before changes. All seven required Paymob User Secrets keys are absent (presence-only check, no values printed). Official inquiry documentation requires auth_token in the JSON body; the prepared PaymobClient now supplies it for both order_id and merchant_order_id. A package-free local intercepted-HTTP probe passed four requests without provider or database access. Updated full-source Release build passed with zero warnings/errors. Existing completed builds/functional checks were not repeated beyond the changed-code build.

Acceptance remains159/169. All ten original task markers stay unchecked; external phone/provider checks were not performed. EXTERNAL-ACCEPTANCE.md now provides operator inputs, safe credential entry, approval-gated temporary HTTPS and exact acceptance evidence. No tunnel client was installed or started; no firewall/certificate/host/database changes occurred. Preserve all seven unfinished Paymob paths, including the inquiry correction, uncommitted pending actual acceptance. A before-edit client copy and original tracked diff are ignored under artifacts/finalization-resume-20261030.

Only reviewed setup/status documentation is eligible for this checkpoint commit. The provider implementation still needs genuine response/recovery and billing acceptance. The original remote-connectivity blocker was resolved using authorized Git network access; verify the final documentation push separately.

## Portfolio finalization — 30 October 2026

Spec Kit implementation prerequisites and all five requirement-quality checklists reviewed (16/16 each). Starting local/remote master verified at f1d8e44. Counts remain159/169: actual Android and provider acceptance are not performed. No task markers were checked from builds or desktop evidence.

Before the subsequent instruction to leave payment integration unchanged, two local corrections were prepared: inquiry evidence now must match the initiating attempt's saved provider order/amount/currency/integration/test mode before shared processing; initiation errors refresh the private payment projection so a committed uncertain attempt can be shown. Nine focused non-success correlation checks passed. Interactive UI acceptance remains pending. All seven provider paths remain uncommitted and preserved; no further payment source edits followed that instruction.

Full-source Debug/Release builds passed with zero warnings/errors; Release publish passed to inactive artifacts/portfolio-finalization-publish. Dependency audit reported no vulnerable packages. Fifteen read-only TLS HTTP/privacy/authorization/disabled-provider checks passed on an isolated loopback host against the existing validation copy; that exact owned host was stopped. No database writes, migrations, fixture cleanup, account/QR changes, public exposure or trust/firewall changes occurred. Prior successful customer/cashier/admin/billing/reporting evidence is reused rather than replayed.

PAYMENTS-ACCESS.md records official eligibility findings and legitimate alternatives. Public Paymob documentation does not confirm company-free developer access; obtain Paymob's sandbox-only eligibility answer before registering a fictitious business or attempting live onboarding. DEPLOYMENT.md prefers approved staging with a separate database. EXTERNAL-ACCEPTANCE.md contains the exact Android operator checklist. These external gates remain pending, and documentation changes alone are eligible for this commit.

## Customer entry correction —2026-10-30

User requested a customer-facing first impression. All three launch profiles now open menu/pickup instead of Swagger, and GET / redirects to /menu/pickup. Swagger remains Development-only. Release build passed with zero warnings/errors; all launch-profile values and three read-only trusted-HTTPS checks passed on isolated7279:302 root redirect, anonymous pickup200 with correct page, Development Swagger200. The exact owned test host was stopped. No database/auth/provider changes, public deployment or new acceptance requirement; original task disposition remains164 complete plus5 removed.


Delivered source/assets/specification commit0dbf97f pushed successfully to origin/master. T006 is now complete after actual push; frontend tasks6/6. Combined active tasks170/170 (original164 plus6 frontend); five original Paymob tasks remain removed, never passed. Preview remains loopback7281; normal Debug Run uses the new root homepage, with7128 left free. No database/auth change or new account feature.
