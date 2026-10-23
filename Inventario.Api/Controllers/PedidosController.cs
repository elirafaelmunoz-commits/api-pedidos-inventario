using Inventario.Api.Dtos;
using Inventario.Api.Exceptions;
using Inventario.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _pedidoService;

    public PedidosController(PedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPedidoRequest request)
    {
        try
        {
            var pedido = await _pedidoService.CrearAsync(request);

            return StatusCode(201, new
            {
                pedido.Id,
                pedido.FechaPedido,
                pedido.Total,
                pedido.Estado
            });
        }
        catch (StockInsuficienteException ex)
        {
            return BadRequest(new
            {
                mensaje = ex.Message,
                productosRechazados = ex.ProductosRechazados
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}