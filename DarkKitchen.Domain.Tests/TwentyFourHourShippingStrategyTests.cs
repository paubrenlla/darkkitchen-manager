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
}
