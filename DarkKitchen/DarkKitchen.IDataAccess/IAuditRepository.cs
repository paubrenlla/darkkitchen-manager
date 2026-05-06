using DarkKitchen.Domain.Audit;

namespace DarkKitchen.IDataAccess;

public interface IAuditRepository
{
    void Save(AuditLog log);
    IEnumerable<AuditLog> GetAudits(DateTime from, DateTime to, string? entityName, Guid? entityId);
}
