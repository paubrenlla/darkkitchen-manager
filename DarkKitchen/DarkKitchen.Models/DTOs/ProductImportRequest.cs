using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class ProductImportRequest
{
    [Required(ErrorMessage = "El nombre del importador es obligatorio.")]
    public required string ImporterName { get; set; }

    [Required(ErrorMessage = "La ruta del archivo es obligatoria.")]
    public required string FilePath { get; set; }
}
