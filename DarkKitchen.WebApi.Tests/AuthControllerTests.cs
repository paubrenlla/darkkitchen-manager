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
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsOkWithLoginResponse()
    {
        var request = new LoginRequest { Email = "test@domain.com", Password = "Valid1Password!@" };
        var loginResponse = new LoginResponse { Token = "mocked.jwt.token", Role = "Cliente" };

        _authServiceMock.Setup(s => s.Login(request.Email, request.Password)).Returns(loginResponse);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as LoginResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("mocked.jwt.token", response.Token);
        Assert.AreEqual("Cliente", response.Role);

        _authServiceMock.Verify(s => s.Login(request.Email, request.Password), Times.Once);
    }

    [TestMethod]
    public void LoginFailed_ReturnsUnauthorized()
    {
        var request = new LoginRequest { Email = "test@domain.com", Password = "WrongPassword" };

        _authServiceMock.Setup(s => s.Login(request.Email, request.Password))
            .Throws(new UnauthorizedAccessException("Invalid Credentials."));

        var result = _authController.Login(request) as UnauthorizedObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }

    [TestMethod]
    public void LoginSuccessful_ReturnsCorrectRole()
    {
        var request = new LoginRequest { Email = "admin@bmb.com", Password = "Valid1Password!@" };
        var loginResponse = new LoginResponse { Token = "mocked.jwt.token", Role = "Administrativo" };

        _authServiceMock.Setup(s => s.Login(request.Email, request.Password)).Returns(loginResponse);

        var result = _authController.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        var response = result.Value as LoginResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("Administrativo", response.Role);
    }
}
