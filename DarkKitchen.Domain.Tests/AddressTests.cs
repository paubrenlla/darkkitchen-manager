using DarkKitchen.Domain.Users;

namespace DarkKitchen.Tests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void CreateAddress_WithValidData_ShouldSetProperties()
    {
        var street = "18 de Julio";
        var number = "1451";
        var apartment = "101";
        var city = "Montevideo";
        var country = "Uruguay";

        var address = new Address(street, number, apartment, city, country);

        Assert.AreEqual(street, address.Street);
        Assert.AreEqual(city, address.City);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateAddress_WithEmptyCity_ShouldThrowException()
    {
        new Address("Calle", "123", "Apt", string.Empty, "Uruguay");
    }
}
