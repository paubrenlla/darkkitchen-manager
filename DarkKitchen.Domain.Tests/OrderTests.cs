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
    public void Constructor_WhenCreated_ShouldSetPendingState()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);

        Assert.AreEqual(OrderState.Pending, order.State);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithoutItems_ShouldThrowArgumentException()
    {
        new Order(_clientId, _address, DeliveryType.Express, []);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithNullItems_ShouldThrowArgumentException()
    {
        new Order(_clientId, _address, DeliveryType.Express, null!);
    }

    [TestMethod]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        DateTime before = DateTime.Now;
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        DateTime after = DateTime.Now;

        Assert.AreNotEqual(Guid.Empty, order.Id);
        Assert.AreEqual(_clientId, order.ClientId);
        Assert.AreEqual(_address, order.DeliveryAddress);
        Assert.AreEqual(DeliveryType.Express, order.Type);
        Assert.IsTrue(order.CreatedAt >= before && order.CreatedAt <= after);
        Assert.IsNull(order.OrderNumber);
    }

    [TestMethod]
    public void Constructor_ShouldSetLastTransitionDateToNow()
    {
        DateTime before = DateTime.Now;
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        DateTime after = DateTime.Now;

        Assert.IsTrue(order.LastTransitionDate >= before && order.LastTransitionDate <= after);
    }

    [TestMethod]
    public void Items_ShouldReturnReadOnlyCollectionWithExpectedItems()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);

        Assert.IsNotNull(order.Items);
        Assert.AreEqual(1, order.Items.Count);
        Assert.IsInstanceOfType(order.Items, typeof(IReadOnlyCollection<OrderItem>));
    }

    [TestMethod]
    public void AssignOrderNumber_ShouldSetOrderNumber()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);

        order.AssignOrderNumber(42);

        Assert.AreEqual(42, order.OrderNumber);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void AssignOrderNumber_WhenAlreadyAssigned_ShouldThrowInvalidOperationException()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        order.AssignOrderNumber(1);

        order.AssignOrderNumber(2);
    }

    [TestMethod]
    public void Subtotal_ShouldCalculateCorrectly()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);

        Assert.AreEqual(100m, order.Subtotal);
    }
    }
}
