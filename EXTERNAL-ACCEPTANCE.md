# External acceptance setup — 30 October 2026

The verified starting commit is `d94c845`, also confirmed on remote master. Acceptance remains **159/169**. The ten open tasks are 001 T051/T056, 002 T059/T063, and 004 T009–T014. Compilation and local protocol checks do not satisfy those external gates. Existing cash, POS, dashboard and navigation evidence remains applicable.

## What the operator must provide

1. A Paymob Egypt merchant sandbox account with Hosted Unified Checkout and an **EGP test card integration**, plus permission to create sandbox test orders. Configure the credentials locally as below; do not send them in chat.
2. Explicit approval to download/use a temporary tunnel client and expose the chosen local app for a bounded test session, or an already approved HTTPS staging origin. Confirm the database to use for test orders; prefer a backed-up, separately selected validation copy. Do not reuse old fixture deletion scripts.
3. A real Android phone with camera/QR reader and Chrome, and an operator to perform the phone steps. Supply Android/Chrome versions and observed outcomes, without sharing private tracking/checkout URLs.
4. For genuine provider failure acceptance, a supported sandbox decline/failure procedure from Paymob support or the merchant account manager. A cancelled browser tab is an interruption test, not a verified declined transaction. Ask Paymob to confirm terminal-intention/expiry and recovery semantics too.

## Paymob account and local configuration

Sign in to the Paymob Egypt merchant dashboard. Select test mode. Under **Developers → Payment Integrations**, select or request an online card integration in EGP whose test mode matches the keys. Obtain the test Secret Key, Public Key, API Key and HMAC secret from the account's integration credentials/settings. Dashboard labels may vary; if unavailable, use the official onboarding wizard or merchant support. Never substitute a live integration.

Official references checked for this resume:

