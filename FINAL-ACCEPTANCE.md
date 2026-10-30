# Final acceptance —30 October2026

## Approved scope and truthful result

The user approved cash-only payments and subsequently explicitly approved automated mobile-browser emulation in place of physical Android acceptance. Current delivery has **164/164 active original tasks complete**. Five original Paymob tasks (004 T009–T013) are removed from scope, not passed. This is not169/169 successful tests.

No Android hardware, camera scanner, physical-phone Chrome or external-phone network was tested. The accepted substitute decoded actual generated QR PNGs and opened their URLs in isolated signed-out touch/mobile browser emulation over trusted loopback HTTPS. No public tunnel, firewall change, certificate bypass or trust installation was used.

## New acceptance execution

Starting application commit: `70fb29f`. Reused its verified cash-only Release publish at artifacts/cash-scope-20261030-publish; application source did not change in this acceptance session. Prior full Debug/Release builds and publish passed with zero warnings/errors; there was no reason to repeat them for documentation-only acceptance updates.

Actual browser: Edge Chromium156, isolated headless profile with Chromium sandbox enabled and normal TLS validation. Emulation: Pixel7 mobile/touch parameters, main viewport390×844; menu widths320/360/390/768/1280; dine-in/pickup review and private-tracking widths320/390/768/1280. The official jsQR decoder was loaded only into an ignored diagnostic page; no application dependency changed.

**55 checks passed**, including:

- Decode two real Admin-generated QR PNGs; each URL matches its saved table and the two differ. Open both without staff cookies/login.
- Rename the first table through the Admin screen on the validation copy; open the previously decoded QR, see the new label for the same public identifier, restore the old label and reopen it.
- Place a dine-in order with explicit review, verify saved reference/type/table and exactly one corresponding cashier-queue record.
- Open its private link, show historical bill/cash-only payment instructions, reload without changing details, consume the credential fragment, and revisit from a second isolated signed-out mobile context.
- Change fulfillment to Preparing through the Cashier screen; refresh customer tracking and observe the update.
- Verify missing and explicitly invalid tracking contexts expose no order. Test keyboard focus after payment controls finish loading.
- Open pickup directly without QR/table, review and submit, verify separate reference and null table association in both customer bill and cashier queue, then reload/revisit without creating duplicate orders.
- Measure no horizontal overflow at the recorded widths for menu, both review flows and private tracking.

Two diagnostic harness issues were corrected before the final successful run: BOM handling for local configuration and use of browser HTTPS validation instead of Node's separate certificate store. An initial focus check also ran before the control finished loading; the final check explicitly waited for enabled state. Earlier incomplete runs were not counted as acceptance passes.

## Preservation and cleanup

Original database was read-only throughout. The existing separately named validation copy hosted all temporary changes. Unique run-owned customer names scoped fixture ownership. Cleanup removed only the two diagnostic orders and their children, restored guarded table labels, then compared every existing table's row fingerprints.

**All15-table fingerprints matched in both original and validation databases**, including Identity/account values, QR identifiers and migration records. Identity sequence allocation can advance when creating/deleting owned test rows; no prior record was changed. The temporary7278 host was stopped only after matching its recorded PID, exact published DLL and port; existing hosts were retained.

Detailed scripts, screenshots, hash-only baselines and raw diagnostic results are ignored under artifacts/mobile-emulation-acceptance. The committed summary in specs/mobile-emulation-result.json omits credentials, private links, cookies and customer data.

## Feature001 final coverage —T056

| Requirement / criteria | Accepted evidence |
| --- | --- |
| FR-001 / SC-005 | Prior T024/T034/T042/T053 backend Admin/current-user/token/cookie checks; new signed-out menu contexts. |
| FR-002–003 / SC-001 | Prior T025–T035 category/product create/edit/availability/price persistence. |
| FR-004 | Prior reopen/restart/failure/reconnect and uncertain-save checks T035/T043/T052/T054. |
| FR-005 | Prior table/QR management T036–T043 plus two real QR PNGs decoded in this run. |
| FR-006 / SC-002 | New two-table navigation, Admin rename, unchanged public identifier and saved-QR URL rescan substitute; restored label. |
| FR-007 / SC-003–004 | New anonymous mobile/touch navigation plus recorded viewport/no-overflow checks. |
| FR-008 / SC-006 | Prior empty/invalid/load/save/reconnect checks T034/T049/T052. |
| FR-009 / SC-007 | Existing create/edit-only management; no catalog/table delete/deactivation added. Product availability remains supported. |
| FR-010 | Prior unavailable-product filtering/restoration for both UI/API. |
| FR-011 / SC-008 | Prior blank/duplicate/precision/positive-price/EGP/SQL constraint checks. |

All three Feature001 stories and their scenarios reconcile to these recorded checks. Original Feature001-only exclusions do not prohibit separately authorized Features002–005. There is no delivery or new catalog deletion/deactivation. Removal of provider runtime source was authorized and byte-preserved in its archive. No new permanent test project, framework replacement or database migration was introduced.

## Feature002 final coverage —T063

| Requirement / criteria | Accepted evidence |
| --- | --- |
| FR-001–002 / SC-001 | Prior anonymous catalog/cart/quantity/same-name-product validation; four native navigation dialog cases already accepted. |
| FR-003–004 / SC-002 | Prior two-table/two-pickup four-order association sample; new QR/rename/mobile dine-in and direct pickup acceptance corroborate entry and private journeys. |
| FR-005 | Prior authoritative review, atomic persistence, invalid/unavailable rejection and rollback checks. |
| FR-006 / SC-004 | Prior increase/decrease/reconfirmation and historical-money checks; new saved-bill reload comparison. |
| FR-007 / SC-003 | Prior idempotency/uncertain-reply/restart recovery; new confirmation/revisit and no duplicate orders. |
| FR-008 | Prior five-state/four-edge lifecycle checks; new real Cashier→customer Preparing update. |
| FR-009 / SC-005 | Prior capability/privacy/all-status checks plus new independent-context revisit and missing/invalid-key denial. |
| FR-010 | Prior role/CSRF/JSON/rate/cookie controls; signed-out customer contexts and separate staff sessions in this run. |
| FR-011 / SC-006 | Prior error/reconnect/keyboard/layout evidence plus new mobile/touch, both review flows, tracking and focus checks. |
| FR-012 / SC-007 | Matching original/copy all-table fingerprints; no altered application source or unapproved features. |
| FR-013 / SC-008 | Prior required/bounded contact-field checks, guest checkout/privacy; new private links from both guest order types. |

All four Feature002 stories and their scenarios reconcile to the existing end-to-end evidence and this final run. Later Cashier/cash/reporting features are separately authorized and integrated; online payment remains removed. Physical-device testing is explicitly outside the revised acceptance, rather than reported as passed.

## Final task disposition

| Feature | Active tasks complete | Removed original tasks |
| --- | ---: | ---: |
|001 Menu & Tables|56/56|0|
|002 Customer Ordering|63/63|0|
|003 Cashier & POS|22/22|0|
|004 Cash Payments & Billing|11/11|5|
|005 Dashboard & Reports|12/12|0|
|Total|164/164|5|

No further active task is blocked by Paymob registration, public staging or a physical phone. Optional future public deployment/real-device acceptance still needs its own approval and evidence.
