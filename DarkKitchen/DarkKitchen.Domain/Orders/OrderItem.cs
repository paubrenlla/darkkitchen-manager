namespace DarkKitchen.Domain.Orders;

public class OrderItem
{
    public OrderItem(Guid productId, int quantity, decimal price, decimal discountPercentage = 0,
        string? appliedPromotionName = null)
    {
        if(quantity <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor a cero.");
        }

        if(price < 0)
        {
            throw new ArgumentException("El precio no puede ser negativo.");
        }

        if(discountPercentage is < 0 or > 100)
        {
            throw new ArgumentException("El descuento no es válido.");
        }

        ProductId = productId;
        Quantity = quantity;
        Price = price;
        DiscountPercentage = discountPercentage;
        AppliedPromotionName = appliedPromotionName;
        Id = Guid.NewGuid();
    }

    public Guid ProductId { get; private set; }
    public int Quantity { get; }
    public decimal Price { get; }
    public Guid Id { get; private set; }

    public decimal DiscountPercentage { get; }
    public string? AppliedPromotionName { get; private set; }

    public decimal CalculateItemTotal()
    {
        var rawTotal = Price * Quantity;
        var discountAmount = rawTotal * (DiscountPercentage / 100m);
        return rawTotal - discountAmount;
    }
}
