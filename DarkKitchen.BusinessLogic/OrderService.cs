using DarkKitchen.Domain.Orders;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Order CreateOrder(Guid clientId, Address deliveryAddress, DeliveryType type, List<OrderItem> items)
    {
        var order = new Order(clientId, deliveryAddress, type, items);
        _orderRepository.Add(order);
        return order;
    }
}
