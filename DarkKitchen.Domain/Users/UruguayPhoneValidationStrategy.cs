using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Users;

public class UruguayPhoneValidationStrategy
{
    public bool IsValid(string phone)
    {
        if(phone != null && phone.Length == 9 && phone.StartsWith("09") && Regex.IsMatch(phone, @"^09[1-9][0-9]{6}$"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
