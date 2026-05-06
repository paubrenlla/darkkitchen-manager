using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IAuditService
{
    IEnumerable<AuditLogResponse> GetAudits(DateTime from, DateTime to, string? entityName, Guid? entityId);
}
