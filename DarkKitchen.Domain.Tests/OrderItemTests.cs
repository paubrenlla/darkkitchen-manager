using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

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

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateItem_WithNegativeQuantity_ShouldThrowException()
    {
        new OrderItem(Guid.NewGuid(), -1, 100);
    }

    [TestMethod]
    public void CalculateItemTotal_WithValidQuantityAndPrice_ShouldReturnCorrectTotal()
    {
        var item = new OrderItem(Guid.NewGuid(), 3, 50m);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(150m, total);
    }

    [TestMethod]
    public void CalculateItemTotal_WithDecimalPrice_ShouldCalculateCorrectly()
    {
        var item = new OrderItem(Guid.NewGuid(), 2, 99.99m);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(199.98m, total);
    }

    [TestMethod]
    public void CalculateItemTotal_WithOneQuantity_ShouldReturnPrice()
    {
        var price = 125.50m;
        var item = new OrderItem(Guid.NewGuid(), 1, price);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(price, total);
    }

    [TestMethod]
    public void CalculateItemTotal_WithZeroPrice_ShouldReturnZero()
    {
        var item = new OrderItem(Guid.NewGuid(), 5, 0m);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(0m, total);
    }

    [TestMethod]
    public void CreateItem_WithDiscount_ShouldSetDiscountPercentage()
    {
        var item = new OrderItem(Guid.NewGuid(), 2, 100m, 10m, "Black Friday");

        Assert.AreEqual(10m, item.DiscountPercentage);
        Assert.AreEqual("Black Friday", item.AppliedPromotionName);
    }

    [TestMethod]
    public void CreateItem_WithNegativeDiscount_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new OrderItem(Guid.NewGuid(), 1, 100m, -5m));
    }

    [TestMethod]
    public void CreateItem_WithDiscountOverHundred_ShouldThrowArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new OrderItem(Guid.NewGuid(), 1, 100m, 101m));
    }

    [TestMethod]
    public void CalculateItemTotal_WithDiscount_ShouldApplyDiscountCorrectly()
    {
        var item = new OrderItem(Guid.NewGuid(), 2, 100m, 10m);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(180m, total);
    }

    [TestMethod]
    public void CalculateItemTotal_WithFullDiscount_ShouldReturnZero()
    {
        var item = new OrderItem(Guid.NewGuid(), 3, 50m, 100m);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(0m, total);
    }

    [TestMethod]
    public void CalculateItemTotal_WithNoDiscount_ShouldReturnFullPrice()
    {
        var item = new OrderItem(Guid.NewGuid(), 2, 100m);

        var total = item.CalculateItemTotal();

        Assert.AreEqual(200m, total);
    }

    [TestMethod]
    public void CreateItem_WithNoDiscountArguments_ShouldDefaultToZeroAndNullPromotion()
    {
        var item = new OrderItem(Guid.NewGuid(), 1, 100m);

        Assert.AreEqual(0m, item.DiscountPercentage);
        Assert.IsNull(item.AppliedPromotionName);
    }
}
