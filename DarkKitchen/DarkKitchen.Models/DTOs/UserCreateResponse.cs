using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Users;

namespace DarkKitchen.Models.DTOs;

[method: SetsRequiredMembers]
public class UserCreateResponse(User user)
{
    public Guid Id { get; set; } = user.Id;
    public string Name { get; set; } = user.Name;
    public string Surname { get; set; } = user.Surname;
    public string Email { get; set; } = user.Email;
    public string Role { get; set; } = user.Role.ToString();
    public string PhoneNumber { get; set; } = user.Phone.Number;
}
