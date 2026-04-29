using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ExpressShippingStrategyTests
{
    [TestMethod]
    public void CanHandle_ExpressDeliveryType_ReturnsTrue()
    {
        var strategy = new ExpressShippingStrategy(100m);

        Assert.IsTrue(strategy.CanHandle(DeliveryType.Express));
    }

    [TestMethod]
    public void CanHandle_TwentyFourHoursDeliveryType_ReturnsFalse()
    {
        var strategy = new ExpressShippingStrategy(100m);

        Assert.IsFalse(strategy.CanHandle(DeliveryType.TwentyFourHours));
    }

    [TestMethod]
    public void Calculate_ShouldReturnConfiguredCost()
    {
        var strategy = new ExpressShippingStrategy(150m);

        Assert.AreEqual(150m, strategy.Calculate());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithNegativeCost_ShouldThrow()
    {
        new ExpressShippingStrategy(-1m);
    }

    [TestMethod]
    public void Constructor_WithZeroCost_ShouldBeValid()
    {
        var strategy = new ExpressShippingStrategy(0m);

        Assert.AreEqual(0m, strategy.Calculate());
    }
}
