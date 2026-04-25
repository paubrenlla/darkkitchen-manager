using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ReportServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private ReportService _reportService = null!;
    private List<Product> _products = null!;
    private Guid _product1Id;
    private Guid _product2Id;
    private Guid _product3Id;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _reportService = new ReportService(_orderRepositoryMock.Object, _productRepositoryMock.Object);

        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        List<ProductImage> images = [new ProductImage("https://example.com/photo.jpg", 50000)];

        var product1 = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, images);
        var product2 = new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", line, category, 200m, images);
        var product3 = new Product("DESA01", "Desayuno Completo Grande", "Desayuno con cafe tostadas y jugo", line, category, 120m, images);

        _product1Id = product1.Id;
        _product2Id = product2.Id;
        _product3Id = product3.Id;

        _products = [product1, product2, product3];
        _productRepositoryMock.Setup(r => r.GetById(_product1Id)).Returns(product1);
        _productRepositoryMock.Setup(r => r.GetById(_product2Id)).Returns(product2);
        _productRepositoryMock.Setup(r => r.GetById(_product3Id)).Returns(product3);
    }

    [TestMethod]
    public void GetTopProducts_ShouldReturnTopFiveOrderedByQuantity()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var order1 = new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 10, 150m)]);
        var order2 = new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product2Id, 5, 200m)]);
        var order3 = new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 3, 150m)]);

        List<Order> orders = [order1, order2, order3];

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);

        IEnumerable<TopProductResponse> result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("BURG01", result.First().Code);
        Assert.AreEqual(13, result.First().QuantitySold);
        Assert.AreEqual("BURG02", result.ElementAt(1).Code);
        Assert.AreEqual(5, result.ElementAt(1).QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_ShouldExcludeCancelledOrders()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var validOrder = new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 5, 150m)]);
        var cancelledOrder = new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product2Id, 100, 200m)]);
        cancelledOrder.TransitionTo(OrderState.Cancelled);

        List<Order> orders = [validOrder, cancelledOrder];

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);

        IEnumerable<TopProductResponse> result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("BURG01", result.First().Code);
        Assert.AreEqual(5, result.First().QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_ShouldReturnMaxFiveProducts()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var line = new ProductLine("Extras");
        var category = new ProductCategory("Varios");
        List<ProductImage> images = [new ProductImage("https://example.com/photo.jpg", 50000)];

        var product4 = new Product("PROD4", "Producto Cuatro Test", "Descripcion del producto cuatro de prueba", line, category, 100m, images);
        var product5 = new Product("PROD5", "Producto Cinco Test", "Descripcion del producto cinco de prueba", line, category, 100m, images);
        var product6 = new Product("PROD6", "Producto Seis Prueba", "Descripcion del producto seis de prueba", line, category, 100m, images);

        _productRepositoryMock.Setup(r => r.GetById(product4.Id)).Returns(product4);
        _productRepositoryMock.Setup(r => r.GetById(product5.Id)).Returns(product5);
        _productRepositoryMock.Setup(r => r.GetById(product6.Id)).Returns(product6);

        List<Order> orders =
        [
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 10, 150m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product2Id, 9, 200m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product3Id, 8, 120m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(product4.Id, 7, 100m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(product5.Id, 6, 100m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(product6.Id, 5, 100m)]),
        ];

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void GetTopProducts_NoOrders_ShouldReturnEmptyList()
    {
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns([]);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopProducts_MultipleOrdersSameProduct_ShouldAggregateQuantities()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        List<Order> orders =
        [
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 3, 150m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 7, 150m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 2, 150m)]),
        ];

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(12, result.First().QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_ProductNotFound_ShouldSkipProduct()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        DateTime from = DateTime.Now.AddDays(-30);
        DateTime to = DateTime.Now;

        var unknownProductId = Guid.NewGuid();
        _productRepositoryMock.Setup(r => r.GetById(unknownProductId)).Returns((Product?)null);

        List<Order> orders =
        [
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(_product1Id, 5, 150m)]),
            new Order(Guid.NewGuid(), address, DeliveryType.Express, [new OrderItem(unknownProductId, 100, 50m)]),
        ];

        _orderRepositoryMock.Setup(r => r.GetByStatus(from, to, null, null)).Returns(orders);

        var result = _reportService.GetTopProducts(from, to).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result.First().Code);
    }
}
