using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CafeManagement.Api.Pages.Customer;
[AllowAnonymous,ResponseCache(NoStore=true,Location=ResponseCacheLocation.None)]
public sealed class LoginModel(UserManager<IdentityUser> users,SignInManager<IdentityUser> signIn):PageModel
{
 [BindProperty]public LoginInput Input{get;set;}=new();[BindProperty(SupportsGet=true)]public string? ReturnUrl{get;set;}
 public void OnGet(){}
 public async Task<IActionResult> OnPostAsync(){if(ModelState.IsValid){var user=await users.FindByEmailAsync(Input.Email.Trim());if(user is not null&&!await users.IsInRoleAsync(user,"Admin")&&!await users.IsInRoleAsync(user,"Cashier")){var result=await signIn.PasswordSignInAsync(user,Input.Password,false,true);if(result.Succeeded)return LocalRedirect(Url.IsLocalUrl(ReturnUrl)?ReturnUrl!:"/customer/orders");}}ModelState.AddModelError("","Unable to sign in. Check your details and try again.");return Page();}
 public sealed class LoginInput{[Required,EmailAddress]public string Email{get;set;}="";[Required,DataType(DataType.Password)]public string Password{get;set;}="";}
}
