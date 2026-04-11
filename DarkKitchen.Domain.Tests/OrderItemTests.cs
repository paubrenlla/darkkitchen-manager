using DarkKitchen.Domain.Orders;

[TestClass]
public class OrderItemTests
{
    [TestMethod]
    public void CreateItem_WithValidData_ShouldSetProperties()
    {
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, 2, 150.5m);

        Assert.AreEqual(productId, item.ProductId);
        Assert.AreEqual(2, item.Quantity);
        Assert.AreEqual(150.5m, item.Price);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateItem_WithZeroQuantity_ShouldThrowException()
    {
        new OrderItem(Guid.NewGuid(), 0, 100);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateItem_WithNegativePrice_ShouldThrowException()
    {
        new OrderItem(Guid.NewGuid(), 5, -5);
    }
}
