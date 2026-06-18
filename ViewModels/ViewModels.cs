using Microsoft.AspNetCore.Mvc.Rendering;
using MacrobioticaLaBendicion.Models;

namespace MacrobioticaLaBendicion.ViewModels
    
{
    // ViewModel para el formulario de creación y edición de productos
    public class ProductoFormViewModel
    {
        public Producto Producto { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = [];
        public IFormFile? ImagenFile { get; set; }
    }

    // ViewModel para la vista de listado de productos con filtros y paginación
    public class ProductoIndexViewModel
    {
        public IEnumerable<Producto> Productos { get; set; } = [];
        public List<SelectListItem> Categorias { get; set; } = [];
        public string? FiltroNombre { get; set; }
        public int? FiltroCategoria { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public const int TamanoPagina = 8;
    }

    // ViewModel para la vista de listado de clientes con filtros y paginación
    public class ClienteIndexViewModel
    {
        public IEnumerable<Cliente> Clientes { get; set; } = [];
        public string? FiltroBusqueda { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public const int TamanoPagina = 10;
    }

    // viewmodel para el dashboard de inicio con estadísticas generales del sistema
    public class HomeDashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalCategorias { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
    }
}
