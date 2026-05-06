using DarkKitchen.BusinessLogic.Events;
using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests.Events;

[TestClass]
public class DomainEventPublisherTests
{
    [TestMethod]
    public void Publish_EntityModifiedEvent_ShouldInvokeAuditObserver()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);
        var publisher = new DomainEventPublisher(observer);

        var oldProduct = new Product("CODE1", "Old Valid Name", "This is an old valid description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        var newProduct = new Product("CODE1", "New Valid Name", "This is an old valid description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);

        typeof(Product).GetProperty("Id")!.SetValue(newProduct, oldProduct.Id);

        var domainEvent = new EntityModifiedEvent<Product>
        {
            EntityId = oldProduct.Id,
            EntityName = "Product",
            ResponsibleUser = "admin@darkkitchen.com",
            OldState = oldProduct,
            NewState = newProduct
        };

        publisher.Publish(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("Name cambió de 'Old Valid Name' a 'New Valid Name'"))), Times.Once);
    }

    [TestMethod]
    public void Publish_EntityCreatedEvent_ShouldInvokeAuditObserver()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);
        var publisher = new DomainEventPublisher(observer);

        var newProduct = new Product("CODE1", "New Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        typeof(Product).GetProperty("Id")!.SetValue(newProduct, Guid.NewGuid());

        var domainEvent = new EntityCreatedEvent<Product>
        {
            EntityId = newProduct.Id,
            EntityName = "Product",
            ResponsibleUser = "admin@darkkitchen.com",
            NewState = newProduct
        };

        publisher.Publish(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("Producto creado exitosamente."))), Times.Once);
    }

    [TestMethod]
    public void Publish_EntityDeactivatedEvent_ShouldInvokeAuditObserver()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);
        var publisher = new DomainEventPublisher(observer);

        var oldProduct = new Product("CODE1", "Old Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        typeof(Product).GetProperty("Id")!.SetValue(oldProduct, Guid.NewGuid());

        var domainEvent = new EntityDeactivatedEvent<Product>
        {
            EntityId = oldProduct.Id,
            EntityName = "Product",
            ResponsibleUser = "admin@darkkitchen.com",
            OldState = oldProduct
        };

        publisher.Publish(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("Producto dado de baja."))), Times.Once);
    }

    [TestMethod]
    public void Publish_EntityActivatedEvent_ShouldInvokeAuditObserver()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);
        var publisher = new DomainEventPublisher(observer);

        var newProduct = new Product("CODE1", "New Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        typeof(Product).GetProperty("Id")!.SetValue(newProduct, Guid.NewGuid());

        var domainEvent = new EntityActivatedEvent<Product>
        {
            EntityId = newProduct.Id,
            EntityName = "Product",
            ResponsibleUser = "admin@darkkitchen.com",
            NewState = newProduct
        };

        publisher.Publish(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("Producto dado de alta."))), Times.Once);
    }

    [TestMethod]
    public void Publish_PromotionCreatedEvent_ShouldInvokeAuditObserver()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);
        var publisher = new DomainEventPublisher(observer);

        var newPromo = new Promotion("SUMMER26", 20, DateTime.Now, DateTime.Now.AddDays(7), []);
        typeof(Promotion).GetProperty("Id")!.SetValue(newPromo, Guid.NewGuid());

        var domainEvent = new EntityCreatedEvent<Promotion>
        {
            EntityId = newPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = "admin@darkkitchen.com",
            NewState = newPromo
        };

        publisher.Publish(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.EntityId == newPromo.Id &&
            log.EntityName == "Promotion" &&
            log.ChangeDescription.Contains("Promoción creada exitosamente."))), Times.Once);
    }

    [TestMethod]
    public void Publish_PromotionModifiedEvent_ShouldInvokeAuditObserver()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);
        var publisher = new DomainEventPublisher(observer);

        var oldPromo = new Promotion("Old Name", 10, DateTime.Now, DateTime.Now.AddDays(7), []);
        var newPromo = new Promotion("New Name", 10, DateTime.Now, DateTime.Now.AddDays(7), []);
        typeof(Promotion).GetProperty("Id")!.SetValue(newPromo, oldPromo.Id);

        var domainEvent = new EntityModifiedEvent<Promotion>
        {
            EntityId = oldPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = "admin@darkkitchen.com",
            OldState = oldPromo,
            NewState = newPromo
        };

        publisher.Publish(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.EntityId == oldPromo.Id &&
            log.EntityName == "Promotion" &&
            log.ChangeDescription.Contains("Name cambió de 'Old Name' a 'New Name'"))), Times.Once);
    }
}
