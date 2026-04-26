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
}
