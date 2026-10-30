# Plan: Café frontend refresh

Existing ASP.NET Core8/Blazor/Bootstrap application, English Café brand. Frontend-only user-approved scope; no new packages/data model/auth changes. Preserve previous164 accepted active tasks.

1. Retain superseded account-expansion draft in archive. Update specification/checklist/tasks; no unresolved scope question.
2. Add public Home.razor at / with photographic hero, order CTA, compact highlights and an about section. Remove the root→pickup redirect so root is a real café homepage. Set all launch profiles to empty launchUrl (root).
3. Replace only PublicLayout header/footer with accessible Home/Menu/About links and an existing staff access link; keep menu routes/cart-navigation guards and staff layouts.
4. Append public-scoped CSS; light cream, forest-green accents, dark coffee hero, readable serif display and restrained spacing. Use one locally copied generated photo; no remote fonts or photo requests. Existing Bootstrap/reconnect behavior remains.
5. Build in Release and verify on an owned loopback HTTPS host with existing configuration: root/rendered hero, image/menu routes, working CTA/cart, widths390/768/1280, no horizontal overflow, keyboard links and existing no-store/auth checks. Inspect screenshots and iterate only for confirmed defects.
6. Reuse existing successful database/order/staff evidence; no schema operation is needed. Stop only owned temporary processes. Deliver current main host/page for user review; review source/assets/secrets, commit/push validated changes and record exact outcome.

Constitution: unchanged principles; confirmed narrowed scope adds presentation only. New account/address requests are not implemented. App migrations and all existing records remain untouched.
