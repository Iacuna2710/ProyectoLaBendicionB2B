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

    // ViewModel para la vista de listado de productos con filtros y paginación.
    // El listado en sí (Productos/TotalPaginas/TotalItems) lo maneja React vía
    // GET /api/productos — DatosIniciales solo precarga la primera respuesta
    // para que la pantalla no arranque con un "Cargando..." innecesario.
    public class ProductoIndexViewModel
    {
        public Api.ProductoListaResponse? DatosIniciales { get; set; }
        public List<SelectListItem> Categorias { get; set; } = [];
        public string? FiltroNombre { get; set; }
        public int? FiltroCategoria { get; set; }
        public int PaginaActual { get; set; } = 1;
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

    // ViewModel para el listado de la bitácora de auditoría con filtros y paginación
    public class BitacoraIndexViewModel
    {
        public IEnumerable<Bitacora> Registros { get; set; } = [];
        public string? FiltroEntidad { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalItems { get; set; }
        public const int TamanoPagina = 20;
    }

    // Una fila agregada del reporte de ventas por fecha (un día)
    public class ReporteVentaFechaItem
    {
        public DateTime Fecha { get; set; }
        public int CantidadPedidos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
    }

    // ViewModel del reporte de ventas por fecha
    public class ReporteVentaFechaViewModel
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public List<ReporteVentaFechaItem> Filas { get; set; } = [];
        public int TotalPedidos => Filas.Sum(f => f.CantidadPedidos);
        public decimal TotalGeneral => Filas.Sum(f => f.Total);
    }

    // Una fila agregada del reporte de ventas por cliente
    public class ReporteVentaClienteItem
    {
        public int id_Cliente { get; set; }
        public string ClienteNombre { get; set; } = null!;
        public int CantidadPedidos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
    }

    // ViewModel del reporte de ventas por cliente
    public class ReporteVentaClienteViewModel
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int? ClienteId { get; set; }
        public List<SelectListItem> Clientes { get; set; } = [];
        public List<ReporteVentaClienteItem> Filas { get; set; } = [];
        public int TotalPedidos => Filas.Sum(f => f.CantidadPedidos);
        public decimal TotalGeneral => Filas.Sum(f => f.Total);
    }
}
