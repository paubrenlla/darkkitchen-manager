namespace DarkKitchen.Domain.Orders;

public class ShippingCostCalculator : IShippingCostCalculator
{
    private readonly IEnumerable<IShippingStrategy> _strategies;

    public ShippingCostCalculator(IEnumerable<IShippingStrategy> strategies)
    {
        _strategies = strategies;
    }

    public decimal CalculateShippingCost(DeliveryType deliveryType)
    {
        IShippingStrategy? strategy = _strategies.FirstOrDefault(s => s.CanHandle(deliveryType));

        if(strategy == null)
        {
            throw new ArgumentException($"Delivery type not supported: {deliveryType}");
        }

        return strategy.Calculate();
    }
}
