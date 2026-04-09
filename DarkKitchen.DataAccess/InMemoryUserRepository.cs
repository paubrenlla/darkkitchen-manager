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
                PhoneNumber.Create("+598", "094222333", uruguayStrategy),
                mockPassword,
                Role.Administrativo),
            new User(
                "Pepe",
                "Ruiz",
                "preparador@bmb.com",
                PhoneNumber.Create("+598", "094333444", uruguayStrategy),
                mockPassword,
                Role.Preparador),
            new User(
                "Juan",
                "Sosa",
                "cliente@bmb.com",
                PhoneNumber.Create("+598", "094444555", uruguayStrategy),
                mockPassword)
        ];
    }

    public User? GetUserByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
