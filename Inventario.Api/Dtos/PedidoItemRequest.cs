using System.ComponentModel.DataAnnotations;

namespace Inventario.Api.Dtos;

public class PedidoItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "ProductoId debe ser mayor que cero.")]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Cantidad debe ser mayor que cero.")]
    public int Cantidad { get; set; }
}