namespace DarkKitchen.Domain;

public class ProductLine
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public ProductLine(string name)
    {
        ValidateName(name);
        Id = Guid.NewGuid();
        Name = name;
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product line name is required.");
        }
    }
}
