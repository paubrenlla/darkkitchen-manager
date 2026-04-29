namespace DarkKitchen.Domain.Orders.Delivery;

public class ShippingCostCalculator(IEnumerable<IShippingStrategy> strategies) : IShippingCostCalculator
{
    public decimal CalculateShippingCost(DeliveryType deliveryType)
    {
        IShippingStrategy? strategy = strategies.FirstOrDefault(s => s.CanHandle(deliveryType));

        if(strategy == null)
        {
            throw new ArgumentException($"Delivery type not supported: {deliveryType}");
        }

        return strategy.Calculate();
    }
}
