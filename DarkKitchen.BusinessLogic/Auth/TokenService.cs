using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DarkKitchen.BusinessLogic.Auth;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var secret = _configuration.GetSection("JwtConfig:Secret").Value!;
        var key = Encoding.ASCII.GetBytes(secret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken? token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
}
