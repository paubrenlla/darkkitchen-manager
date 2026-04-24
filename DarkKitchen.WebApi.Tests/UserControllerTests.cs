using System.Security.Claims;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Tests;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _userController = null!;
    private Guid _callerId;

    [TestInitialize]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _callerId = Guid.NewGuid();
        _userController = new UserController(_userServiceMock.Object);
        SetCallerContext(_callerId, "Administrativo");
    }

    private void SetCallerContext(Guid callerId, string role)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, callerId.ToString()),
            new Claim(ClaimTypes.Role, role),
        ];

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal },
        };
    }

    [TestMethod]
    public void CreateUser_Success_ReturnsCreatedAndUserData()
    {
        var request = new UserCreateRequest
        {
            Name = "Lucia",
            Surname = "Gomez",
            Email = "lucia@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111222",
            Password = "Valid1Password!@",
        };

        var response = new UserCreateResponse
        {
            Id = Guid.NewGuid(),
            Name = "Lucia",
            Surname = "Gomez",
            Email = "lucia@test.com",
            Phone = "+598094111222",
            Role = "Cliente",
        };

        _userServiceMock.Setup(s => s.CreateUser(request)).Returns(response);

        var result = _userController.CreateUser(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);

        var body = result.Value as UserCreateResponse;
        Assert.IsNotNull(body);
        Assert.AreEqual("Lucia", body.Name);
        Assert.AreEqual("lucia@test.com", body.Email);
    }

    [TestMethod]
    public void CreateUser_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        var request = new UserCreateRequest
        {
            Name = string.Empty,
            Surname = "Gomez",
            Email = "lucia@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111222",
            Password = "Valid1Password!@",
        };

        _userServiceMock.Setup(s => s.CreateUser(request))
            .Throws(new ArgumentException("Nombre inválido."));

        var result = _userController.CreateUser(request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [TestMethod]
    public void GetUsers_ReturnsOkWithList()
    {
        List<UserCreateResponse> users =
        [
            new UserCreateResponse
            {
                Id = Guid.NewGuid(),
                Name = "Juan",
                Surname = "Perez",
                Email = "juan@test.com",
                Phone = "+598094123456",
                Role = "Cliente",
            },
        ];

        _userServiceMock.Setup(s => s.GetUsers("Juan", null)).Returns(users);

        var result = _userController.GetUsers("Juan", null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();

        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        var response = new UserCreateResponse
        {
            Id = userId,
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            Phone = "+598094999888",
            Role = "Administrativo",
        };

        _userServiceMock.Setup(s => s.UpdateUser(_callerId, userId, request)).Returns(response);

        var result = _userController.UpdateUser(userId, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_SelfModification_ReturnsBadRequest()
    {
        var request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userServiceMock.Setup(s => s.UpdateUser(_callerId, _callerId, request))
            .Throws(new InvalidOperationException("Un usuario no puede modificarse a sí mismo."));

        var result = _userController.UpdateUser(_callerId, request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_InvalidData_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();

        var request = new UserUpdateRequest
        {
            Name = string.Empty,
            Surname = "Nombre",
            Email = "test@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userServiceMock.Setup(s => s.UpdateUser(_callerId, userId, request))
            .Throws(new ArgumentException("Nombre inválido."));

        var result = _userController.UpdateUser(userId, request) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void DeleteUser_ValidRequest_ReturnsNoContent()
    {
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(s => s.DeleteUser(_callerId, userId));

        var result = _userController.DeleteUser(userId) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
    }

    [TestMethod]
    public void DeleteUser_SelfDeletion_ReturnsBadRequest()
    {
        _userServiceMock.Setup(s => s.DeleteUser(_callerId, _callerId))
            .Throws(new InvalidOperationException("Un usuario no puede eliminarse a sí mismo."));

        var result = _userController.DeleteUser(_callerId) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void DeleteUser_NotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(s => s.DeleteUser(_callerId, userId))
            .Throws(new KeyNotFoundException("Usuario no encontrado."));

        var result = _userController.DeleteUser(userId) as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public void CreateUser_PreparadorCreatesWithRole_ReturnsForbid()
    {
        SetCallerContext(_callerId, "Preparador");

        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Lopez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111333",
            Password = "Valid1Password!@",
            Role = "Preparador",
        };

        var result = _userController.CreateUser(request) as ForbidResult;

        Assert.IsNotNull(result);
    }
}
