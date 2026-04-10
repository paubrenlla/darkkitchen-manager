namespace DarkKitchen.Domain.Orders;

public abstract class BaseOrderState : IOrderState
{
    public abstract string Name { get; }

    public DateTime TransitionDate { get; }

    public void Prepare(Order order)
    {
        throw new NotImplementedException();
    }

    public void Cancel(Order order)
    {
        throw new NotImplementedException();
    }

    public void Ship(Order order)
    {
        throw new NotImplementedException();
    }

    public void Deliver(Order order)
    {
        throw new NotImplementedException();
    }

    public void RejectedDelivery(Order order)
    {
        throw new NotImplementedException();
    }
}
