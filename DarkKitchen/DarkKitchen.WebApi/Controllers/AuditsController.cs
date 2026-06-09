using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrativo")]
public class AuditsController(IAuditService auditService) : ControllerBase
{
    private readonly IAuditService _auditService = auditService;

    [HttpGet]
    public IActionResult GetAudits(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] string? entityName,
        [FromQuery] Guid? entityId)
    {
        var audits = _auditService.GetAudits(from, to, entityName, entityId)
            .Select(a => new AuditLogResponse(a))
            .ToList();
        return Ok(audits);
    }
}
