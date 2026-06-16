using DarkKitchen.Domain.Events;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Events;

public class DomainEventPublisher(IServiceProvider serviceProvider) : IDomainEventPublisher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void Publish<T>(T domainEvent)
    {
        var handlers = (IEnumerable<IAuditEventHandler<T>>?)_serviceProvider.GetService(typeof(IEnumerable<IAuditEventHandler<T>>));

        if(handlers == null)
        {
            return;
        }

        foreach(var handler in handlers)
        {
            handler.Handle(domainEvent);
        }
    }
}
