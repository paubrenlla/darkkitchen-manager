using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ReportService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository) : IReportService
{
    private const int TopProductsLimit = 5;

    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IUserRepository _userRepository = userRepository;

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

    public SalesReportResponse GetSalesReport()
{
    var validOrders = _orderRepository.GetAll()
        .Where(o => o.State != OrderState.Cancelled);

    var periods = validOrders
        .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
        .OrderBy(g => g.Key.Year)
        .ThenBy(g => g.Key.Month)
        .Select(g => new SalesPeriodResponse
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            Clients = [],
            PeriodTotal = 0,
        })
        .ToList();

    return new SalesReportResponse
    {
        Periods = periods,
        GrandTotal = 0,
    };
}
}
