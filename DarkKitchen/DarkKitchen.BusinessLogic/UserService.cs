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
        Role role = ParseRole(request.Role);
        EnsureEmailNotTaken(request.Email);
        var validPhone = CreateValidPhone(request.CountryPrefix, request.PhoneNumber);
        var user = new User(request.Name, request.Surname, request.Email, validPhone, request.Password, role, _passwordHasher);
        _userRepository.Add(user);
        return Converter.ToUserCreateResponse(user);
    }

    private Role ParseRole(string? roleName)
    {
        if(roleName == null)
        {
            return Role.Cliente;
        }

        if(!AllowedAdminRoles.Contains(roleName))
        {
            throw new ArgumentException($"Rol inválido. Los roles permitidos son: {string.Join(", ", AllowedAdminRoles)}.");
        }

        return Enum.Parse<Role>(roleName);
    }

    private void EnsureEmailNotTaken(string email)
    {
        if(_userRepository.GetUserByEmail(email) != null)
        {
            throw new InvalidOperationException($"El email {email} ya está en uso.");
        }
    }

    private Domain.Users.PhoneNumber CreateValidPhone(string prefix, string number)
    {
        IPhoneValidationStrategy strategy = _strategyFactory.GetStrategy(prefix);
        return Domain.Users.PhoneNumber.Create(prefix, number, strategy);
    }

    public IEnumerable<UserCreateResponse> GetUsers(string? name, string? surname)
    {
        return _userRepository.GetByNameAndSurname(name, surname).Select(Converter.ToUserCreateResponse);
    }

    public UserCreateResponse UpdateUser(Guid adminId, Guid userId, UserUpdateRequest request)
    {
        ValidateSelfModification(adminId, userId);

        User existingUser = _userRepository.GetById(userId)
                            ?? throw new KeyNotFoundException($"Usuario {userId} no encontrado.");

        ValidateEmailNotTaken(request.Email, userId);

        var validPhone = CreateValidPhone(request.CountryPrefix, request.PhoneNumber);
        Role role = Enum.Parse<Role>(request.Role);

        existingUser.UpdateDetails(request.Name, request.Surname, request.Email, validPhone, role);
        _userRepository.Update(userId, existingUser);
        return Converter.ToUserCreateResponse(existingUser);
    }

    private void ValidateSelfModification(Guid adminId, Guid userId)
    {
        if(adminId == userId)
        {
            throw new InvalidOperationException("Un usuario no puede modificarse a sí mismo.");
        }
    }

    private void ValidateEmailNotTaken(string email, Guid excludeUserId)
    {
        var userWithEmail = _userRepository.GetUserByEmail(email);
        if(userWithEmail != null && userWithEmail.Id != excludeUserId)
        {
            throw new InvalidOperationException($"El email {email} ya está en uso.");
        }
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
