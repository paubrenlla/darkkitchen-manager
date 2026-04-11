namespace DarkKitchen.Domain.Orders;

public class NotDeliveredState : BaseOrderState
{
    public override string Name => "No entregado";

    public override void Cancel(Order order)
    {
        order.ChangeState(new CancelledState());
    }
}
