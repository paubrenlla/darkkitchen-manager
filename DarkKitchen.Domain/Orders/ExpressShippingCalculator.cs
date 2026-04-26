namespace DarkKitchen.Domain.Orders;

public class ExpressShippingStrategy : IShippingStrategy
{
    private readonly decimal _cost;

    public ExpressShippingStrategy(decimal cost)
    {
        if (cost < 0)
        {
            throw new ArgumentException("Express shipping cost cannot be negative.");
        }

        _cost = cost;
    }

    public bool CanHandle(DeliveryType deliveryType)
    {
        return deliveryType == DeliveryType.Express;
    }

    public decimal Calculate()
    {
        return _cost;
    }
}
