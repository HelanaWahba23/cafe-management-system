using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CafeManagement.Api.Application;

public enum StaffAccessResult
{
    Allowed,
    Unauthenticated,
    Forbidden
}

public sealed class StaffAccess(IServiceScopeFactory scopeFactory)
{
    // Callers supply the server's HTTP/circuit principal, never a deserialized request principal.
    public Task<StaffAccessResult> CheckAdminAsync(ClaimsPrincipal principal) => CheckAsync(principal, "Admin");

    public Task<StaffAccessResult> CheckCashierAsync(ClaimsPrincipal principal) => CheckAsync(principal, "Cashier");

    private async Task<StaffAccessResult> CheckAsync(ClaimsPrincipal principal, string role)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return StaffAccessResult.Unauthenticated;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;
        var user = await users.GetUserAsync(principal);
        var claimedStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
        if (user is null || !users.SupportsUserSecurityStamp || string.IsNullOrEmpty(claimedStamp)
            || !string.Equals(claimedStamp, await users.GetSecurityStampAsync(user), StringComparison.Ordinal)
            || !await users.IsInRoleAsync(user, role))
        {
            return StaffAccessResult.Forbidden;
        }

        return StaffAccessResult.Allowed;
    }
}
