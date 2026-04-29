using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Products;

namespace DarkKitchen.Domain;

public class Promotion
{
    [ExcludeFromCodeCoverage]
    protected Promotion()
    {
        Name = null!;
        Products = [];
    }

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

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DiscountPercentage { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public List<Product> Products { get; private set; } = [];

    public void Update(string name, int discount, DateTime start, DateTime end, List<Product> products)
    {
        ValidateName(name);
        ValidateDiscountPercentage(discount);
        ValidateDates(start, end);

        Name = name;
        DiscountPercentage = discount;
        StartDate = start;
        EndDate = end;
        Products = products;
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 50)
        {
            throw new ArgumentException("Name must be between 3 and 50 characters.");
        }
    }

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

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsVigente(DateTime date)
    {
        return IsActive && date >= StartDate && date <= EndDate;
    }
}
