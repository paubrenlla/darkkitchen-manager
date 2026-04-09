using DarkKitchen.BusinessLogic;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IPhoneStrategyFactory> _strategyFactoryMock = null!;
    private UserService _userService = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _strategyFactoryMock = new Mock<IPhoneStrategyFactory>();
        _userService = new UserService(_userRepositoryMock.Object, _strategyFactoryMock.Object);
    }

    [TestMethod]
    public void CreateUser_ValidRequest_CreatesUserAndCallsRepository()
    {
        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Perez",
            Email = "carlos@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094123456",
            Password = "Valid1Password!@"
        };

        var mockStrategy = new Mock<IPhoneValidationStrategy>();
        mockStrategy.Setup(s => s.CountryPrefix).Returns("+598");
        mockStrategy.Setup(s => s.IsValid("094123456")).Returns(true);

        _strategyFactoryMock.Setup(f => f.GetStrategy("+598")).Returns(mockStrategy.Object);

        var result = _userService.CreateUser(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Carlos", result.Name);
        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void CreateUser_InvalidPhoneStrategy_ThrowsException()
    {
        var request = new UserCreateRequest
        {
            Name = "Carlos",
            Surname = "Perez",
            Email = "carlos@test.com",
            CountryPrefix = "+123",
            PhoneNumber = "12345678",
            Password = "Valid1Password!@"
        };

        _strategyFactoryMock.Setup(f => f.GetStrategy("+123"))
            .Throws(new NotSupportedException("Country prefix not supported."));

        _userService.CreateUser(request);
    }
}
