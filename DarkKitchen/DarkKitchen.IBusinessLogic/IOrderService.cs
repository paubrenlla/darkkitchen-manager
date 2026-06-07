using DarkKitchen.Domain.Orders;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderService
{
    Order CreateOrder(Guid clientId, OrderCreateRequest request);
    Order GetOrderById(Guid orderId);
    void UpdateOrderStatus(Guid orderId, string status);
    IEnumerable<OrderListResponse> GetOrders(Guid callerId, string? callerRole, OrderFilter filter);
    IEnumerable<OrderListResponse> GetOrdersByClient(Guid clientId, OrderFilter filter);
    IEnumerable<OrderListResponse> GetOrdersByStatus(OrderFilter filter);
    void Prepare(Guid orderId);
    void Delay(Guid orderId);
    void Cancel(Guid orderId);
    void Ship(Guid orderId);
    void Deliver(Guid orderId);
    void NotDelivered(Guid orderId);
}
