namespace DarkKitchen.Domain.Orders;

public class CancelledState : BaseOrderState
{
    public override OrderState State => OrderState.Cancelled;
}
