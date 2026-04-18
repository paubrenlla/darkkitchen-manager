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
}
