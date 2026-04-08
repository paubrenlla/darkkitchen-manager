namespace DarkKitchen.Domain.Users;

public class User
{
    public User(string name, string surname, string email, string phone, string password,
        Role role)
    {
        ValidateName(name);

        Id = Guid.NewGuid();
        Name = name;
        Surname = surname;
        Email = email;
        Phone = phone;
        Password = password;
        Role = role;
    }

    public User(string name, string surname, string email, string phone, string password)
        : this(name, surname, email, phone, password, Role.Cliente)
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
}
