using Inventario.Api.Data;
using Inventario.Api.Dtos;
using Inventario.Api.Exceptions;
using Inventario.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Services;

public class PedidoService
{
    private readonly InventarioDbContext _db;

    public PedidoService(InventarioDbContext db)
    {
        _db = db;
    }

    public async Task<Pedido> CrearAsync(CrearPedidoRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("El pedido debe tener al menos un producto.");

        if (request.Items.Any(i => i.ProductoId <= 0 || i.Cantidad <= 0))
            throw new ArgumentException("Los productos y cantidades deben ser mayores que cero.");

        if (request.Items.Select(i => i.ProductoId).Distinct().Count() != request.Items.Count)
            throw new ArgumentException("No repitas un producto dentro del mismo pedido.");

        await using var transaccion = await _db.Database.BeginTransactionAsync();

        try
        {
            var ids = request.Items.Select(i => i.ProductoId).ToList();

            var productos = await _db.Productos
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var inexistentes = ids.Where(id => !productos.ContainsKey(id)).ToList();

            if (inexistentes.Count > 0)
                throw new ArgumentException(
                    $"No existen los productos: {string.Join(", ", inexistentes)}");

            var rechazados = new List<int>();

            // El mismo orden reduce la posibilidad de bloqueos entre pedidos simultáneos.
            foreach (var item in request.Items.OrderBy(i => i.ProductoId))
            {
                var filasAfectadas = await _db.Productos
                    .Where(p => p.Id == item.ProductoId && p.Stock >= item.Cantidad)
                    .ExecuteUpdateAsync(cambios =>
                        cambios.SetProperty(p => p.Stock, p => p.Stock - item.Cantidad));

                if (filasAfectadas == 0)
                    rechazados.Add(item.ProductoId);
            }

            if (rechazados.Count > 0)
                throw new StockInsuficienteException(rechazados);

            var pedido = new Pedido
            {
                FechaPedido = DateTime.UtcNow,
                Estado = "Confirmado",
                Total = request.Items.Sum(i =>
                    productos[i.ProductoId].Precio * i.Cantidad)
            };

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync();

            var detalles = request.Items.Select(item => new PedidoDetalle
            {
                PedidoId = pedido.Id,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = productos[item.ProductoId].Precio
            });

            _db.PedidoDetalles.AddRange(detalles);
            await _db.SaveChangesAsync();

            await transaccion.CommitAsync();
            return pedido;
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }
}