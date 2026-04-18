namespace DarkKitchen.Domain.Orders;

public class ShippingCostCalculator : IShippingCostCalculator
{
    private readonly decimal _expressShippingCost;
    private readonly decimal _standardShippingCost;

    public decimal CalculateShippingCost(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => _expressShippingCost,
        };
    }
}
