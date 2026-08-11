using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.ViewModels;

namespace MacrobioticaLaBendicion.Controllers
// Controlador de solo lectura para consultar el registro de auditoría
{
    [Authorize(Roles = "Admin")]
    public class BitacoraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BitacoraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bitacora
        public async Task<IActionResult> Index(string? filtroEntidad, DateTime? desde, DateTime? hasta, int pagina = 1)
        {
            var query = _context.Bitacoras.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroEntidad))
                query = query.Where(b => b.Entidad == filtroEntidad);

            if (desde.HasValue)
                query = query.Where(b => b.Fecha >= desde.Value.Date);

            if (hasta.HasValue)
                query = query.Where(b => b.Fecha < hasta.Value.Date.AddDays(1));

            var totalItems = await query.CountAsync();
            var tamano     = BitacoraIndexViewModel.TamanoPagina;

            var registros = await query
                .OrderByDescending(b => b.Fecha)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            var vm = new BitacoraIndexViewModel
            {
                Registros      = registros,
                FiltroEntidad  = filtroEntidad,
                Desde          = desde,
                Hasta          = hasta,
                PaginaActual   = pagina,
                TotalItems     = totalItems,
                TotalPaginas   = (int)Math.Ceiling((double)totalItems / tamano)
            };

            return View(vm);
        }
    }
}
