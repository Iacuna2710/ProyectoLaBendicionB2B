using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.ViewModels;

namespace MacrobioticaLaBendicion.Controllers
// Reportes de ventas de solo lectura, agregados sobre Pedidos/PedidoDetalle
// ya existentes — no modifica ni depende de ninguna tabla nueva.
{
    [Authorize(Roles = "Admin,Ventas")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reportes/PorFecha
        public async Task<IActionResult> PorFecha(DateTime? desde, DateTime? hasta)
        {
            var hastaFinal = (hasta ?? DateTime.Today).Date;
            var desdeFinal = (desde ?? hastaFinal.AddDays(-29)).Date;

            var filas = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.Estado != "Cancelado")
                .Where(p => p.Fecha >= desdeFinal && p.Fecha < hastaFinal.AddDays(1))
                .GroupBy(p => p.Fecha.Date)
                .Select(g => new ReporteVentaFechaItem
                {
                    Fecha           = g.Key,
                    CantidadPedidos = g.Count(),
                    Subtotal        = g.Sum(p => p.Subtotal),
                    Impuestos       = g.Sum(p => p.Impuestos),
                    Total           = g.Sum(p => p.Total)
                })
                .OrderByDescending(f => f.Fecha)
                .ToListAsync();

            var vm = new ReporteVentaFechaViewModel
            {
                Desde = desdeFinal,
                Hasta = hastaFinal,
                Filas = filas
            };

            return View(vm);
        }

        // GET: Reportes/PorCliente
        public async Task<IActionResult> PorCliente(int? clienteId, DateTime? desde, DateTime? hasta)
        {
            var query = _context.Pedidos
                .AsNoTracking()
                .Where(p => p.Estado != "Cancelado")
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(p => p.Fecha >= desde.Value.Date);

            if (hasta.HasValue)
                query = query.Where(p => p.Fecha < hasta.Value.Date.AddDays(1));

            if (clienteId.HasValue)
                query = query.Where(p => p.id_Cliente == clienteId.Value);

            var filas = await query
                .GroupBy(p => new { p.id_Cliente, p.Cliente!.Nombre })
                .Select(g => new ReporteVentaClienteItem
                {
                    id_Cliente      = g.Key.id_Cliente,
                    ClienteNombre   = g.Key.Nombre,
                    CantidadPedidos = g.Count(),
                    Subtotal        = g.Sum(p => p.Subtotal),
                    Impuestos       = g.Sum(p => p.Impuestos),
                    Total           = g.Sum(p => p.Total)
                })
                .OrderByDescending(f => f.Total)
                .ToListAsync();

            var vm = new ReporteVentaClienteViewModel
            {
                Desde     = desde,
                Hasta     = hasta,
                ClienteId = clienteId,
                Filas     = filas,
                Clientes  = await _context.Clientes
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem { Value = c.id_Cliente.ToString(), Text = c.Nombre })
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}
