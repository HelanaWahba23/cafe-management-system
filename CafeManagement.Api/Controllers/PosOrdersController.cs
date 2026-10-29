using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Api.Controllers;

[ApiController]
[Authorize(Policy = "Cashier")]
[Route("api/pos/orders")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class PosOrdersController(PosOrderService orders) : ControllerBase
{
    [HttpGet("/api/pos/tables")]
    public async Task<IActionResult> Tables(CancellationToken ct)
    {
        var result = await orders.TablesAsync(User, ct);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpGet("/api/pos/catalog")]
    public async Task<IActionResult> Catalog(CancellationToken ct)
    {
        var result = await orders.CatalogAsync(User, ct);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost("review")]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(CounterOrderInput input, CancellationToken ct)
    {
        var result = await orders.ReviewAsync(User, input, ct);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Place(CounterOrderInput input, CancellationToken ct)
    {
        var result = await orders.PlaceAsync(User, input, ct);
        if (result.Error is null) return StatusCode(result.Created ? 201 : 200, result.Value);
        var problem = ApiErrors.CreateProblem(result.Error);
        if (result.Error.Code == "price_changed" && result.Review is not null) problem.Extensions["review"] = result.Review;
        return new ObjectResult(problem) { StatusCode = problem.Status, ContentTypes = { "application/problem+json" } };
    }

    [HttpGet]
    public async Task<IActionResult> List(string? status, int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await orders.ListAsync(User, status, page, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
    {
        var result = await orders.GetAsync(User, id, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost("{id:long}/status")]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(long id, PosStatusInput input, CancellationToken cancellationToken)
    {
        var result = await orders.ChangeStatusAsync(User, id, input, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }
}
