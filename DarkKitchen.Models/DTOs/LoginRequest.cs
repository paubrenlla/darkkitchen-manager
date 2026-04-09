using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "El email es obligatorio para iniciar sesión.")]
    [EmailAddress(ErrorMessage = "El formato del email ingresado no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria para iniciar sesión.")]
    public required string Password { get; set; }
}
