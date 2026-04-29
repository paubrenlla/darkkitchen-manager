using DarkKitchen.Domain.Users;

namespace DarkKitchen.IBusinessLogic.IAuth;

public interface ITokenService
{
    string GenerateToken(User user);
}
