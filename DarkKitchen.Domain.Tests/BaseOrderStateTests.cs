using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Tests;

[TestClass]
public class OrderStateTests
{
    private Order _order = null!;

    [TestInitialize]
    public void Setup()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _order = new Order(address);
    }

    [TestMethod]
    public void BaseOrderState_WhenCreated_ShouldSetTransitionDateToNow()
    {
        DateTime before = DateTime.Now;
        var state = new PendingState();
        DateTime after = DateTime.Now;

        Assert.IsTrue(state.TransitionDate >= before);
        Assert.IsTrue(state.TransitionDate <= after);
    }

    [TestMethod]
    public void PendingState_Name_ShouldReturnPendiente()
    {
        var state = new PendingState();
        Assert.AreEqual("Pendiente", state.Name);
    }

    [TestMethod]
    public void PreparedOrderState_Name_ShouldReturnPrepared()
    {
        var state = new PreparedState();
        Assert.AreEqual("Preparado", state.Name);
    }

    [TestMethod]
    public void CancelledOrderState_Name_ShouldReturnCancelled()
    {
        var state = new CancelledState();
        Assert.AreEqual("Cancelado", state.Name);
    }

    [TestMethod]
    public void OnItsWayOrderState_Name_ShouldReturnOnItsWay()
    {
        var state = new ShippingState();
        Assert.AreEqual("En camino", state.Name);
    }

    [TestMethod]
    public void DeliveredOrderState_Name_ShouldReturnDelivered()
    {
        var state = new DeliveredState();
        Assert.AreEqual("Entregado", state.Name);
    }

    [TestMethod]
    public void NotDeliveredOrderState_Name_ShouldReturnNotDelivered()
    {
        var state = new NotDeliveredState();
        Assert.AreEqual("No entregado", state.Name);
    }

    [TestMethod]
    public void PendingState_Prepare_ShouldTransitionToPreparedState()
    {
        var state = new PendingState();
        state.Prepare(_order);
        Assert.AreEqual("Preparado", _order.StateName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_Prepare_ShouldThrowInvalidOperationException()
    {
        var state = new PreparedState();
        state.Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Prepare_ShouldThrowInvalidOperationException()
    {
        var state = new CancelledState();
        state.Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ShippingState_Prepare_ShouldThrowInvalidOperationException()
    {
        var state = new ShippingState();
        state.Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Prepare_ShouldThrowInvalidOperationException()
    {
        var state = new DeliveredState();
        state.Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Prepare_ShouldThrowInvalidOperationException()
    {
        var state = new NotDeliveredState();
        state.Prepare(_order);
    }

    [TestMethod]
    public void PendingState_Cancel_ShouldTransitionToCancelledState()
    {
        var state = new PendingState();
        state.Cancel(_order);
        Assert.AreEqual("Cancelado", _order.StateName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_Cancel_ShouldThrowInvalidOperationException()
    {
        var state = new PreparedState();
        state.Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Cancel_ShouldThrowInvalidOperationException()
    {
        var state = new CancelledState();
        state.Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Cancel_ShouldThrowInvalidOperationException()
    {
        var state = new DeliveredState();
        state.Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Cancel_ShouldThrowInvalidOperationException()
    {
        var state = new NotDeliveredState();
        state.Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ShippingState_Cancel_ShouldThrowInvalidOperationException()
    {
        var state = new ShippingState();
        state.Cancel(_order);
    }
}
