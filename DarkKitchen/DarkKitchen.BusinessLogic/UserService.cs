using DarkKitchen.Domain.Users;
using DarkKitchen.Domain.Users.Encryptor;
using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class UserService(IUserRepository userRepository, IPhoneStrategyFactory strategyFactory, IPasswordHasher passwordHasher) : IUserService
{
    private readonly IPhoneStrategyFactory _strategyFactory = strategyFactory;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private static readonly IReadOnlyList<string> AllowedAdminRoles = ["Administrativo", "Preparador"];

    public UserCreateResponse CreateUser(UserCreateRequest request)
    {
        Role role;

        if(request.Role == null)
        {
            role = Role.Cliente;
        }
        else if(!AllowedAdminRoles.Contains(request.Role))
        {
            throw new ArgumentException($"Rol inválido. Los roles permitidos son: {string.Join(", ", AllowedAdminRoles)}.");
        }
        else
        {
            role = Enum.Parse<Role>(request.Role);
        }

        IPhoneValidationStrategy currentStrategy = _strategyFactory.GetStrategy(request.CountryPrefix);
        var validPhone = Domain.Users.PhoneNumber.Create(request.CountryPrefix, request.PhoneNumber, currentStrategy);
        var existingUser = _userRepository.GetUserByEmail(request.Email);
        if(existingUser != null)
        {
            throw new InvalidOperationException($"El email {request.Email} ya está en uso.");
        }

        var user = new User(request.Name, request.Surname, request.Email, validPhone, request.Password, role, _passwordHasher);
        _userRepository.Add(user);
        return Converter.ToUserCreateResponse(user);
    }

    public IEnumerable<UserCreateResponse> GetUsers(string? name, string? surname)
    {
        return _userRepository.GetByNameAndSurname(name, surname).Select(Converter.ToUserCreateResponse);
    }

    public UserCreateResponse UpdateUser(Guid adminId, Guid userId, UserUpdateRequest request)
    {
        if(adminId == userId)
        {
            throw new InvalidOperationException("Un usuario no puede modificarse a sí mismo.");
        }

        User existingUser = _userRepository.GetById(userId)
                            ?? throw new KeyNotFoundException($"Usuario {userId} no encontrado.");

        IPhoneValidationStrategy currentStrategy = _strategyFactory.GetStrategy(request.CountryPrefix);
        var validPhone = Domain.Users.PhoneNumber.Create(request.CountryPrefix, request.PhoneNumber, currentStrategy);
        Role role = Enum.Parse<Role>(request.Role);

        existingUser.UpdateDetails(
            request.Name,
            request.Surname,
            request.Email,
            validPhone,
            role);
        User userWithEmail = _userRepository.GetUserByEmail(request.Email);
        if(userWithEmail != null && userWithEmail.Id != userId)
        {
            throw new InvalidOperationException($"El email {request.Email} ya está en uso.");
        }

        _userRepository.Update(userId, existingUser);
        return Converter.ToUserCreateResponse(existingUser);
    }

    public UserCreateResponse DeleteUser(Guid adminId, Guid userId)
    {
        if(adminId == userId)
        {
            throw new InvalidOperationException("Un usuario no puede eliminarse a sí mismo.");
        }

        User user = _userRepository.GetById(userId)
                    ?? throw new KeyNotFoundException($"Usuario {userId} no encontrado.");

        _userRepository.Delete(userId);
        return Converter.ToUserCreateResponse(user);
    }
}
