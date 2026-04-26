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
}
