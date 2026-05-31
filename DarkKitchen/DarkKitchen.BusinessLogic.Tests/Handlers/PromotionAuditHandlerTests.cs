using DarkKitchen.BusinessLogic.Handlers;
using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests.Handlers;

[TestClass]
public class PromotionAuditHandlerTests
{
    private Mock<IAuditRepository> _mockAuditRepository = null!;
    private PromotionAuditHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAuditRepository = new Mock<IAuditRepository>(MockBehavior.Strict);
        _handler = new PromotionAuditHandler(_mockAuditRepository.Object);
    }

    [TestMethod]
    public void Handle_EntityCreatedEvent_ShouldSaveAuditLog()
    {
        _mockAuditRepository.Setup(r => r.Save(It.IsAny<AuditLog>()));

        var promo = new Promotion("PROMO_SUMMER", 10, DateTime.Now, DateTime.Now.AddDays(1), []);
        var domainEvent = new EntityCreatedEvent<Promotion>
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Promotion",
            ResponsibleUser = "user@test.com",
            NewState = promo
        };

        _handler.Handle(domainEvent);

        _mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.EntityName == "Promotion" &&
            log.ChangeDescription.Contains("Promoción creada exitosamente."))), Times.Once);
        _mockAuditRepository.VerifyAll();
    }

    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldDetectAllChanges()
    {
        _mockAuditRepository.Setup(r => r.Save(It.IsAny<AuditLog>()));

        var oldStart = DateTime.Now.AddDays(1);
        var oldEnd = DateTime.Now.AddDays(10);
        var newStart = DateTime.Now.AddDays(2);
        var newEnd = DateTime.Now.AddDays(11);

        var product1 = new Product("PROD01", "Product One Long Name", "This is a valid long description for product one", new ProductLine("Line"), new ProductCategory("Category"), 100, [new ProductImage("img.jpg", 1)]);
        var product2 = new Product("PROD02", "Product Two Long Name", "This is a valid long description for product two", new ProductLine("Line"), new ProductCategory("Category"), 100, [new ProductImage("img.jpg", 1)]);

        var oldPromo = new Promotion("Old Name Long Enough", 10, oldStart, oldEnd, [product1]);
        var newPromo = new Promotion("New Name Long Enough", 20, newStart, newEnd, [product2]);

        var domainEvent = new EntityModifiedEvent<Promotion>
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Promotion",
            ResponsibleUser = "user@test.com",
            OldState = oldPromo,
            NewState = newPromo
        };

        _handler.Handle(domainEvent);

        _mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Name cambió de 'Old Name Long Enough' a 'New Name Long Enough'") &&
            log.ChangeDescription.Contains("DiscountPercentage cambió de '10' a '20'") &&
            log.ChangeDescription.Contains("StartDate cambió de") &&
            log.ChangeDescription.Contains("EndDate cambió de") &&
            log.ChangeDescription.Contains("La lista de productos de la promoción fue modificada."))), Times.Once);
        _mockAuditRepository.VerifyAll();
    }
}
