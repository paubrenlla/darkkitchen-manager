using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Tests;

[TestClass]
public class OrderStateFactoryTests
{
    [DataTestMethod]
    [DataRow("Pending", typeof(PendingState))]
    [DataRow("Prepared", typeof(PreparedState))]
    [DataRow("OnTheWay", typeof(ShippingState))]
    [DataRow("Delivered", typeof(DeliveredState))]
    [DataRow("NotDelivered", typeof(NotDeliveredState))]
    [DataRow("Cancelled", typeof(CancelledState))]
    public void Create_WithValidStateName_ShouldReturnCorrectType(string stateName, Type expectedType)
    {
        var result = OrderStateFactory.Create(stateName);

        Assert.IsInstanceOfType(result, expectedType);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_WithInvalidStateName_ShouldThrowException()
    {
        OrderStateFactory.Create("EstadoQueNoExiste");
    }
}
