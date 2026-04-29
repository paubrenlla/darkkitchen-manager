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

        var shippingCost = _shippingCalculator.CalculateShippingCost(deliveryType);
        var order = new Order(clientId, address, deliveryType, orderItems, shippingCost);

        _orderRepository.Add(order);
        return Converter.ToOrderCreateResponse(order);
    }

    public OrderDetailResponse GetOrderById(Guid orderId)
    {
        return Converter.ToOrderDetailResponse(GetOrderOrThrow(orderId));
    }

    public IEnumerable<OrderListResponse> GetOrdersByClient(Guid clientId, DateTime? from, DateTime? to, string? state)
    {
        return _orderRepository.GetByClient(clientId, from, to, state)
            .Select(_orderEnricher.EnrichForClient);
    }

    public IEnumerable<OrderListResponse> GetOrdersByStatus(DateTime from, DateTime to, string? state, string? address)
    {
        return _orderRepository.GetByStatus(from, to, state, address)
            .Select(_orderEnricher.EnrichForPreparador);
    }

    public void Prepare(Guid orderId)
    {
        Order o = GetOrderOrThrow(orderId);
        OrderStateFactory.Create(o.State).Prepare(o);
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
