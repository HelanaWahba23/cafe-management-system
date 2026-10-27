using CafeManagement.Api.Infrastructure.Persistence;
using CafeManagement.Api.Infrastructure.Identity;
using CafeManagement.Api.Application;
using CafeManagement.Api.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<StaffAccess>();
builder.Services.AddOptions<PublicMenuOptions>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<BootstrapOptions>()
    .Bind(builder.Configuration.GetSection(BootstrapOptions.SectionName));
builder.Services.AddAuthorization(options =>
    options.AddPolicy("Admin", policy => policy.RequireAuthenticatedUser().RequireRole("Admin")));
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        else
            context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        else
            context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

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
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorPages();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
