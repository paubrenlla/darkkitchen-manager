using DarkKitchen.Domain.Users;

namespace DarkKitchen.DataAccess.Tests;

[TestClass]
public class UserRepositoryTests
{
    private InMemoryUserRepository _userRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepository = new InMemoryUserRepository();
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
            Role.Cliente);

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
}
