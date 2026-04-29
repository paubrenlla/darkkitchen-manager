using DarkKitchen.Domain.Orders.States.Types;

namespace DarkKitchen.Domain.Orders.States;

public static class OrderStateFactory
{
    public static IOrderState Create(OrderState state)
    {
        return state switch
        {
            OrderState.Pending => new PendingState(),
            OrderState.Prepared => new PreparedState(),
            OrderState.Shipping => new ShippingState(),
            OrderState.Delivered => new DeliveredState(),
            OrderState.NotDelivered => new NotDeliveredState(),
            OrderState.Cancelled => new CancelledState(),
            _ => throw new ArgumentException($"Estado no soportado: {state}")
        };
    }
}
