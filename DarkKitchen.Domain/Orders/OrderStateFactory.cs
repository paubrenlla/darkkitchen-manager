namespace DarkKitchen.Domain.Orders;

public static class OrderStateFactory
{
    public static IOrderState Create(string stateName)
    {
        return stateName switch
        {
            "Pending" => new PendingState(),
            "Prepared" => new PreparedState(),
            "Shipping" => new ShippingState(),
            "Delivered" => new DeliveredState(),
            "NotDelivered" => new NotDeliveredState(),
            "Cancelled" => new CancelledState(),
            _ => throw new ArgumentException($"Estado desconocido: {stateName}")
        };
    }
}
