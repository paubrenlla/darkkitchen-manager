namespace DarkKitchen.Domain.Orders;

public class Order
{
    public Order(Address deliveryAddress)
    {
        DeliveryAddress = deliveryAddress;
    }

    public Order()
    {
        throw new NotImplementedException();
    }

    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public DeliveryType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string StateName { get; private set; } = string.Empty;

    public void ChangeState(IOrderState newState)
    {
        StateName = newState.Name;
    }
}