- [Create Intention](https://developers.paymob.com/paymob-docs/intention-apis/create-intention): Token secret-key authorization, cents, matching test integration, special reference and callback/redirect fields.
- [Transaction inquiry](https://developers.paymob.com/paymob-docs/developers/transaction-inquiry-apis/by-order-id-or-reference): JSON `auth_token` and one order/reference selector. The local prepared client was corrected to follow this body contract.
- [HMAC transaction callback](https://developers.paymob.com/paymob-docs/developers/webhook-callbacks-and-hmac/hmac/hmac-transaction-callback): ordered fields and SHA-512.
- [Official onboarding wizard](https://wizard.paymob.com/): select Egypt/EGP and test mode; use its current sandbox test-card instructions. Enter test card data only at hosted Paymob checkout. No real card data belongs in this app.
- [Paymob testing guidance](https://developers.paymob.com/paymob-docs/ai-solutions/ai-agent-skill): ask the account manager for decline simulation; do not invent a decline card.

The project-specific User Secrets file exists, but none of these seven keys were present when checked (values were never printed):

| Key | Required value |
| --- | --- |
| `Paymob:SecretKey` | Test secret, prefix `sk_test_` or `egy_sk_test_` |
| `Paymob:PublicKey` | Test public key, prefix `pk_test_` or `egy_pk_test_` |
| `Paymob:ApiKey` | Merchant API key for authenticated inquiry |
| `Paymob:HmacSecret` | Merchant callback HMAC secret |
| `Paymob:IntegrationId` | Positive test card integration ID |
| `Paymob:CallbackBaseUrl` | Approved public HTTPS origin, no path/query |
| `Paymob:Enabled` | Keep false until ready for sandbox testing |

From the repository root, this PowerShell snippet prompts without echoing secrets or putting them in shell history/command arguments, and merges these keys through standard input. User Secrets are local development storage, not an encrypted vault. It leaves existing database/account secrets intact.

```powershell
$paymobSetup = @{}
foreach ($key in @('SecretKey', 'PublicKey', 'ApiKey', 'HmacSecret')) {
    $inputSecret = Read-Host "Paymob test $key" -AsSecureString
    $paymobSetup["Paymob:$key"] = [System.Net.NetworkCredential]::new('', $inputSecret).Password
}
$paymobSetup['Paymob:IntegrationId'] = Read-Host 'Test EGP card integration ID'
$paymobSetup['Paymob:CallbackBaseUrl'] = Read-Host 'Approved HTTPS origin'
$paymobSetup['Paymob:Enabled'] = 'false'
$paymobSetup | ConvertTo-Json -Compress | dotnet user-secrets set --project CafeManagement.Api
$paymobSetup.Clear()
Remove-Variable inputSecret, paymobSetup
```

Do not run `user-secrets list` into logs/chat. No credential validity or provider transaction has been tested yet.

## Temporary HTTPS plan — prepared, not activated

Use a short-lived [Cloudflare Quick Tunnel](https://developers.cloudflare.com/tunnel/get-started/quick-tunnels/) to an isolated loopback HTTPS host. No firewall change or trusted certificate installation is needed. The client is not currently installed. Download/install and activation require operator approval first. The tunnel URL makes the routed application publicly reachable; an unpredictable hostname alone is not access control. Paymob callbacks cannot pass an interactive email/password gate.

After approval, verify the exact selected host/process and database before using it. Do not stop existing hosts or overwrite their publish folders. Keep origin TLS validation enabled. A proposed tunnel command, **only after approval**, is:

```powershell
cloudflared tunnel --url https://localhost:7128
```

Use that port only if its verified process is the intended test host. If origin certificate validation fails, stop: use an explicitly verified public certificate file with the client's `--origin-ca-pool` option or another approved trusted origin. Do not use `--no-tls-verify`, HTTP forwarding, certificate-warning bypasses or automatic trust installation. Quick Tunnels have no uptime guarantee and the hostname changes after restart; verify Blazor's WebSocket connection on the phone.

Before creating test intentions, configure the same generated HTTPS origin as `PublicWebBaseUrl` and `Paymob:CallbackBaseUrl` in the selected test host's configuration. Apply configuration by restarting only that verified host; preserve prior values for restoration. Published Production hosts do not automatically load development User Secrets: use a controlled Development test host or supply configuration privately to that process. Verify anonymous menu and staff authorization before sharing the URL.

Set provider processed callback to `https://<approved-host>/api/payments/paymob/webhook` and browser return to `https://<approved-host>/payments/return`. The intention request supplies these URLs too. The return page is informational; it cannot settle payments. Enable `Paymob:Enabled` only for the test host when all configuration is ready. Do not start a test intention before callbacks are reachable.

Do not enable request-body/query logging: callbacks and return URLs can contain sensitive data. Keep raw callback evidence local and redacted. Stop the tunnel at session end, disable Paymob, restore previous origin settings and verify the public URL no longer serves the app. Do not delete test records without a separately reviewed exact cleanup scope.

## Acceptance session and evidence

| Tasks | Actual checks required before completion |
| --- | --- |
| 001 T051 | Signed-out Android scans two distinct table QRs; correct table/menu without login; rename one table and rescan the old QR, then restore its label if appropriate. |
| 002 T059 | Same phone completes dine-in and direct pickup without table, checkout, private tracking and revisit; record persistence/table correctness. |
| 001 T056 / 002 T063 | Reconcile all scenarios against prior evidence plus the phone results; respect later-feature scope rather than treating authorized POS/payments as a Feature 001 regression. |
| 004 T009–T012 | Real intention request/response, test method and exact EGP amount, hosted checkout, capability privacy, no redirect settlement and safe unavailable configuration. |
| 004 T013 | Actual success and supported failure, signed callback and authenticated inquiry, duplicate/out-of-order callback, amount/integration/signature mismatch rejection, interruption before/after intention response, and lost-link/terminal recovery. Verify exactly one receipt; uncertain attempts continue blocking cash/rejection. |
| 004 T014 | Actual sandbox bill state/method/time, historical item prices, privacy, revenue exclusion, keyboard and 320/768/desktop controls. |

Record time, host/build, device/browser, provider test transaction ID, observed state and receipt count in the relevant validation documents. Redact capabilities, keys, card/customer data and raw payloads. Do not manufacture a signed successful callback to replace a real provider test. Current failure/unknown-intention behavior deliberately stays blocking; lost checkout links have no validated reopen path, so this usability gate must be resolved before final provider acceptance.

After genuine checks, fix any discovered protocol/recovery issues, run checks appropriate to those changes, update task markers and totals, then commit the exact verified provider files. Until then preserve all seven provider paths uncommitted. No external task is passed by this setup document.
