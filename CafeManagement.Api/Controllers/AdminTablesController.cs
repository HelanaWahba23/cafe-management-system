using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CafeManagement.Api.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/admin/tables")]
public sealed class AdminTablesController : ControllerBase
{
    private readonly TableService tables;
    private readonly TableQrService qr;

    // T041 introduces shared service registration with the table component. Until then,
    // compose the API services from existing registered dependencies without changing the host.
    public AdminTablesController(IDbContextFactory<CafeDbContext> factory, StaffAccess staff, IOptions<PublicMenuOptions> options)
    {
        tables = new TableService(factory, staff, options);
        qr = new TableQrService(tables);
    }

    [HttpGet]
    [ProducesResponseType(typeof(TableResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await tables.ListAsync(User, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var result = await tables.GetAsync(User, id, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(TableResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(TableInput input, CancellationToken cancellationToken)
    {
        var result = await tables.CreateAsync(User, input, cancellationToken);
        return result.Error is null ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : ApiErrors.ToResult(result.Error);
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(TableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Edit(int id, TableInput input, CancellationToken cancellationToken)
    {
        var result = await tables.EditAsync(User, id, input, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpGet("{id:int}/qr")]
    [Produces("image/png", "application/problem+json")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Qr(int id, CancellationToken cancellationToken)
    {
        var result = await qr.GetPngAsync(User, id, cancellationToken);
        Response.Headers.CacheControl = "no-store";
        return result.Error is null ? File(result.Value!, "image/png") : ApiErrors.ToResult(result.Error);
    }
}
