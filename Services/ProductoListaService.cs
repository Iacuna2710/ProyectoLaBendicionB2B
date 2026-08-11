using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.ViewModels;
using MacrobioticaLaBendicion.ViewModels.Api;

namespace MacrobioticaLaBendicion.Services
{
    // Consulta paginada de productos con filtros. La usan tanto la vista
    // Productos/Index (para precargar la primera página desde Razor, así
    // React no tiene que esperar un primer fetch para pintar algo) como el
    // endpoint GET /api/productos (para filtros/paginación posteriores) —
    // un solo lugar con la lógica, para que nunca se desincronicen.
    public class ProductoListaService
    {
        private readonly ApplicationDbContext _context;

        public ProductoListaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductoListaResponse> ListarAsync(string? filtroNombre, int? filtroCategoria, int pagina)
        {
            var query = _context.Productos.Include(p => p.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroNombre))
                query = query.Where(p => p.Nombre.Contains(filtroNombre));

            if (filtroCategoria.HasValue)
                query = query.Where(p => p.id_Categoria == filtroCategoria.Value);

            var totalItems = await query.CountAsync();
            var tamano = ProductoIndexViewModel.TamanoPagina;
            var totalPaginas = (int)Math.Ceiling((double)totalItems / tamano);

            var productos = await query
                .OrderBy(p => p.Nombre)
                .Skip((Math.Max(1, pagina) - 1) * tamano)
                .Take(tamano)
                .Select(p => new ProductoListaItemDto
                {
                    Id              = p.id_Producto,
                    Nombre          = p.Nombre,
                    CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : null,
                    Precio          = p.Precio,
                    ImpuestoPorc    = p.ImpuestoPorc,
                    Stock           = p.Stock,
                    Activo          = p.Activo,
                    UrlImagen       = p.Url_Imagen,
                    UrlThumbnail    = p.Url_Thumbnail
                })
                .ToListAsync();

            return new ProductoListaResponse
            {
                Productos    = productos,
                TotalItems   = totalItems,
                TotalPaginas = totalPaginas,
                PaginaActual = pagina
            };
        }
    }
}
