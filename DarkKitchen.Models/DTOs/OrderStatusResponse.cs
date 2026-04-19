namespace DarkKitchen.Models.DTOs;

public class OrderStatusResponse
{
    public required string Status { get; set; }
    public DateTime LastTransitionDate { get; set; }
}
