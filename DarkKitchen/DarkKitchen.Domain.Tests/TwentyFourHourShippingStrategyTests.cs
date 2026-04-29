using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class TwentyFourHoursShippingStrategyTests
{
    [TestMethod]
    public void CanHandle_TwentyFourHoursDeliveryType_ReturnsTrue()
    {
        var strategy = new TwentyFourHoursShippingStrategy(50m);

        Assert.IsTrue(strategy.CanHandle(DeliveryType.TwentyFourHours));
    }

    [TestMethod]
    public void CanHandle_ExpressDeliveryType_ReturnsFalse()
    {
        var strategy = new TwentyFourHoursShippingStrategy(50m);

        Assert.IsFalse(strategy.CanHandle(DeliveryType.Express));
    }

    [TestMethod]
    public void Calculate_ShouldReturnConfiguredCost()
    {
        var strategy = new TwentyFourHoursShippingStrategy(80m);

        Assert.AreEqual(80m, strategy.Calculate());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithNegativeCost_ShouldThrow()
    {
        new TwentyFourHoursShippingStrategy(-1m);
    }

    [TestMethod]
    public void Constructor_WithZeroCost_ShouldBeValid()
    {
        var strategy = new TwentyFourHoursShippingStrategy(0m);

        Assert.AreEqual(0m, strategy.Calculate());
    }
}
