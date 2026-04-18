namespace DarkKitchen.Domain.Orders;

public interface IShippingCostCalculator
{
    decimal CalculateShippingCost(DeliveryType deliveryType);
}
