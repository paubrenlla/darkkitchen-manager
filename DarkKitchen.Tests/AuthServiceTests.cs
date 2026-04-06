using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class AuthServiceTests
{
    private AuthService _authService = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authService = new AuthService(_userRepositoryMock.Object);
    }

    [TestMethod]
    public void LoginWithValidCredentials_ReturnsUser()
    {
        var email = "test@bmb.com";
        var password = "Password123!";
        var expectedUser = new User { Id = Guid.NewGuid(), Email = email, Password = password, Role = Role.Cliente };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email))
            .Returns(expectedUser);

        User result = _authService.Login(email, password);

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedUser.Id, result.Id);
        Assert.AreEqual(expectedUser.Email, result.Email);
        Assert.AreEqual(expectedUser.Role, result.Role);
        _userRepositoryMock.Verify(repo => repo.GetUserByEmail(email), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithInvalidPassword_ThrowsUnauthorized()
    {
        var email = "test@bmb.com";
        var validPassword = "Password123!";
        var invalidPassword = "WrongPassword";
        var existingUser =
            new User { Id = Guid.NewGuid(), Email = email, Password = validPassword, Role = Role.Cliente };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email))
            .Returns(existingUser);

        _authService.Login(email, invalidPassword);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void LoginWithNonExistentEmail_ThrowsUnauthorized()
    {
        var email = "nonexistent@bmb.com";
        var password = "Password123!";

        _userRepositoryMock.Setup(repo => repo.GetUserByEmail(email))
            .Returns((User)null!);

        _authService.Login(email, password);
    }
}
