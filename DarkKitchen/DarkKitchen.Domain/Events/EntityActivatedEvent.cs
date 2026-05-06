namespace DarkKitchen.Domain.Events;

public class EntityActivatedEvent<T>
{
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
    public T NewState { get; set; } = default!;
}
