using DarkKitchen.Domain.Users.PhoneValidations;
using DarkKitchen.IBusinessLogic.IPhoneNumber;

namespace DarkKitchen.BusinessLogic.PhoneNumber;

public class PhoneStrategyFactory(IEnumerable<IPhoneValidationStrategy> strategies) : IPhoneStrategyFactory
{
    private readonly IEnumerable<IPhoneValidationStrategy> _strategies = strategies;

    public IPhoneValidationStrategy GetStrategy(string countryPrefix)
    {
        IPhoneValidationStrategy? strategy = _strategies.FirstOrDefault(s => s.CountryPrefix == countryPrefix);

        if(strategy == null)
        {
            throw new NotSupportedException($"El país con prefijo '{countryPrefix}' aún no está soportado.");
        }

        return strategy;
    }
}
