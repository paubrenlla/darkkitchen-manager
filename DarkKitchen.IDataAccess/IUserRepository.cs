using DarkKitchen.Domain.Users;

namespace DarkKitchen.IDataAccess;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
}
