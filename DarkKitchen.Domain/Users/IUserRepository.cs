namespace DarkKitchen.Domain.Users;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
}
