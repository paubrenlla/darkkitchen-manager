namespace DarkKitchen.Domain.Orders;

public class PendingState : BaseOrderState
{
    public override string Name => "Pending";

    public override void Prepare(Order order)
    {
        order.ChangeState(new PreparedState());
    }

    public override void Cancel(Order order)
    {
        order.ChangeState(new CancelledState());
    }
}
