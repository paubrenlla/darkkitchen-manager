namespace DarkKitchen.Domain.Orders;

public class ShippingCostCalculator : IShippingCostCalculator
{
    private readonly decimal _expressShippingCost;
    private readonly decimal _standardShippingCost;

        _expressShippingCost = expressShippingCost;
        _standardShippingCost = standardShippingCost;
    public decimal CalculateShippingCost(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => _expressShippingCost,
            DeliveryType.TwentyFourHours => _standardShippingCost,
        };
    }
}
