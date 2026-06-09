using DarkKitchen.Domain.Audit;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class AuditServiceTests
{
    private Mock<IAuditRepository> _mockRepository = null!;
    private AuditService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IAuditRepository>(MockBehavior.Strict);
        _service = new AuditService(_mockRepository.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetAudits_FromDateAfterToDate_ShouldThrowArgumentException()
    {
        var from = new DateTime(2026, 5, 2);
        var to = new DateTime(2026, 5, 1);

        _service.GetAudits(from, to, null, null);
    }

    [TestMethod]
    public void GetAudits_ValidRange_ShouldCallRepository()
    {
        var from = new DateTime(2026, 5, 1);
        var to = new DateTime(2026, 5, 2);
        var logs = new List<AuditLog>
        {
            new AuditLog { EntityName = "Product" }
        };

        _mockRepository.Setup(r => r.GetAudits(from, to, null, null)).Returns(logs);

        var result = _service.GetAudits(from, to, null, null);

        Assert.AreEqual(1, result.Count());
        _mockRepository.VerifyAll();
    }
}
