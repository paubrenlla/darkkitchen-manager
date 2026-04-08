using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Users;

public class UruguayPhoneValidationStrategy
{
    public bool IsValid(string phone)
    {
        if(string.IsNullOrWhiteSpace(phone))
        {
            return false;
        }

        return Regex.IsMatch(phone, @"^09[1-9][0-9]{6}$");
    }
}
