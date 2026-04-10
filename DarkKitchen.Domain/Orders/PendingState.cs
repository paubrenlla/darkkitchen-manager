namespace DarkKitchen.Domain.Orders;

public class PendingState : BaseOrderState
{
    public override string Name => "Pendiente";

    public override void Prepare(Order order)
    {
        order.ChangeState(new PreparedState());
    }
}
