using DarkKitchen.Domain.Orders;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public OrderCreateResponse CreateOrder(Guid clientId, OrderCreateRequest request)
    {
        if(!Enum.TryParse<DeliveryType>(request.DeliveryType, true, out var deliveryType))
        {
            throw new ArgumentException("Tipo de entrega inválido.");
        }

        var address = new Address(
            request.Address.Street,
            request.Address.Number,
            request.Address.Apartment,
            request.Address.City,
            request.Address.Country);

        var items = request.Items.Select(i =>
            new OrderItem(i.ProductId, i.Quantity, 0m)).ToList();

        var order = new Order(clientId, address, deliveryType, items);
        _orderRepository.Add(order);

        return Converter.ToOrderCreateResponse(order);
    }

    public OrderDetailResponse GetOrderById(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        return Converter.ToOrderDetailResponse(order);
    }

    public IEnumerable<OrderListResponse> GetOrdersByClient(Guid clientId, DateTime? from, DateTime? to, string? state)
    {
        return _orderRepository.GetByClient(clientId, from, to, state).Select(Converter.ToOrderListResponse);
    }

    public IEnumerable<OrderListResponse> GetOrdersByStatus(DateTime from, DateTime to, string? state, string? city)
    {
        return _orderRepository.GetByStatus(from, to, state, city).Select(Converter.ToOrderListResponse);
    }

    public void Prepare(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Prepare(order);
        _orderRepository.Update(order);
    }

    public void Cancel(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Cancel(order);
        _orderRepository.Update(order);
    }

    public void Ship(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Ship(order);
        _orderRepository.Update(order);
    }

    public void Deliver(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Deliver(order);
        _orderRepository.Update(order);
    }

    public void NotDelivered(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).NotDelivered(order);
        _orderRepository.Update(order);
    }

    private Order GetOrderOrThrow(Guid orderId)
    {
        return _orderRepository.GetById(orderId)
               ?? throw new KeyNotFoundException($"Pedido {orderId} no encontrado.");
    }
}
