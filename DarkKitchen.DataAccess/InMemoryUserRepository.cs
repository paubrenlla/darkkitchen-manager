using DarkKitchen.Domain.Users;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

    public InMemoryUserRepository()
    {
        var uruguayStrategy = new UruguayPhoneValidationStrategy();
        var mockPassword = "ValidP@ssw0rd!8X";
        _users =
        [
            new User(
                "Admin",
                "Jefe",
                "admin@bmb.com",
                "099123456",
                mockPassword,
                Role.Administrativo,
                uruguayStrategy),
            new User(
                "Pepe",
                "Ruiz",
                "preparador@bmb.com",
                "098765432",
                mockPassword,
                Role.Preparador,
                uruguayStrategy),
            new User(
                "Juan",
                "Sosa",
                "cliente@bmb.com",
                "091234567",
                mockPassword,
                uruguayStrategy)
        ];
    }

    public User? GetUserByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
