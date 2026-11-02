# Quickstart

Follow ../../RUN-RESTAURANT.md. Existing local original database is already backed up/migrated and menu installed. Run https profile; root→restaurant homepage, /menu→catalog/cart, /customer/register or /customer/login→accounts, /customer/profile→saved details/address, /customer/orders→owner history, /menu/t/{existingCode}→table context, /account/login→staff.

Verification is recorded in validation.md and browser-result.json. Do not rerun old fixture cleanup scripts or reset any database. No Paymob/account/online payment or public tunnel needed. Fresh environments require .NET8, chosen SQL connection in private User Secrets, reviewed migrations and optional scripts/restaurant-menu.sql. Existing staff bootstrap remains operator-only.
