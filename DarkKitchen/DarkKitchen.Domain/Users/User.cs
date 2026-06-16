using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DarkKitchen.Domain.Users.Encryptor;

namespace DarkKitchen.Domain.Users;

public class User
{
    [ExcludeFromCodeCoverage]
    protected User()
    {
        Name = null!;
        Surname = null!;
        Email = null!;
        Phone = null!;
        HashedPassword = null!;
    }

    public User(string name, string surname, string email, PhoneNumber phone, string password, Role role, IPasswordHasher passwordHasher)
    {
        ValidateName(name);
        ValidateSurname(surname);
        ValidateEmail(email);
        ValidatePassword(password);

        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Phone = phone;
        HashedPassword = passwordHasher.HashPassword(password);
        Role = role;
    }

    public User(string name, string surname, string email, PhoneNumber phone, string password, IPasswordHasher passwordHasher)
        : this(name, surname, email, phone, password, Role.Cliente, passwordHasher)
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public string HashedPassword { get; private set; }
    public Role Role { get; private set; }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name) || name.Length < 1)
        {
            throw new ArgumentException("El nombre debe tener al menos 1 carácter.");
        }
    }

    private static void ValidateSurname(string surname)
    {
        if(string.IsNullOrWhiteSpace(surname) || surname.Length < 3 || surname.Length > 25)
        {
            throw new ArgumentException("El apellido debe tener entre 3 y 25 caracteres.");
        }
    }

    private static void ValidateEmail(string email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9._-]+\.[a-zA-Z]{2,}$");

        if(string.IsNullOrWhiteSpace(email) || !emailRegex.IsMatch(email))
        {
            throw new ArgumentException("El formato del email es inválido.");
        }
    }

    private static void ValidatePassword(string password)
    {
        if(string.IsNullOrWhiteSpace(password) || password.Length < 15 || password.Length > 25)
        {
            throw new ArgumentException("La contraseña debe tener entre 15 y 25 caracteres.");
        }

        if(!Regex.IsMatch(password, @"[A-Z]"))
        {
            throw new ArgumentException("La contraseña debe contener al menos una letra mayúscula.");
        }

        if(!Regex.IsMatch(password, @"[a-z]"))
        {
            throw new ArgumentException("La contraseña debe contener al menos una letra minúscula.");
        }

        if(!Regex.IsMatch(password, @"[0-9]"))
        {
            throw new ArgumentException("La contraseña debe contener al menos un número.");
        }

        if(!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
        {
            throw new ArgumentException("La contraseña debe contener al menos un símbolo.");
        }

        if(HasSequentialChars(password))
        {
            throw new ArgumentException("La contraseña no puede contener secuencias.");
        }
    }

    private static bool HasSequentialChars(string text)
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

    public void UpdateDetails(string name, string surname, string email, PhoneNumber phone, Role role)
    {
        ValidateName(name);
        ValidateSurname(surname);
        ValidateEmail(email);

        Name = name;
        Surname = surname;
        Email = email;
        Phone = phone;
        Role = role;
    }
}
