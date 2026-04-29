namespace DarkKitchen.Domain.Orders.States.Types;

public class NotDeliveredState : BaseOrderState
{
    public override OrderState State => OrderState.NotDelivered;
}
