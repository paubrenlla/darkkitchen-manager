using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
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
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private Mock<IPromotionService> _promotionServiceMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _promotionServiceMock = new Mock<IPromotionService>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            _promotionServiceMock.Object);

        _address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _items = [new OrderItem(Guid.NewGuid(), 1, 100m)];
        _clientId = Guid.NewGuid();
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
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Prepare_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns((Order?)null);

        _orderService.Prepare(orderId);
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
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Cancel_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns((Order?)null);

        _orderService.Cancel(orderId);
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

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Ship_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns((Order?)null);

        _orderService.Ship(orderId);
    }

    [TestMethod]
    public void Deliver_WhenOrderIsShipping_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        order.TransitionTo(OrderState.Prepared);
        order.TransitionTo(OrderState.Shipping);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Deliver(orderId);

        Assert.AreEqual(OrderState.Delivered, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Deliver_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns((Order?)null);

        _orderService.Deliver(orderId);
    }

    [TestMethod]
    public void NotDelivered_WhenOrderIsShipping_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        order.TransitionTo(OrderState.Prepared);
        order.TransitionTo(OrderState.Shipping);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.NotDelivered(orderId);

        Assert.AreEqual(OrderState.NotDelivered, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void NotDelivered_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns((Order?)null);

        _orderService.NotDelivered(orderId);
    }

    [TestMethod]
    public void GetOrdersByClient_ShouldDelegateToRepository()
    {
        var orders = new List<Order> { new(_clientId, _address, DeliveryType.Express, _items) };
        DateTime from = DateTime.Now.AddDays(-7);
        DateTime to = DateTime.Now;

        _orderRepositoryMock.Setup(r => r.GetByClient(_clientId, from, to, "Pending")).Returns(orders);

        var result = _orderService.GetOrdersByClient(_clientId, from, to, "Pending").ToList();

        Assert.AreEqual(1, result.Count);
        _orderRepositoryMock.Verify(r => r.GetByClient(_clientId, from, to, "Pending"), Times.Once);
    }

    [TestMethod]
    public void GetOrdersByStatus_ShouldDelegateToRepository()
    {
        var orders = new List<Order> { new(_clientId, _address, DeliveryType.Express, _items) };
        DateTime from = DateTime.Now.AddDays(-7);
        DateTime to = DateTime.Now;

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, "Pending", "Montevideo")).Returns(orders);

        var result = _orderService.GetOrdersByStatus(from, to, "Pending", "Montevideo").ToList();

        Assert.AreEqual(1, result.Count);
        _orderRepositoryMock.Verify(r => r.GetByStatus(from, to, "Pending", "Montevideo"), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_InvalidDeliveryType_ShouldThrow()
    {
        var request = new OrderCreateRequest
        {
            DeliveryType = "InvalidType",
            Address = new OrderAddressDto
            {
                Street = "Rivera", Number = "1234", City = "Montevideo", Country = "Uruguay"
            },
            Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        _orderService.CreateOrder(_clientId, request);
    }

    [TestMethod]
    public void GetOrderById_WhenExists_ShouldReturnDetailResponse()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, DeliveryType.Express, _items);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        OrderDetailResponse result = _orderService.GetOrderById(orderId);

        Assert.IsNotNull(result);
        Assert.AreEqual(_clientId, result.ClientId);
        Assert.AreEqual("Pending", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetOrderById_WhenNotFound_ShouldThrow()
    {
        _orderRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Order?)null);

        _orderService.GetOrderById(Guid.NewGuid());
    }

    [TestMethod]
    public void CreateOrder_ShouldApplyPromotionAndPriceFromProduct()
    {
        var productId = Guid.NewGuid();
        var validImages = new List<ProductImage> { new("https://darkkitchen.com/pizza.jpg", 100 * 1024) };

        var validName = "Pizza Napolitana Especial";
        var validDescription = "Deliciosa pizza con muzzarella, tomate y albahaca fresca.";
        var validCode = "PROD001";

        var product = new Product(
            validCode,
            validName,
            validDescription,
            new ProductLine("Minutas"),
            new ProductCategory("Fritos"),
            500m,
            validImages);

        _productRepositoryMock.Setup(r => r.GetAll()).Returns([product]);

        _promotionServiceMock.Setup(p => p.GetBestPromotionForProduct(product.Id, It.IsAny<DateTime>()))
            .Returns(("Promo Test", 10m));

        var request = new OrderCreateRequest
        {
            DeliveryType = "Express",
            Address = new OrderAddressDto
            {
                Street = "Rivera", Number = "1234", City = "Montevideo", Country = "Uruguay"
            },
            Items = [new OrderItemDto { ProductId = product.Id, Quantity = 2 }]
        };

        OrderCreateResponse result = _orderService.CreateOrder(_clientId, request);

        Assert.IsNotNull(result);

        Assert.AreEqual(900m, result.Subtotal);
        _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }
}
