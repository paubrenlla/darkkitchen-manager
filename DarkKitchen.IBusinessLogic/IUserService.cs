using DarkKitchen.Domain.Users;
using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    User CreateUser(UserCreateRequest request);
}
