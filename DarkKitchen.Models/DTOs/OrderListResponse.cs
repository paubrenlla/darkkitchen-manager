namespace DarkKitchen.Models.DTOs;

public class OrderListResponse
{
    public int? OrderNumber { get; set; }
    public Guid ClientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public decimal Total { get; set; }
    public int ProductCount { get; set; }
}
