using System.ComponentModel.DataAnnotations;
using DarkKitchen.Domain;
using DarkKitchen.Domain.Orders;
using DarkKitchen.Domain.Products;
using DarkKitchen.Domain.Users;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.Models.Tests;

[TestClass]
public class ModelTests
{
    [TestMethod]
    public void Converter_ToUserCreateResponse_MapsCorrectData()
    {
        var phone = PhoneNumber.Create("+598", "094111222", new UruguayPhoneValidationStrategy());
        var user = new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente);

        var result = Converter.ToUserCreateResponse(user);

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
            Role = "Administrativo"
        };

        Assert.AreEqual("Administrativo", request.Role);
    }

    [TestMethod]
    public void ToLoginResponse_ShouldMapCorrectly()
    {
        var phone = PhoneNumber.Create("+598", "094111222", new UruguayPhoneValidationStrategy());
        var user = new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente);

        var result = Converter.ToLoginResponse("my.jwt.token", user);

        Assert.AreEqual("my.jwt.token", result.Token);
        Assert.AreEqual("Cliente", result.Role);
    }

    [TestMethod]
    public void ToProductResponse_ShouldMapCorrectly()
    {
        var line = new ProductLine("Combo burgers");
        var category = new ProductCategory("Parrilla");

        List<ProductImage> images = [new("photo.jpg", 100000)];
        var product = new Product("BURG01", "Hamburguesa Clasica", "Hamburguesa clasica con queso cheddar", line,
            category, 150m, images);
        var result = Converter.ToProductResponse(product);

        Assert.AreEqual("BURG01", result.Code);
        Assert.AreEqual("Hamburguesa Clasica", result.Name);
        Assert.AreEqual("Hamburguesa clasica con queso cheddar", result.Description);
        Assert.AreEqual(150m, result.Price);
        Assert.AreEqual("Combo burgers", result.Line);
        Assert.AreEqual("Parrilla", result.Category);
    }

    [TestMethod]
    public void ToUserCreateResponse_ShouldMapCorrectly()
    {
        var phone = PhoneNumber.Create("+598", "094111222", new UruguayPhoneValidationStrategy());
        var user = new User("Juan", "Perez", "juan@test.com", phone, "Valid1Password!@", Role.Cliente);

        var result = Converter.ToUserCreateResponse(user);

        Assert.AreEqual(user.Id, result.Id);
        Assert.AreEqual("Juan", result.Name);
        Assert.AreEqual("Perez", result.Surname);
        Assert.AreEqual("juan@test.com", result.Email);
        Assert.AreEqual("+598094111222", result.Phone);
        Assert.AreEqual("Cliente", result.Role);
    }

    [TestMethod]
    public void ToOrderCreateResponse_ShouldMapCorrectly()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 2, 100m) };
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);
        order.AssignOrderNumber(42);

        var result = Converter.ToOrderCreateResponse(order);

        Assert.AreEqual(order.ClientId, result.ClientId);
        Assert.AreEqual(42, result.OrderNumber);
        Assert.AreEqual(order.Subtotal, result.Subtotal);
        Assert.AreEqual(order.Total, result.Total);
    }

    [TestMethod]
    public void ToOrderStatusResponse_ShouldMapCorrectly()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 1, 100m) };
        var order = new Order(Guid.NewGuid(), address, DeliveryType.Express, items);

        var result = Converter.ToOrderStatusResponse(order);

        Assert.AreEqual("Pending", result.Status);
        Assert.AreEqual(order.LastTransitionDate, result.LastTransitionDate);
    }

    [TestMethod]
    public void ToOrderDetailResponse_ShouldMapCorrectly()
    {
        var productId = Guid.NewGuid();
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(productId, 3, 50m) };
        var clientId = Guid.NewGuid();
        var order = new Order(clientId, address, DeliveryType.Express, items);
        order.AssignOrderNumber(10);

        var result = Converter.ToOrderDetailResponse(order);

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
    public void ToOrderListResponse_ShouldMapCorrectly()
    {
        var address = new Address("Rivera", "1234", null, "Montevideo", "Uruguay");
        var items = new List<OrderItem> { new(Guid.NewGuid(), 2, 100m), new(Guid.NewGuid(), 3, 50m) };
        var clientId = Guid.NewGuid();
        var order = new Order(clientId, address, DeliveryType.Express, items);
        order.AssignOrderNumber(5);

        var result = Converter.ToOrderListResponse(order);

        Assert.AreEqual(5, result.OrderNumber);
        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual("Pending", result.Status);
        Assert.AreEqual(order.Total, result.Total);
        Assert.AreEqual(5, result.ProductCount);
    }

    [TestMethod]
    public void ToPromotionCreateResponse_ShouldMapCorrectly()
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

        var result = Converter.ToPromotionCreateResponse(promotion);

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
            Name = null!, DiscountPercentage = 10, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(7)
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
