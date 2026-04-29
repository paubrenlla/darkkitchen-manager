namespace DarkKitchen.Domain.Orders.Delivery;

public interface IShippingStrategy
{
    bool CanHandle(DeliveryType deliveryType);

    decimal Calculate();
}
