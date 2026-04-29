namespace DarkKitchen.Domain.Orders.States.Types;

public class CancelledState : BaseOrderState
{
    public override OrderState State => OrderState.Cancelled;
}
