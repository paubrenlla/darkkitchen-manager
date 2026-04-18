using DarkKitchen.Domain;
using DarkKitchen.Domain.Users;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.Models.Converters;

public static class Converter
{
    public static LoginResponse ToLoginResponse(string token, User user)
    {
        return new LoginResponse
        {
            Token = token,
            Role = user.Role.ToString(),
        };
    }

    public static ProductResponse ToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Line = product.Line.Name,
            Category = product.Category.Name,
        };
    }

    public static UserCreateResponse ToUserCreateResponse(User user)
    {
        return new UserCreateResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Phone = $"{user.Phone.CountryPrefix}{user.Phone.Number}",
            Role = user.Role.ToString(),
        };
    }
}
