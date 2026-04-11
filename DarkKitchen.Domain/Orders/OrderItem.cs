namespace DarkKitchen.Domain.Orders;

public class OrderItem
{
    public OrderItem(Guid productId, int quantity, decimal price)
    {
        if(quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if(price < 0)
        {
            throw new ArgumentException("Price cannot be less than zero.");
        }

        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
}
