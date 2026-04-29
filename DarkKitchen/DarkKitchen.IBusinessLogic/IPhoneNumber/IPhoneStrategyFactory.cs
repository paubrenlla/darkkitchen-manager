using DarkKitchen.Domain.Users.PhoneValidations;

namespace DarkKitchen.IBusinessLogic.IPhoneNumber;

public interface IPhoneStrategyFactory
{
    IPhoneValidationStrategy GetStrategy(string countryPrefix);
}
