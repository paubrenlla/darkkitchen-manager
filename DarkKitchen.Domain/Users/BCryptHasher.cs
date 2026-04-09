using BCrypt.Net;
namespace DarkKitchen.Domain.Users;

public class BCryptHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }
}
