using DarkKitchen.Domain.Orders;

namespace DarkKitchen.Domain.Tests;

[TestClass]
public class OrderTests
{
    [TestMethod]
    public void Constructor_WhenCreated_ShouldSetPendingState()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        var clientId = Guid.NewGuid();

        var order = new Order(clientId, address, DeliveryType.Express, items);

        Assert.AreEqual(OrderState.Pending, order.State);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Constructor_WithoutItems_ShouldThrowException()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        new Order(Guid.NewGuid(), address, DeliveryType.Express, []);
    }

    [TestMethod]
    public void Properties_ShouldReturnExpectedValues()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        var clientId = Guid.NewGuid();

        var order = new Order(clientId, address, DeliveryType.Express, items);

        Assert.AreNotEqual(Guid.Empty, order.Id);
        Assert.IsNotNull(order.DeliveryAddress);
        Assert.AreEqual(DeliveryType.Express, order.Type);
        Assert.IsTrue(order.CreatedAt <= DateTime.Now);
        Assert.AreEqual(0, order.OrderNumber);
        Assert.AreEqual(clientId, order.ClientId);
    }

    [TestMethod]
    public void Items_Property_ShouldReturnReadOnlyList()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        Assert.IsNotNull(order.Items);
        Assert.AreEqual(1, order.Items.Count);
        Assert.IsInstanceOfType(order.Items, typeof(IReadOnlyCollection<OrderItem>));
    }

    [TestMethod]
    public void SetState_ShouldChangeOrderState()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        List<OrderItem> items = [new(Guid.NewGuid(), 1, 100m)];
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        order.SetState(OrderState.Prepared);

        Assert.AreEqual(OrderState.Prepared, order.State);
    }
}
