namespace DarkKitchen.Domain.Orders.States.Types;

public class PreparedState : BaseOrderState
{
    public override OrderState State => OrderState.Prepared;

    public override void Ship(Order order)
    {
        order.TransitionTo(OrderState.Shipping);
    }
}
