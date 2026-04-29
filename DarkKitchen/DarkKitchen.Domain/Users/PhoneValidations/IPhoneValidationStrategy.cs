namespace DarkKitchen.Domain.Users.PhoneValidations;

public interface IPhoneValidationStrategy
{
    string CountryPrefix { get; }
    bool IsValid(string phoneNumber);
}
