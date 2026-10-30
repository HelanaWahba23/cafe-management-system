> Historical research only. Paymob was removed from current scope by user approval on2026-10-30. No registration or credentials are needed. See README.md and archive/paymob-20261030.

# Payment sandbox eligibility investigation — 30 October 2026

This is a personal portfolio project with no registered company. No account was created, no credentials invented, no support message sent, and no provider was replaced. The existing Paymob code remains preserved locally. This investigation supersedes any earlier instruction to complete merchant onboarding before confirming developer eligibility.

## What official Paymob sources establish

| Question | Finding |
| --- | --- |
| Can an individual in Egypt obtain developer-only access without a company? | **Not confirmed by the public official sources reviewed.** Do not treat developer documentation or a free demo as proof of eligibility. |
| Can test credentials be obtained before merchant verification? | **Not established.** The overview lists merchant account/dashboard/business verification, while the wizard separates test setup from live approval. That does not explicitly establish an exemption for an individual portfolio developer. |
| Is there an anonymous shared credential set for real integration testing? | None found in these sources. Code Lab requires account keys or account sign-in. Test card numbers are not API credentials. |
| Is a company definitely mandatory for every sandbox applicant? | The public sources do not resolve that exact question. Ask Paymob to confirm developer-only eligibility; do not invent a company or assume live onboarding documents are required for sandbox. |

The [official overview](https://developers.paymob.com/paymob-docs/getting-started/overview) lists a merchant account, dashboard access and completed business verification as integration prerequisites. Egypt uses `https://accept.paymob.com/`; test/live mode is selected by credentials and integration IDs.

The [official Egypt registration page](https://egypt.paymob.com/portal/en/register) starts with a phone number. The [official wizard](https://wizard.paymob.com/) shows region, first/last name, mobile, business email, company name, password/confirmation and mobile-code verification. It separates test integration setup from optional go-live document submission. It does not publish an explicit company-free developer registration path. Its example configuration and showroom do not verify transactions for this project.

The [credential guide](https://developers.paymob.com/paymob-docs/need-help/setup-guides/getting-integration-credentials) places mode-specific secret/public keys under Settings → API Keys, with the API key shared across modes; test integration IDs are under Settings → Payment Integrations. The publicly accessible pages do not prove that a new unverified individual account can access these screens. Later authenticated verification requirements cannot be verified without an eligible account.

## Easiest legitimate path preserving the existing integration

First ask Paymob for an explicit sandbox-only eligibility decision. Its [official developer portal](https://developers.paymob.com/) lists `support@paymob.com`, dashboard support and the Egypt hotline. The developer community is linked there too. This draft is ready for the operator to send; no password, identity document or secret is needed:

> I am an individual developer in Egypt building a personal, non-commercial Cafe Management portfolio project. I do not have a registered company, commercial registration or tax registration. Can you provide developer-only Paymob Egypt sandbox access for EGP card payments through Hosted Unified Checkout, including test secret/public keys, API key, HMAC secret and a test integration ID, without merchant activation or live-payment onboarding? Please confirm the minimum truthful registration fields and verification needed. I will never process live payments. Please also provide a supported sandbox failure/decline test and the documented rules for expired/failed intentions and lost-checkout recovery.

Only if Paymob confirms eligibility: follow its supplied signup path using real personal details and the approved description for a portfolio project; verify the phone/email as requested; keep Test mode selected; obtain an EGP test-card integration. Configure keys locally using EXTERNAL-ACCEPTANCE.md, never in chat. If company details or documents are compulsory, stop and obtain provider guidance instead of supplying fictitious information.

## Official alternatives investigated — no integration change authorized

- **Braintree sandbox:** its [official sandbox help](https://developer.paypal.com/braintree/help/sandboxHelp) explicitly describes testing before applying for a merchant account and offers a free separate sandbox. Its [testing guide](https://developer.paypal.com/braintree/articles/get-started/try-it-out) explains sandbox merchant accounts/currency selection and cautions that availability varies by region. This is the clearest documented alternative to investigate, but Egyptian individual eligibility and the desired currency must still be confirmed. Do not choose a false business country. It requires a separate provider implementation and revised acceptance criteria; it cannot pass the existing Paymob tasks.
- **PayPal sandbox:** [official documentation](https://developer.paypal.com/sandbox-testing/accounts) supplies virtual buyer and seller accounts with test credentials. However, its [current API onboarding guide](https://developer.paypal.com/api/get-started/) says a Business account is needed to test integrations outside the US. It is not a verified registration-free shortcut for this Egyptian project. The [REST currency list](https://developer.paypal.com/api/codes/currency) also needs to be checked for the chosen checkout product; do not equate foreign-exchange presentment support with EGP checkout settlement.
- **Stripe:** [official country availability](https://stripe.com/global) does not list Egypt for payments. That list does not by itself decide sandbox eligibility, but it is insufficient to recommend Egyptian signup as a guaranteed solution. Do not misrepresent residence or form a foreign company just for this portfolio.

Recommendation: request Paymob developer-only access first, preserving EGP and the existing implementation. If refused, consider a separately approved Braintree sandbox path after confirming regional eligibility. Until then, deliver the verified cash/POS/reporting system with online payment disabled and genuine external payment acceptance explicitly pending. Provider-hosted demos or local cryptographic diagnostics must not be presented as end-to-end provider acceptance.
