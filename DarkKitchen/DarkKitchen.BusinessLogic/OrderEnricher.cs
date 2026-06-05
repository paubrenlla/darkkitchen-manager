using DarkKitchen.Domain.Orders;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class OrderEnricher(IUserRepository userRepository, IProductRepository productRepository) : IOrderEnricher
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public OrderListResponse EnrichForClient(Order order)
    {
        return new OrderListResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientName = ResolveClientName(order.ClientId),
            CreatedAt = order.CreatedAt,
            Status = order.State.ToString(),
            Total = order.Total,
            ProductCount = order.Items.Sum(i => i.Quantity),
            Items = []
        };
    }

    public OrderListResponse EnrichForPreparador(Order order)
    {
        return new OrderListResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientName = ResolveClientName(order.ClientId),
            CreatedAt = order.CreatedAt,
            Status = order.State.ToString(),
            Total = order.Total,
            ProductCount = order.Items.Sum(i => i.Quantity),
            Items = order.Items.Select(BuildItemSummary).ToList()
        };
    }

    private string ResolveClientName(Guid clientId)
    {
        var user = _userRepository.GetById(clientId);
        return user != null ? $"{user.Name} {user.Surname}" : string.Empty;
    }

    private OrderItemSummaryDto BuildItemSummary(OrderItem item)
    {
        var product = _productRepository.GetById(item.ProductId);
        return new OrderItemSummaryDto
        {
            ProductId = item.ProductId,
            ProductCode = product?.Code ?? string.Empty,
            ProductName = product?.Name ?? string.Empty,
            Quantity = item.Quantity,
            Price = item.Price,
            ItemTotal = item.CalculateItemTotal()
        };
    }
}
