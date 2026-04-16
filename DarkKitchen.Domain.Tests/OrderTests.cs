using System.Reflection;
using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class OrderTests
{
    private Order _order = null!;

    [TestInitialize]
    public void Setup()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        _order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);
    }

    [TestMethod]
    public void Constructor_WhenCreated_ShouldSetPendingState()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        var clientId = Guid.NewGuid();

        var order = new Order(clientId, address, DeliveryType.Express, items);

        Assert.AreEqual(OrderState.Pending, order.State);
        Assert.IsInstanceOfType(order.CurrentState, typeof(PendingState));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Order_Constructor_WithoutItems_ShouldThrowException()
    {
        var address = new Address("Cuareim", "1451", null, "Montevideo", "Uruguay");
        new Order(Guid.NewGuid(), address, DeliveryType.Express, []);
    }

    [TestMethod]
    public void Prepare_WhenPending_ShouldChangeStateToPrepared()
    {
        _order.Prepare();
        Assert.AreEqual(OrderState.Prepared, _order.State);
    }

    [TestMethod]
    public void Cancel_WhenPending_ShouldChangeStateToCancelled()
    {
        _order.Cancel();
        Assert.AreEqual(OrderState.Cancelled, _order.State);
    }

    [TestMethod]
    public void Ship_WhenPrepared_ShouldChangeStateToShipping()
    {
        _order.Prepare();
        _order.Ship();
        Assert.AreEqual(OrderState.Shipping, _order.State);
    }

    [TestMethod]
    public void Deliver_WhenShipping_ShouldChangeStateToDelivered()
    {
        _order.Prepare();
        _order.Ship();
        _order.Deliver();
        Assert.AreEqual(OrderState.Delivered, _order.State);
    }

    [TestMethod]
    public void NotDelivered_WhenShipping_ShouldChangeStateToNotDelivered()
    {
        _order.Prepare();
        _order.Ship();
        _order.NotDelivered();
        Assert.AreEqual(OrderState.NotDelivered, _order.State);
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
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        IOrderState state1 = order.CurrentState;
        IOrderState state2 = order.CurrentState;

        Assert.AreSame(state1, state2, "Debería devolver la misma instancia de estado");
    }

    [TestMethod]
    public void Properties_ShouldReturnExpectedValues()
    {
        Assert.AreNotEqual(Guid.Empty, _order.Id);
        Assert.IsNotNull(_order.DeliveryAddress);
        Assert.AreEqual(DeliveryType.Express, _order.Type);
        Assert.IsTrue(_order.CreatedAt <= DateTime.Now);
        Assert.AreEqual(0, _order.OrderNumber);
        Assert.AreNotEqual(Guid.Empty, _order.ClientId);
    }

    [TestMethod]
    public void Items_Property_ShouldReturnReadOnlyList()
    {
        Assert.IsNotNull(_order.Items);
        Assert.AreEqual(1, _order.Items.Count);
        Assert.IsInstanceOfType(_order.Items, typeof(IReadOnlyCollection<OrderItem>));
    }

    [TestMethod]
    public void CurrentState_WhenInternalStateIsNull_ShouldHydrateFromFactory()
    {
        FieldInfo? field = typeof(Order).GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(_order, null);

        IOrderState state = _order.CurrentState;

        Assert.IsNotNull(state);
        Assert.AreEqual(OrderState.Pending, state.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDelivered_WhenInInvalidState_ShouldThrowException()
    {
        _order.NotDelivered();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Prepare_WhenAlreadyPrepared_ShouldThrowException()
    {
        _order.Prepare();
        _order.Prepare();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Ship_WhenAlreadyShipped_ShouldThrowException()
    {
        _order.Prepare();
        _order.Ship();
        _order.Ship();
    }
}
