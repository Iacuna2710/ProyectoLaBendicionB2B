using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;
using MacrobioticaLaBendicion.ViewModels;

namespace MacrobioticaLaBendicion.Controllers
// Controlador para el CRUD de los clientes del sistema
{
    [Authorize(Roles = "Admin,Ventas")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(string? filtroBusqueda, int pagina = 1)
        {
            var query = _context.Clientes.AsQueryable();

            // Búsqueda por nombre O cédula 
            if (!string.IsNullOrWhiteSpace(filtroBusqueda))
                query = query.Where(c =>
                    c.Nombre.Contains(filtroBusqueda) ||
                    c.Cedula.Contains(filtroBusqueda));

            var totalItems = await query.CountAsync();
            var tamano     = ClienteIndexViewModel.TamanoPagina;

            var clientes = await query
                .OrderBy(c => c.Nombre)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            var vm = new ClienteIndexViewModel
            {
                Clientes       = clientes,
                FiltroBusqueda = filtroBusqueda,
                PaginaActual   = pagina,
                TotalItems     = totalItems,
                TotalPaginas   = (int)Math.Ceiling((double)totalItems / tamano)
            };

            return View(vm);
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null)
                return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.id_Cliente == id);
            if (cliente is null)
                return NotFound();

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View(new Cliente());
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            // Verifica si la cédula esta duplicada
            if (await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula))
            {
                ModelState.AddModelError("Cedula", "Ya existe un cliente con esa cédula.");
                return View(cliente);
            }

            _context.Add(cliente);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cliente registrado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
                return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente is null)
                return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.id_Cliente)
                return NotFound();

            if (!ModelState.IsValid)
                return View(cliente);

            try
            {
                var clienteDb = await _context.Clientes.FindAsync(id);
                if (clienteDb is null)
                    return NotFound();

                // Verifica si la cédula esta duplicada en otro cliente
                if (await _context.Clientes.AnyAsync(c => c.Cedula == cliente.Cedula && c.id_Cliente != id))
                {
                    ModelState.AddModelError("Cedula", "Ya existe otro cliente con esa cédula.");
                    return View(cliente);
                }

                clienteDb.Nombre              = cliente.Nombre;
                clienteDb.Cedula              = cliente.Cedula;
                clienteDb.Correo              = cliente.Correo;
                clienteDb.Telefono            = cliente.Telefono;
                clienteDb.Direccion           = cliente.Direccion;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clientes.AnyAsync(e => e.id_Cliente == cliente.id_Cliente))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.id_Cliente == id);
            if (cliente is null)
                return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente is not null)
            {
                bool tienePedidos = await _context.Pedidos.AnyAsync(p => p.id_Cliente == id);
                if (tienePedidos)
                {
                    TempData["SuccessMessage"] = "No se puede eliminar: el cliente tiene pedidos registrados.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cliente eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
