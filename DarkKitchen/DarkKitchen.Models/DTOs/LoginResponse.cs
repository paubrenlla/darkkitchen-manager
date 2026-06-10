using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Users;

namespace DarkKitchen.Models.DTOs;

[method: SetsRequiredMembers]
public class LoginResponse(LoginResult result)
{
    public required string Token { get; set; } = result.Token;
    public required string Role { get; set; } = result.User.Role.ToString();
}
