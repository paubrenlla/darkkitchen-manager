using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Tests;

[TestClass]
public class OrderTests
{
    [TestMethod]
    public void Constructor_WhenCreated_ShouldSetPendingState()
    {
        var address = new Address("Av Italia", "1111", null, "Montevideo", "Uruguay");
        var clientId = Guid.NewGuid();

        var order = new Order(clientId, address, DeliveryType.Express);

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
}
