# Tasks: Admin Dashboard & Reports

**Input**:spec/plan/research/data-model/contracts/quickstart. No automated test suite requested.

## Phase1 Setup

- [x] T001 Preserve current Git/data baseline and complete spec quality/clarification in specs/005-admin-dashboard-reports/spec.md and checklists/requirements.md (FR-008).
- [x] T002 Complete research, technical plan/contracts and consistency analysis in specs/005-admin-dashboard-reports/plan.md and research.md (all FR/SC).

## Phase2 Foundation

- [x] T003 Define aggregate DTOs/date conventions in CafeManagement.Api/Contracts/AdminReportContracts.cs; implement bounded grouped historical reads and current Admin access in Application/AdminReportService.cs (FR-001–005).
- [x] T004 Add protected no-store GET endpoint in CafeManagement.Api/Controllers/AdminReportsController.cs and Program.cs service registration (afterT003; FR-001/005).

## Phase3 US1 Basic management reporting

Independent acceptance:known order-created and receipt-recorded windows give exact counts/cash sums; empty/invalid/security outcomes pass without private fields.

- [x] T005 [US1] Add CafeManagement.Api/Components/Pages/Admin/Dashboard.razor withUTCdatefilter,metrics,orderbreakdowns,dailycash,sandboxtestseparation andbusy/error/empty states (afterT004; FR-002–005).
- [x] T006 [US1] AddDashboard navigation in Components/Layout/AdminLayout.razor andAdmin default landing in Pages/Account/Login.cshtml.cs, preserveReturnUrl/Cashier/management (afterT005; FR-006).
- [x] T007 [US1] Validate known/boundary/outside/empty/invalid windows, exact cash history, unchanged catalog effects andcurrent Admin/anonymous/Cashier/stamp denial; record specs/005-admin-dashboard-reports/validation.md (afterT006; SC-001/002).

## Phase4 US2 Consistent responsive interface

Independent acceptance:customer,POS,Admin,login share coherent readable theme andretain workflows at320/768/desktop/keyboard.

- [x] T008 [US2] Extend CafeManagement.Api/wwwroot/css/site.css and stylesheet versioning in Components/App.razor / Pages/Account/Login.cshtml with cafe palette,typography,cards/buttons/focus/touch/wrapping whilepreserving reconnect overlay andpendingnavigation code (afterT007; FR-007/008).
- [x] T009 [US2] Refine existing Components/Layout/AdminLayout.razor,PosLayout.razor andPages/Account/Login.cshtml presentation only where needed; preservelogoutCSRF,rolelinks,publicroutes andlocalReturnUrl (afterT008; FR-006/007/008).
- [x] T010 [US2] Browser-validateactual320/768/desktop andkeyboard onpublicmenu/cart/privatepaymenttracking,POSqueue/bill,Adminmanagement/dashboard/login; fixconfirmedpresentationdefects,saveevidence andleavephone/native/provideracceptance pending in specs/005-admin-dashboard-reports/validation.md (afterT009; SC-003).

## Phase5 Integration / delivery

- [x] T011 Runrestore/Debug/Release/publish,authorization/privacy/diffreview andfinaloriginal/copy preservation; reconcilecross-featuretaskstatus andwrite README.md setup/currentblockers plus specs/005-admin-dashboard-reports/implementation-handoff.md (afterT010; FR-008,SC-004).
- [x] T012 Commit/pushonlyvalidatedreport/theme/setup changes; keepunverifiedPaymob andFeature002 UIunchanged/unstaged,recordmilestone in specs/005-admin-dashboard-reports/implementation-handoff.md (afterT011; SC-004).

## Dependencies / strategy

Setup→service/contracts→endpoint→dashboard/navigation→targetedreportvalidation→sharedtheme→cross-screenvalidation→delivery. Report implementation is independent of pending actual Paymob sandbox setup. Noparallel edits ofProgram/layout/styles; read-onlyplanresearch canrunindependently. MVPUS1 thenUS2 continuesautomatically. Knownprovider/phone/nativegapsdonotbecomecompleted tasks fromstyling/buildalone. No newDBmigration,export/analytics packageorbusinessrules.
