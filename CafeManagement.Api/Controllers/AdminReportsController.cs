using CafeManagement.Api.Application;
using CafeManagement.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeManagement.Api.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/admin/reports")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AdminReportsController(AdminReportService reports) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> Overview(DateOnly? from, DateOnly? to, CancellationToken ct)
    {
        var result = await reports.GetAsync(User, from, to, ct);
        return result.Error is null ? Ok(result.Value) : ApiErrors.ToResult(result.Error);
    }
}
