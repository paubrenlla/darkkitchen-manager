namespace DarkKitchen.Domain.Users;

public class PhoneNumber
{
    public PhoneNumber(string countryPrefix, string number, IPhoneValidationStrategy validationStrategy)
    {
        if(!validationStrategy.IsValid(countryPrefix, number))
        {
            throw new ArgumentException($"Invalid phone number for country prefix {validationStrategy.CountryPrefix}.");
        }

        CountryPrefix = countryPrefix;
        Number = number;
    }

    public string Number { get; private set; }
    public string CountryPrefix { get; private set; }
}
