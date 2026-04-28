namespace DarkKitchen.Models.DTOs;

public class OrderListResponse
{
    public Guid Id { get; set; }
    public int? OrderNumber { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public decimal Total { get; set; }
    public int ProductCount { get; set; }
    public List<OrderItemSummaryDto> Items { get; set; } = [];
}

public class OrderItemSummaryDto
{
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal ItemTotal { get; set; }
}
