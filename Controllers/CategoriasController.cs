using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;

namespace MacrobioticaLaBendicion.Controllers
// Controlador para el CRUD de las categorías de productos
{
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync());
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var categoria = await _context.Categorias.FirstOrDefaultAsync(m => m.id_Categoria == id);
            if (categoria is null)
                return NotFound();

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View(new Categoria());
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);

            _context.Add(categoria);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Categoría creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria is null)
                return NotFound();

            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.id_Categoria)
                return NotFound();

            if (!ModelState.IsValid)
                return View(categoria);

            try
            {
                var categoriaDb = await _context.Categorias.FindAsync(id);
                if (categoriaDb is null)
                    return NotFound();

                categoriaDb.Nombre = categoria.Nombre;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Categoría actualizada exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categorias.AnyAsync(e => e.id_Categoria == categoria.id_Categoria))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var categoria = await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(m => m.id_Categoria == id);
            if (categoria is null)
                return NotFound();

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.id_Categoria == id);

            if (categoria is not null)
            {
                if (categoria.Productos.Any())
                {
                    TempData["SuccessMessage"] = "No se puede eliminar: la categoría tiene productos asociados.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Categoría eliminada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
