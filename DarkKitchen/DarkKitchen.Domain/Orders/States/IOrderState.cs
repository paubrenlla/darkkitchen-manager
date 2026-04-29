namespace DarkKitchen.Domain.Orders.States;

public interface IOrderState
{
    OrderState State { get; }
    void Prepare(Order order);
    void Cancel(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void NotDelivered(Order order);
}
