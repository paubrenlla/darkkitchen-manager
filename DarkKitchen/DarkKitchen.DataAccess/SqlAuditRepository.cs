using DarkKitchen.Domain.Audit;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess;

public class SqlAuditRepository(DarkKitchenContext context) : IAuditRepository
{
    private readonly DarkKitchenContext _context = context;

    public void Save(AuditLog log)
    {
        _context.AuditLogs.Add(log);
        _context.SaveChanges();
    }

    public IEnumerable<AuditLog> GetAudits(DateTime from, DateTime to, string? entityName, Guid? entityId)
    {
        IQueryable<AuditLog> query = _context.AuditLogs
            .Where(l => l.Timestamp >= from && l.Timestamp <= to);

        if(!string.IsNullOrEmpty(entityName))
        {
            query = query.Where(l => l.EntityName == entityName);
        }

        if(entityId.HasValue)
        {
            query = query.Where(l => l.EntityId == entityId.Value);
        }

        return query.ToList();
    }
}
