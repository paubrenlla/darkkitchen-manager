using System.Diagnostics.CodeAnalysis;

namespace DarkKitchen.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> _items;
    [ExcludeFromCodeCoverage]
    protected Order()
    {
        _items = [];
        DeliveryAddress = null!;
        Type = null!;
    }

    public Order(Guid clientId, Address deliveryAddress, string type, List<OrderItem> items, decimal shippingCost)
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
        ShippingCost = shippingCost;
    }

    public Guid Id { get; private set; }
    public int? OrderNumber { get; private set; }
    public Guid ClientId { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public string Type { get; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastTransitionDate { get; private set; }
    public OrderState State { get; private set; }
    public decimal ShippingCost { get; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Subtotal => _items.Sum(i => i.CalculateItemTotal());
    public decimal Tax => Subtotal * 0.22m;
    public decimal Total => Subtotal + Tax + ShippingCost;

    public void TransitionTo(OrderState newState)
    {
        State = newState;
        LastTransitionDate = DateTime.Now;
    }

    public void AssignOrderNumber(int orderNumber)
    {
        if(OrderNumber.HasValue)
        {
            throw new InvalidOperationException("El número de pedido ya fue asignado.");
        }

        OrderNumber = orderNumber;
    }

    public void SetCreatedAt(DateTime date)
    {
        CreatedAt = date;
    }
}
