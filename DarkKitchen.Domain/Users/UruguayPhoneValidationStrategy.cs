namespace DarkKitchen.Domain.Users;

public class UruguayPhoneValidationStrategy
{
    public bool IsValid(string phone)
    {
        if(phone.Length == 9 && phone.StartsWith("09"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
