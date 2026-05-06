namespace DarkKitchen.Domain.Events;

public class EntityDeactivatedEvent<T>
{
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
    public T OldState { get; set; } = default!;
}
