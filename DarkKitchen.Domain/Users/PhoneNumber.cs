using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Users;

public class PhoneNumber
{
    private PhoneNumber(string countryPrefix, string number)
    {
        CountryPrefix = countryPrefix;
        Number = number;
    }

    public string Number { get; private set; }
    public string CountryPrefix { get; private set; }

    public static PhoneNumber Create(string prefix, string number, IPhoneValidationStrategy validationStrategy)
    {
        ArgumentNullException.ThrowIfNull(validationStrategy);

        var cleanNumber = Regex.Replace(number ?? string.Empty, @"[^\d]", string.Empty);

        if(!validationStrategy.IsValid(cleanNumber))
        {
            throw new ArgumentException($"Invalid phone number for country prefix {validationStrategy.CountryPrefix}.");
        }

        return new PhoneNumber(validationStrategy.CountryPrefix, cleanNumber);
    }
}
