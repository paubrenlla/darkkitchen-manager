using System.Text.RegularExpressions;

namespace DarkKitchen.Domain.Users;

public class User
{
    public User(string name, string surname, string email, string phone, string password,
        Role role, IPhoneValidationStrategy phoneStrategy)
    {
        ArgumentNullException.ThrowIfNull(phoneStrategy);

        ValidateName(name);
        ValidateSurname(surname);
        ValidateEmail(email);
        ValidatePassword(password);
        ValidatePhone(phone, phoneStrategy);

        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Phone = phone;
        Password = password;
        Role = role;
        Phone = phoneStrategy.CountryPrefix + phone;
    }

    public User(string name, string surname, string email, string phone, string password, IPhoneValidationStrategy phoneStrategy)
        : this(name, surname, email, phone, password, Role.Cliente, phoneStrategy)
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length < 1)
        {
            throw new ArgumentException("Name must have at least 1 character.");
        }
    }

    private static void ValidateSurname(string surname)
    {
        if(string.IsNullOrWhiteSpace(surname) || surname.Length < 3 || surname.Length > 25)
        {
            throw new ArgumentException("Surname must be between 3 and 25 characters.");
        }
    }

    private static void ValidateEmail(string email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9._-]+\.[a-zA-Z]{2,}$");

        if(string.IsNullOrWhiteSpace(email) || !emailRegex.IsMatch(email))
        {
            throw new ArgumentException("Invalid email format.");
        }
    }

    private static void ValidatePassword(string password)
    {
        if(string.IsNullOrWhiteSpace(password) || password.Length < 15 || password.Length > 25)
        {
            throw new ArgumentException("Password must be between 15 and 25 characters.");
        }

        if(!Regex.IsMatch(password, @"[A-Z]"))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }

        if(!Regex.IsMatch(password, @"[a-z]"))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }

        if(!Regex.IsMatch(password, @"[0-9]"))
        {
            throw new ArgumentException("Password must contain at least one number.");
        }

        if(!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
        {
            throw new ArgumentException("Password must contain at least one symbol.");
        }

        if(HasSequentialChars(password))
        {
            throw new ArgumentException("Password cannot contain sequences.");
        }

        bool HasSequentialChars(string text)
        {
            for(var i = 0; i < text.Length - 2; i++)
            {
                if(text[i] + 1 == text[i + 1] && text[i + 1] + 1 == text[i + 2])
                {
                    return true;
                }

                if(text[i] - 1 == text[i + 1] && text[i + 1] - 1 == text[i + 2])
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static void ValidatePhone(string phone, IPhoneValidationStrategy strategy)
    {
        if (!strategy.IsValid(phone))
        {
            throw new ArgumentException("Invalid phone format.");
        }
    }
}
