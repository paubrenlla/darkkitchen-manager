using DarkKitchen.Domain.Users;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

    public InMemoryUserRepository()
    {
        _users =
        [
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@bmb.com",
                Password = "Password123!",
                Role = Role.Administrativo
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "preparador@bmb.com",
                Password = "Password123!",
                Role = Role.Preparador
            },
            new User { Id = Guid.NewGuid(), Email = "cliente@bmb.com", Password = "Password123!", Role = Role.Cliente }
        ];
    }

    public User? GetUserByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
