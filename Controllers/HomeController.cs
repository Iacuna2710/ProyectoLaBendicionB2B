using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;
using Pedidos360.ViewModels;
using System.Diagnostics;

namespace Pedidos360.Controllers
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

        public async Task<IActionResult> Index()
        {
            var vm = new HomeDashboardViewModel
            {
                TotalProductos  = await _context.Productos.CountAsync(),
                TotalClientes   = await _context.Clientes.CountAsync(),
                TotalPedidos    = await _context.Pedidos.CountAsync(),
                TotalCategorias = await _context.Categorias.CountAsync(),
                ConnectionString = _configuration.GetConnectionString("Pedidos360Db") ?? "—"
            };
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
