namespace DarkKitchen.Domain.Users;

public class UruguayPhoneValidationStrategy
{
    public bool IsValid(string phone)
    {
        if(phone.Length == 9)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
