using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;
using Pedidos360.ViewModels;

namespace Pedidos360.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<ProductosController> logger)
        {
            _context = context;
            _env     = env;
            _logger  = logger;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string? filtroNombre, int? filtroCategoria, int pagina = 1)
        {
            var query = _context.Productos.Include(p => p.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroNombre))
                query = query.Where(p => p.Nombre.Contains(filtroNombre));

            if (filtroCategoria.HasValue)
                query = query.Where(p => p.id_Categoria == filtroCategoria.Value);

            var totalItems = await query.CountAsync();
            var tamano     = ProductoIndexViewModel.TamanoPagina;

            var productos = await query
                .OrderBy(p => p.Nombre)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            var vm = new ProductoIndexViewModel
            {
                Productos       = productos,
                Categorias      = await _context.Categorias.OrderBy(c => c.Nombre)
                                    .Select(c => new SelectListItem { Value = c.id_Categoria.ToString(), Text = c.Nombre })
                                    .ToListAsync(),
                FiltroNombre    = filtroNombre,
                FiltroCategoria = filtroCategoria,
                PaginaActual    = pagina,
                TotalItems      = totalItems,
                TotalPaginas    = (int)Math.Ceiling((double)totalItems / tamano)
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
        public async Task<IActionResult> Create()
        {
            return View(await BuildViewModelAsync(new Producto { Activo = true, ImpuestoPorc = 13 }));
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoFormViewModel viewModel)
        {
            // La imagen es obligatoria al crear
            if (viewModel.ImagenFile is null || viewModel.ImagenFile.Length == 0)
                ModelState.AddModelError("ImagenFile", "La imagen es obligatoria al crear un producto.");

            if (!ModelState.IsValid)
                return View(await BuildViewModelAsync(viewModel.Producto));

            viewModel.Producto.Url_Imagen      = await GuardarImagenAsync(viewModel.ImagenFile!);

            _context.Add(viewModel.Producto);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Producto «{viewModel.Producto.Nombre}» creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Edit/5
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

                // Solo reemplaza imagen si se sube una nueva
                if (viewModel.ImagenFile is not null && viewModel.ImagenFile.Length > 0)
                {
                    BorrarImagen(productoDb.Url_Imagen);
                    productoDb.Url_Imagen = await GuardarImagenAsync(viewModel.ImagenFile);
                }

                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto is not null)
            {
                // Si tiene pedidos se desactiva (soft delete), igual que Paciente en ClaseEF
                bool tieneDetalles = await _context.PedidoDetalles.AnyAsync(d => d.id_Producto == id);
                if (tieneDetalles)
                {
                    producto.Activo              = false;
                    _context.Productos.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Producto desactivado (tiene pedidos asociados).";
                }
                else
                {
                    BorrarImagen(producto.Url_Imagen);
                    _context.Productos.Remove(producto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ── Helper: construye ViewModel con categorías para el dropdown ───────
        // Igual que BuildViewModelAsync en ServiciosController de ClaseEF
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

        // ── Helper: guarda imagen en wwwroot/uploads/productos/ ───────────────
        private async Task<string> GuardarImagenAsync(IFormFile file)
        {
            var ext            = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] permitidos = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

            if (!permitidos.Contains(ext))
                throw new InvalidOperationException("Tipo de imagen no permitido.");

            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("La imagen no puede superar 2 MB.");

            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "productos");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{ext}";
            var rutaCompleta  = Path.Combine(carpeta, nombreArchivo);

            await using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/productos/{nombreArchivo}";
        }

        // ── Helper: borra imagen del disco ────────────────────────────────────
        private void BorrarImagen(string? imagenUrl)
        {
            if (string.IsNullOrEmpty(imagenUrl)) return;
            try
            {
                var ruta = Path.Combine(_env.WebRootPath, imagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(ruta))
                    System.IO.File.Delete(ruta);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo borrar imagen: {Url}", imagenUrl);
            }
        }
    }
}
