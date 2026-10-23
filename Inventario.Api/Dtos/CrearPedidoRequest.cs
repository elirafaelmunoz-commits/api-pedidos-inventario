using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Dtos;

public class CrearPedidoRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "El pedido debe tener al menos un producto.")]
    public List<PedidoItemRequest> Items { get; set; } = new();
}