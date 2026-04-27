using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    UserCreateResponse CreateUser(UserCreateRequest request);
    IEnumerable<UserCreateResponse> GetUsers(string? name, string? surname);
    UserCreateResponse UpdateUser(Guid callerId, Guid userId, UserUpdateRequest request);
    UserCreateResponse DeleteUser(Guid adminId, Guid userId);
}
