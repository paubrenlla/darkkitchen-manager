using DarkKitchen.Domain;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class SqlPromotionRepositoryTests
{
    private DarkKitchenContext _context = null!;
    private SqlPromotionRepository _repository = null!;
    private Product _product1 = null!;
    private Product _product2 = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var images = new List<ProductImage> { new ProductImage("https://example.com/photo.jpg", 50000) };

        _product1 = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m, images);
        _product2 = new Product("BURG02", "Hamburguesa Doble Grande", "Hamburguesa doble con queso y bacon", line, category, 200m, images);

        _context.ProductLines.Add(line);
        _context.ProductCategories.Add(category);
        _context.Products.AddRange(_product1, _product2);
        _context.SaveChanges();

        _repository = new SqlPromotionRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private Promotion CreatePromotion(string name = "Black Friday", List<Product>? products = null)
    {
        return new Promotion(
            name,
            10,
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddDays(1),
            products ?? [_product1]);
    }

    [TestMethod]
    public void Add_ShouldPersistPromotion()
    {
        var promotion = CreatePromotion();

        _repository.Add(promotion);

        var result = _repository.GetById(promotion.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Black Friday", result.Name);
    }

    [TestMethod]
    public void Add_ShouldPersistProducts()
    {
        var promotion = CreatePromotion();

        _repository.Add(promotion);
        _context.ChangeTracker.Clear();

        var result = _repository.GetById(promotion.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Products.Count);
        Assert.AreEqual("BURG01", result.Products[0].Code);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllPromotions()
    {
        _repository.Add(CreatePromotion("Black Friday"));
        _repository.Add(CreatePromotion("Cyber Monday"));
        _context.ChangeTracker.Clear();

        var result = _repository.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_ShouldIncludeProductsWithLineAndCategory()
    {
        _repository.Add(CreatePromotion());
        _context.ChangeTracker.Clear();

        var result = _repository.GetAll().First();

        Assert.IsNotNull(result.Products.First().Line);
        Assert.IsNotNull(result.Products.First().Category);
    }

    [TestMethod]
    public void GetById_ExistingPromotion_ReturnsPromotion()
    {
        var promotion = CreatePromotion();
        _repository.Add(promotion);
        _context.ChangeTracker.Clear();

        var result = _repository.GetById(promotion.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(promotion.Id, result.Id);
    }

    [TestMethod]
    public void GetById_NonExisting_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldPersistChanges()
    {
        var promotion = CreatePromotion();
        _repository.Add(promotion);
        _context.ChangeTracker.Clear();

        var fresh = _repository.GetById(promotion.Id)!;
        fresh.Update("Semana de Turismo", 20, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), [_product1, _product2]);
        _repository.Update(fresh);
        _context.ChangeTracker.Clear();

        var result = _repository.GetById(promotion.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Semana de Turismo", result.Name);
        Assert.AreEqual(20, result.DiscountPercentage);
        Assert.AreEqual(2, result.Products.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExisting_Throws()
    {
        var promotion = CreatePromotion();
        _repository.Update(promotion);
    }
}
