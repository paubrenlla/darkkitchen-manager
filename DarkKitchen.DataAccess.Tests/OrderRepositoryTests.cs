using DarkKitchen.Domain.Orders;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class OrderRepositoryTests
{
    private InMemoryOrderRepository _repository = null!;
    private Address _address = null!;
    private List<OrderItem> _items = null!;
    private Guid _clientId;

    [TestInitialize]
    public void Setup()
    {
        _repository = new InMemoryOrderRepository();
        _address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _items = [new OrderItem(Guid.NewGuid(), 1, 100m)];
        _clientId = Guid.NewGuid();
    }

    [TestMethod]
    public void Add_ShouldAssignOrderNumberAndStore()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);

        _repository.Add(order);

        var found = _repository.GetById(order.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(1, found.OrderNumber);
    }
}
