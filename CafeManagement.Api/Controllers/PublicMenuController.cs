using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/menu/tables")]
public sealed class PublicMenuController(PublicMenuService menu) : ControllerBase
{
    [HttpGet("{publicCode}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ProducesResponseType(typeof(PublicMenuResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(string publicCode, CancellationToken cancellationToken)
    {
        var result = await menu.GetAsync(publicCode, cancellationToken);
        return result.Error is null ? Ok(PublicMenuResponse.From(result.Value!)) : ApiErrors.ToResult(result.Error);
    }
}
