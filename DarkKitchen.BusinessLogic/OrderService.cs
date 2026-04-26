using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPromotionService promotionService) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPromotionService _promotionService = promotionService;

    public OrderCreateResponse CreateOrder(Guid clientId, OrderCreateRequest request)
    {
        if(!Enum.TryParse(request.DeliveryType, true, out DeliveryType deliveryType))
        {
            throw new ArgumentException("Tipo de entrega inválido.");
        }

        var address = new Address(
            request.Address.Street,
            request.Address.Number,
            request.Address.Apartment,
            request.Address.City,
            request.Address.Country);

        var orderItems = new List<OrderItem>();

        foreach(OrderItemDto itemReq in request.Items)
        {
            Product product = _productRepository.GetAll().FirstOrDefault(p => p.Id == itemReq.ProductId)
                              ?? throw new KeyNotFoundException($"El producto {itemReq.ProductId} no existe.");

            if(!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"No es posible realizar el pedido porque el producto '{product.Name}' está inactivo.");
            }

            var (promoName, discount) = _promotionService.GetBestPromotionForProduct(product.Id, DateTime.Now);

            orderItems.Add(new OrderItem(
                product.Id,
                itemReq.Quantity,
                product.Price,
                discount,
                promoName));
        }

        var order = new Order(clientId, address, deliveryType, orderItems);
        _orderRepository.Add(order);

        return Converter.ToOrderCreateResponse(order);
    }

    public OrderDetailResponse GetOrderById(Guid orderId)
    {
        Order order = GetOrderOrThrow(orderId);
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
        Order order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Prepare(order);
        _orderRepository.Update(order);
    }

    public void Cancel(Guid orderId)
    {
        Order order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Cancel(order);
        _orderRepository.Update(order);
    }

    public void Ship(Guid orderId)
    {
        Order order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Ship(order);
        _orderRepository.Update(order);
    }

    public void Deliver(Guid orderId)
    {
        Order order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).Deliver(order);
        _orderRepository.Update(order);
    }

    public void NotDelivered(Guid orderId)
    {
        Order order = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(order.State).NotDelivered(order);
        _orderRepository.Update(order);
    }

    private Order GetOrderOrThrow(Guid orderId)
    {
        return _orderRepository.GetById(orderId)
               ?? throw new KeyNotFoundException($"Pedido {orderId} no encontrado.");
    }
}
