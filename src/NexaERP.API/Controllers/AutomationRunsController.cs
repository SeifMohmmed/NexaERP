using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Automation;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Identity;

namespace NexaERP.API.Controllers;

[ApiController]
[Route("automation-runs")]
[Authorize(Roles = Roles.Admin)]
public sealed class AutomationRunsController(
    IAutomationService automationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AutomationRunDto>>> GetAll(
        [FromQuery] Guid? ruleId,
        [FromQuery] string? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            return BadRequest(new
            {
                message = "'from' must be earlier than or equal to 'to'."
            });
        }

        var runs = await automationService.GetRunsAsync(
            ruleId,
            status,
            from,
            to,
            ct);

        return Ok(runs);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AutomationRunDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var run = await automationService.GetRunByIdAsync(id, ct);

        if (run is null)
        {
            return NotFound();
        }

        return Ok(run);
    }
}
