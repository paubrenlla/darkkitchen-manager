using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic.IAuth;

public interface IAuthService
{
    LoginResponse Login(string email, string password);
}
