using DarkKitchen.Domain.Orders.Delivery;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class SqlShippingTypeRepositoryTests
{
    private DarkKitchenContext _context = null!;
    private SqlShippingTypeRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new SqlShippingTypeRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldPersistShippingType()
    {
        var shippingType = new ShippingType("Express", 150m);

        _repository.Add(shippingType);

        var result = _repository.GetById(shippingType.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Express", result.Name);
        Assert.AreEqual(150m, result.Cost);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllShippingTypes()
    {
        _repository.Add(new ShippingType("Express", 150m));
        _repository.Add(new ShippingType("Dia Siguiente", 80m));

        var result = _repository.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetById_ExistingId_ReturnsShippingType()
    {
        var shippingType = new ShippingType("Express", 150m);
        _repository.Add(shippingType);

        var result = _repository.GetById(shippingType.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(shippingType.Id, result.Id);
    }

    [TestMethod]
    public void GetById_NonExisting_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByName_ExistingName_ReturnsShippingType()
    {
        _repository.Add(new ShippingType("Express", 150m));

        var result = _repository.GetByName("Express");

        Assert.IsNotNull(result);
        Assert.AreEqual("Express", result.Name);
    }

    [TestMethod]
    public void GetByName_NonExisting_ReturnsNull()
    {
        var result = _repository.GetByName("NoExiste");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldPersistChanges()
    {
        var shippingType = new ShippingType("Express", 150m);
        _repository.Add(shippingType);
        _context.ChangeTracker.Clear();

        var fresh = _repository.GetById(shippingType.Id)!;
        fresh.UpdateDetails("Express Premium", 250m);
        _repository.Update(fresh);
        _context.ChangeTracker.Clear();

        var result = _repository.GetById(shippingType.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Express Premium", result.Name);
        Assert.AreEqual(250m, result.Cost);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExisting_ShouldThrow()
    {
        var shippingType = new ShippingType("Express", 150m);
        _repository.Update(shippingType);
    }

    [TestMethod]
    public void Delete_ShouldRemoveShippingType()
    {
        var shippingType = new ShippingType("Express", 150m);
        _repository.Add(shippingType);

        _repository.Delete(shippingType.Id);

        var result = _repository.GetById(shippingType.Id);
        Assert.IsNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Delete_NonExisting_ShouldThrow()
    {
        _repository.Delete(Guid.NewGuid());
    }
}
