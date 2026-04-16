namespace DarkKitchen.Domain.Orders;

public class ShippingState : BaseOrderState
{
    public override OrderState State => OrderState.Shipping;

    public override void Deliver(Order order)
    {
        order.SetState(OrderState.Delivered);
    }

    public override void NotDelivered(Order order)
    {
        order.SetState(OrderState.NotDelivered);
    }
}
