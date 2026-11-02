using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CafeManagement.Api.Controllers;
[ApiController,Authorize,Route("api/customer"),ResponseCache(NoStore=true,Location=ResponseCacheLocation.None)]
public sealed class CustomerController(CustomerService customers):ControllerBase
{
 [HttpGet("profile")] public async Task<IActionResult> Profile(){var value=await customers.ProfileAsync(User);return value is null?NotFound():Ok(new{value.DisplayName,value.Phone,value.Address,RowVersion=Convert.ToBase64String(value.RowVersion)});}
 [HttpPut("profile"),ValidateAntiForgeryToken,Consumes("application/json")] public async Task<IActionResult> Save(CustomerProfileInput input)=>await customers.SaveProfileAsync(User,input)?Ok():Conflict(new{message="Unable to save. Check details and reload your profile."});
 [HttpGet("orders")] public async Task<IActionResult> Orders()=>await customers.UserIdAsync(User) is null?Forbid():Ok(await customers.OrdersAsync(User));
 [HttpGet("orders/{reference:guid}")] public async Task<IActionResult> Order(Guid reference){var value=await customers.OrderAsync(User,reference);return value is null?NotFound():Ok(value);}
}
