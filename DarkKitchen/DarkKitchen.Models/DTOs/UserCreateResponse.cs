using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Users;

namespace DarkKitchen.Models.DTOs;

public class UserCreateResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public UserCreateResponse()
    {
    }

    [SetsRequiredMembers]
    public UserCreateResponse(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Surname = user.Surname;
        Email = user.Email;
        Role = user.Role.ToString();
        PhoneNumber = user.Phone.Number;
    }
}
