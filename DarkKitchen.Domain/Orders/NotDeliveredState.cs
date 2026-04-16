namespace DarkKitchen.Domain.Orders;

public class NotDeliveredState : BaseOrderState
{
    public override OrderState State => OrderState.NotDelivered;
}
