using DarkKitchen.Domain.Audit;

namespace DarkKitchen.IBusinessLogic;

public interface IAuditService
{
    IEnumerable<AuditLog> GetAudits(DateTime from, DateTime to, string? entityName, Guid? entityId);
}
