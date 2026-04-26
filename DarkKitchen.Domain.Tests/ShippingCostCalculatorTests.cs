using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ShippingCostCalculatorTests
{
    private ShippingCostCalculator _calculator = null!;

    [TestInitialize]
    public void Setup()
    {
        var strategies = new List<IShippingStrategy>
        {
            new ExpressShippingStrategy(150m), new TwentyFourHoursShippingStrategy(50m)
        };
        _calculator = new ShippingCostCalculator(strategies);
    }

    [TestMethod]
    public void CalculateShippingCost_Express_ShouldReturnExpressCost()
    {
        var result = _calculator.CalculateShippingCost(DeliveryType.Express);

        Assert.AreEqual(150m, result);
    }

    [TestMethod]
    public void CalculateShippingCost_TwentyFourHours_ShouldReturnTwentyFourHoursCost()
    {
        var result = _calculator.CalculateShippingCost(DeliveryType.TwentyFourHours);

        Assert.AreEqual(50m, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CalculateShippingCost_UnsupportedDeliveryType_ShouldThrow()
    {
        var emptyCalculator = new ShippingCostCalculator([]);

        emptyCalculator.CalculateShippingCost(DeliveryType.Express);
    }

    [TestMethod]
    public void CalculateShippingCost_WhenMultipleStrategiesRegistered_UsesCorrectOne()
    {
        Assert.AreEqual(150m, _calculator.CalculateShippingCost(DeliveryType.Express));
        Assert.AreEqual(50m, _calculator.CalculateShippingCost(DeliveryType.TwentyFourHours));
    }
}
