using DarkKitchen.Domain.Users;

namespace DarkKitchen.IBusinessLogic.IPhoneNumber;

public interface IPhoneStrategyFactory
{
    IPhoneValidationStrategy GetStrategy(string countryPrefix);
}
