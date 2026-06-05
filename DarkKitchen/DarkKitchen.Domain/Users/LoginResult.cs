namespace DarkKitchen.Domain.Users;

public class LoginResult(string token, User user)
{
    public string Token { get; } = token;
    public User User { get; } = user;
}
