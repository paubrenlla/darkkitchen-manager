namespace DarkKitchen.Domain;

public class Promotion
{
    public Promotion(string name, int discountPercentage, DateTime startDate, DateTime endDate,
        List<Product> products)
    {
        Id = Guid.NewGuid();
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
        Products = products;

        IsActive = true;
    }

    private Guid Id { get; set; }
    private string Name { get; set; } = null!;
    private bool IsActive { get; set; }
    private int DiscountPercentage { get; set; }
    private DateTime StartDate { get; set; }
    private DateTime EndDate { get; set; }
    private List<Product> Products { get; set; } = new();
}
