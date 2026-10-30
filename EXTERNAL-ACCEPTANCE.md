# Remaining external acceptance — cash-only scope

Paymob/online payment was removed by user approval on2026-10-30. No account, API keys, provider callback, sandbox transaction or merchant registration is needed for this release. Earlier provider setup/eligibility instructions are historical and preserved under archive/paymob-20261030.

The four remaining active tasks are Feature001 T051/T056 and Feature002 T059/T063. They require real Android QR/rescan/pickup/private tracking evidence and final acceptance review. They are not failed tests and were not waived or marked passed.

Prefer approved isolated HTTPS staging with separate test database and accounts; see DEPLOYMENT.md. Any public deployment/tunnel, firewall changes or certificate trust installation still requires approval. No public exposure has been created. Desktop responsive/browser evidence already passed and should not be repeated unnecessarily.

## Android operator record — all results currently NOT RUN

Record date/time, Android version, Chrome version, test build and approved origin before starting. Use a signed-out Chrome session with no staff cookies and normal privacy/TLS settings. Keep staff actions in a separate browser/session. Use only the approved test database; record any created order IDs privately for later reviewed cleanup.

1. Display/download the two existing table QR codes from the staging Admin Tables screen. Scan the first with the phone camera, open Chrome, and confirm correct table label, products/prices and no login requirement. Repeat with the second; verify the context changes to the second table.
2. In the staff session, record the original label and rename the first table. Scan its **previously saved** QR on Android. Confirm the new label and same underlying table; record the result, then restore the original label if the session plan calls for it. Do not regenerate/change its public identifier.
3. Add an available item from that dine-in menu; change quantity; review total and table. Submit a test order with operator-approved contact details. Confirm one order/reference and correct table in the cashier view. Keep the generated private tracking link on the phone.
4. Open/revisit the retained tracking link in the same Android session. Confirm itemization and total remain historical. Have the Cashier move the order through approved fulfillment states and refresh tracking to confirm updates. A different/missing key must not reveal the order. Do not paste the private URL into an evidence file.
5. Navigate directly to `/menu/pickup` in a fresh signed-out tab. Confirm no table/QR requirement. Add an item, complete pickup checkout and verify the private link and cashier queue both identify pickup without a table. Confirm one order after a deliberate refresh/revisit, not another submission.
6. On the actual device, check readable labels, touch controls, quantity/validation messages, keyboard behavior, no horizontal scrolling and Blazor reconnect feedback. Record any failure with a redacted screenshot and exact step. Existing desktop responsive/native-dialog evidence is supplementary.
7. Record each observed result against 001 T051 and 002 T059. Only after every required scenario passes, reconcile final acceptance for 001 T056 and 002 T063. Leave failed or unperformed checks pending. Do not mark a phone result passed from desktop emulation or from merely loading a public URL.
