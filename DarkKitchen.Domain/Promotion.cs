namespace DarkKitchen.Domain;

public class Promotion
{
    public Promotion(string name, int discountPercentage, DateTime startDate, DateTime endDate,
        List<Product> products)
    {
        ValidateName(name);
        ValidateDiscountPercentage(discountPercentage);
        ValidateDates(startDate, endDate);

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
    private List<Product> Products { get; set; } = [];

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if(startDate > endDate)
        {
            throw new ArgumentException("Start date must be before or equal to end date.");
        }
    }

    private static void ValidateDiscountPercentage(int discountPercentage)
    {
        if(discountPercentage <= 0)
        {
            throw new ArgumentException("Discount percentage must be greater than zero.");
        }
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 50)
        {
            throw new ArgumentException("Name must be between 3 and 50 characters.");
        }
    }
}
