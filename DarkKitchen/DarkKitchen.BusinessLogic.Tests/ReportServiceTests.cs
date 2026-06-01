using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ReportServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private ReportService _reportService = null!;

    private Guid _product1Id;
    private Guid _product2Id;
    private Guid _product3Id;
    private List<Product> _products = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepositoryMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");

        _reportService = new ReportService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            _userRepositoryMock.Object);

        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        List<ProductImage> images = [new("https://example.com/photo.jpg", 50000)];

        var product1 = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, images);
        var product2 = new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", line, category, 200m, images);
        var product3 = new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", line, category, 120m, images);

        _product1Id = product1.Id;
        _product2Id = product2.Id;
        _product3Id = product3.Id;
        _products = [product1, product2, product3];
    }

    private void SetupOrderGetByStatus(DateTime from, DateTime to, List<Order> orders) =>
        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);

    private void SetupOrderGetAll(List<Order> orders) =>
        _orderRepositoryMock.Setup(r => r.GetAll()).Returns(orders);

    private void SetupProductGetById(Guid id, Product? product) =>
        _productRepositoryMock.Setup(r => r.GetById(id)).Returns(product);

    private void SetupUserGetById(Guid id, User? user) =>
        _userRepositoryMock.Setup(r => r.GetById(id)).Returns(user);

    private static Address BuildAddress() =>
        new Address("Rivera", "1234", null, "Montevideo", "Uruguay");

    private static Order CreateOrderWithDate(Guid clientId, Address address, DateTime date, decimal itemPrice)
    {
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, itemPrice) };
        var order = new Order(clientId, address, "Express", items, 0m);
        order.SetCreatedAt(date);
        return order;
    }

    private User CreateUser(string name, string surname)
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094123456", strategy);
        return new User(name, surname, $"{name}@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_ShouldReturnTopFiveOrderedByQuantity()
    {
        var address = BuildAddress();
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var orders = new List<Order>
        {
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 10, 150m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product2Id, 5, 200m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 3, 150m)], 0m),
        };

        SetupOrderGetByStatus(from, to, orders);
        SetupProductGetById(_product1Id, _products[0]);
        SetupProductGetById(_product2Id, _products[1]);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("BURG01", result.First().Code);
        Assert.AreEqual(13, result.First().QuantitySold);
        Assert.AreEqual("BURG02", result.ElementAt(1).Code);
        Assert.AreEqual(5, result.ElementAt(1).QuantitySold);
        _orderRepositoryMock.VerifyAll();
        _productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_ShouldExcludeCancelledOrders()
    {
        var address = BuildAddress();
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var validOrder = new Order(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 5, 150m)], 0m);
        var cancelledOrder = new Order(Guid.NewGuid(), address, "Express", [new OrderItem(_product2Id, 100, 200m)], 0m);
        cancelledOrder.TransitionTo(OrderState.Cancelled);

        SetupOrderGetByStatus(from, to, [validOrder, cancelledOrder]);
        SetupProductGetById(_product1Id, _products[0]);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result.First().Code);
        Assert.AreEqual(5, result.First().QuantitySold);
        _orderRepositoryMock.VerifyAll();
        _productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_ShouldReturnMaxFiveProducts()
    {
        var address = BuildAddress();
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var line = new ProductLine("Extras");
        var category = new ProductCategory("Varios");
        List<ProductImage> images = [new("https://example.com/photo.jpg", 50000)];

        var product4 = new Product("PROD4", "Producto Cuatro Test", "Descripcion del producto cuatro de prueba", line, category, 100m, images);
        var product5 = new Product("PROD5", "Producto Cinco Test", "Descripcion del producto cinco de prueba", line, category, 100m, images);
        var product6 = new Product("PROD6", "Producto Seis Prueba", "Descripcion del producto seis de prueba", line, category, 100m, images);

        var orders = new List<Order>
        {
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 10, 150m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product2Id, 9, 200m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product3Id, 8, 120m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(product4.Id, 7, 100m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(product5.Id, 6, 100m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(product6.Id, 5, 100m)], 0m),
        };

        SetupOrderGetByStatus(from, to, orders);
        SetupProductGetById(_product1Id, _products[0]);
        SetupProductGetById(_product2Id, _products[1]);
        SetupProductGetById(_product3Id, _products[2]);
        SetupProductGetById(product4.Id, product4);
        SetupProductGetById(product5.Id, product5);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(5, result.Count);
        _orderRepositoryMock.VerifyAll();
        _productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_NoOrders_ShouldReturnEmptyList()
    {
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        SetupOrderGetByStatus(from, to, []);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(0, result.Count);
        _orderRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_MultipleOrdersSameProduct_ShouldAggregateQuantities()
    {
        var address = BuildAddress();
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var orders = new List<Order>
        {
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 3, 150m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 7, 150m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 2, 150m)], 0m),
        };

        SetupOrderGetByStatus(from, to, orders);
        SetupProductGetById(_product1Id, _products[0]);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(12, result.First().QuantitySold);
        _orderRepositoryMock.VerifyAll();
        _productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetTopProducts_ProductNotFound_ShouldSkipProduct()
    {
        var address = BuildAddress();
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var unknownProductId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new(Guid.NewGuid(), address, "Express", [new OrderItem(_product1Id, 5, 150m)], 0m),
            new(Guid.NewGuid(), address, "Express", [new OrderItem(unknownProductId, 100, 50m)], 0m),
        };

        SetupOrderGetByStatus(from, to, orders);
        SetupProductGetById(_product1Id, _products[0]);
        SetupProductGetById(unknownProductId, null);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result.First().Code);
        _orderRepositoryMock.VerifyAll();
        _productRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_NoOrders_ShouldReturnEmptyReport()
    {
        SetupOrderGetAll([]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(0, result.Periods.Count);
        Assert.AreEqual(0, result.GrandTotal);
        _orderRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ShouldGroupByYearAndMonth()
    {
        var address = BuildAddress();
        var clientId = Guid.NewGuid();
        var user = CreateUser("Juan", "Perez");

        SetupUserGetById(clientId, user);

        var order1 = CreateOrderWithDate(clientId, address, new DateTime(2026, 1, 15), 100m);
        var order2 = CreateOrderWithDate(clientId, address, new DateTime(2026, 2, 10), 200m);
        SetupOrderGetAll([order1, order2]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(2, result.Periods.Count);
        Assert.AreEqual(2026, result.Periods[0].Year);
        Assert.AreEqual(1, result.Periods[0].Month);
        Assert.AreEqual(2026, result.Periods[1].Year);
        Assert.AreEqual(2, result.Periods[1].Month);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ShouldExcludeCancelledOrders()
    {
        var address = BuildAddress();
        var clientId = Guid.NewGuid();
        var user = CreateUser("Juan", "Perez");

        SetupUserGetById(clientId, user);

        var validOrder = CreateOrderWithDate(clientId, address, new DateTime(2026, 1, 15), 100m);
        var cancelledOrder = CreateOrderWithDate(clientId, address, new DateTime(2026, 1, 20), 9999m);
        cancelledOrder.TransitionTo(OrderState.Cancelled);
        SetupOrderGetAll([validOrder, cancelledOrder]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(1, result.Periods.Count);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ShouldGroupClientsByPeriod()
    {
        var address = BuildAddress();
        var clientId1 = Guid.NewGuid();
        var clientId2 = Guid.NewGuid();

        SetupUserGetById(clientId1, CreateUser("Juan", "Perez"));
        SetupUserGetById(clientId2, CreateUser("Yuri", "Gagarin"));

        var order1 = CreateOrderWithDate(clientId1, address, new DateTime(2026, 1, 15), 100m);
        var order2 = CreateOrderWithDate(clientId2, address, new DateTime(2026, 1, 20), 200m);
        SetupOrderGetAll([order1, order2]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(2, result.Periods[0].Clients.Count);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_UnknownClient_ShouldUseFallbackName()
    {
        var address = BuildAddress();
        var unknownClientId = Guid.NewGuid();

        SetupUserGetById(unknownClientId, null);

        var order = CreateOrderWithDate(unknownClientId, address, new DateTime(2026, 1, 10), 100m);
        SetupOrderGetAll([order]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual("Cliente desconocido", result.Periods[0].Clients[0].ClientName);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ShouldCalculateClientTotal()
    {
        var address = BuildAddress();
        var clientId = Guid.NewGuid();
        SetupUserGetById(clientId, CreateUser("Juan", "Perez"));

        var order1 = CreateOrderWithDate(clientId, address, new DateTime(2026, 1, 5), 100m);
        var order2 = CreateOrderWithDate(clientId, address, new DateTime(2026, 1, 20), 200m);
        SetupOrderGetAll([order1, order2]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(order1.Total + order2.Total, result.Periods[0].Clients[0].Total);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ShouldCalculatePeriodTotal()
    {
        var address = BuildAddress();
        var clientId1 = Guid.NewGuid();
        var clientId2 = Guid.NewGuid();
        SetupUserGetById(clientId1, CreateUser("Juan", "Perez"));
        SetupUserGetById(clientId2, CreateUser("Yuri", "Gagarin"));

        var order1 = CreateOrderWithDate(clientId1, address, new DateTime(2026, 1, 15), 100m);
        var order2 = CreateOrderWithDate(clientId2, address, new DateTime(2026, 1, 20), 200m);
        SetupOrderGetAll([order1, order2]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(order1.Total + order2.Total, result.Periods[0].PeriodTotal);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }

    [TestMethod]
    public void GetSalesReport_ShouldCalculateGrandTotal()
    {
        var address = BuildAddress();
        var clientId = Guid.NewGuid();
        SetupUserGetById(clientId, CreateUser("Juan", "Perez"));

        var order1 = CreateOrderWithDate(clientId, address, new DateTime(2026, 1, 15), 100m);
        var order2 = CreateOrderWithDate(clientId, address, new DateTime(2026, 2, 10), 200m);
        SetupOrderGetAll([order1, order2]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(order1.Total + order2.Total, result.GrandTotal);
        _orderRepositoryMock.VerifyAll();
        _userRepositoryMock.VerifyAll();
    }
}
