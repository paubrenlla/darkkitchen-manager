namespace DarkKitchen.Models.DTOs;

public class OrderCreateResponse
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public int OrderNumber { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
}
