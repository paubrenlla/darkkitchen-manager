namespace DarkKitchen.Domain.Orders;

public class PreparedState : BaseOrderState
{
    public override string Name => "Preparado";

    public override void Prepare(Order order)
    {
        order.ChangeState(new PreparedState());
    }
}
