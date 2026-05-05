using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderService
{
    OrderCreateResponse CreateOrder(Guid clientId, OrderCreateRequest request);
    OrderDetailResponse GetOrderById(Guid orderId);
    IEnumerable<OrderListResponse> GetOrdersByClient(Guid clientId, DateTime? from, DateTime? to, string? state);
    IEnumerable<OrderListResponse> GetOrdersByStatus(DateTime from, DateTime to, string? state, string? address);
    void Prepare(Guid orderId);
    void Delay(Guid orderId);
    void Cancel(Guid orderId);
    void Ship(Guid orderId);
    void Deliver(Guid orderId);
    void NotDelivered(Guid orderId);
}
