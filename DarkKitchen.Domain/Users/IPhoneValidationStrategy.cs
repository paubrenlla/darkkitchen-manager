namespace DarkKitchen.Domain.Users;

public interface IPhoneValidationStrategy
{
    bool isValid(string phoneNumber);
}
