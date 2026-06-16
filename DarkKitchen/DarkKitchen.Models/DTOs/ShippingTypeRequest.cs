using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class ShippingTypeRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public required string Name { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo no puede ser negativo.")]
    public decimal Cost { get; set; }
}
