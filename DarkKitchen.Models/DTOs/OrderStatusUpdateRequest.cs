using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class OrderStatusUpdateRequest
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public required string Status { get; set; }
}
