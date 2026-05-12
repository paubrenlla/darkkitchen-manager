using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Orders.Delivery;
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
    private Mock<IShippingCostCalculator> _shippingCalculatorMock = null!;
    private Mock<IOrderEnricher> _orderEnricherMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _promotionServiceMock = new Mock<IPromotionService>();
        _shippingCalculatorMock = new Mock<IShippingCostCalculator>();
        _orderEnricherMock = new Mock<IOrderEnricher>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            _promotionServiceMock.Object,
            _shippingCalculatorMock.Object,
            _orderEnricherMock.Object);

        _address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _items = [new OrderItem(Guid.NewGuid(), 1, 100m)];
        _clientId = Guid.NewGuid();

        _shippingCalculatorMock.Setup(s => s.CalculateShippingCost(It.IsAny<string>())).Returns(0m);
    }

    [TestMethod]
    public void CreateOrder_ShouldApplyPromotionAndPriceFromProduct()
    {
        var productId = Guid.NewGuid();
        var validImages = new List<ProductImage> { new("https://darkkitchen.com/pizza.jpg", 100 * 1024) };

        var product = new Product(
            "PROD001",
            "Pizza Napolitana Especial",
            "Deliciosa pizza con muzzarella y tomate natural hecha en horno de barro.",
            new ProductLine("Minutas"),
            new ProductCategory("Pizzas"),
            500m,
            validImages);

        _productRepositoryMock.Setup(r => r.GetAll()).Returns([product]);

        _promotionServiceMock.Setup(p => p.GetBestPromotionForProduct(product.Id, It.IsAny<DateTime>()))
            .Returns(("Promo Test", 10m));

        _shippingCalculatorMock.Setup(s => s.CalculateShippingCost("Express")).Returns(100m);

        var request = new OrderCreateRequest
        {
            DeliveryType = "Express",
            Address = new OrderAddressDto
            {
                Street = "Av. Rivera",
                Number = "1234",
                City = "Montevideo",
                Country = "Uruguay"
            },
            Items = [new OrderItemDto { ProductId = product.Id, Quantity = 2 }]
        };

        OrderCreateResponse result = _orderService.CreateOrder(_clientId, request);
        Assert.AreNotEqual(Guid.Empty, result.Id);
        Assert.AreEqual(900m, result.Subtotal);
        _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }

    [TestMethod]
    public void Prepare_WhenOrderExists_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Prepare(orderId);

        Assert.AreEqual(OrderState.Prepared, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Prepare_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Order?)null);
        _orderService.Prepare(Guid.NewGuid());
    }

    [TestMethod]
    public void Cancel_WhenOrderExists_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Cancel(orderId);

        Assert.AreEqual(OrderState.Cancelled, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void Delay_WhenOrderIsPending_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Delay(orderId);

        Assert.AreEqual(OrderState.Delayed, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Delay_WhenOrderNotFound_ShouldThrowKeyNotFoundException()
    {
        _orderRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Order?)null);
        _orderService.Delay(Guid.NewGuid());
    }

    [TestMethod]
    public void Ship_WhenOrderIsPrepared_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        order.TransitionTo(OrderState.Prepared);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Ship(orderId);

        Assert.AreEqual(OrderState.Shipping, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void Deliver_WhenOrderIsShipping_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        order.TransitionTo(OrderState.Prepared);
        order.TransitionTo(OrderState.Shipping);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.Deliver(orderId);

        Assert.AreEqual(OrderState.Delivered, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void NotDelivered_WhenOrderIsShipping_ShouldTransitionAndUpdate()
    {
        var orderId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        order.TransitionTo(OrderState.Prepared);
        order.TransitionTo(OrderState.Shipping);
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        _orderService.NotDelivered(orderId);

        Assert.AreEqual(OrderState.NotDelivered, order.State);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void GetOrdersByClient_ShouldDelegateToRepository()
    {
        var orders = new List<Order> { new(_clientId, _address, "Express", _items, 0m) };
        var expectedResponse = new OrderListResponse { Status = "Pending", ClientName = "Juan Sosa" };

        _orderRepositoryMock
            .Setup(r => r.GetByClient(_clientId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>()))
            .Returns(orders);
        _orderEnricherMock.Setup(e => e.EnrichForClient(It.IsAny<Order>())).Returns(expectedResponse);

        var result = _orderService.GetOrdersByClient(_clientId, null, null, null).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Juan Sosa", result[0].ClientName);
        _orderEnricherMock.Verify(e => e.EnrichForClient(It.IsAny<Order>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_InvalidDeliveryType_ShouldThrow()
    {
        _shippingCalculatorMock
            .Setup(s => s.CalculateShippingCost("TipoInvalido"))
            .Throws(new ArgumentException("Tipo de envío 'TipoInvalido' no existe."));

        var request = new OrderCreateRequest
        {
            DeliveryType = "TipoInvalido",
            Address = new OrderAddressDto
            {
                Street = "Rivera",
                Number = "1234",
                City = "Montevideo",
                Country = "Uruguay",
            },
            Items = [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 }],
        };

        _orderService.CreateOrder(_clientId, request);
    }

    [TestMethod]
    public void GetOrderById_WhenExists_ShouldReturnDetailResponse()
    {
        var order = new Order(_clientId, _address, "Express", _items, 0m);
        var orderId = order.Id;
        _orderRepositoryMock.Setup(r => r.GetById(orderId)).Returns(order);

        OrderDetailResponse result = _orderService.GetOrderById(orderId);
        Assert.IsNotNull(result);
        Assert.AreEqual(orderId, result.Id);
        Assert.AreEqual("Pending", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CreateOrder_WhenProductIsInactive_ShouldThrowException()
    {
        var validImages = new List<ProductImage> { new("https://darkkitchen.com/foto.jpg", 50 * 1024) };

        var product = new Product(
            "INACT01",
            "Producto Inactivo Test",
            "Esta es una descripción suficientemente larga para pasar las validaciones.",
            new ProductLine("Línea"),
            new ProductCategory("Categoría"),
            100m,
            validImages);

        product.Deactivate();

        _productRepositoryMock.Setup(r => r.GetAll()).Returns([product]);

        var request = new OrderCreateRequest
        {
            DeliveryType = "Express",
            Address = new OrderAddressDto
            {
                Street = "Calle Falsa",
                Number = "123",
                City = "Montevideo",
                Country = "Uruguay"
            },
            Items = [new OrderItemDto { ProductId = product.Id, Quantity = 1 }]
        };

        _orderService.CreateOrder(_clientId, request);
    }

    [TestMethod]
    public void GetOrdersByStatus_ShouldDelegateToEnricher()
    {
        var from = DateTime.Now.AddDays(-1);
        var to = DateTime.Now.AddDays(1);
        var orders = new List<Order> { new(_clientId, _address, "Express", _items, 0m) };
        var expectedResponse = new OrderListResponse { Status = "Pending", ClientName = "Juan Sosa" };

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);
        _orderEnricherMock.Setup(e => e.EnrichForPreparador(It.IsAny<Order>())).Returns(expectedResponse);

        var result = _orderService.GetOrdersByStatus(from, to, null, null).ToList();

        Assert.AreEqual(1, result.Count);
        _orderEnricherMock.Verify(e => e.EnrichForPreparador(It.IsAny<Order>()), Times.Once);
    }
}
