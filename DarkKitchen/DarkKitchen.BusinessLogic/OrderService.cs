using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.Domain.Orders.States;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPromotionService promotionService,
    IShippingCostCalculator shippingCalculator,
    IOrderEnricher orderEnricher) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPromotionService _promotionService = promotionService;
    private readonly IShippingCostCalculator _shippingCalculator = shippingCalculator;
    private readonly IOrderEnricher _orderEnricher = orderEnricher;

    public OrderCreateResponse CreateOrder(Guid clientId, OrderCreateRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.DeliveryType))
        {
            throw new ArgumentException("El tipo de entrega es obligatorio.");
        }

        var shippingCost = _shippingCalculator.CalculateShippingCost(request.DeliveryType);

        var address = new Address(
            request.Address.Street,
            request.Address.Number,
            request.Address.Apartment,
            request.Address.City,
            request.Address.Country);

        var orderItems = new List<OrderItem>();

        foreach(var itemReq in request.Items)
        {
            var product = _productRepository.GetAll().FirstOrDefault(p => p.Id == itemReq.ProductId)
                          ?? throw new KeyNotFoundException($"El producto {itemReq.ProductId} no existe.");

            if(!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"No es posible realizar el pedido porque el producto '{product.Name}' está inactivo.");
            }

            var (promoName, discount) = _promotionService.GetBestPromotionForProduct(product.Id, DateTime.Now);

            orderItems.Add(new OrderItem(product.Id, itemReq.Quantity, product.Price, discount, promoName));
        }

        var order = new Order(clientId, address, request.DeliveryType, orderItems, shippingCost);

        _orderRepository.Add(order);
        return Converter.ToOrderCreateResponse(order);
    }

    public OrderDetailResponse GetOrderById(Guid orderId)
    {
        return Converter.ToOrderDetailResponse(GetOrderOrThrow(orderId));
    }

    public IEnumerable<OrderListResponse> GetOrdersByClient(Guid clientId, OrderFilter filter)
    {
        return _orderRepository.GetByClient(clientId, filter.From, filter.To, filter.State)
            .Select(_orderEnricher.EnrichForClient);
    }

    public IEnumerable<OrderListResponse> GetOrdersByStatus(OrderFilter filter)
    {
        return _orderRepository.GetByStatus(filter.From!.Value, filter.To!.Value, filter.State, filter.Address)
            .Select(_orderEnricher.EnrichForPreparador);
    }

    public void Prepare(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).Prepare(o);
        _orderRepository.Update(o);
    }

    public void Delay(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).Delay(o);
        _orderRepository.Update(o);
    }

    public void Cancel(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).Cancel(o);
        _orderRepository.Update(o);
    }

    public void Ship(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).Ship(o);
        _orderRepository.Update(o);
    }

    public void Deliver(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).Deliver(o);
        _orderRepository.Update(o);
    }

    public void NotDelivered(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).NotDelivered(o);
        _orderRepository.Update(o);
    }

    private Order GetOrderOrThrow(Guid orderId)
    {
        return _orderRepository.GetById(orderId)
            ?? throw new KeyNotFoundException($"Pedido {orderId} no encontrado.");
    }
}
