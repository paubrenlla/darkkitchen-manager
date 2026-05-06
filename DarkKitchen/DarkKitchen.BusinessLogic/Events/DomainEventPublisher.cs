using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;

namespace DarkKitchen.BusinessLogic.Events;

public class DomainEventPublisher(AuditObserver auditObserver) : IDomainEventPublisher
{
    private readonly AuditObserver _auditObserver = auditObserver;

    public void Publish<T>(T domainEvent)
    {
        if (domainEvent is EntityModifiedEvent<Product> productEvent)
        {
            _auditObserver.Handle(productEvent);
        }
        else if (domainEvent is EntityCreatedEvent<Product> createdEvent)
        {
            _auditObserver.Handle(createdEvent);
        }
        else if (domainEvent is EntityDeactivatedEvent<Product> deactivatedEvent)
        {
            _auditObserver.Handle(deactivatedEvent);
        }
        else if (domainEvent is EntityActivatedEvent<Product> activatedEvent)
        {
            _auditObserver.Handle(activatedEvent);
        }
    }
}
