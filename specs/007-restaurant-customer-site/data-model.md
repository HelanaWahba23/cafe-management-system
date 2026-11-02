# Data model

CustomerProfile: UserId PK/restrict IdentityUser FK; DisplayName120, Phone40, Address500, concurrency token. Order gains nullable CustomerUserId restrictive Identity FK/index, DeliveryAddress500. Existing Orders and capability/intent hashes untouched; new Delivery snapshots include address. Category IsVisible bool defaults true. Check constraints expand supported modes/states/address consistency; retain all historical migrations.
