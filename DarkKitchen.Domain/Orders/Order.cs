namespace DarkKitchen.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> _items = new();

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

        ChangeState(new PendingState());
    }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public DeliveryType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public string StateName { get; private set; } = string.Empty;

    public IOrderState CurrentState => OrderStateFactory.Create(StateName);

    public void ChangeState(IOrderState newState)
    {
        StateName = newState.Name;
    }

    public void Prepare()
    {
        CurrentState.Prepare(this);
    }

    public void Cancel()
    {
        CurrentState.Cancel(this);
    }

    public void Ship()
    {
        CurrentState.Ship(this);
    }
}
