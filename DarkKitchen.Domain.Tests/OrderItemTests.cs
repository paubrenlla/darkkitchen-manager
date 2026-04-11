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
}
