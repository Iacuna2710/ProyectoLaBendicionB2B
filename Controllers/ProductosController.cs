using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;
using MacrobioticaLaBendicion.Services;
using MacrobioticaLaBendicion.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MacrobioticaLaBendicion.Controllers
// Controlador para el CRUD de los productos del sistema
{
    [Authorize(Roles = "Admin,Ventas,Operaciones")]
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductosController> _logger;
        private readonly ProductoListaService _lista;
        private readonly BitacoraService _bitacora;

        public ProductosController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<ProductosController> logger,
            ProductoListaService lista,
            BitacoraService bitacora)
        {
            _context  = context;
            _env      = env;
            _logger   = logger;
            _lista    = lista;
            _bitacora = bitacora;
        }

        // GET: Productos
        // La tabla la pinta React (Views/Productos/Index.cshtml) consumiendo
        // GET /api/productos para filtros/paginación posteriores — pero la
        // primera página se precarga aquí con el mismo ProductoListaService,
        // para que React la pinte de inmediato sin esperar un primer fetch.
        public async Task<IActionResult> Index(string? filtroNombre, int? filtroCategoria, int pagina = 1)
        {
            var vm = new ProductoIndexViewModel
            {
                DatosIniciales  = await _lista.ListarAsync(filtroNombre, filtroCategoria, pagina),
                Categorias      = await _context.Categorias.OrderBy(c => c.Nombre)
                                    .Select(c => new SelectListItem { Value = c.id_Categoria.ToString(), Text = c.Nombre })
                                    .ToListAsync(),
                FiltroNombre    = filtroNombre,
                FiltroCategoria = filtroCategoria,
                PaginaActual    = pagina
            };

            return View(vm);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.id_Producto == id);
            if (producto is null)
                return NotFound();

            return View(producto);
        }

        // GET: Productos/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return View(await BuildViewModelAsync(new Producto { Activo = true, ImpuestoPorc = 13 }));
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductoFormViewModel viewModel)
        {
            // La imagen es obligatoria al crear un producto
            if (viewModel.ImagenFile is null || viewModel.ImagenFile.Length == 0)
                ModelState.AddModelError("ImagenFile", "La imagen es obligatoria al crear un producto.");

            if (!ModelState.IsValid)
                return View(await BuildViewModelAsync(viewModel.Producto));

            (viewModel.Producto.Url_Imagen, viewModel.Producto.Url_Thumbnail) = await GuardarImagenAsync(viewModel.ImagenFile!);

            _context.Add(viewModel.Producto);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Producto {Nombre} creado por {Usuario}",
                viewModel.Producto.Nombre, User.Identity?.Name);
            await _bitacora.RegistrarAsync(UsuarioIdActual(), User.Identity?.Name ?? "?", "Crear", "Producto",
                viewModel.Producto.id_Producto, viewModel.Producto.Nombre);

            TempData["SuccessMessage"] = $"Producto «{viewModel.Producto.Nombre}» creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "Admin,Operaciones")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto is null)
                return NotFound();

            return View(await BuildViewModelAsync(producto));
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Operaciones")]
        public async Task<IActionResult> Edit(int id, ProductoFormViewModel viewModel)
        {
            if (id != viewModel.Producto.id_Producto)
                return NotFound();

            if (!ModelState.IsValid)
                return View(await BuildViewModelAsync(viewModel.Producto));

            try
            {
                var productoDb = await _context.Productos.FindAsync(id);
                if (productoDb is null)
                    return NotFound();

                productoDb.Nombre              = viewModel.Producto.Nombre;
                productoDb.id_Categoria        = viewModel.Producto.id_Categoria;
                productoDb.Precio              = viewModel.Producto.Precio;
                productoDb.ImpuestoPorc        = viewModel.Producto.ImpuestoPorc;
                productoDb.Stock               = viewModel.Producto.Stock;
                productoDb.Activo              = viewModel.Producto.Activo;

                // La imagen se actualiza solo si se sube una nueva imaggen, de lo contrario se mantiene la existente
                if (viewModel.ImagenFile is not null && viewModel.ImagenFile.Length > 0)
                {
                    BorrarImagen(productoDb.Url_Imagen, productoDb.Url_Thumbnail);
                    (productoDb.Url_Imagen, productoDb.Url_Thumbnail) = await GuardarImagenAsync(viewModel.ImagenFile);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Producto {Nombre} editado por {Usuario}",
                    productoDb.Nombre, User.Identity?.Name);
                await _bitacora.RegistrarAsync(UsuarioIdActual(), User.Identity?.Name ?? "?", "Editar", "Producto",
                    productoDb.id_Producto, productoDb.Nombre);

                TempData["SuccessMessage"] = $"Producto «{productoDb.Nombre}» actualizado exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Productos.AnyAsync(e => e.id_Producto == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.id_Producto == id);
            if (producto is null)
                return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto is not null)
            {
                // Si tiene pedidos se desactiva con soft delete
                bool tieneDetalles = await _context.PedidoDetalles.AnyAsync(d => d.id_Producto == id);
                if (tieneDetalles)
                {
                    producto.Activo              = false;
                    _context.Productos.Update(producto);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Producto {Nombre} desactivado (soft delete) por {Usuario}",
                        producto.Nombre, User.Identity?.Name);
                    await _bitacora.RegistrarAsync(UsuarioIdActual(), User.Identity?.Name ?? "?", "Desactivar", "Producto",
                        producto.id_Producto, producto.Nombre);

                    TempData["SuccessMessage"] = "Producto desactivado (tiene pedidos asociados).";
                }
                else
                {
                    BorrarImagen(producto.Url_Imagen, producto.Url_Thumbnail);
                    _context.Productos.Remove(producto);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Producto {Nombre} eliminado por {Usuario}",
                        producto.Nombre, User.Identity?.Name);
                    await _bitacora.RegistrarAsync(UsuarioIdActual(), User.Identity?.Name ?? "?", "Eliminar", "Producto",
                        producto.id_Producto, producto.Nombre);

                    TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ── Helper: id del usuario autenticado, para la bitácora ────────────────
        private string UsuarioIdActual() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "?";

        // ── Helper: construye el view model con lista de categorías ─────────────
        private async Task<ProductoFormViewModel> BuildViewModelAsync(Producto producto)
        {
            return new ProductoFormViewModel
            {
                Producto   = producto,
                Categorias = await _context.Categorias
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem
                    {
                        Value = c.id_Categoria.ToString(),
                        Text  = c.Nombre
                    })
                    .ToListAsync()
            };
        }

        // ── Helper: guarda imagen original + thumbnail en wwwroot/uploads/productos/ ──
        private const int ThumbnailAncho = 200;

        private async Task<(string UrlImagen, string UrlThumbnail)> GuardarImagenAsync(IFormFile file)
        {
            var ext            = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] permitidos = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

            if (!permitidos.Contains(ext))
                throw new InvalidOperationException("Tipo de imagen no permitido.");

            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("La imagen no puede superar 2 MB.");

            var carpeta      = Path.Combine(_env.WebRootPath, "uploads", "productos");
            var carpetaThumb = Path.Combine(carpeta, "thumbs");
            Directory.CreateDirectory(carpeta);
            Directory.CreateDirectory(carpetaThumb);

            var nombreArchivo = $"{Guid.NewGuid()}{ext}";
            var rutaCompleta  = Path.Combine(carpeta, nombreArchivo);

            await using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                await file.CopyToAsync(stream);

            var nombreThumb  = $"{Path.GetFileNameWithoutExtension(nombreArchivo)}.webp";
            var rutaThumb    = Path.Combine(carpetaThumb, nombreThumb);

            using (var imagen = await Image.LoadAsync(rutaCompleta))
            {
                imagen.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(ThumbnailAncho, 0),
                    Mode = ResizeMode.Max
                }));
                await imagen.SaveAsWebpAsync(rutaThumb);
            }

            return ($"/uploads/productos/{nombreArchivo}", $"/uploads/productos/thumbs/{nombreThumb}");
        }

        // ── Helper: borra imagen original + thumbnail del servidor ────────────────
        private void BorrarImagen(string? imagenUrl, string? thumbnailUrl = null)
        {
            BorrarArchivo(imagenUrl);
            BorrarArchivo(thumbnailUrl);
        }

        private void BorrarArchivo(string? url)
        {
            if (string.IsNullOrEmpty(url)) return;
            try
            {
                var ruta = Path.Combine(_env.WebRootPath, url.TrimStart('/'));
                if (System.IO.File.Exists(ruta))
                    System.IO.File.Delete(ruta);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo borrar archivo: {Url}", url);
            }
        }
    }
}
