namespace DarkKitchen.Domain.Orders;

public class PendingState : BaseOrderState
{
    public override OrderState State => OrderState.Pending;

    public override void Prepare(Order order)
    {
        order.SetState(OrderState.Prepared);
    }

    public override void Cancel(Order order)
    {
        order.SetState(OrderState.Cancelled);
    }
}
