using Microsoft.AspNetCore.Identity;
using Pedidos360.Models;

namespace Pedidos360.Data
// Clase encargada de sembrar datos iniciales en la base de datos, incluyendo roles, usuarios, categorías, productos y clientes de prueba para facilitar el desarrollo y las pruebas del sistema
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var db          = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // ── 1. Roles ──────────────────────────────────────────────────────
            string[] roles = ["Admin", "Ventas", "Operaciones"];
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ── 2. Usuarios de prueba ─────────────────────────────────────────
            await EnsureUser(userManager, "admin@demo.local",  "Passw0rd!", "Admin");
            await EnsureUser(userManager, "ventas@demo.local", "Passw0rd!", "Ventas");
            await EnsureUser(userManager, "ops@demo.local",    "Passw0rd!", "Operaciones");

            // ── 3. Categorías ─────────────────────────────────────────────────
            if (!db.Categorias.Any())
            {
                db.Categorias.AddRange(
                    new Categoria { Nombre = "Electrónica" },
                    new Categoria { Nombre = "Alimentos" },
                    new Categoria { Nombre = "Hogar" },
                    new Categoria { Nombre = "Oficina" }
                );
                await db.SaveChangesAsync();
            }

            // ── 4. Productos (12 mínimos según enunciado) ─────────────────────
            if (!db.Productos.Any())
            {
                var catElec  = db.Categorias.First(c => c.Nombre == "Electrónica").id_Categoria;
                var catAlim  = db.Categorias.First(c => c.Nombre == "Alimentos").id_Categoria;
                var catHogar = db.Categorias.First(c => c.Nombre == "Hogar").id_Categoria;
                var catOfic  = db.Categorias.First(c => c.Nombre == "Oficina").id_Categoria;

                db.Productos.AddRange(
                    new Producto { Nombre = "Laptop HP 15",          id_Categoria = catElec,  Precio = 549000, ImpuestoPorc = 13, Stock = 15,  Activo = true },
                    new Producto { Nombre = "Mouse Logitech M100",   id_Categoria = catElec,  Precio = 12500,  ImpuestoPorc = 13, Stock = 80,  Activo = true },
                    new Producto { Nombre = "Teclado Mecánico",      id_Categoria = catElec,  Precio = 35000,  ImpuestoPorc = 13, Stock = 40,  Activo = true },
                    new Producto { Nombre = "Monitor 24 Full HD",    id_Categoria = catElec,  Precio = 185000, ImpuestoPorc = 13, Stock = 20,  Activo = true },
                    new Producto { Nombre = "Audífonos Bluetooth",   id_Categoria = catElec,  Precio = 28000,  ImpuestoPorc = 13, Stock = 55,  Activo = true },
                    new Producto { Nombre = "Café Molido 500g",      id_Categoria = catAlim,  Precio = 4500,   ImpuestoPorc = 1,  Stock = 200, Activo = true },
                    new Producto { Nombre = "Galletas Surtidas x12", id_Categoria = catAlim,  Precio = 1800,   ImpuestoPorc = 1,  Stock = 150, Activo = true },
                    new Producto { Nombre = "Botella de Agua 1L",    id_Categoria = catAlim,  Precio = 650,    ImpuestoPorc = 1,  Stock = 500, Activo = true },
                    new Producto { Nombre = "Silla Ergonómica",      id_Categoria = catHogar, Precio = 98000,  ImpuestoPorc = 13, Stock = 12,  Activo = true },
                    new Producto { Nombre = "Escritorio Plegable",   id_Categoria = catHogar, Precio = 65000,  ImpuestoPorc = 13, Stock = 8,   Activo = true },
                    new Producto { Nombre = "Resma Papel A4",        id_Categoria = catOfic,  Precio = 5200,   ImpuestoPorc = 13, Stock = 300, Activo = true },
                    new Producto { Nombre = "Bolígrafos x10",        id_Categoria = catOfic,  Precio = 1200,   ImpuestoPorc = 13, Stock = 400, Activo = true }
                );
                await db.SaveChangesAsync();
            }

            // ── 5. Clientes (8 mínimos según enunciado) ───────────────────────
            if (!db.Clientes.Any())
            {
                db.Clientes.AddRange(
                    new Cliente { Nombre = "Distribuidora El Sol S.A.",  Cedula = "3-101-123456", Correo = "sol@ejemplo.cr",        Telefono = "2222-0001", Direccion = "San José Centro" },
                    new Cliente { Nombre = "Tech Parts CR",              Cedula = "3-102-654321", Correo = "info@techparts.cr",     Telefono = "2233-0002", Direccion = "Zapote, San José" },
                    new Cliente { Nombre = "Laura Vargas Mora",          Cedula = "1-0800-0123",  Correo = "lvargas@gmail.com",     Telefono = "8888-1234", Direccion = "Heredia Centro" },
                    new Cliente { Nombre = "Supermercado Buen Precio",   Cedula = "3-103-789012", Correo = "compras@buenprecio.cr", Telefono = "2244-0003", Direccion = "Alajuela Centro" },
                    new Cliente { Nombre = "Carlos Jiménez Solano",      Cedula = "2-0500-0456",  Correo = "cjimenez@hotmail.com",  Telefono = "7777-5678", Direccion = "Cartago" },
                    new Cliente { Nombre = "Importaciones Delta Ltda.",  Cedula = "3-104-345678", Correo = "delta@importa.cr",      Telefono = "2255-0004", Direccion = "La Uruca, San José" },
                    new Cliente { Nombre = "María Fernández Castro",     Cedula = "4-0300-0789",  Correo = "mfcastro@empresa.cr",   Telefono = "6666-9012", Direccion = "Liberia, Guanacaste" },
                    new Cliente { Nombre = "Colegio Santa María",        Cedula = "3-008-123890", Correo = "admin@sma.ed.cr",       Telefono = "2266-0005", Direccion = "Tibás, San José" }
                );
                await db.SaveChangesAsync();
            }
        }

        private static async Task EnsureUser(
            UserManager<ApplicationUser> um, string email, string pass, string role)
        {
            if (await um.FindByEmailAsync(email) is null)
            {
                var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, Rol = role };
                var result = await um.CreateAsync(user, pass);
                if (result.Succeeded)
                    await um.AddToRoleAsync(user, role);
            }
        }
    }
}
