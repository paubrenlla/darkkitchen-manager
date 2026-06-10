using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Domain.Users;

namespace DarkKitchen.Models.DTOs;

public class LoginResponse
{
    public required string Token { get; set; }
    public required string Role { get; set; }

    [SetsRequiredMembers]
    public LoginResponse(LoginResult result)
    {
        Token = result.Token;
        Role = result.User.Role.ToString();
    }
}
