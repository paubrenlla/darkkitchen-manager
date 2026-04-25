using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ReportService(IOrderRepository orderRepository, IProductRepository productRepository) : IReportService
{
    private const int TopProductsLimit = 5;

    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public IEnumerable<TopProductResponse> GetTopProducts(DateTime from, DateTime to)
    {
        IEnumerable<Order> orders = _orderRepository
            .GetByStatus(from, to)
            .Where(o => o.State != OrderState.Cancelled);

        IEnumerable<TopProductResponse> topProducts = orders
            .SelectMany(o => o.Items)
            .GroupBy(item => item.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                TotalQuantity = group.Sum(item => item.Quantity),
            })
            .OrderByDescending(g => g.TotalQuantity)
            .Take(TopProductsLimit)
            .Select(g =>
            {
                Product? product = _productRepository.GetById(g.ProductId);
                if(product == null)
                {
                    return null;
                }

                return new TopProductResponse
                {
                    Code = product.Code,
                    Name = product.Name,
                    QuantitySold = g.TotalQuantity,
                    Images = product.Images.Select(i => i.Url).ToList(),
                };
            })
            .Where(r => r != null)
            .Cast<TopProductResponse>();

        return topProducts;
    }
}
