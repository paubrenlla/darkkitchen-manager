namespace DarkKitchen.Domain.Orders.Delivery;

public interface IShippingCostCalculator
{
    decimal CalculateShippingCost(string shippingTypeName);
}
