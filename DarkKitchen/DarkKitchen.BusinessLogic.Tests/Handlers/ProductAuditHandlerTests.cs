using DarkKitchen.BusinessLogic.Handlers;
using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests.Handlers;

[TestClass]
public class ProductAuditHandlerTests
{
    private Mock<IAuditRepository> _mockAuditRepository = null!;
    private ProductAuditHandler _handler = null!;

    private const string ValidCode = "CODE123";
    private const string ValidName = "Valid Product Name Long Enough";
    private const string ValidDesc = "This is a valid long description for testing purposes";
    private static List<ProductImage> ValidImages => [new ProductImage("http://image.com/img.jpg", 100)];

    [TestInitialize]
    public void Setup()
    {
        _mockAuditRepository = new Mock<IAuditRepository>(MockBehavior.Strict);
        _handler = new ProductAuditHandler(_mockAuditRepository.Object);
    }

    [TestMethod]
    public void Handle_EntityCreatedEvent_ShouldSaveAuditLog()
    {
        _mockAuditRepository.Setup(r => r.Save(It.IsAny<AuditLog>()));

        var product = new Product(ValidCode, ValidName, ValidDesc, new ProductLine("L"), new ProductCategory("C"), 100, ValidImages);
        var domainEvent = new EntityCreatedEvent<Product>
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Product",
            ResponsibleUser = "user@test.com",
            NewState = product
        };

        _handler.Handle(domainEvent);

        _mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.EntityName == "Product" &&
            log.ResponsibleUser == "user@test.com" &&
            log.ChangeDescription.Contains("Producto creado exitosamente."))), Times.Once);
        _mockAuditRepository.VerifyAll();
    }

    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldDetectAllChanges()
    {
        _mockAuditRepository.Setup(r => r.Save(It.IsAny<AuditLog>()));

        var oldProduct = new Product(ValidCode, "Old Valid Name", "Old description that is long enough", new ProductLine("Old Line"), new ProductCategory("Old Cat"), 100, ValidImages);
        var newProduct = new Product(ValidCode, "New Valid Name", "New description that is long enough", new ProductLine("New Line"), new ProductCategory("New Cat"), 150, [new ProductImage("http://new.com/img.jpg", 200)]);
        newProduct.Deactivate();

        var domainEvent = new EntityModifiedEvent<Product>
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Product",
            ResponsibleUser = "user@test.com",
            OldState = oldProduct,
            NewState = newProduct
        };

        _handler.Handle(domainEvent);

        _mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Name cambió de 'Old Valid Name' a 'New Valid Name'") &&
            log.ChangeDescription.Contains("Description cambió de 'Old description that is long enough' a 'New description that is long enough'") &&
            log.ChangeDescription.Contains("Price cambió de '100' a '150'") &&
            log.ChangeDescription.Contains("IsActive cambió de 'True' a 'False'") &&
            log.ChangeDescription.Contains("Line cambió de 'Old Line' a 'New Line'") &&
            log.ChangeDescription.Contains("Category cambió de 'Old Cat' a 'New Cat'") &&
            log.ChangeDescription.Contains("Las imágenes del producto fueron modificadas."))), Times.Once);
        _mockAuditRepository.VerifyAll();
    }

    [TestMethod]
    public void Handle_EntityDeactivatedEvent_ShouldSaveAuditLog()
    {
        _mockAuditRepository.Setup(r => r.Save(It.IsAny<AuditLog>()));

        var product = new Product(ValidCode, ValidName, ValidDesc, new ProductLine("L"), new ProductCategory("C"), 100, ValidImages);
        var domainEvent = new EntityDeactivatedEvent<Product>
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Product",
            ResponsibleUser = "user@test.com",
            OldState = product
        };

        _handler.Handle(domainEvent);

        _mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Producto dado de baja."))), Times.Once);
        _mockAuditRepository.VerifyAll();
    }

    [TestMethod]
    public void Handle_EntityActivatedEvent_ShouldSaveAuditLog()
    {
        _mockAuditRepository.Setup(r => r.Save(It.IsAny<AuditLog>()));

        var product = new Product(ValidCode, ValidName, ValidDesc, new ProductLine("L"), new ProductCategory("C"), 100, ValidImages);
        var domainEvent = new EntityActivatedEvent<Product>
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Product",
            ResponsibleUser = "user@test.com",
            NewState = product
        };

        _handler.Handle(domainEvent);

        _mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Producto dado de alta."))), Times.Once);
        _mockAuditRepository.VerifyAll();
    }
}
