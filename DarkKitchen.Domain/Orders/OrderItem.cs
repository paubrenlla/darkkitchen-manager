namespace DarkKitchen.Domain.Orders;

public class OrderItem
{
    protected OrderItem()
    {
    }

    public OrderItem(Guid productId, int quantity, decimal price)
    {
        if(quantity <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor a cero.");
        }

        if(price < 0)
        {
            throw new ArgumentException("El precio no puede ser negativo.");
        }

        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
}
