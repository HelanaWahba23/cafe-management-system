using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CafeManagement.Api.Infrastructure.Identity;

public sealed class IdentityRevalidatingAuthenticationStateProvider(
    ILoggerFactory loggerFactory,
    IServiceScopeFactory scopeFactory,
    IOptions<IdentityOptions> options)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    // .NET 8 Identity template interval. Protected operations still check access each time.
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var scope = scopeFactory.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await users.GetUserAsync(authenticationState.User);
        if (user is null || !users.SupportsUserSecurityStamp)
        {
            return false;
        }

        var claimedStamp = authenticationState.User.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
        return !string.IsNullOrEmpty(claimedStamp)
            && string.Equals(claimedStamp, await users.GetSecurityStampAsync(user), StringComparison.Ordinal);
    }
}
