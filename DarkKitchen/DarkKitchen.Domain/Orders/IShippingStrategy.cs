namespace DarkKitchen.Domain.Orders;

public interface IShippingStrategy
{
    bool CanHandle(DeliveryType deliveryType);

    decimal Calculate();
}
