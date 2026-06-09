using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class AuthControllerTests
{
    private AuthController _authController = null!;
    private Mock<IAuthService> _authServiceMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>(MockBehavior.Strict);
        _authController = new AuthController(_authServiceMock.Object);
    }

    private static LoginResult CreateTestLoginResult(string token = "mocked.jwt.token", Role role = Role.Cliente)
    {
        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        var strategy = new UruguayPhoneValidationStrategy();
        var phone = Domain.Users.PhoneNumber.Create("+598", "094111222", strategy);
        var user = new User("Test", "User", "test@domain.com", phone, "Valid1Password!@", role, hasher.Object);
        return new LoginResult(token, user);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsOkWithLoginResponse()
    {
        var request = new LoginRequest { Email = "test@domain.com", Password = "Valid1Password!@" };
        var loginResult = CreateTestLoginResult("mocked.jwt.token", Role.Cliente);
        _authServiceMock.Setup(s => s.Login(request.Email, request.Password)).Returns(loginResult);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var response = result.Value as LoginResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("mocked.jwt.token", response.Token);
        Assert.AreEqual("Cliente", response.Role);
        _authServiceMock.VerifyAll();
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsCorrectRole()
    {
        var request = new LoginRequest { Email = "admin@bmb.com", Password = "Valid1Password!@" };
        var loginResult = CreateTestLoginResult("mocked.jwt.token", Role.Administrativo);
        _authServiceMock.Setup(s => s.Login(request.Email, request.Password)).Returns(loginResult);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        var response = result.Value as LoginResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Administrativo", response.Role);
        _authServiceMock.VerifyAll();
    }
}
