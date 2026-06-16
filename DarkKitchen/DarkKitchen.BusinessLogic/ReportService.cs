using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class ReportService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository) : IReportService
{
    private const int TopProductsLimit = 5;
    private const string UnknownClientFallback = "Cliente desconocido";

    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public IEnumerable<TopProductResponse> GetTopProducts(DateTime from, DateTime to)
    {
        if(to.TimeOfDay == TimeSpan.Zero)
        {
            to = to.Date.AddDays(1).AddSeconds(-1);
        }

        if(from > to)
        {
            throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

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
            .Select(g => BuildPeriodResponse(g.Key.Year, g.Key.Month, g))
            .ToList();

        return new SalesReportResponse
        {
            Periods = periods,
            GrandTotal = periods.Sum(p => p.PeriodTotal),
        };
    }

    private SalesPeriodResponse BuildPeriodResponse(int year, int month, IEnumerable<Order> orders)
    {
        var clients = orders
            .GroupBy(o => o.ClientId)
            .Select(g => BuildClientResponse(g.Key, g))
            .ToList();

        return new SalesPeriodResponse
        {
            Year = year,
            Month = month,
            Clients = clients,
            PeriodTotal = clients.Sum(c => c.Total),
        };
    }

    private SalesClientResponse BuildClientResponse(Guid clientId, IEnumerable<Order> orders)
    {
        var user = _userRepository.GetById(clientId);
        var clientName = user != null ? $"{user.Name} {user.Surname}" : UnknownClientFallback;

        return new SalesClientResponse
        {
            ClientName = clientName,
            Total = orders.Sum(o => o.Total),
        };
    }
}
