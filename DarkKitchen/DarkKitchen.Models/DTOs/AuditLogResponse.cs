using DarkKitchen.Domain.Audit;

namespace DarkKitchen.Models.DTOs;

public class AuditLogResponse
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string ChangeDescription { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;

    public AuditLogResponse()
    {
    }

    public AuditLogResponse(AuditLog log)
    {
        Id = log.Id;
        Timestamp = log.Timestamp;
        EntityName = log.EntityName;
        EntityId = log.EntityId;
        ChangeDescription = log.ChangeDescription;
        ResponsibleUser = log.ResponsibleUser;
    }
}
