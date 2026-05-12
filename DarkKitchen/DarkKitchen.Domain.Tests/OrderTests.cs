using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class OrderTests
{
    private Address _address = null!;
    private Guid _clientId;
    private List<OrderItem> _items = null!;

    [TestInitialize]
    public void Setup()
    {
        _address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _items = [new OrderItem(Guid.NewGuid(), 1, 100m)];
        _clientId = Guid.NewGuid();
    }

    [TestMethod]
    public void Constructor_WhenCreated_ShouldSetPropertiesAndPendingState()
    {
        var shippingCost = 150m;
        var order = new Order(_clientId, _address, "Express", _items, shippingCost);

        Assert.AreEqual(OrderState.Pending, order.State);
        Assert.AreEqual(shippingCost, order.ShippingCost);
        Assert.AreEqual(_clientId, order.ClientId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithoutItems_ShouldThrowArgumentException()
    {
        new Order(_clientId, _address, "Express", [], 0m);
    }

    [TestMethod]
    public void Subtotal_ShouldCalculateCorrectly()
    {
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        Assert.AreEqual(100m, order.Subtotal);
    }

    [TestMethod]
    public void Total_ShouldIncludeSubtotalPlusTaxPlusShipping()
    {
        var shippingCost = 50m;
        var order = new Order(_clientId, _address, "Express", _items, shippingCost);

        Assert.AreEqual(172m, order.Total);
    }

    [TestMethod]
    public void TransitionTo_ShouldUpdateStateAndLastTransitionDate()
    {
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        DateTime originalDate = order.LastTransitionDate;

        Thread.Sleep(10);
        order.TransitionTo(OrderState.Prepared);

        Assert.AreEqual(OrderState.Prepared, order.State);
        Assert.IsTrue(order.LastTransitionDate > originalDate);
    }

    [TestMethod]
    public void Subtotal_WithDiscountedItems_ShouldReflectDiscounts()
    {
        var items = new List<OrderItem> { new(Guid.NewGuid(), 2, 100m, 10m) };
        var order = new Order(_clientId, _address, "Express", items, 0m);

        Assert.AreEqual(180m, order.Subtotal);
    }

    [TestMethod]
    public void AssignOrderNumber_ShouldSetOrderNumber()
    {
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        order.AssignOrderNumber(42);
        Assert.AreEqual(42, order.OrderNumber);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AssignOrderNumber_WhenAlreadyAssigned_ShouldThrow()
    {
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        order.AssignOrderNumber(1);
        order.AssignOrderNumber(2);
    }
}
