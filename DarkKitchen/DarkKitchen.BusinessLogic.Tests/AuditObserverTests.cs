using DarkKitchen.Domain.Audit;
using DarkKitchen.Domain.Events;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class AuditObserverTests
{
    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldGenerateCorrectChangeDescriptionAndSave()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldProduct = new Product("CODE1", "Old Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"),
            100m, [new ProductImage("img.jpg", 1000)]);
        var newProduct = new Product("CODE1", "New Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"),
            150m, [new ProductImage("img.jpg", 1000)]);

        typeof(Product).GetProperty("Id")!.SetValue(newProduct, oldProduct.Id);

        var domainEvent = new EntityModifiedEvent<Product>
        {
            EntityId = oldProduct.Id,
            EntityName = "Product",
            ResponsibleUser = "admin@darkkitchen.com",
            OldState = oldProduct,
            NewState = newProduct
        };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.EntityId == oldProduct.Id &&
                log.ChangeDescription.Contains("Name cambió de 'Old Valid Name' a 'New Valid Name'") &&
                log.ChangeDescription.Contains("Price cambió de '100' a '150'"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldLogDescriptionChange()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldProduct = new Product("CODE1", "Valid Name", "This is an old valid description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        var newProduct = new Product("CODE1", "Valid Name", "This is a new valid description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);

        typeof(Product).GetProperty("Id")!.SetValue(newProduct, oldProduct.Id);

        var domainEvent = new EntityModifiedEvent<Product> { EntityId = oldProduct.Id, EntityName = "Product", ResponsibleUser = "admin", OldState = oldProduct, NewState = newProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("Description cambió de 'This is an old valid description' a 'This is a new valid description'"))), Times.Once);
    }

    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldLogIsActiveChange()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        var newProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);

        typeof(Product).GetProperty("IsActive")!.SetValue(newProduct, false);

        typeof(Product).GetProperty("Id")!.SetValue(newProduct, oldProduct.Id);

        var domainEvent = new EntityModifiedEvent<Product> { EntityId = oldProduct.Id, EntityName = "Product", ResponsibleUser = "admin", OldState = oldProduct, NewState = newProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("IsActive cambió de 'True' a 'False'"))), Times.Once);
    }

    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldLogLineAndCategoryChange()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Old Line"), new ProductCategory("Old Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        var newProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("New Line"), new ProductCategory("New Cat"), 100m, [new ProductImage("img.jpg", 1000)]);

        typeof(Product).GetProperty("Id")!.SetValue(newProduct, oldProduct.Id);

        var domainEvent = new EntityModifiedEvent<Product> { EntityId = oldProduct.Id, EntityName = "Product", ResponsibleUser = "admin", OldState = oldProduct, NewState = newProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.ChangeDescription.Contains("Line cambió de 'Old Line' a 'New Line'") &&
                log.ChangeDescription.Contains("Category cambió de 'Old Cat' a 'New Cat'"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_EntityModifiedEvent_ShouldLogImagesChange()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img1.jpg", 1000)]);
        var newProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img2.jpg", 2000)]);

        typeof(Product).GetProperty("Id")!.SetValue(newProduct, oldProduct.Id);

        var domainEvent = new EntityModifiedEvent<Product> { EntityId = oldProduct.Id, EntityName = "Product", ResponsibleUser = "admin", OldState = oldProduct, NewState = newProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(r => r.Save(It.Is<AuditLog>(log => log.ChangeDescription.Contains("Las imágenes del producto fueron modificadas."))), Times.Once);
    }

    [TestMethod]
    public void Handle_EntityCreatedEvent_ShouldLogCreation()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var newProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        typeof(Product).GetProperty("Id")!.SetValue(newProduct, Guid.NewGuid());

        var domainEvent = new EntityCreatedEvent<Product> { EntityId = newProduct.Id, EntityName = "Product", ResponsibleUser = "admin", NewState = newProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.ChangeDescription.Contains("Producto creado exitosamente.") &&
                log.ChangeDescription.Contains($"ID Interno: {newProduct.Id}") &&
                log.ChangeDescription.Contains("Código: CODE1") &&
                log.ChangeDescription.Contains("Nombre: Valid Name"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_EntityDeactivatedEvent_ShouldLogDeactivation()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        typeof(Product).GetProperty("Id")!.SetValue(oldProduct, Guid.NewGuid());

        var domainEvent = new EntityDeactivatedEvent<Product> { EntityId = oldProduct.Id, EntityName = "Product", ResponsibleUser = "admin", OldState = oldProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.ChangeDescription.Contains("Producto dado de baja.") &&
                log.ChangeDescription.Contains($"ID Interno: {oldProduct.Id}") &&
                log.ChangeDescription.Contains("Código: CODE1") &&
                log.ChangeDescription.Contains("Nombre: Valid Name"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_EntityActivatedEvent_ShouldLogActivation()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var newProduct = new Product("CODE1", "Valid Name", "This is a valid long description", new ProductLine("Line"), new ProductCategory("Cat"), 100m, [new ProductImage("img.jpg", 1000)]);
        typeof(Product).GetProperty("Id")!.SetValue(newProduct, Guid.NewGuid());

        var domainEvent = new EntityActivatedEvent<Product> { EntityId = newProduct.Id, EntityName = "Product", ResponsibleUser = "admin", NewState = newProduct };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.ChangeDescription.Contains("Producto dado de alta.") &&
                log.ChangeDescription.Contains($"ID Interno: {newProduct.Id}") &&
                log.ChangeDescription.Contains("Código: CODE1") &&
                log.ChangeDescription.Contains("Nombre: Valid Name"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_PromotionCreatedEvent_ShouldLogCreation()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var newPromo = new Promotion("SUMMER26", 20, DateTime.Now, DateTime.Now.AddDays(7), []);
        typeof(Promotion).GetProperty("Id")!.SetValue(newPromo, Guid.NewGuid());

        var domainEvent = new EntityCreatedEvent<Promotion>
        {
            EntityId = newPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = "admin@darkkitchen.com",
            NewState = newPromo
        };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.EntityId == newPromo.Id &&
                log.EntityName == "Promotion" &&
                log.ChangeDescription.Contains("Promoción creada exitosamente.") &&
                log.ChangeDescription.Contains($"Nombre: SUMMER26") &&
                log.ChangeDescription.Contains("Descuento: 20%"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_PromotionModifiedEvent_ShouldLogChanges()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var oldPromo = new Promotion("Old Name", 10, DateTime.Now, DateTime.Now.AddDays(7), []);
        var newPromo = new Promotion("New Name", 15, DateTime.Now, DateTime.Now.AddDays(7), []);
        typeof(Promotion).GetProperty("Id")!.SetValue(newPromo, oldPromo.Id);

        var domainEvent = new EntityModifiedEvent<Promotion>
        {
            EntityId = oldPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = "admin@darkkitchen.com",
            OldState = oldPromo,
            NewState = newPromo
        };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.EntityId == oldPromo.Id &&
                log.ChangeDescription.Contains("Name cambió de 'Old Name' a 'New Name'") &&
                log.ChangeDescription.Contains("DiscountPercentage cambió de '10' a '15'"))),
            Times.Once);
    }

    [TestMethod]
    public void Handle_PromotionModifiedEvent_ShouldLogProductsChange()
    {
        var mockAuditRepository = new Mock<IAuditRepository>();
        var observer = new AuditObserver(mockAuditRepository.Object);

        var line = new ProductLine("Line");
        var cat = new ProductCategory("Cat");
        var prod1 = new Product("CODE1", "Product Valid Name 1", "Description description description", line, cat, 100m, [new ProductImage("img.jpg", 1000)]);
        var prod2 = new Product("CODE2", "Product Valid Name 2", "Description description description", line, cat, 200m, [new ProductImage("img.jpg", 1000)]);

        var oldPromo = new Promotion("Promo", 10, DateTime.Now, DateTime.Now.AddDays(7), [prod1]);
        var newPromo = new Promotion("Promo", 10, DateTime.Now, DateTime.Now.AddDays(7), [prod2]);
        typeof(Promotion).GetProperty("Id")!.SetValue(newPromo, oldPromo.Id);

        var domainEvent = new EntityModifiedEvent<Promotion>
        {
            EntityId = oldPromo.Id,
            EntityName = "Promotion",
            ResponsibleUser = "admin@darkkitchen.com",
            OldState = oldPromo,
            NewState = newPromo
        };

        observer.Handle(domainEvent);

        mockAuditRepository.Verify(
            r => r.Save(It.Is<AuditLog>(log =>
                log.ChangeDescription.Contains("La lista de productos de la promoción fue modificada."))),
            Times.Once);
    }
}
