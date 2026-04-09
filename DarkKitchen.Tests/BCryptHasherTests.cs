using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

[TestClass]
public class BCryptHasherTests
{
    private const string RawPassword = "Valid1Password!@";
    private BCryptHasher _hasher = null!;

    [TestInitialize]
    public void Setup()
    {
        _hasher = new BCryptHasher();
    }

    [TestMethod]
    public void HashPassword_ShouldReturnHashedString()
    {
        var hashedPassword = _hasher.HashPassword(RawPassword);

        Assert.IsNotNull(hashedPassword);
        Assert.AreNotEqual(hashedPassword, RawPassword);
        Assert.IsTrue(hashedPassword.StartsWith("$2"));
    }

    [TestMethod]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var hash = _hasher.HashPassword(RawPassword);

        bool result = _hasher.VerifyPassword(RawPassword, hash);

        Assert.IsTrue(result);
    }
}
