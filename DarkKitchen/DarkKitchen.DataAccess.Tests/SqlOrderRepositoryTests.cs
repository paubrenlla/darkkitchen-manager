using DarkKitchen.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class SqlOrderRepositoryTests
{
    private DarkKitchenContext _context = null!;
    private SqlOrderRepository _repository = null!;
    private Guid _clientId;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _clientId = Guid.NewGuid();

        _repository = new SqlOrderRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private Order CreateOrder(Guid? clientId = null)
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        return new Order(
            clientId ?? _clientId,
            address,
            "Express",
            [new OrderItem(Guid.NewGuid(), 2, 100m)],
            150m);
    }

    [TestMethod]
    public void Add_ShouldPersistOrder()
    {
        var order = CreateOrder();

        _repository.Add(order);

        var result = _repository.GetById(order.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(_clientId, result.ClientId);
    }

    [TestMethod]
    public void Add_ShouldPersistItems()
    {
        var order = CreateOrder();

        _repository.Add(order);

        var result = _repository.GetById(order.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
    }

    [TestMethod]
    public void GetById_NonExisting_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldPersistStateChange()
    {
        var order = CreateOrder();
        _repository.Add(order);

        order.TransitionTo(OrderState.Prepared);
        _repository.Update(order);

        _context.ChangeTracker.Clear();
        var result = _repository.GetById(order.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(OrderState.Prepared, result.State);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExisting_Throws()
    {
        var order = CreateOrder();
        _repository.Update(order);
    }

    [TestMethod]
    public void GetByClient_ShouldFilterByClientId()
    {
        var otherClientId = Guid.NewGuid();
        _repository.Add(CreateOrder(_clientId));
        _repository.Add(CreateOrder(otherClientId));

        var result = _repository.GetByClient(_clientId).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_clientId, result[0].ClientId);
    }

    [TestMethod]
    public void GetByClient_WithStateFilter_ShouldFilter()
    {
        var order1 = CreateOrder();
        var order2 = CreateOrder();
        _repository.Add(order1);
        _repository.Add(order2);

        order2.TransitionTo(OrderState.Prepared);
        _repository.Update(order2);

        var result = _repository.GetByClient(_clientId, state: "Prepared").ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByStatus_ShouldFilterByDateRange()
    {
        var order = CreateOrder();
        _repository.Add(order);

        var from = DateTime.Now.AddHours(-1);
        var to = DateTime.Now.AddHours(1);
        var result = _repository.GetByStatus(from, to).ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByStatus_WithAddressFilter_ShouldFilter()
    {
        _repository.Add(CreateOrder());

        var from = DateTime.Now.AddHours(-1);
        var to = DateTime.Now.AddHours(1);

        var resultRivera = _repository.GetByStatus(from, to, address: "Rivera").ToList();
        var resultOtra = _repository.GetByStatus(from, to, address: "Otra calle").ToList();

        Assert.AreEqual(1, resultRivera.Count);
        Assert.AreEqual(0, resultOtra.Count);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllOrders()
    {
        _repository.Add(CreateOrder());
        _repository.Add(CreateOrder());

        var result = _repository.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetByClient_WithFromDate_ShouldFilterByFromDate()
    {
        var order = CreateOrder();
        _repository.Add(order);

        var from = DateTime.Now.AddHours(-1);
        var result = _repository.GetByClient(_clientId, from: from).ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByClient_WithToDate_ShouldFilterByToDate()
    {
        var order = CreateOrder();
        _repository.Add(order);

        var to = DateTime.Now.AddHours(1);
        var result = _repository.GetByClient(_clientId, to: to).ToList();

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetByClient_WithFromDateAfterOrder_ShouldReturnEmpty()
    {
        var order = CreateOrder();
        _repository.Add(order);

        var from = DateTime.Now.AddHours(1);
        var result = _repository.GetByClient(_clientId, from: from).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByClient_WithToDateBeforeOrder_ShouldReturnEmpty()
    {
        var order = CreateOrder();
        _repository.Add(order);

        var to = DateTime.Now.AddHours(-1);
        var result = _repository.GetByClient(_clientId, to: to).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByStatus_WithStateFilter_ShouldFilterByState()
    {
        var order1 = CreateOrder();
        var order2 = CreateOrder();
        _repository.Add(order1);
        _repository.Add(order2);

        order2.TransitionTo(OrderState.Prepared);
        _repository.Update(order2);
        _context.ChangeTracker.Clear();

        var from = DateTime.Now.AddHours(-1);
        var to = DateTime.Now.AddHours(1);

        var result = _repository.GetByStatus(from, to, state: "Prepared").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(OrderState.Prepared, result[0].State);
    }

    [TestMethod]
    public void Add_ShouldAssignOrderNumber()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        var order = new Order(Guid.NewGuid(), address, "Express", items, 50m);

        _repository.Add(order);

        var result = _repository.GetById(order.Id);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.OrderNumber.HasValue);
        Assert.AreEqual(1, result.OrderNumber);
    }
}
