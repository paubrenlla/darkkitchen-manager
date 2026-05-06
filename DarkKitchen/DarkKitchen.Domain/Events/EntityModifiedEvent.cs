namespace DarkKitchen.Domain.Events;

public class EntityModifiedEvent<T>
{
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string ResponsibleUser { get; set; } = string.Empty;
    public T OldState { get; set; } = default!;
    public T NewState { get; set; } = default!;
}
