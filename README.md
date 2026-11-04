# Café Management System

A web app for managing a café or small restaurant, built with ASP.NET Core 8, Blazor, and SQL Server.

Customers can browse the menu, place delivery or pickup orders, and order from their table using a QR code. Staff can manage orders and cash receipts, while admins handle the menu, tables, and reports.

## Features

- **Menu and cart:** Browse dishes by category, view prices in EGP, and adjust quantities before checkout.
- **Customer accounts:** Save an address, update a profile, and view personal order history.
- **Guest checkout:** Place an order without an account and follow its progress through a private tracking link.
- **Table orders:** Open the menu from a table's QR code and place a dine-in order.
- **Cashier workspace:** Process orders, update their status, and record cash receipts.
- **Admin tools:** Manage the catalog, tables, staff accounts, and reports.

Payments are currently cash only. Order progress is updated by staff. Online payments, delivery service integrations, GPS tracking, and email sending are not configured.

## Tech stack

- ASP.NET Core 8 and Blazor
- Entity Framework Core with SQL Server
- ASP.NET Core Identity for accounts and roles
- QRCoder for table QR codes

## Run locally

### Requirements

- Windows
- .NET 8 SDK
- SQL Server Express LocalDB, or an installed SQL Server instance

You can also use Visual Studio 2022 with the ASP.NET and web development workload and LocalDB installed.

### Setup

Clone the repository and run the setup script:

```powershell
git clone https://github.com/HelanaWahba23/cafe-management-system.git
cd cafe-management-system
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\setup-local.ps1
```

The script restores and builds the project, creates a new database, applies migrations, and adds 27 demo dishes across six categories. It also creates an admin account with a randomly generated password and sets up the HTTPS development certificate.

Save the admin credentials printed at the end of setup. You do not need an existing database or the original developer's credentials.

Start the app:

```powershell
dotnet run --project CafeManagement.Api --launch-profile https
```

Open [https://localhost:7128/](https://localhost:7128/).

If you prefer Visual Studio, open `CafeManagement.sln`, set `CafeManagement.Api` as the startup project, select the **https** profile, and press **F5**. Stop any running terminal instance first so only one server is using the port.

### Database options

Setup requires a new database. If the default name is already in use, choose another:

```powershell
.\scripts\setup-local.ps1 -DatabaseName CafeManagementDemo2
```

To use SQL Server Express instead of LocalDB:

```powershell
.\scripts\setup-local.ps1 -Server '.\SQLEXPRESS' -DatabaseName CafeManagementDemo
```

These commands use Windows authentication. See the [setup guide](RUN-RESTAURANT.md) for manual setup, SQL authentication, and troubleshooting.

## Using the app

Once the app is running, these pages are a good place to start:

| Page | What you can do |
| --- | --- |
| `/menu` | Browse the demo menu and place a delivery or pickup order. |
| `/customer/register` | Create a customer account. |
| `/account/login` | Sign in with the staff credentials created during setup. |
| `/admin/tables` | Create tables and open their QR menu links. |
| `/pos/orders` | Manage orders and cash receipts. |

Admins can create a separate cashier account by following the setup guide.

## Local configuration

Store machine-specific settings in `CafeManagement.Api/appsettings.Local.json` or .NET User Secrets. The local JSON file is ignored by Git and excluded from build and publish output.

The app reads this file only in Development, when a database connection has not already been configured. Environment variables and User Secrets take precedence over the local fallback.

Keep passwords, database backups, and customer data out of the repository.

## Build checks

To build the project and check that the Entity Framework model matches its migrations:

```powershell
dotnet tool restore
dotnet restore CafeManagement.sln
dotnet build CafeManagement.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project CafeManagement.Api --configuration Release --no-build
```

GitHub Actions runs the build, checks migration consistency, and publishes the app as part of the workflow.

## Further reading

- [Setup and troubleshooting](RUN-RESTAURANT.md)
- [Deployment](DEPLOYMENT.md)
- [Fresh installation validation](specs/007-restaurant-customer-site/publication-validation.md)
- [Feature validation](specs/007-restaurant-customer-site/validation.md)
- [Implementation notes](IMPLEMENTATION-HANDOFF.md)

The validation documents record checks from their original environment. A new installation still needs its own database setup. Earlier Paymob work is kept in `archive/` for reference and is excluded from compilation.
