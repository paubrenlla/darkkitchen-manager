using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class OrderEnricher(IUserRepository userRepository, IProductRepository productRepository) : IOrderEnricher
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public OrderListResponse EnrichForClient(Order order)
    {
        var clientName = ResolveClientName(order.ClientId);
        return Converter.ToOrderListResponse(order, clientName);
    }

    public OrderListResponse EnrichForPreparador(Order order)
    {
        var clientName = ResolveClientName(order.ClientId);
        var items = order.Items.Select(BuildItemSummary).ToList();
        return Converter.ToOrderListResponse(order, clientName, items);
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
            ItemTotal = item.CalculateItemTotal(),
        };
    }
}
