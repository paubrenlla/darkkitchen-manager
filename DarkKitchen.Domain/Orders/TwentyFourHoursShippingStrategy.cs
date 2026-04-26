namespace DarkKitchen.Domain.Orders;

public class TwentyFourHoursShippingStrategy : IShippingStrategy
{
    private readonly decimal _cost;

    public TwentyFourHoursShippingStrategy(decimal cost)
    {
        if(cost < 0)
        {
            throw new ArgumentException("24-hours shipping cost cannot be negative.");
        }

        _cost = cost;
    }

    public bool CanHandle(DeliveryType deliveryType)
    {
        return deliveryType == DeliveryType.TwentyFourHours;
    }

    public decimal Calculate()
    {
        return _cost;
    }
}
