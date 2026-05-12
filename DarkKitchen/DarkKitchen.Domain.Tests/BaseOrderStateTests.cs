using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Orders.States.Types;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class OrderStateTests
{
    private Order _order = null!;

    [TestInitialize]
    public void Setup()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        _order = new Order(Guid.NewGuid(), address, "Express", items, 0m);
    }

    [TestMethod]
    public void PendingState_State_ShouldReturnPending()
    {
        Assert.AreEqual(OrderState.Pending, new PendingState().State);
    }

    [TestMethod]
    public void PreparedState_State_ShouldReturnPrepared()
    {
        Assert.AreEqual(OrderState.Prepared, new PreparedState().State);
    }

    [TestMethod]
    public void ShippingState_State_ShouldReturnShipping()
    {
        Assert.AreEqual(OrderState.Shipping, new ShippingState().State);
    }

    [TestMethod]
    public void DeliveredState_State_ShouldReturnDelivered()
    {
        Assert.AreEqual(OrderState.Delivered, new DeliveredState().State);
    }

    [TestMethod]
    public void NotDeliveredState_State_ShouldReturnNotDelivered()
    {
        Assert.AreEqual(OrderState.NotDelivered, new NotDeliveredState().State);
    }

    [TestMethod]
    public void DelayedState_State_ShouldReturnDelayed()
    {
        Assert.AreEqual(OrderState.Delayed, new DelayedState().State);
    }

    [TestMethod]
    public void CancelledState_State_ShouldReturnCancelled()
    {
        Assert.AreEqual(OrderState.Cancelled, new CancelledState().State);
    }

    [TestMethod]
    public void PendingState_Delay_ShouldTransitionToDelayed()
    {
        new PendingState().Delay(_order);

        Assert.AreEqual(OrderState.Delayed, _order.State);
    }

    [TestMethod]
    public void PendingState_Prepare_ShouldTransitionToPrepared()
    {
        new PendingState().Prepare(_order);

        Assert.AreEqual(OrderState.Prepared, _order.State);
    }

    [TestMethod]
    public void PendingState_Cancel_ShouldTransitionToCancelled()
    {
        new PendingState().Cancel(_order);

        Assert.AreEqual(OrderState.Cancelled, _order.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PendingState_Ship_ShouldThrowInvalidOperationException()
    {
        new PendingState().Ship(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PendingState_Deliver_ShouldThrowInvalidOperationException()
    {
        new PendingState().Deliver(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PendingState_NotDelivered_ShouldThrowInvalidOperationException()
    {
        new PendingState().NotDelivered(_order);
    }

    [TestMethod]
    public void DelayedState_Prepare_ShouldTransitionToPrepared()
    {
        new DelayedState().Prepare(_order);

        Assert.AreEqual(OrderState.Prepared, _order.State);
    }

    [TestMethod]
    public void DelayedState_Cancel_ShouldTransitionToCancelled()
    {
        new DelayedState().Cancel(_order);

        Assert.AreEqual(OrderState.Cancelled, _order.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DelayedState_Delay_ShouldThrowInvalidOperationException()
    {
        new DelayedState().Delay(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DelayedState_Ship_ShouldThrowInvalidOperationException()
    {
        new DelayedState().Ship(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DelayedState_Deliver_ShouldThrowInvalidOperationException()
    {
        new DelayedState().Deliver(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DelayedState_NotDelivered_ShouldThrowInvalidOperationException()
    {
        new DelayedState().NotDelivered(_order);
    }

    // --- PreparedState tests ---

    [TestMethod]
    public void PreparedState_Ship_ShouldTransitionToShipping()
    {
        new PreparedState().Ship(_order);

        Assert.AreEqual(OrderState.Shipping, _order.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_Delay_ShouldThrowInvalidOperationException()
    {
        new PreparedState().Delay(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_Prepare_ShouldThrowInvalidOperationException()
    {
        new PreparedState().Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_Cancel_ShouldThrowInvalidOperationException()
    {
        new PreparedState().Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_Deliver_ShouldThrowInvalidOperationException()
    {
        new PreparedState().Deliver(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void PreparedState_NotDelivered_ShouldThrowInvalidOperationException()
    {
        new PreparedState().NotDelivered(_order);
    }

    [TestMethod]
    public void ShippingState_Deliver_ShouldTransitionToDelivered()
    {
        new ShippingState().Deliver(_order);

        Assert.AreEqual(OrderState.Delivered, _order.State);
    }

    [TestMethod]
    public void ShippingState_NotDelivered_ShouldTransitionToNotDelivered()
    {
        new ShippingState().NotDelivered(_order);

        Assert.AreEqual(OrderState.NotDelivered, _order.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ShippingState_Delay_ShouldThrowInvalidOperationException()
    {
        new ShippingState().Delay(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ShippingState_Prepare_ShouldThrowInvalidOperationException()
    {
        new ShippingState().Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ShippingState_Cancel_ShouldThrowInvalidOperationException()
    {
        new ShippingState().Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ShippingState_Ship_ShouldThrowInvalidOperationException()
    {
        new ShippingState().Ship(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Delay_ShouldThrowInvalidOperationException()
    {
        new DeliveredState().Delay(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Prepare_ShouldThrowInvalidOperationException()
    {
        new DeliveredState().Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Cancel_ShouldThrowInvalidOperationException()
    {
        new DeliveredState().Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Ship_ShouldThrowInvalidOperationException()
    {
        new DeliveredState().Ship(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_Deliver_ShouldThrowInvalidOperationException()
    {
        new DeliveredState().Deliver(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void DeliveredState_NotDelivered_ShouldThrowInvalidOperationException()
    {
        new DeliveredState().NotDelivered(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Delay_ShouldThrowInvalidOperationException()
    {
        new NotDeliveredState().Delay(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Prepare_ShouldThrowInvalidOperationException()
    {
        new NotDeliveredState().Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Cancel_ShouldThrowInvalidOperationException()
    {
        new NotDeliveredState().Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Ship_ShouldThrowInvalidOperationException()
    {
        new NotDeliveredState().Ship(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_Deliver_ShouldThrowInvalidOperationException()
    {
        new NotDeliveredState().Deliver(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void NotDeliveredState_NotDelivered_ShouldThrowInvalidOperationException()
    {
        new NotDeliveredState().NotDelivered(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Delay_ShouldThrowInvalidOperationException()
    {
        new CancelledState().Delay(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Prepare_ShouldThrowInvalidOperationException()
    {
        new CancelledState().Prepare(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Cancel_ShouldThrowInvalidOperationException()
    {
        new CancelledState().Cancel(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Ship_ShouldThrowInvalidOperationException()
    {
        new CancelledState().Ship(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_Deliver_ShouldThrowInvalidOperationException()
    {
        new CancelledState().Deliver(_order);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CancelledState_NotDelivered_ShouldThrowInvalidOperationException()
    {
        new CancelledState().NotDelivered(_order);
    }
}
