using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class TokenServiceTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPassword = "Valid1Password!@";
    private static readonly IPhoneValidationStrategy UruguayStrategy = new UruguayPhoneValidationStrategy();
    private static readonly Domain.Users.PhoneNumber ValidPhone =
        Domain.Users.PhoneNumber.Create("+598", "094123456", UruguayStrategy);

    private Mock<IConfiguration> _configurationMock = null!;
    private Mock<IConfigurationSection> _configSectionMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private TokenService _tokenService = null!;

    [TestInitialize]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
        _configSectionMock = new Mock<IConfigurationSection>(MockBehavior.Strict);
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");

        _configSectionMock.Setup(s => s.Value).Returns("we_are_just_engineers_in_progress_trying_to_get_a_70_in_DA2");
        _configurationMock.Setup(c => c.GetSection("JwtConfig:Secret")).Returns(_configSectionMock.Object);

        _tokenService = new TokenService(_configurationMock.Object);
    }

    private User CreateTestUser() =>
        new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword, Role.Cliente, _passwordHasherMock.Object);

    [TestMethod]
    public void GenerateToken_ValidUser_ReturnsNonEmptyString()
    {
        var token = _tokenService.GenerateToken(CreateTestUser());

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));
        _configurationMock.VerifyAll();
        _configSectionMock.VerifyAll();
    }

    [TestMethod]
    public void GenerateToken_ValidUser_ReturnsJwtFormat()
    {
        var token = _tokenService.GenerateToken(CreateTestUser());

        var parts = token.Split('.');
        Assert.AreEqual(3, parts.Length, "A JWT token must consist of 3 parts separated by dots.");
        _configurationMock.VerifyAll();
        _configSectionMock.VerifyAll();
    }
}
