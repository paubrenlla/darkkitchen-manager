namespace DarkKitchen.IBusinessLogic;

public class OrderFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? State { get; set; }
    public string? Address { get; set; }
}
