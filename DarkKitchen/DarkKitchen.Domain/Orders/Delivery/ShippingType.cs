using System.Diagnostics.CodeAnalysis;

namespace DarkKitchen.Domain.Orders.Delivery;

public class ShippingType
{
    [ExcludeFromCodeCoverage]
    protected ShippingType()
    {
        Name = null!;
    }

    public ShippingType(string name, decimal cost)
    {
        ValidateName(name);
        ValidateCost(cost);

        Id = Guid.NewGuid();
        Name = name;
        Cost = cost;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Cost { get; private set; }

    public void UpdateDetails(string name, decimal cost)
    {
        ValidateName(name);
        ValidateCost(cost);

        Name = name;
        Cost = cost;
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del tipo de envío es obligatorio.");
        }
    }

    private static void ValidateCost(decimal cost)
    {
        if(cost < 0)
        {
            throw new ArgumentException("El costo del tipo de envío no puede ser negativo.");
        }
    }
}
