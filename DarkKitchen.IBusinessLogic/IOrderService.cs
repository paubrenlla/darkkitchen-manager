using DarkKitchen.Domain.Orders;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderService
{
    void Prepare(Order order);
    void Cancel(Order order);
    void Ship(Order order);
    void Deliver(Order order);
    void NotDelivered(Order order);
}
