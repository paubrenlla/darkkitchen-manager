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
        LastTransitionDate = DateTime.Now;
        State = OrderState.Pending;
        _items = new List<OrderItem>(items);
    }

    public Guid Id { get; private set; }
    public int? OrderNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public DeliveryType Type { get; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastTransitionDate { get; private set; }
    public OrderState State { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    {
        State = newState;
        LastTransitionDate = DateTime.Now;
    }

    public void AssignOrderNumber(int orderNumber)
    {
    }
}
