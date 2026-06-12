using DarkKitchen.BusinessLogic.Events;
using DarkKitchen.BusinessLogic.Handlers;
using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests.Events;

[TestClass]
public class DomainEventPublisherTests
{
    private ServiceProvider BuildProvider(IAuditRepository auditRepo)
    {
        var services = new ServiceCollection();
        services.AddScoped<IAuditRepository>(_ => auditRepo);
        services.AddScoped<ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityModifiedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityCreatedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityDeactivatedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityActivatedEvent<Product>>, ProductAuditHandler>();
        services.AddScoped<PromotionAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityCreatedEvent<Promotion>>, PromotionAuditHandler>();
        services.AddScoped<IAuditEventHandler<EntityModifiedEvent<Promotion>>, PromotionAuditHandler>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        return services.BuildServiceProvider();
    }

    [TestMethod]
    public void Publish_EntityModifiedEvent_ShouldInvokeHandlers()
    {
        var mockRepo = new Mock<IAuditRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.Save(It.IsAny<AuditLog>()));

        using var provider = BuildProvider(mockRepo.Object);
        var publisher = provider.GetRequiredService<IDomainEventPublisher>();

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

        mockRepo.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Name cambió de 'Old Valid Name' a 'New Valid Name'"))), Times.Once);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void Publish_EntityCreatedEvent_ShouldInvokeHandlers()
    {
        var mockRepo = new Mock<IAuditRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.Save(It.IsAny<AuditLog>()));

        using var provider = BuildProvider(mockRepo.Object);
        var publisher = provider.GetRequiredService<IDomainEventPublisher>();

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

        mockRepo.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Producto creado exitosamente."))), Times.Once);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void Publish_PromotionCreatedEvent_ShouldInvokeHandlers()
    {
        var mockRepo = new Mock<IAuditRepository>(MockBehavior.Strict);
        mockRepo.Setup(r => r.Save(It.IsAny<AuditLog>()));

        using var provider = BuildProvider(mockRepo.Object);
        var publisher = provider.GetRequiredService<IDomainEventPublisher>();

        var line = new ProductLine("Combos");
        var category = new ProductCategory("Hamburguesas");
        var product = new Product("BURG01", "Hamburguesa Clásica", "Hamburguesa clásica con queso", line, category, 150m, [new ProductImage("img.jpg", 1000)]);

        var newPromo = new Promotion("SUMMER26", 20, DateTime.Now, DateTime.Now.AddDays(7), [product]);
        typeof(Promotion).GetProperty("Id")!.SetValue(newPromo, Guid.NewGuid());

        var domainEvent = new EntityCreatedEvent<Promotion>
        {
            EntityId = newPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = "admin@darkkitchen.com",
            NewState = newPromo
        };

        publisher.Publish(domainEvent);

        mockRepo.Verify(r => r.Save(It.Is<AuditLog>(log =>
            log.ChangeDescription.Contains("Promoción creada exitosamente."))), Times.Once);
        mockRepo.VerifyAll();
    }

    [TestMethod]
    public void Publish_UnknownEventType_ShouldNotThrow()
    {
        var mockRepo = new Mock<IAuditRepository>(MockBehavior.Strict);
        using var provider = BuildProvider(mockRepo.Object);
        var publisher = provider.GetRequiredService<IDomainEventPublisher>();

        publisher.Publish("un evento desconocido");

        mockRepo.Verify(r => r.Save(It.IsAny<AuditLog>()), Times.Never);
    }

    [TestMethod]
    public void Publish_WhenNoHandlersRegistered_ShouldNotThrow()
    {
        var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IEnumerable<IAuditEventHandler<string>>)))
            .Returns(null);

        var publisher = new DomainEventPublisher(mockServiceProvider.Object);

        publisher.Publish("test event");

        mockServiceProvider.VerifyAll();
    }
}
