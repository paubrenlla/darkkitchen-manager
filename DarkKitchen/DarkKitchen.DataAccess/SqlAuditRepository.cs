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
}
