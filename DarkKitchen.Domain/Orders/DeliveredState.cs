namespace DarkKitchen.Domain.Orders;

public class DeliveredState : BaseOrderState
{
    public override OrderState State => OrderState.Delivered;
}
