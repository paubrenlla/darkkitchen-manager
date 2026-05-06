using DarkKitchen.Domain.Audit;

namespace DarkKitchen.IDataAccess;

public interface IAuditRepository
{
    void Save(AuditLog log);
}
