namespace DarkKitchen.Domain.Orders.States.Types;

public class ShippingState : BaseOrderState
{
    public override OrderState State => OrderState.Shipping;

    public override void Deliver(Order order)
    {
        order.TransitionTo(OrderState.Delivered);
    }

    public override void NotDelivered(Order order)
    {
        order.TransitionTo(OrderState.NotDelivered);
    }
}
