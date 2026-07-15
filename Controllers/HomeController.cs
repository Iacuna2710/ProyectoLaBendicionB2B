using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;
using MacrobioticaLaBendicion.ViewModels;
using System.Diagnostics;

namespace MacrobioticaLaBendicion.Controllers
// Controlador para la página de inicio y el dashboard principal
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // El dashboard muestra estadísticas del negocio: requiere sesión iniciada
        // (Error y ManejarCodigoEstado quedan libres para usuarios anónimos)
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var vm = new HomeDashboardViewModel
            {
                TotalProductos = await _context.Productos.CountAsync(),
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalPedidos = await _context.Pedidos.CountAsync(),
                TotalCategorias = await _context.Categorias.CountAsync(),
                ConnectionString = _configuration.GetConnectionString("MacrobioticaDb") ?? "—"
            };
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Punto de entrada al que UseStatusCodePagesWithReExecute redirige
        // cuando el servidor responde con un código de error (404, 403, etc.)
        // Nota: se llama "ManejarCodigoEstado" (y no "StatusCode") para no
        // chocar con el método StatusCode() que Controller ya trae incorporado.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Home/StatusCode/{code:int}")]
        public IActionResult ManejarCodigoEstado(int code)
        {
            if (code == 404)
            {
                Response.StatusCode = 404;
                return View("NotFound");
            }

            Response.StatusCode = code;
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}