namespace DarkKitchen.Domain.Events;

public interface IDomainEventPublisher
{
    void Publish<T>(T domainEvent);
}
