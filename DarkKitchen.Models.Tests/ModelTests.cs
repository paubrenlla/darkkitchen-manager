using System.ComponentModel.DataAnnotations;
using DarkKitchen.Domain.Users;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.Tests;

[TestClass]
public class ModelTests
{
    [TestMethod]
    public void UserCreateResponse_FromUser_MapsCorrectData()
    {
        // Arrange
        var phone = PhoneNumber.Create("+598", "094111222", new UruguayPhoneValidationStrategy());
        var user = new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente);

        // Act
        var result = UserCreateResponse.FromUser(user);

        // Assert
        Assert.AreEqual(user.Id, result.Id);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.Surname);
        Assert.AreEqual("juan@test.com", result.Email);
        Assert.AreEqual("+598094111222", result.Phone);
        Assert.AreEqual("Cliente", result.Role);
    }

    [TestMethod]
    public void UserCreateRequest_MissingName_ReturnsValidationErrors()
    {
        var request = new UserCreateRequest
        {
            Name = null!,
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111222",
            Password = "Pass123!"
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, null, null);
        var isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(validationResults.Any(v => v.ErrorMessage == "El nombre es obligatorio."));
    }

    [TestMethod]
    public void LoginRequest_InvalidEmail_ReturnsValidationErrors()
    {
        var request = new LoginRequest { Email = "invalid-email", Password = "ValidPassword123" };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, null, null);
        var isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(validationResults.Any(v => v.ErrorMessage == "El formato del email ingresado no es válido."));
    }

    [TestMethod]
    public void UserCreateRequest_SetAndGetRole_ReturnsCorrectValue()
    {
        var request = new UserCreateRequest
        {
            Name = "Juan",
            Surname = "Perez",
            Email = "juan@test.com",
            CountryPrefix = "+598",
            PhoneNumber = "094111222",
            Password = "Pass123!",
            Role = Role.Administrativo
        };

        Assert.AreEqual(Role.Administrativo, request.Role);
    }
}
