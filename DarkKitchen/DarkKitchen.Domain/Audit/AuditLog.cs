namespace DarkKitchen.Domain.Audit;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string ChangeDescription { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
}
