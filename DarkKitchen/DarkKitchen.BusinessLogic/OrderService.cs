using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Orders.Delivery;
using DarkKitchen.Domain.Orders.States;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
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

    public Order CreateOrder(Guid clientId, OrderCreateRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.DeliveryType))
        {
            throw new ArgumentException("El tipo de entrega es obligatorio.");
        }

        var shippingCost = _shippingCalculator.CalculateShippingCost(request.DeliveryType);
        var address = BuildAddress(request.Address);
        var orderItems = BuildOrderItems(request.Items);
        var order = new Order(clientId, address, request.DeliveryType, orderItems, shippingCost);

        _orderRepository.Add(order);
        return order;
    }

    private Address BuildAddress(OrderAddressDto dto)
    {
        return new Address(dto.Street, dto.Number, dto.Apartment, dto.City, dto.Country);
    }

    private List<OrderItem> BuildOrderItems(IEnumerable<OrderItemDto> itemRequests)
    {
        var orderItems = new List<OrderItem>();

        foreach(var itemReq in itemRequests)
        {
            var product = GetActiveProduct(itemReq.ProductId);
            var (promoName, discount) = _promotionService.GetBestPromotionForProduct(product.Id, DateTime.Now);
            orderItems.Add(new OrderItem(product.Id, itemReq.Quantity, product.Price, discount, promoName));
        }

        return orderItems;
    }

    private Product GetActiveProduct(Guid productId)
    {
        var product = _productRepository.GetAll().FirstOrDefault(p => p.Id == productId)
                      ?? throw new KeyNotFoundException($"El producto {productId} no existe.");

        if(!product.IsActive)
        {
            throw new InvalidOperationException(
                $"No es posible realizar el pedido porque el producto '{product.Name}' está inactivo.");
        }

        return product;
    }

    public Order GetOrderById(Guid orderId)
    {
        return GetOrderOrThrow(orderId);
    }

    public IEnumerable<OrderListResponse> GetOrdersByClient(Guid clientId, OrderFilter filter)
    {
        var toDate = filter.To;
        if(toDate.HasValue && toDate.Value.TimeOfDay == TimeSpan.Zero)
        {
            toDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);
        }

        return _orderRepository.GetByClient(clientId, filter.From, toDate, filter.State)
            .Select(_orderEnricher.EnrichForClient);
    }

    public IEnumerable<OrderListResponse> GetOrdersByStatus(OrderFilter filter)
    {
        var toDate = filter.To;
        if(toDate.HasValue && toDate.Value.TimeOfDay == TimeSpan.Zero)
        {
            toDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);
        }

        return _orderRepository.GetByStatus(filter.From!.Value, toDate!.Value, filter.State, filter.Address)
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

    public void UpdateOrderStatus(Guid orderId, string status)
    {
        switch(status.ToLower())
        {
            case "preparado": Prepare(orderId); break;
            case "demorado": Delay(orderId); break;
            case "cancelado": Cancel(orderId); break;
            case "encamino": Ship(orderId); break;
            case "entregado": Deliver(orderId); break;
            case "noentregado": NotDelivered(orderId); break;
            default:
                throw new ArgumentException($"Estado '{status}' no válido.");
        }
    }

    public IEnumerable<OrderListResponse> GetOrders(Guid callerId, string? callerRole, OrderFilter filter)
    {
        if(callerRole == "Preparador")
        {
            if(filter.From == null || filter.To == null)
            {
                throw new ArgumentException("El rango de fechas es obligatorio para el preparador.");
            }

            return GetOrdersByStatus(filter);
        }

        return GetOrdersByClient(callerId, filter);
    }
}
