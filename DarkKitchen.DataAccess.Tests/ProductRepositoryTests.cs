using DarkKitchen.Domain.Products;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class ProductRepositoryTests
{
    private InMemoryProductRepository _productRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _productRepository = new InMemoryProductRepository();
    }

    [TestMethod]
    public void GetAll_ReturnsSeededProducts()
    {
        IEnumerable<Product> result = _productRepository.GetAll();

        Assert.IsNotNull(result);
        var productList = result.ToList();
        Assert.AreEqual(3, productList.Count);

        Assert.IsTrue(productList.Any(p => p.Code == "BURG01"));
        Assert.IsTrue(productList.Any(p => p.Code == "BURG02"));
        Assert.IsTrue(productList.Any(p => p.Code == "DESA01"));
    }

    [TestMethod]
    public void GetById_ExistingProduct_ReturnsProduct()
    {
        Product? result = _productRepository.GetById(_productRepository.GetAll().First().Id);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetById_NonExistingProduct_ReturnsNull()
    {
        Product? result = _productRepository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Add_ShouldAddProductToRepository()
    {
        var line = new ProductLine("Desayunos");
        var category = new ProductCategory("Bebidas");
        List<ProductImage> images = [new ProductImage("https://example.com/photo.jpg", 50000)];
        var product = new Product("NEWPR", "Nuevo Producto Test", "Descripcion del nuevo producto de prueba", line, category, 100m, images);

        _productRepository.Add(product);

        Assert.IsNotNull(_productRepository.GetById(product.Id));
    }

    [TestMethod]
    public void Update_ExistingProduct_ShouldPersistChanges()
    {
        Product product = _productRepository.GetAll().First();
        Guid originalId = product.Id;
        var newLine = new ProductLine("Desayunos");
        var newCategory = new ProductCategory("Bebidas");
        List<ProductImage> newImages = [new ProductImage("https://example.com/new.jpg", 50000)];

        product.UpdateDetails("Nombre Actualizado Largo", "Descripcion actualizada del producto de prueba", newLine, newCategory, 999m, newImages);
        _productRepository.Update(product.Id, product);

        Product? result = _productRepository.GetById(originalId);
        Assert.IsNotNull(result);
        Assert.AreEqual("Nombre Actualizado Largo", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExistingProduct_ShouldThrow()
    {
        var line = new ProductLine("Desayunos");
        var category = new ProductCategory("Bebidas");
        List<ProductImage> images = [new ProductImage("https://example.com/photo.jpg", 50000)];
        var product = new Product("NEWPR", "Nuevo Producto Test", "Descripcion del nuevo producto de prueba", line, category, 100m, images);

        _productRepository.Update(Guid.NewGuid(), product);
    }
}
