using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Tests;

[TestClass]
public class OrderTests
{
    private Order _order = null!;

    [TestInitialize]
    public void Setup()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        _order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);
    }

    [TestMethod]
    public void Constructor_WhenCreated_ShouldSetPendingState()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");

        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };

        var clientId = Guid.NewGuid();

        var order = new Order(clientId, address, DeliveryType.Express, items);

        Assert.AreEqual("Pending", order.StateName);
        Assert.IsInstanceOfType(order.CurrentState, typeof(PendingState));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Order_Constructor_WithoutItems_ShouldThrowException()
    {
        var address = new Address("Cuareim", "1451", null, "Montevideo", "Uruguay");
        new Order(Guid.NewGuid(), address, DeliveryType.Express, new List<OrderItem>());
    }

    [TestMethod]
    public void Prepare_WhenPending_ShouldChangeStateToPrepared()
    {
        _order.Prepare();
        Assert.AreEqual("Prepared", _order.StateName);
    }

    [TestMethod]
    public void Cancel_WhenPending_ShouldChangeStateToCancelled()
    {
        _order.Cancel();
        Assert.AreEqual("Cancelled", _order.StateName);
    }

    [TestMethod]
    public void Ship_WhenPrepared_ShouldChangeStateToShipping()
    {
        _order.Prepare();
        _order.Ship();
        Assert.AreEqual("Shipping", _order.StateName);
    }

    [TestMethod]
    public void Deliver_WhenShipping_ShouldChangeStateToDelivered()
    {
        _order.Prepare();
        _order.Ship();
        _order.Deliver();
        Assert.AreEqual("Delivered", _order.StateName);
    }

    [TestMethod]
    public void NotDelivered_WhenShipping_ShouldChangeStateToNotDelivered()
    {
        _order.Prepare();
        _order.Ship();
        _order.NotDelivered();
        Assert.AreEqual("NotDelivered", _order.StateName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Deliver_WhenPending_ShouldThrowException()
    {
        _order.Deliver();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Ship_WhenPending_ShouldThrowException()
    {
        _order.Ship();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Cancel_WhenPrepared_ShouldThrowException()
    {
        _order.Prepare();
        _order.Cancel();
    }

    [TestMethod]
    public void CurrentState_ShouldReturnSameInstance_WhenStateHasNotChanged()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        IOrderState state1 = order.CurrentState;
        IOrderState state2 = order.CurrentState;

        Assert.AreSame(state1, state2, "Debería devolver la misma instancia de estado");
    }
}
