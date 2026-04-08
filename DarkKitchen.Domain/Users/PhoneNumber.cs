namespace DarkKitchen.Domain.Users;

public class PhoneNumber
{
    public string Number { get; private set; }
    public string CountryPrefix { get; private set; }

    public PhoneNumber(string number, IPhoneValidationStrategy validationStrategy)
    {
        if(!validationStrategy.IsValid(number))
        {
            throw new ArgumentException($"Invalid phone number for country prefix {validationStrategy.CountryPrefix}.");
        }

        Number = number;
        CountryPrefix = validationStrategy.CountryPrefix;
    }
}
