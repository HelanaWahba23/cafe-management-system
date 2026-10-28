using System.Net.Http.Headers;
using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CafeManagement.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/orders")]
[EnableRateLimiting("GuestOrders")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class PublicOrdersController(OrderService orders) : ControllerBase
{
    [HttpPost("review")]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(OrderReview), StatusCodes.Status200OK)]
    public async Task<IActionResult> Review(OrderDraft draft, CancellationToken cancellationToken)
    {
        var result = await orders.ReviewAsync(Capability(), draft, cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(typeof(OrderPlacementResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(OrderPlacementResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Place(OrderPlacementInput input, CancellationToken cancellationToken)
    {
        var result = await orders.PlaceAsync(Capability(), input, cancellationToken);
        if (result.Error is not null) return ApiErrors.ToOrderResult(result);
        // The client builds the fragment link from its retained credential; no bearer Location.
        return result.Created ? StatusCode(StatusCodes.Status201Created, result.Value) : Ok(result.Value);
    }

    [HttpGet("tracking")]
    [ProducesResponseType(typeof(OrderTracking), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Tracking(CancellationToken cancellationToken)
    {
        var result = await orders.TrackAsync(Capability(), cancellationToken);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }

    private string? Capability()
    {
        // Opaque guest credential only; never use cookies/roles or replace Identity's scheme.
        var values = Request.Headers.Authorization;
        return values.Count == 1 && AuthenticationHeaderValue.TryParse(values[0], out var header)
            && header.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase)
                ? header.Parameter : null;
    }
}
