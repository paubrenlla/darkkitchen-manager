using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class TokenServiceTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPassword = "Valid1Password!@";
    private static readonly IPhoneValidationStrategy UruguayStrategy = new UruguayPhoneValidationStrategy();
    private static readonly PhoneNumber ValidPhone = PhoneNumber.Create("+598", "094123456", UruguayStrategy);

    private Mock<IConfiguration> _configurationMock = null!;
    private Mock<IPhoneValidationStrategy> _phoneStrategyMock = null!;
    private TokenService _tokenService = null!;

    [TestInitialize]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>();
        _phoneStrategyMock = new Mock<IPhoneValidationStrategy>();
        _phoneStrategyMock.Setup(s => s.IsValid(It.IsAny<string>())).Returns(true);

        // Set up the mock configuration to return a dummy secret key
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(s => s.Value).Returns("we_are_just_engineers_in_progress_trying_to_get_a_70_in_DA2");

        _configurationMock.Setup(c => c.GetSection("JwtConfig:Secret")).Returns(configSectionMock.Object);

        _tokenService = new TokenService(_configurationMock.Object);
    }

    [TestMethod]
    public void GenerateToken_ValidUser_ReturnsNonEmptyString()
    {
        var user = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente);

        var token = _tokenService.GenerateToken(user);

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));
    }

    [TestMethod]
    public void GenerateToken_ValidUser_ReturnsJwtFormat()
    {
        var user = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente);

        var token = _tokenService.GenerateToken(user);

        var parts = token.Split('.');
        Assert.AreEqual(3, parts.Length, "A JWT token must consist of 3 parts separated by dots.");
    }
}
