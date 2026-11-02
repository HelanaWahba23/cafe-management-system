# Contracts

Customer Identity Razor POST registration/login/logout requires antiforgery. /customer/profile and /customer/orders are authenticated; service checks current database identity/security stamp and rejects staff as customer. /api/customer/profile GET/PUT and /api/customer/orders GET/{reference} are cookie-authenticated owner-only and no-store. Order creation retains private capability and CSRF; authenticated owner is resolved server-side, never from JSON.

OrderDraft adds DeliveryAddress only for Delivery. Address is normalized/bounded and part of delivery intent hash. Delivery requires explicit UI address confirmation. State names preserve Submitted(Pending UI)/Completed(Delivered UI for delivery), add Accepted/OutForDelivery. No online payment routes.
