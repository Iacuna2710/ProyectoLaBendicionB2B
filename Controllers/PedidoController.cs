using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;
using MacrobioticaLaBendicion.Services;
using MacrobioticaLaBendicion.ViewModels;
using MacrobioticaLaBendicion.ViewModels.Api;

namespace MacrobioticaLaBendicion.Controllers
// Controlador para crear y consultar pedidos: cálculo de totales en vivo,
// descuento de stock y persistencia de auditoría (usuario/fecha/totales)
{
    [Authorize] // requiere usuario autenticado: id_Usuario es obligatorio en el pedido
    public class PedidoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CalculoPedidoService _calculo;
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(
            ApplicationDbContext context,
            CalculoPedidoService calculo,
            ILogger<PedidoController> logger)
        {
            _context = context;
            _calculo = calculo;
            _logger  = logger;
        }

        // GET: Pedido
        public async Task<IActionResult> Index(int pagina = 1)
        {
            var query = _context.Pedidos.Include(p => p.Cliente).AsQueryable();

            var totalItems = await query.CountAsync();
            var tamano     = PedidoIndexViewModel.TamanoPagina;

            var pedidos = await query
                .OrderByDescending(p => p.Fecha)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            var vm = new PedidoIndexViewModel
            {
                Pedidos      = pedidos,
                PaginaActual = pagina,
                TotalItems   = totalItems,
                TotalPaginas = (int)Math.Ceiling((double)totalItems / tamano)
            };

            return View(vm);
        }

        // GET: Pedido/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.id_Pedido == id);

            if (pedido is null)
                return NotFound();

            return View(pedido);
        }

        // GET: Pedido/Create
        public async Task<IActionResult> Create()
        {
            return View(await BuildFormAsync());
        }

        // POST: Pedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoFormViewModel viewModel)
        {
            if (viewModel.id_Cliente <= 0)
                ModelState.AddModelError(nameof(viewModel.id_Cliente), "Debe seleccionar un cliente.");

            if (viewModel.Lineas is null || viewModel.Lineas.Count == 0)
                ModelState.AddModelError(string.Empty, "El pedido debe tener al menos un producto.");

            if (!ModelState.IsValid)
                return View(await BuildFormAsync(viewModel));

            // El servidor recalcula todo — nunca confía en los totales que manda el navegador
            var lineasDto = viewModel.Lineas!.Select(l => new CalcularLineaDto
            {
                ProductoId = l.id_Producto,
                Cantidad   = l.Cantidad,
                Descuento  = l.Descuento
            });

            var calculo = await _calculo.CalcularAsync(lineasDto);

            var lineaConStockInsuficiente = calculo.Lineas.FirstOrDefault(l => l.StockInsuficiente);
            if (lineaConStockInsuficiente is not null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Stock insuficiente para «{lineaConStockInsuficiente.Nombre}» " +
                    $"(disponible: {lineaConStockInsuficiente.StockDisponible}, solicitado: {lineaConStockInsuficiente.Cantidad}).");
                return View(await BuildFormAsync(viewModel));
            }

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");

            var pedido = new Pedido
            {
                id_Cliente = viewModel.id_Cliente,
                id_Usuario = usuarioId,
                Fecha      = DateTime.Now,
                Subtotal   = calculo.Subtotal,
                Impuestos  = calculo.Impuestos,
                Total      = calculo.Total,
                Estado     = "Confirmado"
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var linea in calculo.Lineas)
                {
                    var producto = await _context.Productos.FindAsync(linea.ProductoId);
                    if (producto is null || producto.Stock < linea.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para «{linea.Nombre}».");

                    producto.Stock -= linea.Cantidad;

                    pedido.Detalles.Add(new PedidoDetalle
                    {
                        id_Producto    = linea.ProductoId,
                        Cantidad       = linea.Cantidad,
                        PrecioUnitario = linea.PrecioUnitario,
                        Descuento      = linea.Descuento,
                        Porcentaje_Imp = linea.ImpuestoPorc,
                        Total_Linea    = linea.TotalLinea
                    });
                }

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Pedido {PedidoId} confirmado por {Usuario} para cliente {ClienteId}. Total: {Total}",
                    pedido.id_Pedido, usuarioId, pedido.id_Cliente, pedido.Total);

                TempData["SuccessMessage"] = $"Pedido #{pedido.id_Pedido} confirmado exitosamente. Total: ₡{pedido.Total:N2}";
                return RedirectToAction(nameof(Details), new { id = pedido.id_Pedido });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al confirmar pedido para cliente {ClienteId}", viewModel.id_Cliente);
                ModelState.AddModelError(string.Empty, "No se pudo confirmar el pedido: " + ex.Message);
                return View(await BuildFormAsync(viewModel));
            }
        }

        // ── Helper: arma el ViewModel con clientes, categorías y productos ────
        private async Task<PedidoFormViewModel> BuildFormAsync(PedidoFormViewModel? existente = null)
        {
            var vm = existente ?? new PedidoFormViewModel();

            vm.Clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.id_Cliente.ToString(), Text = c.Nombre })
                .ToListAsync();

            vm.Categorias = await _context.Categorias
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.id_Categoria.ToString(), Text = c.Nombre })
                .ToListAsync();

            vm.Productos = await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new PedidoProductoOptionViewModel
                {
                    Id          = p.id_Producto,
                    Nombre      = p.Nombre,
                    CategoriaId = p.id_Categoria,
                    Precio      = p.Precio,
                    Stock       = p.Stock
                })
                .ToListAsync();

            return vm;
        }
    }
}
