namespace DarkKitchen.Domain.Users;

public class LoginResult
{
    public string Token { get; }
    public User User { get; }

    public LoginResult(string token, User user)
    {
        Token = token;
        User = user;
    }
}
