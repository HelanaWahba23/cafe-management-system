using Microsoft.AspNetCore.Identity;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CafeManagement.Api.Pages.Customer;
public sealed class SignOutModel(SignInManager<IdentityUser> signIn):PageModel{public void OnGet(){}public async Task<IActionResult> OnPostAsync(){await signIn.SignOutAsync();return LocalRedirect("/");}}
