using System.ComponentModel.DataAnnotations;
using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Promotions;
using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.Models.DTOs;
using Moq;

namespace DarkKitchen.Models.Tests;

[TestClass]
public class ModelTests
{
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
            Role = "Administrativo"
        };

        Assert.AreEqual("Administrativo", request.Role);
    }

    [TestMethod]
    public void LoginResponse_Constructor_MapsCorrectly()
    {
        var phone = PhoneNumber.Create("+598", "094111222", new UruguayPhoneValidationStrategy());
        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashed");
        var user = new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente, hasher.Object);
        var loginResult = new LoginResult("my.jwt.token", user);

        var result = new LoginResponse(loginResult);

        Assert.AreEqual("my.jwt.token", result.Token);
        Assert.AreEqual("Cliente", result.Role);
    }

    [TestMethod]
    public void ProductResponse_Constructor_MapsCorrectly()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var images = new List<ProductImage> { new("photo.jpg", 100000) };
        var product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar",
            line, category, 150m, images);

        var result = new ProductResponse(product);

        Assert.AreEqual("BURG01", result.Code);
        Assert.AreEqual("Hamburguesa Clasica", result.Name);
        Assert.AreEqual("Hamburguesa clasica con queso cheddar", result.Description);
        Assert.AreEqual(150m, result.Price);
        Assert.AreEqual("Combo burgers", result.Line);
        Assert.AreEqual("Parrilla", result.Category);
        Assert.AreEqual(1, result.Images.Count);
        Assert.AreEqual("photo.jpg", result.Images[0].Url);
        Assert.AreEqual(100000, result.Images[0].SizeInBytes);
    }

    [TestMethod]
    public void OrderCreateResponse_Constructor_MapsCorrectly()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 2, 100m) };
        var order = new Order(Guid.NewGuid(), address, "Express", items, 0m);
        order.AssignOrderNumber(42);

        var result = new OrderCreateResponse(order);

        Assert.AreEqual(order.ClientId, result.ClientId);
        Assert.AreEqual(42, result.OrderNumber);
        Assert.AreEqual(order.Subtotal, result.Subtotal);
        Assert.AreEqual(order.Total, result.Total);
    }

    [TestMethod]
    public void OrderDetailResponse_Constructor_MapsCorrectly()
    {
        var productId = Guid.NewGuid();
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(productId, 3, 50m) };
        var clientId = Guid.NewGuid();
        var order = new Order(clientId, address, "Express", items, 0m);
        order.AssignOrderNumber(10);

        var result = new OrderDetailResponse(order);

        Assert.AreEqual(10, result.OrderNumber);
        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual("Pending", result.Status);
        Assert.AreEqual(order.Total, result.Total);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(productId, result.Items[0].ProductId);
        Assert.AreEqual(3, result.Items[0].Quantity);
        Assert.AreEqual(50m, result.Items[0].Price);
        Assert.AreEqual(150m, result.Items[0].ItemTotal);
    }

    [TestMethod]
    public void PromotionCreateResponse_Constructor_MapsCorrectly()
    {
        var start = new DateTime(2025, 1, 1);
        var end = new DateTime(2025, 1, 31);
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");
        var products = new List<Product>
        {
            new("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line, category, 150m,
                [new ProductImage("img1.jpg", 100000)]),
            new("BURG02", "Hamburguesa Doble", "Hamburguesa doble con queso cheddar y bacon", line, category, 200m,
                [new ProductImage("img2.jpg", 150000)])
        };
        var promotion = new Promotion("Promo Verano", 20, start, end, products);

        var result = new PromotionCreateResponse(promotion);

        Assert.AreEqual("Promo Verano", result.Name);
        Assert.AreEqual(20, result.DiscountPercentage);
        Assert.AreEqual(start, result.StartDate);
        Assert.AreEqual(end, result.EndDate);
        Assert.AreEqual(2, result.Products.Count);
        Assert.IsTrue(result.Products.Contains("BURG01"));
        Assert.IsTrue(result.Products.Contains("BURG02"));
    }

    [TestMethod]
    public void PromotionCreateRequest_MissingName_ReturnsValidationError()
    {
        var request = new PromotionCreateRequest
        {
            Name = null!,
            DiscountPercentage = 10,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7)
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, null, null);
        var isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(validationResults.Any(v => v.ErrorMessage == "El nombre es obligatorio."));
    }

    [TestMethod]
    public void PromotionCreateRequest_DiscountBelowRange_ReturnsValidationError()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 0,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7)
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, null, null);
        var isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(validationResults.Any(v => v.ErrorMessage == "El descuento debe ser un número entre 1 y 100."));
    }

    [TestMethod]
    public void PromotionCreateRequest_DiscountAboveRange_ReturnsValidationError()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 101,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7)
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, null, null);
        var isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(validationResults.Any(v => v.ErrorMessage == "El descuento debe ser un número entre 1 y 100."));
    }

    [TestMethod]
    public void PromotionCreateRequest_ValidData_PassesValidation()
    {
        var request = new PromotionCreateRequest
        {
            Name = "Promo Verano",
            DiscountPercentage = 20,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7),
            ProductCodes = ["BURG01", "BURG02"]
        };

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, null, null);
        var isValid = Validator.TryValidateObject(request, context, validationResults, true);

        Assert.IsTrue(isValid);
        Assert.AreEqual(0, validationResults.Count);
    }
}
