namespace DarkKitchen.Domain.Orders;

public class ShippingCostCalculator : IShippingCostCalculator
{
    private readonly decimal _expressShippingCost;
    private readonly decimal _twentyFourHoursShippingCost;

    public ShippingCostCalculator(decimal expressShippingCost, decimal twentyFourHoursShippingCost)
    {
        if(expressShippingCost < 0)
        {
            throw new ArgumentException("Express shipping cost cannot be negative.");
        }

        if(twentyFourHoursShippingCost < 0)
        {
            throw new ArgumentException("24-hours shipping cost cannot be negative.");
        }

        _expressShippingCost = expressShippingCost;
        _twentyFourHoursShippingCost = twentyFourHoursShippingCost;
    }

    public decimal CalculateShippingCost(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => _expressShippingCost,
            DeliveryType.TwentyFourHours => _twentyFourHoursShippingCost,
            _ => throw new ArgumentException($"Delivery type not supported: {deliveryType}")
        };
    }
}
