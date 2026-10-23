using Inventario.Api.Data;
using Inventario.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly InventarioDbContext _db;

    public ProductosController(InventarioDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Producto>>> Get()
    {
        return await _db.Productos.AsNoTracking().ToListAsync();
    }
}