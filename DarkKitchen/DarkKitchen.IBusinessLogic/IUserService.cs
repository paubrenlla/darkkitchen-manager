using DarkKitchen.Domain.Users;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    User CreateUser(UserCreateRequest request);
    IEnumerable<User> GetUsers(string? name, string? surname);
    User UpdateUser(Guid callerId, Guid userId, UserUpdateRequest request);
    void DeleteUser(Guid adminId, Guid userId);
}
