using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic.Auth;

public class AuthService(IUserRepository userRepository, ITokenService tokenService, IPasswordHasher passwordHasher) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public LoginResponse Login(string email, string password)
    {
        User? user = _userRepository.GetUserByEmail(email);

        if(user == null || !_passwordHasher.VerifyPassword(password, user.HashedPassword))
        {
            throw new UnauthorizedAccessException("Credenciales inválidas.");
        }

        var token = _tokenService.GenerateToken(user);
        return Converter.ToLoginResponse(token, user);
    }
}
