using DarkKitchen.BusinessLogic.Auth;
using DarkKitchen.Domain.Users;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class TokenServiceTests
{
    private Mock<IConfiguration> _configurationMock = null!;
    private TokenService _tokenService = null!;

    [TestInitialize]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>();

        // Set up the mock configuration to return a dummy secret key
        var configSectionMock = new Mock<IConfigurationSection>();
        configSectionMock.Setup(s => s.Value).Returns("we_are_just_engineers_in_progress_trying_to_get_a_70_in_DA2");

        _configurationMock.Setup(c => c.GetSection("JwtConfig:Secret")).Returns(configSectionMock.Object);

        _tokenService = new TokenService(_configurationMock.Object);
    }

    [TestMethod]
    public void GenerateToken_ValidUser_ReturnsNonEmptyString()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@bmb.com", Password = "Pass", Role = Role.Cliente };

        var token = _tokenService.GenerateToken(user);

        Assert.IsFalse(string.IsNullOrWhiteSpace(token));
    }

    [TestMethod]
    public void GenerateToken_ValidUser_ReturnsJwtFormat()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@bmb.com", Password = "Pass", Role = Role.Cliente };

        var token = _tokenService.GenerateToken(user);

        var parts = token.Split('.');
        Assert.AreEqual(3, parts.Length, "A JWT token must consist of 3 parts separated by dots.");
    }
}
