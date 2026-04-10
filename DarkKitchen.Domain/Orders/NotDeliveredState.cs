namespace DarkKitchen.Domain.Orders;

public class NotDeliveredState : BaseOrderState
{
    public override string Name => "No entregado";

    public override void Prepare(Order order)
    {
        order.ChangeState(new PreparedState());
    }
}
