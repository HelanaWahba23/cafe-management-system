using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Api.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/admin/products")]
public sealed class AdminProductsController(MenuService menu) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ProductResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await menu.ListProductsAsync(User, cancellationToken);
        return result.Error is null ? Ok(result.Value!.Select(ProductResponse.From)) : ApiErrors.ToResult(result.Error);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await menu.GetProductAsync(User, id, cancellationToken);
        return result.Error is null ? Ok(ProductResponse.From(result.Value!)) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(ProductInput input, CancellationToken cancellationToken)
    {
        var result = await menu.CreateProductAsync(User, input.ToEditModel(), cancellationToken);
        return result.Error is null ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, ProductResponse.From(result.Value))
            : ApiErrors.ToResult(result.Error);
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit(int id, ProductInput input, CancellationToken cancellationToken)
    {
        var result = await menu.EditProductAsync(User, id, input.ToEditModel(), cancellationToken);
        return result.Error is null ? Ok(ProductResponse.From(result.Value!)) : ApiErrors.ToResult(result.Error);
    }
}
