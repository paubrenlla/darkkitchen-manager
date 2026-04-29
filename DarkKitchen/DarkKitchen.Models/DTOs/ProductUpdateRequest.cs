using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class ProductUpdateRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "La línea es obligatoria.")]
    public required string Line { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public required string Category { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Las imágenes son obligatorias.")]
    public required List<ProductImageDto> Images { get; set; }

    public bool? IsActive { get; set; }
}
