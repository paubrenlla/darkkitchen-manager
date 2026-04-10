namespace DarkKitchen.Domain.Orders;

public class CancelledState : BaseOrderState
{
    public override string Name => "Cancelado";

    public override void Prepare(Order order)
    {
        order.ChangeState(new PreparedState());
    }
}
