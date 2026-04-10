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
        var state = new ShippedState();
        Assert.AreEqual("En camino", state.Name);
    }
}
