using DarkKitchen.Domain.Orders;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderService
{
    Order CreateOrder(Guid clientId, Address deliveryAddress, DeliveryType type, List<OrderItem> items);
    void Prepare(Guid orderId);
    void Cancel(Guid orderId);
    void Ship(Guid orderId);
    void Deliver(Guid orderId);
    void NotDelivered(Guid orderId);
    IEnumerable<Order> GetOrdersByClient(Guid clientId, DateTime? from, DateTime? to, string? state);
}
