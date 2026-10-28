using CafeManagement.Api.Infrastructure.Persistence;
using CafeManagement.Api.Infrastructure.Identity;
using CafeManagement.Api.Application;
using CafeManagement.Api.Components;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;
using System.Threading.RateLimiting;

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
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.InvalidModelStateResponseFactory = ApiErrors.InvalidModelState);

var guestOrderPermitLimit = builder.Configuration.GetValue("GuestOrders:PermitLimit", 60);
if (guestOrderPermitLimit <= 0)
    throw new InvalidOperationException("GuestOrders:PermitLimit must be positive.");
builder.Services.AddRateLimiter(options =>
{
    // Opt-in only: applied to the new guest order endpoints when those endpoints are added.
    options.AddPolicy("GuestOrders", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = guestOrderPermitLimit,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    options.OnRejected = async (context, cancellationToken) =>
    {
        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var delay)
            ? Math.Max(1, (int)Math.Ceiling(delay.TotalSeconds)) : 60;
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString(CultureInfo.InvariantCulture);
        await context.HttpContext.Response.WriteAsJsonAsync(ApiErrors.ForStatus(429),
            options: (System.Text.Json.JsonSerializerOptions?)null,
            contentType: "application/problem+json", cancellationToken: cancellationToken);
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<StaffAccess>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<TableService>();
builder.Services.AddScoped<TableQrService>();
builder.Services.AddScoped<PublicMenuService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CartState>();
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
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var documentPath = path.Value?.TrimEnd('/');
    var privateDocument = path.StartsWithSegments("/menu/t")
        || string.Equals(documentPath, "/menu/pickup", StringComparison.OrdinalIgnoreCase)
        || string.Equals(documentPath, "/orders/track", StringComparison.OrdinalIgnoreCase);
    if (privateDocument || path.StartsWithSegments("/api/orders"))
    {
        // OnStarting also preserves these headers when downstream error handling clears a response.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.CacheControl = "no-store";
            if (privateDocument) context.Response.Headers["Referrer-Policy"] = "no-referrer";
            return Task.CompletedTask;
        });
    }
    await next(context);
});
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), api =>
{
    api.Use(async (context, next) =>
    {
        try
        {
            await next(context);
        }
        catch (Exception) when (!context.Response.HasStarted && !context.RequestAborted.IsCancellationRequested)
        {
            // Log a correlation ID without leaking exception/configuration text.
            app.Logger.LogError("API request failed. Trace identifier: {TraceIdentifier}", context.TraceIdentifier);
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(ApiErrors.ForStatus(500),
                options: (System.Text.Json.JsonSerializerOptions?)null,
                contentType: "application/problem+json", cancellationToken: context.RequestAborted);
        }
    });
    api.UseStatusCodePages(async (Microsoft.AspNetCore.Diagnostics.StatusCodeContext statusContext) =>
    {
        var response = statusContext.HttpContext.Response;
        await response.WriteAsJsonAsync(ApiErrors.ForStatus(response.StatusCode),
            options: (System.Text.Json.JsonSerializerOptions?)null,
            contentType: "application/problem+json", cancellationToken: statusContext.HttpContext.RequestAborted);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorPages();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
