using DarkKitchen.Domain.Audit;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic;

public class AuditService(IAuditRepository auditRepository) : IAuditService
{
    private readonly IAuditRepository _auditRepository = auditRepository;

    public IEnumerable<AuditLog> GetAudits(DateTime from, DateTime to, string? entityName, Guid? entityId)
    {
        if (to.TimeOfDay == TimeSpan.Zero)
        {
            to = to.Date.AddDays(1).AddSeconds(-1);
        }

        if(from > to)
        {
            throw new ArgumentException("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
        }

        return _auditRepository.GetAudits(from, to, entityName, entityId);
    }
}
