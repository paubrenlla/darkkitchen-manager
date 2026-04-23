using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.Converters;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class UserService(IUserRepository userRepository, IPhoneStrategyFactory strategyFactory) : IUserService
{
    private readonly IPhoneStrategyFactory _strategyFactory = strategyFactory;
    private readonly IUserRepository _userRepository = userRepository;

    public UserCreateResponse CreateUser(UserCreateRequest request)
    {
        IPhoneValidationStrategy currentStrategy = _strategyFactory.GetStrategy(request.CountryPrefix);

        var validPhone = Domain.Users.PhoneNumber.Create(request.CountryPrefix, request.PhoneNumber, currentStrategy);

        var role = request.Role != null ? Enum.Parse<Role>(request.Role) : Role.Cliente;

        var user = new User(
            request.Name,
            request.Surname,
            request.Email,
            validPhone,
            request.Password,
            role);

        _userRepository.Add(user);
        return Converter.ToUserCreateResponse(user);
    }

    public IEnumerable<UserCreateResponse> GetUsers(string? name, string? surname)
    {
        return _userRepository.GetByNameAndSurname(name, surname).Select(Converter.ToUserCreateResponse);
    }
}
