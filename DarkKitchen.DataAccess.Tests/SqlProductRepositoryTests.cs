using DarkKitchen.DataAccess;
using DarkKitchen.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class SqlProductRepositoryTests
{
    private DarkKitchenContext _context = null!;
    private SqlProductRepository _repository = null!;
    private ProductLine _defaultLine = null!;
    private ProductCategory _defaultCategory = null!;
    private List<ProductImage> _defaultImages = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _defaultLine = new ProductLine("Combo burgers");
        _defaultCategory = new ProductCategory("Parrilla");
        _defaultImages = [new ProductImage("https://example.com/photo.jpg", 50000)];

        _context.ProductLines.Add(_defaultLine);
        _context.ProductCategories.Add(_defaultCategory);
        _context.SaveChanges();

        _repository = new SqlProductRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private Product CreateProduct(string code = "BURG01", string name = "Hamburguesa Clasica")
    {
        return new Product(
            code,
            name,
            "Hamburguesa clasica con queso cheddar",
            _defaultLine,
            _defaultCategory,
            150m,
            _defaultImages);
    }

    [TestMethod]
    public void Add_ShouldPersistProduct()
    {
        var product = CreateProduct();

        _repository.Add(product);

        var result = _repository.GetById(product.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("BURG01", result.Code);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllProducts()
    {
        _repository.Add(CreateProduct("BURG01", "Hamburguesa Clasica"));
        _repository.Add(CreateProduct("BURG02", "Hamburguesa Doble Grande"));

        var result = _repository.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_ShouldIncludeLineAndCategoryAndImages()
    {
        _repository.Add(CreateProduct());

        var result = _repository.GetAll().First();

        Assert.IsNotNull(result.Line);
        Assert.IsNotNull(result.Category);
        Assert.AreEqual(1, result.Images.Count);
    }

    [TestMethod]
    public void GetById_ExistingProduct_ReturnsProduct()
    {
        var product = CreateProduct();
        _repository.Add(product);

        var result = _repository.GetById(product.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(product.Id, result.Id);
    }

    [TestMethod]
    public void GetById_NonExisting_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ExistingProduct_PersistsChanges()
    {
        var product = CreateProduct();
        _repository.Add(product);

        var newLine = new ProductLine("Desayunos");
        var newCategory = new ProductCategory("Bebidas");
        _context.ProductLines.Add(newLine);
        _context.ProductCategories.Add(newCategory);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        var newImages = new List<ProductImage>
        {
            new ProductImage("https://example.com/new.jpg", 60000),
        };

        product.UpdateDetails("Hamburguesa Nueva Doble", "Descripcion actualizada del producto", newLine, newCategory, 200m, newImages);
        _repository.Update(product.Id, product);

        var result = _repository.GetById(product.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Hamburguesa Nueva Doble", result.Name);
        Assert.AreEqual(200m, result.Price);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExisting_Throws()
    {
        var product = CreateProduct();
        _repository.Update(Guid.NewGuid(), product);
    }

    [TestMethod]
    public void Update_Deactivate_ShouldPersist()
    {
        var product = CreateProduct();
        _repository.Add(product);
        product.Deactivate();

        _repository.Update(product.Id, product);

        var result = _repository.GetById(product.Id);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsActive);
    }
}
