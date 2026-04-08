namespace DarkKitchen.Domain.Users;

public interface IPhoneValidationStrategy
{
    bool IsValid(string phoneNumber);
}
