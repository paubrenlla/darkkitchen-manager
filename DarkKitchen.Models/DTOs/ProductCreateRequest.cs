using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class ProductCreateRequest
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    public required string Code { get; set; }

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
}

public class ProductImageDto
{
    [Required]
    public required string Url { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "El tamaño debe ser mayor a cero.")]
    public long SizeInBytes { get; set; }
}
