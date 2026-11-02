# Setup and run Café

## Requirements

- Windows with the .NET 8 SDK (not only the runtime).
- SQL Server Express LocalDB, SQL Server Express, or SQL Server Developer. The quick setup defaults to `(localdb)\MSSQLLocalDB` and Windows authentication.
- Optional: Visual Studio 2022 with ASP.NET/web development support for .NET 8.

There is no bundled production database, shared admin password, SQL backup, or original-developer User Secrets. Each installation creates its own accounts and table QR codes.

## Recommended: fresh local installation

From the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\setup-local.ps1
```

The command targets a new `CafeManagementDemo` database, restores .NET tools/packages, builds, trusts the HTTPS developer certificate (Windows may ask you to approve this), applies migrations, installs 27 menu items and creates an Admin. It prints a random password and saves it in private local configuration. No fixed password is shipped. To skip certificate trust during automated validation, add `-SkipCertificateTrust` and configure your browser certificate separately.

If LocalDB is absent, install it or select an existing SQL Server instance:

```powershell
.\scripts\setup-local.ps1 -Server '.\SQLEXPRESS' -DatabaseName CafeManagementDemo -AdminEmail 'owner@example.test'
```

The setup script refuses to overwrite existing local settings or an existing database. For a separate installation, use a fresh checkout and a new database name. It does not reset passwords or upgrade existing installations. If setup fails after creating the database, use the manual steps below to finish that selected installation; do not repeatedly recreate accounts or delete a database containing useful data.

## Run the web app

```powershell
dotnet run --project CafeManagement.Api --launch-profile https
```

Open https://localhost:7128/. Or open `CafeManagement.sln` in Visual Studio, choose the API project and **https**, then press F5. Use one running server. A port-in-use error means another instance already owns 7128/5004; stop it in its terminal or debugger first.

The setup script saves `CafeManagement.Api/appsettings.Local.json`. It is ignored by Git and excluded from build/publish output. Development startup reads it only when `ConnectionStrings:CafeDatabase` is missing from normal configuration. An existing environment variable or User Secret takes precedence; remove an obsolete override or choose the intended database explicitly if working with multiple checkouts. Bootstrap values are used only by the explicit bootstrap command, never during web startup.

## Manual setup (including SQL authentication)

1. Copy `CafeManagement.Api/appsettings.Local.example.json` to `CafeManagement.Api/appsettings.Local.json`. Set `ConnectionStrings:CafeDatabase` to your own dedicated database. For SQL authentication, supply your own login/password privately in that connection; never commit it. Use an account allowed to create the new development database. Keep `PublicWebBaseUrl` at `https://localhost:7128` locally.
2. Alternatively configure the same keys in User Secrets using Visual Studio **Manage User Secrets**. Never paste credentials into committed files, issues, or pull requests.
3. From the repository root:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:DOTNET_ENVIRONMENT = 'Development'
dotnet tool restore
dotnet restore CafeManagement.sln
dotnet build CafeManagement.sln
dotnet dev-certs https --trust
dotnet ef database update --project CafeManagement.Api --no-build
dotnet run --project CafeManagement.Api --no-build --no-launch-profile -- --seed-demo-menu
```

`--seed-demo-menu` works only in Development and skips databases that already contain menu categories. It never replaces an existing menu. The optional SQL scripts are for explicitly selected development databases; `restaurant-menu.sql` can change category visibility, so use the safe CLI seed for a new installation.

4. Add a private `Bootstrap` object alongside `ConnectionStrings` in your local JSON (or User Secrets): `Email`, `Password` (your own strong password), and `Role` (`Admin` or `Cashier`). Then run:

```powershell
dotnet run --project CafeManagement.Api --no-build --no-launch-profile -- --bootstrap-user
```

The bootstrap command creates the requested account and staff roles. It never resets an existing password or promotes an existing account with a different role. To create a separate Cashier, change the private Bootstrap values to a different email, password, and `Cashier`, then run it again. Remove Bootstrap values from private configuration when no longer needed, after saving your credentials privately.

## Workflows

- Customers sign up at `/customer/register`, then edit their profile/address and view their own orders.
- Browse `/menu`, add dishes, change quantities and choose delivery or pickup at checkout. Delivery requires an address and confirmation; payment is cash.
- Staff sign in at `/account/login`. Admin manages menu categories/products, tables/QR codes, and reports.
- For dine-in, create a table in `/admin/tables` and open that table's QR menu link. Checkout retains the table context.
- Cashier receives orders at `/pos/orders` and updates Accepted → Preparing → Ready. Delivery continues through OutForDelivery → Completed; pickup/table orders go to Completed. Cash receipt recording is explicit.
- Guests should retain their private tracking links. Customer accounts do not claim earlier guest orders by matching a phone number or name.

## Troubleshooting and existing installations

- **Unable to load the menu:** verify the selected database connection, SQL Server service, and migrations. Visual Studio Output → Debug includes the server error; do not post credentials with logs.
- **ConnectionString has not been initialized:** create the private local JSON or configure User Secrets. An original developer's machine settings are intentionally absent from GitHub.
- **Menu is empty:** run `--seed-demo-menu` on a migrated empty development database, or add your own categories/products through Admin.
- **Database already exists / local settings already exist:** the one-time setup preserves them. Start the app or finish the manual setup against the intended database.
- **Address already in use:** run only one instance, through Visual Studio or the terminal.
- **HTTPS certificate:** run `dotnet dev-certs https --trust` and restart the browser.

For an existing installation, back up and verify the intended database before applying new migrations. Do not run first-install setup or menu SQL against unrelated data. Ordinary startup does not automatically migrate, seed, or provision accounts.

Production requires a separately configured database connection and HTTPS origin via deployment configuration. Local JSON is not published or loaded in Production. Hosting, email delivery, online payments, courier integration and password recovery are not configured by this repository.
