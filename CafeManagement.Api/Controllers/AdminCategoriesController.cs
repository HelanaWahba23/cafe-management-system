using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Api.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController(MenuService menu) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(CategoryResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await menu.ListCategoriesAsync(User, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await menu.GetCategoryAsync(User, id, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CategoryInput input, CancellationToken cancellationToken)
    {
        var result = await menu.CreateCategoryAsync(User, input, cancellationToken);
        return result.Error is null ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : ApiErrors.ToResult(result.Error);
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit(int id, CategoryInput input, CancellationToken cancellationToken)
    {
        var result = await menu.EditCategoryAsync(User, id, input, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }
}
