using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class AuditService(IAuditRepository auditRepository) : IAuditService
{
    private readonly IAuditRepository _auditRepository = auditRepository;

    public IEnumerable<AuditLogResponse> GetAudits(DateTime from, DateTime to, string? entityName, Guid? entityId)
    {
        if(from > to)
        {
            throw new ArgumentException("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
        }

        var audits = _auditRepository.GetAudits(from, to, entityName, entityId);
        return audits.Select(Converter.ToAuditLogResponse);
    }
}
