using DarkKitchen.Domain.Users;

namespace DarkKitchen.IBusinessLogic.IAuth;

public interface IAuthService
{
    LoginResult Login(string email, string password);
}
