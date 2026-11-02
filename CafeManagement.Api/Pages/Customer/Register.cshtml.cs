using System.ComponentModel.DataAnnotations;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CafeManagement.Api.Pages.Customer;
[AllowAnonymous,ResponseCache(NoStore=true,Location=ResponseCacheLocation.None)]
public sealed class RegisterModel(UserManager<IdentityUser> users,SignInManager<IdentityUser> signIn,CafeDbContext db):PageModel
{
 [BindProperty] public Registration Input {get;set;}=new();
 public IActionResult OnGet()=>User.Identity?.IsAuthenticated==true?LocalRedirect("/customer/profile"):Page();
 public async Task<IActionResult> OnPostAsync(){if(!ModelState.IsValid)return Page();await using var tx=await db.Database.BeginTransactionAsync();var user=new IdentityUser{UserName=Input.Email.Trim(),Email=Input.Email.Trim()};var created=await users.CreateAsync(user,Input.Password);if(!created.Succeeded){foreach(var error in created.Errors)ModelState.AddModelError("",error.Description);return Page();}db.CustomerProfiles.Add(new CustomerProfile{UserId=user.Id,DisplayName=Input.DisplayName.Trim(),Phone=Input.Phone.Trim()});await db.SaveChangesAsync();await tx.CommitAsync();await signIn.SignInAsync(user,false);return LocalRedirect("/customer/profile");}
 public sealed class Registration { [Required,StringLength(120)] public string DisplayName{get;set;}="";[Required,EmailAddress,StringLength(256)] public string Email{get;set;}="";[Required,StringLength(40)] public string Phone{get;set;}="";[Required,StringLength(128,MinimumLength=8),DataType(DataType.Password)] public string Password{get;set;}="";[Required,Compare(nameof(Password)),DataType(DataType.Password)] public string ConfirmPassword{get;set;}=""; }
}
