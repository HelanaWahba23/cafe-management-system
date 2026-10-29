using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CafeManagement.Api.Pages.Account;

[AllowAnonymous]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class LoginModel(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public bool AccessDenied { get; private set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true && !User.IsInRole("Admin") && !User.IsInRole("Cashier"))
        {
            AccessDenied = true;
            Response.StatusCode = StatusCodes.Status403Forbidden;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var result = await signIn.PasswordSignInAsync(Input.Email, Input.Password,
                isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var user = await users.FindByNameAsync(Input.Email);
                if (user is not null && await users.IsInRoleAsync(user, "Admin"))
                {
                    return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/admin/dashboard");
                }

                if (user is not null && await users.IsInRoleAsync(user, "Cashier"))
                    return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/pos/orders");

                return RedirectToPage("/Account/Login");
            }
        }

        ModelState.AddModelError(string.Empty, "Unable to sign in. Check your credentials or try again later.");
        return Page();
    }

    public sealed class LoginInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
