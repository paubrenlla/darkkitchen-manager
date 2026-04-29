namespace DarkKitchen.Domain.Orders.States.Types;

public class PendingState : BaseOrderState
{
    public override OrderState State => OrderState.Pending;

    public override void Prepare(Order order)
    {
        order.TransitionTo(OrderState.Prepared);
    }

    public override void Cancel(Order order)
    {
        order.TransitionTo(OrderState.Cancelled);
    }
}
