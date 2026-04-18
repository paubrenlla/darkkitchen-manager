using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ShippingCostCalculatorTests
{
    [TestMethod]
    public void CalculateShippingCost_Express_ShouldReturnExpressPrice()
    {
        var calculator = new ShippingCostCalculator(250m, 100m);

        var cost = calculator.CalculateShippingCost(DeliveryType.Express);

        Assert.AreEqual(250m, cost);
    }

    [TestMethod]
    public void CalculateShippingCost_TwentyFourHours_ShouldReturnStandardPrice()
    {
        var calculator = new ShippingCostCalculator(200m, 80m);

        var cost = calculator.CalculateShippingCost(DeliveryType.TwentyFourHours);

        Assert.AreEqual(80m, cost);
    }
}
