using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class OrderEnricherTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private OrderEnricher _enricher = null!;
    private Address _address = null!;
    private Guid _clientId;
    private User _user = null!;
    private Product _product = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _enricher = new OrderEnricher(_userRepositoryMock.Object, _productRepositoryMock.Object);

        _address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        _clientId = Guid.NewGuid();

        var hasherMock = new Mock<IPasswordHasher>();
        hasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094123456", strategy);
        _user = new User("Juan", "Sosa", "juan@test.com", phone, "Valid1Password!@", Role.Cliente, hasherMock.Object);

        var images = new List<ProductImage> { new ProductImage("https://example.com/photo.jpg", 50000) };
        _product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", new ProductLine("Combo"), new ProductCategory("Parrilla"), 150m, images);
    }

    [TestMethod]
    public void EnrichForClient_ShouldIncludeClientName()
    {
        var order = new Order(_clientId, _address, "Express", [new OrderItem(_product.Id, 1, 150m)], 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns(_user);

        var result = _enricher.EnrichForClient(order);

        Assert.AreEqual("Juan Sosa", result.ClientName);
    }

    [TestMethod]
    public void EnrichForClient_WhenUserNotFound_ShouldReturnEmptyClientName()
    {
        var order = new Order(_clientId, _address, "Express", [new OrderItem(_product.Id, 1, 150m)], 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns((User?)null);

        var result = _enricher.EnrichForClient(order);

        Assert.AreEqual(string.Empty, result.ClientName);
    }

    [TestMethod]
    public void EnrichForClient_ShouldNotIncludeItems()
    {
        var order = new Order(_clientId, _address, "Express", [new OrderItem(_product.Id, 1, 150m)], 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns(_user);

        var result = _enricher.EnrichForClient(order);

        Assert.AreEqual(0, result.Items.Count);
    }

    [TestMethod]
    public void EnrichForClient_ShouldIncludeProductCount()
    {
        var items = new List<OrderItem>
        {
            new OrderItem(_product.Id, 2, 150m),
            new OrderItem(Guid.NewGuid(), 3, 100m),
        };
        var order = new Order(_clientId, _address, "Express", items, 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns(_user);

        var result = _enricher.EnrichForClient(order);

        Assert.AreEqual(5, result.ProductCount);
    }

    [TestMethod]
    public void EnrichForPreparador_ShouldIncludeClientName()
    {
        var order = new Order(_clientId, _address, "Express", [new OrderItem(_product.Id, 1, 150m)], 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns(_user);
        _productRepositoryMock.Setup(r => r.GetById(_product.Id)).Returns(_product);

        var result = _enricher.EnrichForPreparador(order);

        Assert.AreEqual("Juan Sosa", result.ClientName);
    }

    [TestMethod]
    public void EnrichForPreparador_ShouldIncludeItemsWithProductDetails()
    {
        var order = new Order(_clientId, _address, "Express", [new OrderItem(_product.Id, 2, 150m)], 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns(_user);
        _productRepositoryMock.Setup(r => r.GetById(_product.Id)).Returns(_product);

        var result = _enricher.EnrichForPreparador(order);

        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("BURG01", result.Items[0].ProductCode);
        Assert.AreEqual("Hamburguesa Clasica", result.Items[0].ProductName);
        Assert.AreEqual(2, result.Items[0].Quantity);
    }

    [TestMethod]
    public void EnrichForPreparador_WhenProductNotFound_ShouldUseEmptyStrings()
    {
        var unknownProductId = Guid.NewGuid();
        var order = new Order(_clientId, _address, "Express", [new OrderItem(unknownProductId, 1, 100m)], 0m);
        _userRepositoryMock.Setup(r => r.GetById(_clientId)).Returns(_user);
        _productRepositoryMock.Setup(r => r.GetById(unknownProductId)).Returns((Product?)null);

        var result = _enricher.EnrichForPreparador(order);

        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(string.Empty, result.Items[0].ProductCode);
        Assert.AreEqual(string.Empty, result.Items[0].ProductName);
    }
}
