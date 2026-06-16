namespace DarkKitchen.Domain.Orders.States.Types;

public class DelayedState : BaseOrderState
{
    public override OrderState State => OrderState.Delayed;

    public override void Prepare(Order order)
    {
        order.TransitionTo(OrderState.Prepared);
    }

    public override void Cancel(Order order)
    {
        order.TransitionTo(OrderState.Cancelled);
    }
}
