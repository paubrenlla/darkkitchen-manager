using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    UserCreateResponse CreateUser(UserCreateRequest request);
    IEnumerable<UserCreateResponse> GetUsers(string? name, string? surname);
    UserCreateResponse UpdateUser(Guid adminId, Guid userId, UserUpdateRequest request);
}
