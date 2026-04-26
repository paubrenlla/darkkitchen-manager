using DarkKitchen.Domain.Users;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess;

public class SqlUserRepository(DarkKitchenContext context) : IUserRepository
{
    private readonly DarkKitchenContext _context = context;

    public User? GetUserByEmail(string email)
    {
        return _context.Users
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public User? GetById(Guid id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public IEnumerable<User> GetByNameAndSurname(string? name, string? surname)
    {
        var query = _context.Users.AsQueryable();

        if(!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(u => u.Name.Contains(name));
        }

        if(!string.IsNullOrWhiteSpace(surname))
        {
            query = query.Where(u => u.Surname.Contains(surname));
        }

        return query.ToList();
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(Guid id, User user)
    {
        var existing = _context.Users.FirstOrDefault(u => u.Id == id)
                       ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        _context.Entry(existing).CurrentValues.SetValues(user);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id)
                   ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        _context.Users.Remove(user);
        _context.SaveChanges();
    }
}
