namespace DarkKitchen.Models.DTOs;

public class SalesReportResponse
{
    public required List<SalesPeriodResponse> Periods { get; set; }
    public decimal GrandTotal { get; set; }
}

public class SalesPeriodResponse
{
    public int Year { get; set; }
    public int Month { get; set; }
    public required List<SalesClientResponse> Clients { get; set; }
    public decimal PeriodTotal { get; set; }
}

public class SalesClientResponse
{
    public required string ClientName { get; set; }
    public decimal Total { get; set; }
}
