using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.ViewModels.Api;

namespace MacrobioticaLaBendicion.Controllers.Api
{
    [ApiController]
    [Route("api/productos")]
    [Authorize] // solo usuarios autenticados pueden consumir el endpoint
    public class ProductosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/productos/buscar?q=jabon
        [HttpGet("buscar")]
        public async Task<ActionResult<List<ProductoBusquedaDto>>> Buscar([FromQuery] string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Ok(new List<ProductoBusquedaDto>());

            var resultados = await _context.Productos
                .Where(p => p.Activo && p.Nombre.Contains(q))
                .OrderBy(p => p.Nombre)
                .Take(10)
                .Select(p => new ProductoBusquedaDto
                {
                    Id       = p.id_Producto,
                    Nombre   = p.Nombre,
                    Precio   = p.Precio,
                    Impuesto = p.ImpuestoPorc,
                    Stock    = p.Stock
                })
                .ToListAsync();

            return Ok(resultados);
        }
    }
}
