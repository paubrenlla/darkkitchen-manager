using DarkKitchen.Domain.Audit;

namespace DarkKitchen.Models.DTOs;

public class AuditLogResponse(AuditLog log)
{
    public Guid Id { get; set; } = log.Id;
    public DateTime Timestamp { get; set; } = log.Timestamp;
    public string EntityName { get; set; } = log.EntityName;
    public Guid EntityId { get; set; } = log.EntityId;
    public string ChangeDescription { get; set; } = log.ChangeDescription;
    public string ResponsibleUser { get; set; } = log.ResponsibleUser;
}
