using DarkKitchen.DataAccess;
using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

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
}
