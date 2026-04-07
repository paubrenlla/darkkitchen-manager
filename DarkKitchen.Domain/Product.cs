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
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        Description = description;
        Line = line;
        Category = category;
        Price = price;
        IsActive = true;
    }
}
