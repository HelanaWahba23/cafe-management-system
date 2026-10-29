# Remaining manual acceptance — 2026-10-29

Current Feature002 count: **61/63**. Completed functional, responsive, security and preservation evidence is in [end-to-end-validation.md](end-to-end-validation.md).

## Native navigation resolved: T021, T023, T046

All four actual beforeunload cases passed using standard Playwright1.64.1 with a fresh isolated Edge156.0.4314.50 profile, sandbox and normal TLS validation. Reload Stay and pickup Stay retained cart/table; reload Leave cleared cart and retained table; pickup Leave opened an empty pickup menu without table. No order/payment was submitted, existing profile accessed or security control bypassed. Do not request these checks again merely because an older tool could only dismiss the dialog.

## Physical phone: T059

An approved trusted HTTPS origin reachable from an actual phone is required for QR entry, rescan, direct pickup and private-link journeys. Current hosts listen only on loopback. No approved external origin or physical-phone result exists. Desktop responsive emulation does not satisfy this check. Do not change firewall, certificates or network access without authorization.

## Final acceptance: T063

Leave pending until actual-phone requirements pass and the acceptance matrix is reconciled. Feature001's separate T051/T056 gaps remain unchanged. All non-phone Feature002 navigation gates are accepted.

The verified current local build remains at https://localhost:7128/menu/t/691d9592-3b19-4466-ad62-314b0334af17. The older requested7268 manual host is also retained; use7128 for current theme/POS/cash/reports.
