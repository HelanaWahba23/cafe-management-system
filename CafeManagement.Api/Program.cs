using CafeManagement.Api.Infrastructure.Persistence;
using CafeManagement.Api.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// One registration supplies a singleton factory/options and HTTP-scoped contexts for Identity.
// Missing local connection configuration is supplied before database operations in later tasks.
builder.Services.AddDbContextFactory<CafeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CafeDatabase")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<CafeDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Operator-only command: never run provisioning during ordinary web startup.
if (args.Contains("--bootstrap-user", StringComparer.Ordinal))
{
    try
    {
        await using var scope = app.Services.CreateAsyncScope();
        var options = builder.Configuration.GetSection(BootstrapOptions.SectionName)
            .Get<BootstrapOptions>() ?? new BootstrapOptions();
        var result = await IdentityBootstrapper.RunAsync(scope.ServiceProvider, options);
        Console.WriteLine(result.Succeeded ? "Bootstrap completed." : "Bootstrap failed; check local configuration and account role.");
        Environment.ExitCode = result.Succeeded ? 0 : 1;
    }
    catch
    {
        // Do not disclose configuration, credentials or raw database exceptions.
        Console.Error.WriteLine("Bootstrap failed; verify the dedicated database and local configuration.");
        Environment.ExitCode = 1;
    }

    await app.DisposeAsync();
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
