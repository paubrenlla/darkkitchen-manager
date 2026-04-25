using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class PromotionCreateRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public required string Name { get; set; }

    [Range(1, 100, ErrorMessage = "El descuento debe ser un número entre 1 y 100.")]

    public int DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public List<string> ProductCodes { get; set; } = [];
}
