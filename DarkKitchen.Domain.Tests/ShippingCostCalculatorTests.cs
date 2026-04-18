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

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithNegativeExpressPrice_ShouldThrowException()
    {
        new ShippingCostCalculator(-50m, 100m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithNegative24hsPrice_ShouldThrowException()
    {
        new ShippingCostCalculator(200m, -50m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CalculateShippingCost_WithInvalidDeliveryType_ShouldThrowException()
    {
        var calculator = new ShippingCostCalculator(200m, 100m);

        calculator.CalculateShippingCost((DeliveryType)999);
    }

    [TestMethod]
    public void CustomCalculator_CanBeSet_OnOrder()
    {
        var customCalculator = new ShippingCostCalculator(500m, 50m);
        Order.SetShippingCostCalculator(customCalculator);

        var address = new Address("Test", "123", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        Assert.AreEqual(500m, order.ShippingCost);
    }

    [TestMethod]
    public void Order_WithoutCalculator_ShippingCostShouldBeZero()
    {
        Order.SetShippingCostCalculator(null!);

        var address = new Address("Test", "123", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        Assert.AreEqual(0m, order.ShippingCost);
    }
}
