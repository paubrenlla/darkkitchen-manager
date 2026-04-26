using DarkKitchen.Domain.Users;
using Moq;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class UserRepositoryTests
{
    private InMemoryUserRepository _userRepository = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        _userRepository = new InMemoryUserRepository(new BCryptHasher());
    }

    [TestMethod]
    public void GetUserByEmail_ExistingEmail_ReturnsUser()
    {
        var email = "admin@bmb.com";

        var result = _userRepository.GetUserByEmail(email);

        Assert.IsNotNull(result);
        Assert.AreEqual(email, result.Email);
        Assert.AreEqual(Role.Administrativo, result.Role);
    }

    [TestMethod]
    public void GetUserByEmail_NonExistingEmail_ReturnsNull()
    {
        var email = "doesnotexist@bmb.com";

        var result = _userRepository.GetUserByEmail(email);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetUserByEmail_CaseInsensitive_ReturnsUser()
    {
        var email = "ADMIN@BMB.COM";

        var result = _userRepository.GetUserByEmail(email);

        Assert.IsNotNull(result);
        Assert.AreEqual("admin@bmb.com", result.Email);
    }

    [TestMethod]
    public void Add_ValidUser_ShouldInsertIntoRepository()
    {
        var user = new User(
            "Marta",
            "Suarez",
            "marta@test.com",
            PhoneNumber.Create("+598", "094444555", new UruguayPhoneValidationStrategy()),
            "Valid1Password!@",
            Role.Cliente, _passwordHasherMock.Object);

        _userRepository.Add(user);

        var retrievedUser = _userRepository.GetUserByEmail("marta@test.com");
        Assert.IsNotNull(retrievedUser);
        Assert.AreEqual("Marta", retrievedUser.Name);
    }

    [TestMethod]
    public void GetById_ExistingUser_ReturnsUser()
    {
        User user = _userRepository.GetUserByEmail("admin@bmb.com")!;

        User? result = _userRepository.GetById(user.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.Id);
    }

    [TestMethod]
    public void GetById_NonExistingId_ReturnsNull()
    {
        User? result = _userRepository.GetById(Guid.NewGuid());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void FilterByName_ReturnsMatchingUsers()
    {
        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = PhoneNumber.Create("+598", "094111222", strategy);
        var user = new User("Lucia", "Gomez", "lucia@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object);
        _userRepository.Add(user);

        IEnumerable<User> result = _userRepository.GetByNameAndSurname("Lucia", null);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetByNameAndSurname_FilterBySurname_ReturnsMatchingUsers()
    {
        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = PhoneNumber.Create("+598", "094111222", strategy);
        var user = new User("Carlos", "Perez", "carlos@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object);
        _userRepository.Add(user);

        IEnumerable<User> result = _userRepository.GetByNameAndSurname(null, "Perez");

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public void GetByNameAndSurname_NoFilters_ReturnsAllUsers()
    {
        IEnumerable<User> result = _userRepository.GetByNameAndSurname(null, null);

        Assert.IsTrue(result.Any());
    }

    [TestMethod]
    public void Update_ExistingUser_ShouldPersistChanges()
    {
        User user = _userRepository.GetUserByEmail("admin@bmb.com")!;
        Guid originalId = user.Id;

        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var newPhone = PhoneNumber.Create("+598", "094999888", strategy);

        user.UpdateDetails("NuevoNombre", "NuevoApellido", "admin@bmb.com", newPhone, Role.Administrativo);
        _userRepository.Update(user.Id, user);

        User? result = _userRepository.GetById(originalId);
        Assert.IsNotNull(result);
        Assert.AreEqual("NuevoNombre", result.Name);
        Assert.AreEqual(originalId, result.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Update_NonExistingUser_ShouldThrow()
    {
        IPhoneValidationStrategy strategy = new UruguayPhoneValidationStrategy();
        var phone = PhoneNumber.Create("+598", "094111222", strategy);
        var user = new User("Test", "Test", "test@test.com", phone, "Valid1Password!@", Role.Cliente, _passwordHasherMock.Object);

        _userRepository.Update(Guid.NewGuid(), user);
    }

    [TestMethod]
    public void Delete_ExistingUser_ShouldRemoveUser()
    {
        User user = _userRepository.GetUserByEmail("admin@bmb.com")!;

        _userRepository.Delete(user.Id);

        User? result = _userRepository.GetById(user.Id);
        Assert.IsNull(result);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void Delete_NonExistingUser_ShouldThrow()
    {
        _userRepository.Delete(Guid.NewGuid());
    }
}
