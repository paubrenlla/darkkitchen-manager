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

    [TestMethod]
    public void Update_ReplaceImages_ShouldDeleteOldAndPersistNew()
    {
        var product = CreateProduct();
        _repository.Add(product);
        var oldImageId = product.Images[0].Id;

        var newImages = new List<ProductImage>
        {
            new ProductImage("https://example.com/img1.jpg", 10000),
            new ProductImage("https://example.com/img2.jpg", 20000)
        };

        product.UpdateDetails(product.Name, product.Description, product.Line, product.Category, product.Price, newImages);
        _repository.Update(product.Id, product);

        var result = _repository.GetById(product.Id);

        Assert.AreEqual(2, result.Images.Count);
        Assert.IsFalse(result.Images.Any(i => i.Id == oldImageId));
        Assert.IsTrue(result.Images.Any(i => i.Url == "https://example.com/img1.jpg"));
    }

    [TestMethod]
    public void GetAllLines_ShouldReturnAllLines()
    {
        var line2 = new ProductLine("Línea 2");
        _context.ProductLines.Add(line2);
        _context.SaveChanges();

        var result = _repository.GetAllLines().ToList();

        Assert.AreEqual(2, result.Count); // _defaultLine + line2
        Assert.IsTrue(result.Any(l => l.Name == "Combo burgers"));
        Assert.IsTrue(result.Any(l => l.Name == "Línea 2"));
    }

    [TestMethod]
    public void GetAllCategories_ShouldReturnAllCategories()
    {
        var cat2 = new ProductCategory("Categoría 2");
        _context.ProductCategories.Add(cat2);
        _context.SaveChanges();

        var result = _repository.GetAllCategories().ToList();

        Assert.AreEqual(2, result.Count); // _defaultCategory + cat2
        Assert.IsTrue(result.Any(c => c.Name == "Parrilla"));
        Assert.IsTrue(result.Any(c => c.Name == "Categoría 2"));
    }

    [TestMethod]
    public void Update_WithNewLine_ShouldCreateLineAndPersistProduct()
    {
        var product = CreateProduct();
        _repository.Add(product);
        _context.ChangeTracker.Clear();

        var newLine = new ProductLine("Linea Completamente Nueva");
        var newImages = new List<ProductImage>
        {
            new("https://example.com/new.jpg", 60000)
        };

        product.UpdateDetails(product.Name, product.Description, newLine, _defaultCategory, product.Price, newImages);
        _repository.Update(product.Id, product);

        var result = _repository.GetById(product.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Linea Completamente Nueva", result.Line.Name);
        Assert.IsTrue(_context.ProductLines.Any(l => l.Name == "Linea Completamente Nueva"));
    }

    [TestMethod]
    public void Update_WithNewCategory_ShouldCreateCategoryAndPersistProduct()
    {
        var product = CreateProduct();
        _repository.Add(product);
        _context.ChangeTracker.Clear();

        var newCategory = new ProductCategory("Categoria Completamente Nueva");
        var newImages = new List<ProductImage>
        {
            new("https://example.com/new.jpg", 60000)
        };

        product.UpdateDetails(product.Name, product.Description, _defaultLine, newCategory, product.Price, newImages);
        _repository.Update(product.Id, product);

        var result = _repository.GetById(product.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Categoria Completamente Nueva", result.Category.Name);
        Assert.IsTrue(_context.ProductCategories.Any(c => c.Name == "Categoria Completamente Nueva"));
    }
}
