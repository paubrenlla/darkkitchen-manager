namespace DarkKitchen.Domain.Orders;

public class PreparedState : BaseOrderState
{
    public override string Name => "Preparado";

    public override void Ship(Order order)
    {
        order.ChangeState(new ShippingState());
    }
}
