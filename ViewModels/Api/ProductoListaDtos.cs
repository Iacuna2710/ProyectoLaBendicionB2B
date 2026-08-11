namespace MacrobioticaLaBendicion.ViewModels.Api
{
    // Una fila del listado devuelto por GET /api/productos
    public class ProductoListaItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? CategoriaNombre { get; set; }
        public decimal Precio { get; set; }
        public decimal ImpuestoPorc { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
        public string? UrlImagen { get; set; }
        public string? UrlThumbnail { get; set; }
    }

    // Respuesta paginada de GET /api/productos
    public class ProductoListaResponse
    {
        public List<ProductoListaItemDto> Productos { get; set; } = [];
        public int TotalItems { get; set; }
        public int TotalPaginas { get; set; }
        public int PaginaActual { get; set; }
    }
}
