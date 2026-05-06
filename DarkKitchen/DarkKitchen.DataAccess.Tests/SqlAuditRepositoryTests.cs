using DarkKitchen.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class SqlAuditRepositoryTests
{
    private DarkKitchenContext _context = null!;
    private SqlAuditRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new SqlAuditRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [TestMethod]
    public void Save_ShouldPersistAuditLog()
    {
        var log = new AuditLog
        {
            EntityId = Guid.NewGuid(),
            EntityName = "Product",
            ResponsibleUser = "admin@test.com",
            ChangeDescription = "Name changed"
        };

        _repository.Save(log);

        var result = _context.AuditLogs.FirstOrDefault(l => l.Id == log.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Product", result.EntityName);
        Assert.AreEqual("admin@test.com", result.ResponsibleUser);
    }
}
