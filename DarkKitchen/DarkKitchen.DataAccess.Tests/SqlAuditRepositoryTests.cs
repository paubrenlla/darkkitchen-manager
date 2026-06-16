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

    [TestMethod]
    public void GetAudits_WithFilters_ShouldReturnCorrectLogs()
    {
        var log1 = new AuditLog { Timestamp = new DateTime(2026, 5, 1), EntityName = "Product", EntityId = Guid.NewGuid() };
        var log2 = new AuditLog { Timestamp = new DateTime(2026, 5, 2), EntityName = "Promotion", EntityId = Guid.NewGuid() };
        var log3 = new AuditLog { Timestamp = new DateTime(2026, 5, 3), EntityName = "Product", EntityId = Guid.NewGuid() };

        _context.AuditLogs.AddRange(log1, log2, log3);
        _context.SaveChanges();

        var from = new DateTime(2026, 5, 1);
        var to = new DateTime(2026, 5, 2, 23, 59, 59);

        // Filter by date only
        var resultDate = _repository.GetAudits(from, to, null, null).ToList();
        Assert.AreEqual(2, resultDate.Count);

        // Filter by date and entity name
        var resultEntity = _repository.GetAudits(from, to, "Product", null).ToList();
        Assert.AreEqual(1, resultEntity.Count);
        Assert.AreEqual(log1.Id, resultEntity[0].Id);
    }

    [TestMethod]
    public void GetAudits_WithEntityIdFilter_ShouldReturnCorrectLogs()
    {
        var targetId = Guid.NewGuid();
        var log1 = new AuditLog { Timestamp = new DateTime(2026, 5, 1), EntityName = "Product", EntityId = targetId };
        var log2 = new AuditLog { Timestamp = new DateTime(2026, 5, 1), EntityName = "Product", EntityId = Guid.NewGuid() };

        _context.AuditLogs.AddRange(log1, log2);
        _context.SaveChanges();

        var from = new DateTime(2026, 5, 1);
        var to = new DateTime(2026, 5, 1, 23, 59, 59);

        var result = _repository.GetAudits(from, to, "Product", targetId).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(log1.Id, result[0].Id);
        Assert.AreEqual(targetId, result[0].EntityId);
    }
}
