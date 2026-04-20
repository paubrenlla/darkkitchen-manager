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
    Order GetOrderById(Guid orderId);
    IEnumerable<Order> GetOrdersByClient(Guid clientId, DateTime? from, DateTime? to, string? state);
    IEnumerable<Order> GetOrdersByStatus(DateTime from, DateTime to, string? state, string? city);
}
