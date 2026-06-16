using DarkKitchen.Domain.Orders.Delivery;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class ShippingTypeTests
{
    [TestMethod]
    public void CreateShippingType_WithValidData_ShouldSucceed()
    {
        var shippingType = new ShippingType("Express Día", 150m);

        Assert.AreEqual("Express Día", shippingType.Name);
        Assert.AreEqual(150m, shippingType.Cost);
        Assert.AreNotEqual(Guid.Empty, shippingType.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateShippingType_WithEmptyName_ShouldThrow()
    {
        new ShippingType(string.Empty, 100m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateShippingType_WithNegativeCost_ShouldThrow()
    {
        new ShippingType("Express", -1m);
    }

    [TestMethod]
    public void CreateShippingType_WithZeroCost_ShouldSucceed()
    {
        var shippingType = new ShippingType("Gratis", 0m);

        Assert.AreEqual(0m, shippingType.Cost);
    }

    [TestMethod]
    public void UpdateDetails_ShouldChangNameAndCost()
    {
        var shippingType = new ShippingType("Express", 150m);

        shippingType.UpdateDetails("Express Premium", 250m);

        Assert.AreEqual("Express Premium", shippingType.Name);
        Assert.AreEqual(250m, shippingType.Cost);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateDetails_WithEmptyName_ShouldThrow()
    {
        var shippingType = new ShippingType("Express", 150m);

        shippingType.UpdateDetails(string.Empty, 150m);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateDetails_WithNegativeCost_ShouldThrow()
    {
        var shippingType = new ShippingType("Express", 150m);

        shippingType.UpdateDetails("Express", -1m);
    }
}
