using DarkKitchen.Domain.Orders;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class OrderServiceTests
{
    private Address _address = null!;
    private Guid _clientId;
    private List<OrderItem> _items = null!;
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private IOrderService _orderService = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object);
        _address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _items = [new OrderItem(Guid.NewGuid(), 1, 100m)];
        _clientId = Guid.NewGuid();
    }

    [TestMethod]
    public void CreateOrder_ShouldCreateOrderAndAddToRepository()
    {
        Order order = _orderService.CreateOrder(_clientId, _address, DeliveryType.Express, _items);

        Assert.IsNotNull(order);
        Assert.AreEqual(_clientId, order.ClientId);
        _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }

    [TestMethod]
    public void Prepare_WhenOrderExists_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Prepare(orderId);

        Assert.AreEqual(OrderState.Prepared, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void Cancel_WhenOrderExists_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Cancel(orderId);

        Assert.AreEqual(OrderState.Cancelled, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void Ship_WhenOrderIsPrepared_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        order.TransitionTo(OrderState.Prepared);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Ship(orderId);

        Assert.AreEqual(OrderState.Shipping, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }
}
