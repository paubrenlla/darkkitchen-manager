using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.Models.DTOs;

public class OrderCreateRequest
{
    [Required(ErrorMessage = "El tipo de entrega es obligatorio.")]
    public required string DeliveryType { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    public required OrderAddressDto Address { get; set; }

    [Required(ErrorMessage = "La lista de productos es obligatoria.")]
    [MinLength(1, ErrorMessage = "El pedido debe tener al menos un producto.")]
    public required List<OrderItemDto> Items { get; set; }
}

public class OrderAddressDto
{
    [Required]
    public required string Street { get; set; }

    [Required]
    public required string Number { get; set; }

    public string? Apartment { get; set; }

    [Required]
    public required string City { get; set; }

    [Required]
    public required string Country { get; set; }
}

public class OrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Quantity { get; set; }
}
