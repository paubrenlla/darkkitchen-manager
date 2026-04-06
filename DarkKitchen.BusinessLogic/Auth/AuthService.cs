using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;

namespace DarkKitchen.BusinessLogic.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User Login(string email, string password)
    {
        User? user = _userRepository.GetUserByEmail(email);

        if(user == null || user.Password != password)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        return user;
    }
}
