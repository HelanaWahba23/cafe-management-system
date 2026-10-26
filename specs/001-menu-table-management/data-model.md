# Data Model: Menu and Table Management

Design only. No entity code, migrations, database, or seed accounts were created.

## Domain entities

| Entity | Fields | Relationships and constraints |
| --- | --- | --- |
| MenuCategory | Id int identity PK; Name nvarchar(120) required | One category has zero or more products; name duplicates allowed. |
| Product | Id int identity PK; CategoryId int required; Name nvarchar(120) required; Price decimal(18,2) required; IsAvailable bit required | CategoryId FK -> MenuCategory.Id, index on FK, delete restricted; product name duplicates allowed; CHECK Price > 0. |
| CafeTable | Id int identity PK; Label nvarchar(50) required; NormalizedLabel nvarchar(50) required; PublicCode uniqueidentifier required | Unique index on NormalizedLabel (binary comparison) and unique index on PublicCode; no product/menu ownership relationship. |

The same cafe menu is shared by tables. Table context is a link, not a separate menu copy.
Currency is the fixed application constant EGP, not an editable multi-currency table/field.
QR images are derived from immutable PublicCode and trusted PublicWebBaseUrl; never stored.

## Identity

Use standard IdentityUser, IdentityRole and EF Identity join/claim/token tables in the same
CafeDbContext. Roles: Admin and Cashier. Admin is required for every feature management read
and write. Cashier can authenticate and inspect its session but cannot access admin data.
There is no customer account entity for anonymous browsing and no public role/account creation.
Bootstrap creates only missing roles and the explicitly supplied account, with a hashed password
through UserManager; never reassign roles or reset existing passwords silently on every startup.
This is an operator-only one-time procedure, not a public endpoint.

## Validation and normalization

1. Trim names/labels, reject whitespace-only input with string.IsNullOrWhiteSpace semantics,
   enforce name length 1–120 and label length 1–50 after trimming. SQL columns are NOT NULL
   with nonempty-after-space-trim checks; full Unicode whitespace validation remains in the
   application. No unique index on category/product names.
2. Set NormalizedLabel from trimmed Label.ToUpperInvariant(); reject normalized length over 50.
   Explicit binary column/index comparison makes the stored normalized key authoritative;
   A1 and a1 conflict by this proposed engineering normalization. Retain display casing.
3. Check uniqueness before save for feedback, then rely on the unique index for race safety.
   Map duplicate-label database violations to 409, not an unhandled error. PublicCode is a
   server-generated random GUID unique per table; a collision is retried, never reused.
4. Preserve price as an invariant decimal string at the HTTP API boundary. Blazor forms and
   application services use C# decimal without an internal HTTP roundtrip. API input must be
   positive, contain no exponent/group separators/sign, have at most two
   fractional digits, and fit decimal(18,2): at most 9999999999999999.99. Normalize outputs to
   exactly two places (25 -> "25.00"). No silent rounding of excessive precision.
5. Require an existing CategoryId on product create/edit; FK prevents orphan products even
   under conflicting requests. Validate name, price and association before any write.
6. Availability must be supplied explicitly on product create/edit; no invented default.
   No deletion/deactivation fields or routes. IsAvailable changes public visibility only.

Length limits, case normalization, price transport, and numeric range are documented engineering
assumptions for review. They do not add a product feature or change the approved positive-price,
nonblank-name and unique-table business rules.

## State and consistency

- Category: create -> view/edit; no deletion or deactivation.
- Product: create -> view/edit; available <-> unavailable through edit. Admin sees both states.
- Table: create -> view/edit; PublicCode remains immutable and Label remains unique on edit.
- Login session: signed out -> HTTP Identity login -> full navigation/new authenticated circuit
  -> HTTP logout -> full navigation/new anonymous state. No registration journey.
- EF SaveChanges atomically persists one management change. Create/dispose a context per service
  operation using IDbContextFactory<CafeDbContext>; never keep contexts/tracked entities in the
  component or circuit. No external service transaction.
- Last successful write wins for concurrent edits; constraint conflicts still return 409.
- Public reads project only available products on the server. Admin reads include all products.
- No order, bill, payment, stock, reporting, soft-delete, image, or reservation entities.

## Blazor service and Identity lifetimes

The existing Web SDK project hosts both the API and Blazor Web App. Both call the same domain
services and use the same database/model; components do not call back into their own HTTP API.
Controllers supply HTTP User; components obtain the trusted circuit principal from
AuthenticationStateProvider. All management service reads/writes check Admin access on the server,
including a current-user/security-stamp/role check in a short Identity scope. No browser-supplied
role or public table code grants management access.

Register the factory/context options consistently so Identity HTTP forms can use scoped EF stores
while domain operations use separately disposed factory-created contexts. Do not retain
UserManager, SignInManager or a scoped Identity context in an interactive component; circuit
revalidation creates short scopes. Identity cookie login/logout runs only in normal HTTP pages.
Prerendered reads are side-effect-free; initialization and reconnect never create/update records.

## Migration and recovery approach

Future initial migration adds Identity and these three tables to a new dedicated database.
Review generated SQL and database name before applying; do not use EnsureCreated beside migrations
or automatically migrate on ordinary production startup. Review Down migration for rollback;
rollback/drop is permitted only for a verified disposable development database or after explicit
approval and backup of data that must be preserved. Future order-related migrations must retain
historical values, but no such tables or changes are planned here.
