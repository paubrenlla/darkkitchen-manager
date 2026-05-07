namespace DarkKitchen.IBusinessLogic;

public interface IAuditEventHandler<T>
{
    void Handle(T domainEvent);
}
