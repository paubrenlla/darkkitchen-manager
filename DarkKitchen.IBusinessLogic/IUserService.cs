using DarkKitchen.Models.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    UserCreateResponse CreateUser(UserCreateRequest request);
}
