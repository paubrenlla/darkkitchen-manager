namespace DarkKitchen.Domain.Orders;

public class PreparedState : BaseOrderState
{
    public override string Name => "Prepared";

    public override void Ship(Order order)
    {
        order.ChangeState(new ShippingState());
    }
}
