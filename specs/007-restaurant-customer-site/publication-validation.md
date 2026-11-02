# Fresh installation and publication validation — 2026-11-02

The current source was exported to a separate checkout directory without private local configuration or build output. A distinct User Secrets ID simulated a new developer machine with no existing secrets. The first-install PowerShell script ran against a newly named database on a local SQL Server instance with Windows authentication, using `-SkipCertificateTrust` because the local HTTPS certificate was already configured.

Verified:

- Package/tool restore, build, and all migrations completed from the exported source.
- First-install script generated a new private admin password and local configuration.
- Database contained 6 menu categories, 27 products, 1 user, and 1 Admin assignment.
- HTTPS `/menu` returned 200 with product/add-to-cart markup and no load error.
- The generated admin signed in through the real antiforgery-protected staff login and reached `/admin/dashboard`.
- Repeated menu seeding skipped the existing catalog; repeated setup rejected existing private settings.
- Release build completed with zero warnings/errors; EF reported no pending model changes.
- Release publish included the application and excluded `appsettings.Local.json`.
- Git candidate files and repository history were checked for the local saved passwords and common private-key/token patterns; no matches were found. Database backups and private local configuration are excluded.

The SQL Server instance path was tested. The LocalDB default and Windows certificate trust prompt require the documented prerequisites and were not reinstalled during validation. Production hosting and external services are outside this local setup validation. Earlier broader workflow checks remain recorded in `validation.md`.
