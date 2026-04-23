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

    [TestInitialize]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
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

        OkObjectResult? result = _userController.GetUsers("Juan", null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_ValidRequest_ReturnsOk()
    {
        Guid adminId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        UserUpdateRequest request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        UserCreateResponse response = new UserCreateResponse
        {
            Id = userId,
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            Phone = "+598094999888",
            Role = "Administrativo",
        };

        _userServiceMock.Setup(s => s.UpdateUser(adminId, userId, request)).Returns(response);

        OkObjectResult? result = _userController.UpdateUser(userId, request, adminId) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_SelfModification_ReturnsBadRequest()
    {
        Guid adminId = Guid.NewGuid();

        UserUpdateRequest request = new UserUpdateRequest
        {
            Name = "Nuevo",
            Surname = "Nombre",
            Email = "nuevo@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userServiceMock.Setup(s => s.UpdateUser(adminId, adminId, request))
            .Throws(new InvalidOperationException("Un usuario no puede modificarse a sí mismo."));

        BadRequestObjectResult? result = _userController.UpdateUser(adminId, request, adminId) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateUser_InvalidData_ReturnsBadRequest()
    {
        Guid adminId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        UserUpdateRequest request = new UserUpdateRequest
        {
            Name = string.Empty,
            Surname = "Nombre",
            Email = "test@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094999888",
            Role = "Administrativo",
        };

        _userServiceMock.Setup(s => s.UpdateUser(adminId, userId, request))
            .Throws(new ArgumentException("Nombre inválido."));

        BadRequestObjectResult? result = _userController.UpdateUser(userId, request, adminId) as BadRequestObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void DeleteUser_ValidRequest_ReturnsNoContent()
    {
        Guid adminId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        _userServiceMock.Setup(s => s.DeleteUser(adminId, userId));

        NoContentResult? result = _userController.DeleteUser(userId, adminId) as NoContentResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(204, result.StatusCode);
    }
}
