# Remaining manual browser validation — 2026-10-29

Current counts: **57/63**. Detailed completed browser/backend evidence is in
[end-to-end-validation.md](end-to-end-validation.md). Do not repeat completed checks merely
because they appeared in the earlier handoff.

## Native navigation: T021–T023 and T046

The verified manual host stays running at:
https://localhost:7268/menu/t/691d9592-3b19-4466-ad62-314b0334af17

1. Add one item. Reload and choose Stay/Cancel: warning appears, item and correct table remain.
2. Reload and choose Leave/Reload: item clears and the correct table label remains.
3. After the prepared pickup-link fix is deployed to this manual host, verify leaving a
   nonempty table cart for pickup warns: Stay retains table/cart; Leave shows Pickup and
   clears the old cart with no hidden table association. Do not restart the host mid-check.

The supported API cannot accept native before-unload dialogs. Retaining the cart after
a blocked navigation attempt is not treated as proof of native Leave acceptance. The old
pickup link bypassed the guard through enhanced navigation; the local fix opts that link out.
Menu/layout files carrying these guards remain uncommitted until the required check passes.

## Physical phone: T059

An approved trusted HTTPS origin reachable from an actual phone is required for QR entry,
rescan, direct pickup and private-link journeys. Current hosts listen only on loopback.
No approved external origin or physical-phone result exists. Do not change firewall,
certificates, network access or claim desktop responsive emulation satisfies this check.

## Final acceptance: T063

Leave pending until the above results and all integration commits are complete. Feature 001's
separate T051/T056 gaps remain unchanged. T022 registration/tab-isolation and T023 regression
have supporting evidence but retain the T021 prerequisite gate.
