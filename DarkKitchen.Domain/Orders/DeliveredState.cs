namespace DarkKitchen.Domain.Orders;

public class DeliveredState : BaseOrderState
{
    public override string Name => "Entregado";

    public override void Cancel(Order order)
    {
        order.ChangeState(new CancelledState());
    }
}
