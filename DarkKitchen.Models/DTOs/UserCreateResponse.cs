using DarkKitchen.Domain.Users;

namespace DarkKitchen.Models.DTOs;

public class UserCreateResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Role { get; set; }

    public static UserCreateResponse FromUser(User user)
    {
        var fullPhone = $"{user.Phone.CountryPrefix}{user.Phone.Number}";

        return new UserCreateResponse
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Phone = fullPhone,
            Role = user.Role.ToString()
        };
    }
}
