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

    // ViewModel para la pantalla de creación de pedido.
    // Las líneas se arman y recalculan en el navegador vía AJAX
    // y se envían al servidor solo al confirmar.
    public class PedidoFormViewModel
    {
        public int id_Cliente { get; set; }

        public List<SelectListItem> Clientes { get; set; } = [];

        // Dropdowns de categoría/producto como alternativa al buscador de texto
        public List<SelectListItem> Categorias { get; set; } = [];
        public List<PedidoProductoOptionViewModel> Productos { get; set; } = [];

        public List<PedidoLineaInputViewModel> Lineas { get; set; } = [];
    }

    // Datos de un producto para poblar el <select> y filtrarlo por categoría en el navegador
    public class PedidoProductoOptionViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int CategoriaId { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    // Una línea del pedido tal como la envía el formulario al confirmar
    public class PedidoLineaInputViewModel
    {
        public int id_Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }
    }

    // ViewModel para el listado de pedidos con paginación
    public class PedidoIndexViewModel
    {
        public IEnumerable<Pedido> Pedidos { get; set; } = [];
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public const int TamanoPagina = 10;
    }
}
