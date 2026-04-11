namespace DarkKitchen.Domain.Orders;

public class CancelledState : BaseOrderState
{
    public override string Name => "Cancelado";

    public override void Cancel(Order order)
    {
        order.ChangeState(new CancelledState());
    }
}
