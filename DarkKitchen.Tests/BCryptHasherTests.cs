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
        string hashedPassword = _hasher.HashPassword(RawPassword);

        Assert.IsNotNull(hashedPassword);
        Assert.AreEqual(hashedPassword, RawPassword);
        Assert.IsTrue(hashedPassword.StartsWith("$2"));
    }
}
