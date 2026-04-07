using System.Text.RegularExpressions;

namespace DarkKitchen.Domain;

public class Product
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProductLine Line { get; private set; }
    public ProductCategory Category { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    public Product(string code, string name, string description, ProductLine line, ProductCategory category, decimal price)
    {
        ValidateCode(code);
        ValidateName(name);
        ValidateDescription(description);
        ValidateLine(line);
        ValidateCategory(category);
        ValidatePrice(price);

        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        Description = description;
        Line = line;
        Category = category;
        Price = price;
        IsActive = true;
    }

    private static void ValidateCode(string code)
    {
        if(string.IsNullOrWhiteSpace(code) || code.Length < 5 || code.Length > 20)
        {
            throw new ArgumentException("Code must be between 5 and 20 alphanumeric characters.");
        }

        if(!Regex.IsMatch(code, @"^[a-zA-Z0-9]+$"))
        {
            throw new ArgumentException("Code must contain only alphanumeric characters.");
        }
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length < 10 || name.Length > 50)
        {
            throw new ArgumentException("Name must be between 10 and 50 characters.");
        }
    }

    private static void ValidateDescription(string description)
    {
        if(string.IsNullOrWhiteSpace(description) || description.Length < 20 || description.Length > 500)
        {
            throw new ArgumentException("Description must be between 20 and 500 characters.");
        }
    }

    private static void ValidateLine(ProductLine line)
    {
        if(line == null)
        {
            throw new ArgumentException("Product line is required.");
        }
    }

    private static void ValidateCategory(ProductCategory category)
    {
        if(category == null)
        {
            throw new ArgumentException("Product category is required.");
        }
    }

    private static void ValidatePrice(decimal price)
    {
        if(price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }
    }
}
