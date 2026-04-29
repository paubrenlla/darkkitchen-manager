namespace DarkKitchen.Domain.Users;

public interface IPhoneValidationStrategy
{
    string CountryPrefix { get; }
    bool IsValid(string phoneNumber);
}
