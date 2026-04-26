namespace DarkKitchen.Models.DTOs;

public class OrderDetailResponse
{
    public int? OrderNumber { get; set; }
    public Guid ClientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public required List<OrderItemDetailDto> Items { get; set; }
    public decimal Total { get; set; }
}

public class OrderItemDetailDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? AppliedPromotion { get; set; }
    public decimal ItemTotal { get; set; }
}
