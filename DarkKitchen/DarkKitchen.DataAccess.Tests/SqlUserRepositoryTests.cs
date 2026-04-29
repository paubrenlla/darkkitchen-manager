using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class SqlUserRepositoryTests
{
    private DarkKitchenContext _context = null!;
    private SqlUserRepository _repository = null!;
    private IPasswordHasher _hasher = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        var hasherMock = new Mock<IPasswordHasher>();
        hasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        _hasher = hasherMock.Object;

        _repository = new SqlUserRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private User CreateUser(string name, string surname, string email, Role role = Role.Cliente)
    {
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = PhoneNumber.Create("+598", "094123456", strategy);
        return new User(name, surname, email, phone, "Valid1Password!@", role, _hasher);
    }

    [TestMethod]
    public void Add_ShouldPersistUser()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");

        _repository.Add(user);

        var result = _repository.GetById(user.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("juan@test.com", result.Email);
    }

    [TestMethod]
    public void GetUserByEmail_ExistingEmail_ReturnsUser()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");
        _repository.Add(user);

        var result = _repository.GetUserByEmail("juan@test.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("Juan", result.Name);
    }

    [TestMethod]
    public void GetUserByEmail_CaseInsensitive_ReturnsUser()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");
        _repository.Add(user);

        var result = _repository.GetUserByEmail("JUAN@TEST.COM");

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetUserByEmail_NonExisting_ReturnsNull()
    {
        var result = _repository.GetUserByEmail("noexiste@test.com");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetById_ExistingId_ReturnsUser()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");
        _repository.Add(user);

        var result = _repository.GetById(user.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.Id);
    }

    [TestMethod]
    public void GetById_NonExistingId_ReturnsNull()
    {
        var result = _repository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByNameAndSurname_FilterByName_ReturnsMatching()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@test.com"));
        _repository.Add(CreateUser("Carlos", "Lopez", "carlos@test.com"));

        var result = _repository.GetByNameAndSurname("Juan", null).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Juan", result[0].Name);
    }

    [TestMethod]
    public void GetByNameAndSurname_FilterBySurname_ReturnsMatching()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@test.com"));
        _repository.Add(CreateUser("Carlos", "Lopez", "carlos@test.com"));

        var result = _repository.GetByNameAndSurname(null, "Lopez").ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Carlos", result[0].Name);
    }

    [TestMethod]
    public void GetByNameAndSurname_NoFilters_ReturnsAll()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@test.com"));
        _repository.Add(CreateUser("Carlos", "Lopez", "carlos@test.com"));

        var result = _repository.GetByNameAndSurname(null, null).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void Update_ExistingUser_PersistsChanges()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");
        _repository.Add(user);

        var strategy = new UruguayPhoneValidationStrategy();
        var newPhone = PhoneNumber.Create("+598", "094999888", strategy);
        user.UpdateDetails("Nuevo", "Nombre", "juan@test.com", newPhone, Role.Administrativo);
        _repository.Update(user.Id, user);

        var result = _repository.GetById(user.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Nuevo", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExisting_Throws()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");
        _repository.Update(Guid.NewGuid(), user);
    }

    [TestMethod]
    public void Delete_ExistingUser_RemovesFromDb()
    {
        var user = CreateUser("Juan", "Perez", "juan@test.com");
        _repository.Add(user);

        _repository.Delete(user.Id);

        var result = _repository.GetById(user.Id);
        Assert.IsNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Delete_NonExisting_Throws()
    {
        _repository.Delete(Guid.NewGuid());
    }
}
