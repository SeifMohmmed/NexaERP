using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Automation;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Identity;
using NexaERP.DAL.Services;

namespace NexaERP.API.Controllers;
[ApiController]
[Route("automation-rules")]
[Authorize(Roles = Roles.Admin)]
public sealed class AutomationRulesController(
    IAutomationService automationService,
    UserContext userContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AutomationRuleDto>>>
        GetAll(CancellationToken ct)
    {
        var rules = await automationService.GetRulesAsync(ct);

        return Ok(rules);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AutomationRuleDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var rule = await automationService.GetRuleByIdAsync(id, ct);

        if (rule is null)
        {
            return NotFound();
        }

        return Ok(rule);
    }

    [HttpPost]
    public async Task<ActionResult<AutomationRuleDto>> Create(
        [FromBody] CreateAutomationRuleDto request,
        CancellationToken ct)
    {
        var createdBy = await  userContext.GetUserIdAsync(ct);

        if (createdBy is null)
        {
            return Unauthorized();
        }

        var rule = await automationService.CreateRuleAsync(
            request,
            createdBy.Value,
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = rule.Id },
            rule);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AutomationRuleDto>> Update(
        Guid id,
        [FromBody] UpdateAutomationRuleDto request,
        CancellationToken ct)
    {
        var rule = await automationService.UpdateRuleAsync(
            id,
            request,
            ct);

        if (rule is null)
        {
            return NotFound();
        }

        return Ok(rule);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
    {
        var deleted = await automationService.DeleteRuleAsync(id, ct);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpPatch("{id:guid}/enabled")]
    public async Task<IActionResult> SetEnabled(
        Guid id,
        [FromBody] SetAutomationRuleEnabledDto request,
        CancellationToken ct)
    {
        var updated = await automationService.SetRuleEnabledAsync(
            id,
            request.Enabled,
            ct);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}
