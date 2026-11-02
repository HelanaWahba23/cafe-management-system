[CmdletBinding()]
param(
    [string]$Server = '(localdb)\MSSQLLocalDB',
    [string]$DatabaseName = 'CafeManagementDemo',
    [string]$AdminEmail = 'admin@example.test',
    [switch]$SkipCertificateTrust
)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectRoot 'CafeManagement.Api/CafeManagement.Api.csproj'
$localSettings = Join-Path $projectRoot 'CafeManagement.Api/appsettings.Local.json'

function Invoke-DotNet {
    param([string[]]$Arguments)
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) { throw "dotnet $($Arguments[0]) failed. Fix the reported error before continuing." }
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'Install the .NET 8 SDK first. See README.md.'
}
if (Test-Path -LiteralPath $localSettings) {
    throw 'This checkout already has appsettings.Local.json. Existing settings were preserved. Follow RUN-RESTAURANT.md to run or maintain this installation.'
}
if ($DatabaseName -notmatch '^[A-Za-z][A-Za-z0-9_]{0,100}$') {
    throw 'DatabaseName must start with a letter and contain only letters, digits, or underscores.'
}
if ($AdminEmail -notmatch '^[^\s@]+@[^\s@]+\.[^\s@]+$') {
    throw 'Provide a valid AdminEmail.'
}

# Integrated authentication only; no SQL credentials or shared admin password are shipped.
$connectionBuilder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$connectionBuilder['Data Source'] = $Server
$connectionBuilder['Initial Catalog'] = $DatabaseName
$connectionBuilder['Integrated Security'] = $true
$connectionBuilder['TrustServerCertificate'] = $true
$connectionBuilder['MultipleActiveResultSets'] = $true
$connectionString = $connectionBuilder.ConnectionString

# Setup is for a fresh clone, never an upgrade of an existing operator database.
$connectionBuilder['Initial Catalog'] = 'master'
$probe = New-Object System.Data.SqlClient.SqlConnection($connectionBuilder.ConnectionString)
try {
    $probe.Open()
    $command = $probe.CreateCommand()
    $command.CommandText = 'SELECT DB_ID(@name)'
    [void]$command.Parameters.AddWithValue('@name', $DatabaseName)
    if ($command.ExecuteScalar() -isnot [DBNull]) {
        throw 'The database already exists. Choose a new -DatabaseName. Existing data and accounts were preserved.'
    }
}
finally { $probe.Dispose() }

$password = 'Aa1!' + [Guid]::NewGuid().ToString('N')
$settings = @{
    ConnectionStrings = @{ CafeDatabase = $connectionString }
    PublicWebBaseUrl = 'https://localhost:7128'
    Bootstrap = @{ Email = $AdminEmail; Password = $password; Role = 'Admin' }
}

$environmentNames = @('ASPNETCORE_ENVIRONMENT', 'DOTNET_ENVIRONMENT',
    'ConnectionStrings__CafeDatabase', 'Bootstrap__Email', 'Bootstrap__Password', 'Bootstrap__Role')
$previousEnvironment = @{}
foreach ($name in $environmentNames) { $previousEnvironment[$name] = [Environment]::GetEnvironmentVariable($name, 'Process') }

Push-Location $projectRoot
try {
    # Force these child commands to target the chosen demo database even if User Secrets exist.
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $env:DOTNET_ENVIRONMENT = 'Development'
    $env:ConnectionStrings__CafeDatabase = $connectionString
    $env:Bootstrap__Email = $AdminEmail
    $env:Bootstrap__Password = $password
    $env:Bootstrap__Role = 'Admin'

    Invoke-DotNet -Arguments @('tool', 'restore')
    Invoke-DotNet -Arguments @('restore', 'CafeManagement.sln')
    Invoke-DotNet -Arguments @('build', $projectFile, '--no-restore')
    if (-not $SkipCertificateTrust) { Invoke-DotNet -Arguments @('dev-certs', 'https', '--trust') }

    Write-Host "Applying migrations to $Server / $DatabaseName. Use a new, dedicated demo database."
    Invoke-DotNet -Arguments @('ef', 'database', 'update', '--project', $projectFile, '--no-build')
    Invoke-DotNet -Arguments @('run', '--project', $projectFile, '--no-build', '--no-launch-profile', '--', '--seed-demo-menu')
    Invoke-DotNet -Arguments @('run', '--project', $projectFile, '--no-build', '--no-launch-profile', '--', '--bootstrap-user')

    # Persist only after setup succeeds; this file is ignored and excluded from publishing.
    $settings | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $localSettings -Encoding UTF8
    Write-Host ''
    Write-Host 'Setup complete. Save these private local admin credentials:'
    Write-Host "Email: $AdminEmail"
    Write-Host "Password: $password"
    Write-Host 'Staff login: https://localhost:7128/account/login'
    Write-Host 'Run: dotnet run --project CafeManagement.Api --launch-profile https'
    Write-Host 'Or open CafeManagement.sln in Visual Studio and start the https profile.'
}
finally {
    foreach ($name in $environmentNames) { [Environment]::SetEnvironmentVariable($name, $previousEnvironment[$name], 'Process') }
    Pop-Location
}
