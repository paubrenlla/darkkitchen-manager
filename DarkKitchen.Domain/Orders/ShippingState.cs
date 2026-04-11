namespace DarkKitchen.Domain.Orders;

public class ShippingState : BaseOrderState
{
    public override string Name => "En camino";

    public override void Cancel(Order order)
    {
        order.ChangeState(new CancelledState());
    }
}
