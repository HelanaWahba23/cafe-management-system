# Research decisions

Reuse ASP.NET Core8 Identity password/cookie and antiforgery rather than adding an external auth provider. Profile is a restrictive Identity-linked table; services validate current stored security stamp and reject staff as customer. Account history uses authenticated server owner ID, never contact matching. Guest private capabilities remain valid. No email service is configured or falsely simulated.

Delivery extends existing authoritative order service/transaction, not separate checkout. Normalize and validate address, include it in delivery intent hash and immutable snapshot; preserve original non-delivery hash format. Add Accepted/OutForDelivery states while retaining legacy Submitted→Preparing edge for compatibility; UI uses explicit acceptance. Delivery-only transition is enforced by lifecycle. Cash receipt remains unchanged and explicit.

Keep old menu rows/prices and QR codes. Add public category visibility and curated portfolio menu rather than overwrite diagnostic data. Image is built-in image_gen decorative restaurant food concept, served locally; not claimed actual business photography. No real address/hours/reviews fabricated. Full prompt/asset:generic food editorial burger/pizza/salad/lemonade on right,dark left negative space,warm forest-green/cream restaurant,no text/logos/people; assetwwwroot/images/restaurant-hero.png.
