using DarkKitchen.Domain.Users;

namespace DarkKitchen.IBusinessLogic.IAuth;

public interface IAuthService
{
    User Login(string email, string password);
}
