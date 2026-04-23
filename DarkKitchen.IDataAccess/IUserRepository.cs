using DarkKitchen.Domain.Users;

namespace DarkKitchen.IDataAccess;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    void Add(User user);
    User? GetById(Guid id);
    IEnumerable<User> GetByNameAndSurname(string? name, string? surname);
    void Update(Guid id, User user);
}
