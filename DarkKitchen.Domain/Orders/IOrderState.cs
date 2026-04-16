namespace DarkKitchen.Domain.Orders;

public interface IOrderState
{
    OrderState State { get; }
    DateTime TransitionDate { get; }
    void Prepare(Order order);
    void Cancel(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void NotDelivered(Order order);
}
