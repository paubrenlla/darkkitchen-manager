using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Products;

public class Product
{
    private const int MinImages = 1;
    private const int MaxImages = 3;
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProductLine Line { get; private set; }
    public ProductCategory Category { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    private readonly List<ProductImage> _images;

    [ExcludeFromCodeCoverage]
    protected Product()
    {
        Code = null!;
        Name = null!;
        Description = null!;
        Line = null!;
        Category = null!;
        _images = [];
    }

    public Product(string code, string name, string description, ProductLine line, ProductCategory category,
        decimal price, List<ProductImage> images)
    {
        ValidateCode(code);
        ValidateName(name);
        ValidateDescription(description);
        ValidateLine(line);
        ValidateCategory(category);
        ValidatePrice(price);
        ValidateImages(images);

        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        Description = description;
        Line = line;
        Category = category;
        Price = price;
        IsActive = true;
        _images = new List<ProductImage>(images);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
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

    private static void ValidateImages(List<ProductImage> images)
    {
        if(images == null || images.Count < MinImages || images.Count > MaxImages)
        {
            throw new ArgumentException($"Product must have between {MinImages} and {MaxImages} images.");
        }
    }

    public void UpdateDetails(string name, string description, ProductLine line, ProductCategory category,
        decimal price, List<ProductImage> images)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidateLine(line);
        ValidateCategory(category);
        ValidatePrice(price);
        ValidateImages(images);

        Name = name;
        Description = description;
        Line = line;
        Category = category;
        Price = price;
        _images.Clear();
        _images.AddRange(images);
    }

    public Product Clone()
    {
        var clone = new Product(Code, Name, Description, Line, Category, Price, new List<ProductImage>(_images));
        typeof(Product).GetProperty(nameof(Id))!.SetValue(clone, Id);

        if(!IsActive)
        {
            clone.Deactivate();
        }

        return clone;
    }
}
