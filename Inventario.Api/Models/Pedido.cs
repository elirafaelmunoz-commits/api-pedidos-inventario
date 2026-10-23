namespace Inventario.Api.Models;

public class Pedido
{
    public int Id { get; set; }
    public DateTime FechaPedido { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
}