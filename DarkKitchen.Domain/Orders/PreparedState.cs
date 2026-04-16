namespace DarkKitchen.Domain.Orders;

public class PreparedState : BaseOrderState
{
    public override OrderState State => OrderState.Prepared;

    public override void Ship(Order order)
    {
        order.TransitionTo(OrderState.Shipping);
    }
}
