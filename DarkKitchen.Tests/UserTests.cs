using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

[TestClass]
public class UserTests
{
    private const string ValidName = "Juan";
    private const string ValidSurname = "Perez";
    private const string ValidEmail = "test@domain.com";
    private const string ValidPhone = "094123456";
    private const string ValidPassword = "Valid1Password!@";

    [TestMethod]
    public void CreateUser_WithValidData_CreatesSuccessfully()
    {
        var user = new User(ValidName, ValidSurname, ValidEmail, ValidPhone, ValidPassword);

        Assert.IsNotNull(user);
        Assert.AreEqual(ValidName, user.Name);
        Assert.AreEqual(Role.Cliente, user.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Name must have at least 1 character.")]
    public void CreateUser_WithEmptyName_ThrowsArgumentException()
    {
        var name = string.Empty;
        new User(name, ValidSurname, ValidEmail, ValidPhone, ValidPassword);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException), "Name must have at least 1 character.")]
    public void CreateUser_WithNullName_ThrowsArgumentException()
    {
        string name = null!;
        new User(name, ValidSurname, ValidEmail, ValidPhone, ValidPassword);
    }
}
