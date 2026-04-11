namespace DarkKitchen.Domain.Orders;

public class ShippingState : BaseOrderState
{
    public override string Name => "En camino";

    public override void Deliver(Order order)
    {
        order.ChangeState(new DeliveredState());
    }

    public override void NotDelivered(Order order)
    {
        order.ChangeState(new NotDeliveredState());
    }
}
