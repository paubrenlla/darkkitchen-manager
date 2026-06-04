using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Auth;

public class AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public LoginResult Login(string email, string password)
    {
        User? user = _userRepository.GetUserByEmail(email);

        if(user == null || !_passwordHasher.VerifyPassword(password, user.HashedPassword))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var token = _tokenService.GenerateToken(user);
        return new LoginResult(token, user);
    }
}
