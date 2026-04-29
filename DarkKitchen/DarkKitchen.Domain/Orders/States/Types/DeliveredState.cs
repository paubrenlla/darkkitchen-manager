namespace DarkKitchen.Domain.Orders.States.Types;

public class DeliveredState : BaseOrderState
{
    public override OrderState State => OrderState.Delivered;
}
