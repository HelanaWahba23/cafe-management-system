using System.ComponentModel.DataAnnotations;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace CafeManagement.Api.Infrastructure.Identity;

public static class IdentityBootstrapper
{
    public static async Task<IdentityResult> RunAsync(IServiceProvider scopedServices, BootstrapOptions options)
    {
        if (!Validator.TryValidateObject(options, new ValidationContext(options), null, true)
            || options.Role is not ("Admin" or "Cashier"))
        {
            return Failure("InvalidConfiguration");
        }

        var users = scopedServices.GetRequiredService<UserManager<IdentityUser>>();
        var roles = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
        var database = scopedServices.GetRequiredService<CafeDbContext>();
        var existingUser = await users.FindByEmailAsync(options.Email);

        // An existing account is never promoted or given a new password by this command.
        if (existingUser is not null && !await users.IsInRoleAsync(existingUser, options.Role))
        {
            return Failure("ExistingAccountRoleMismatch");
        }

        await using var transaction = await database.Database.BeginTransactionAsync();
        foreach (var role in new[] { "Admin", "Cashier" })
        {
            if (!await roles.RoleExistsAsync(role))
            {
                var result = await roles.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    return Failure("RoleCreationFailed");
                }
            }
        }

        if (existingUser is null)
        {
            var user = new IdentityUser { UserName = options.Email, Email = options.Email };
            var created = await users.CreateAsync(user, options.Password);
            if (!created.Succeeded)
            {
                return Failure("AccountCreationFailed");
            }

            var assigned = await users.AddToRoleAsync(user, options.Role);
            if (!assigned.Succeeded)
            {
                return Failure("RoleAssignmentFailed");
            }
        }

        await transaction.CommitAsync();
        return IdentityResult.Success;
    }

    private static IdentityResult Failure(string code) => IdentityResult.Failed(new IdentityError
    {
        Code = code,
        Description = "Bootstrap did not complete. Check local configuration and the existing account's role."
    });
}
