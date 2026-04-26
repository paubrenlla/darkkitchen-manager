using DarkKitchen.Domain.Orders;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class OrderRepositoryTests
{
    private Address _address = null!;
    private Guid _clientId;
    private List<OrderItem> _items = null!;
    private InMemoryOrderRepository _repository = null!;

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
        var order = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);

        _repository.Add(order);

        Order? found = _repository.GetById(order.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(1, found.OrderNumber);
    }

    [TestMethod]
    public void Add_MultiplOrders_ShouldIncrementOrderNumber()
    {
        var order1 = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        var order2 = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);

        _repository.Add(order1);
        _repository.Add(order2);

        Assert.AreEqual(1, order1.OrderNumber);
        Assert.AreEqual(2, order2.OrderNumber);
    }

    [TestMethod]
    public void GetById_NonExistent_ShouldReturnNull()
    {
        Order? result = _repository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByClient_ShouldFilterByClientId()
    {
        var otherClientId = Guid.NewGuid();
        var order1 = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        var order2 = new Order(otherClientId, _address, DeliveryType.Express, _items, 0m);

        _repository.Add(order1);
        _repository.Add(order2);

        var result = _repository.GetByClient(_clientId).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_clientId, result[0].ClientId);
    }

    [TestMethod]
    public void GetByClient_WithStateFilter_ShouldFilterByState()
    {
        var order1 = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        var order2 = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        order2.TransitionTo(OrderState.Prepared);

        _repository.Add(order1);
        _repository.Add(order2);

        var result = _repository.GetByClient(_clientId, state: "Prepared").ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByStatus_ShouldFilterByDateRange()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        _repository.Add(order);

        DateTime from = DateTime.Now.AddHours(-1);
        DateTime to = DateTime.Now.AddHours(1);
        var result = _repository.GetByStatus(from, to).ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByStatus_ShouldFilterByState()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        order.TransitionTo(OrderState.Prepared);
        _repository.Add(order);

        var order2 = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        _repository.Add(order2);

        DateTime from = DateTime.Now.AddHours(-1);
        DateTime to = DateTime.Now.AddHours(1);
        var result = _repository.GetByStatus(from, to, "Prepared").ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByStatus_ShouldFilterByCity()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var order = new Order(_clientId, address, DeliveryType.Express, _items, 0m);
        _repository.Add(order);

        var address2 = new Address("Sarandi", "1234", null, "Rosario", "Uruguay");
        var order2 = new Order(_clientId, address2, DeliveryType.Express, _items, 0m);
        _repository.Add(order2);

        DateTime from = DateTime.Now.AddHours(-1);
        DateTime to = DateTime.Now.AddHours(1);
        var result = _repository.GetByStatus(from, to, null, "Rosario").ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void Update_ShouldPersistChanges()
    {
        var order = new Order(_clientId, _address, DeliveryType.Express, _items, 0m);
        _repository.Add(order);

        order.TransitionTo(OrderState.Prepared);
        _repository.Update(order);

        Order? found = _repository.GetById(order.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(OrderState.Prepared, found.State);
    }
}
