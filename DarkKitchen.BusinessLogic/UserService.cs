using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IPhoneNumber;
using DarkKitchen.IDataAccess;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.BusinessLogic;

public class UserService(IUserRepository userRepository, IPhoneStrategyFactory strategyFactory) : IUserService
{
    private readonly IPhoneStrategyFactory _strategyFactory = strategyFactory;
    private readonly IUserRepository _userRepository = userRepository;

    public User CreateUser(UserCreateRequest request)
    {
        IPhoneValidationStrategy currentStrategy = _strategyFactory.GetStrategy(request.CountryPrefix);

        var validPhone = Domain.Users.PhoneNumber.Create(request.CountryPrefix, request.PhoneNumber, currentStrategy);

        var user = new User(
            request.Name,
            request.Surname,
            request.Email,
            validPhone,
            request.Password,
            Role.Cliente);

        _userRepository.Add(user);
        return user;
    }
}
