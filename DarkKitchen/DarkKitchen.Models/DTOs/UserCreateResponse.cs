namespace DarkKitchen.Models.DTOs;

public class UserCreateResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Role { get; set; }
}
