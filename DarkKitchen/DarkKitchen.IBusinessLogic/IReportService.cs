using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IReportService
{
    IEnumerable<TopProductResponse> GetTopProducts(DateTime from, DateTime to);
    SalesReportResponse GetSalesReport();
}
