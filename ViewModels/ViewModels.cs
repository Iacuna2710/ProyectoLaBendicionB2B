using Microsoft.AspNetCore.Mvc.Rendering;
using Pedidos360.Models;

namespace Pedidos360.ViewModels
    
{
    public class ProductoFormViewModel
    {
        public Producto Producto { get; set; } = new();
        public List<SelectListItem> Categorias { get; set; } = [];
        public IFormFile? ImagenFile { get; set; }
    }

 
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


    public class ClienteIndexViewModel
    {
        public IEnumerable<Cliente> Clientes { get; set; } = [];
        public string? FiltroBusqueda { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public const int TamanoPagina = 10;
    }


    public class HomeDashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalCategorias { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
    }
}
