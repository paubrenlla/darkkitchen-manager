namespace DarkKitchen.Domain.Orders;

public class Order
{
    public Order(Guid clientId, Address deliveryAddress, DeliveryType type)
    {
        Id = Guid.NewGuid();
        ClientId = clientId;
        DeliveryAddress = deliveryAddress;
        Type = type;
        CreatedAt = DateTime.Now;

        ChangeState(new PendingState());
    }

    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public DeliveryType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public string StateName { get; private set; } = string.Empty;

    public IOrderState CurrentState => OrderStateFactory.Create(StateName);

    internal void ChangeState(IOrderState newState)
    {
        StateName = newState.Name;
    }
}
