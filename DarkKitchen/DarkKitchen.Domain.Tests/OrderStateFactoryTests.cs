using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class OrderStateFactoryTests
{
    [DataTestMethod]
    [DataRow(OrderState.Pending, typeof(PendingState))]
    [DataRow(OrderState.Prepared, typeof(PreparedState))]
    [DataRow(OrderState.Shipping, typeof(ShippingState))]
    [DataRow(OrderState.Delivered, typeof(DeliveredState))]
    [DataRow(OrderState.NotDelivered, typeof(NotDeliveredState))]
    [DataRow(OrderState.Cancelled, typeof(CancelledState))]
    public void Create_WithValidState_ShouldReturnCorrectType(OrderState state, Type expectedType)
    {
        IOrderState result = OrderStateFactory.Create(state);

        Assert.IsInstanceOfType(result, expectedType);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_WithInvalidState_ShouldThrowException()
    {
        OrderStateFactory.Create((OrderState)999);
    }
}
