using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class UserUpdateRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public required string Surname { get; set; }

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El código de país es obligatorio.")]
    public required string CountryPrefix { get; set; }

    [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
    public required string PhoneNumber { get; set; }

    [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
    public required string Role { get; set; }
}
