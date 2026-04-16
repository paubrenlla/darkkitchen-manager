namespace DarkKitchen.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> _items;

    public Order(Guid clientId, Address deliveryAddress, DeliveryType type, List<OrderItem> items)
    {
        if(items == null || !items.Any())
        {
            throw new ArgumentException("El pedido debe tener al menos un producto.");
        }

        Id = Guid.NewGuid();
        ClientId = clientId;
        DeliveryAddress = deliveryAddress;
        Type = type;
        CreatedAt = DateTime.Now;
        _items = items;
        State = OrderState.Pending;
        OrderNumber = 0;
    }

    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public DeliveryType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderState State { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void SetState(OrderState newState)
    {
        State = newState;
    }
}
