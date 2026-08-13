using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Models;

namespace MacrobioticaLaBendicion.Data;

// Crea los roles (Admin/Ventas/Operaciones) y los 3 usuarios de prueba
// pedidos en el enunciado del proyecto, para que la demo y la corrección
// se puedan hacer sin pasos manuales adicionales.
public static class SeedData
{
    private const string AdminRole       = "Admin";
    private const string VentasRole      = "Ventas";
    private const string OperacionesRole = "Operaciones";

    private const string TestPassword = "Passw0rd!";

    public static async Task InicializarAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await CrearRolSiNoExisteAsync(roleManager, AdminRole);
        await CrearRolSiNoExisteAsync(roleManager, VentasRole);
        await CrearRolSiNoExisteAsync(roleManager, OperacionesRole);

        await CrearUsuarioSiNoExisteAsync(userManager, "admin@labendicion.local", AdminRole);
        await CrearUsuarioSiNoExisteAsync(userManager, "ventas@labendicion.local", VentasRole);
        await CrearUsuarioSiNoExisteAsync(userManager, "ops@labendicion.local", OperacionesRole);
    }

    private static async Task CrearRolSiNoExisteAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo crear el rol '{roleName}': {errors}");
            }
        }
    }

    private static async Task CrearUsuarioSiNoExisteAsync(
        UserManager<ApplicationUser> userManager, string email, string rol)
    {
        var usuario = await userManager.FindByEmailAsync(email);

        if (usuario is null)
        {
            usuario = new ApplicationUser
            {
                UserName       = email,
                Email          = email,
                EmailConfirmed = true,
                Rol            = rol
            };

            var createResult = await userManager.CreateAsync(usuario, TestPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"No se pudo crear el usuario de prueba '{email}': {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(usuario, rol))
            await userManager.AddToRoleAsync(usuario, rol);
    }

    // Carga datos de prueba (categorías, productos, clientes y un pedido de
    // ejemplo) para poder revisar la app con contenido real sin cargarlo
    // a mano. Solo inserta si las tablas están vacías, así se puede llamar
    // en cada arranque sin duplicar datos.
    public static async Task SeedDatosPruebaAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await db.Categorias.AnyAsync())
        {
            db.Categorias.AddRange(
                new Categoria { Nombre = "Granos y Cereales" },
                new Categoria { Nombre = "Legumbres" },
                new Categoria { Nombre = "Aceites y Vinagres" },
                new Categoria { Nombre = "Endulzantes Naturales" },
                new Categoria { Nombre = "Snacks Saludables" },
                new Categoria { Nombre = "Tés e Infusiones" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Productos.AnyAsync())
        {
            var cat = await db.Categorias.ToDictionaryAsync(c => c.Nombre, c => c.id_Categoria);

            db.Productos.AddRange(
                new Producto { Nombre = "Arroz Integral 1kg",        id_Categoria = cat["Granos y Cereales"],        Precio = 1850, ImpuestoPorc = 13, Stock = 120, Activo = true },
                new Producto { Nombre = "Quinoa Orgánica 500g",      id_Categoria = cat["Granos y Cereales"],        Precio = 3200, ImpuestoPorc = 13, Stock = 60,  Activo = true },
                new Producto { Nombre = "Avena en Hojuelas 1kg",     id_Categoria = cat["Granos y Cereales"],        Precio = 1650, ImpuestoPorc = 13, Stock = 90,  Activo = true },
                new Producto { Nombre = "Frijol Negro 900g",         id_Categoria = cat["Legumbres"],                Precio = 1450, ImpuestoPorc = 13, Stock = 100, Activo = true },
                new Producto { Nombre = "Lenteja 900g",              id_Categoria = cat["Legumbres"],                Precio = 1400, ImpuestoPorc = 13, Stock = 80,  Activo = true },
                new Producto { Nombre = "Garbanzo 900g",             id_Categoria = cat["Legumbres"],                Precio = 1550, ImpuestoPorc = 13, Stock = 70,  Activo = true },
                new Producto { Nombre = "Aceite de Coco 500ml",      id_Categoria = cat["Aceites y Vinagres"],       Precio = 4200, ImpuestoPorc = 13, Stock = 40,  Activo = true },
                new Producto { Nombre = "Aceite de Oliva Extra Virgen 500ml", id_Categoria = cat["Aceites y Vinagres"], Precio = 5900, ImpuestoPorc = 13, Stock = 35, Activo = true },
                new Producto { Nombre = "Vinagre de Manzana 500ml",  id_Categoria = cat["Aceites y Vinagres"],       Precio = 2100, ImpuestoPorc = 13, Stock = 55,  Activo = true },
                new Producto { Nombre = "Miel de Abeja 500g",        id_Categoria = cat["Endulzantes Naturales"],    Precio = 3600, ImpuestoPorc = 13, Stock = 45,  Activo = true },
                new Producto { Nombre = "Panela Granulada 500g",     id_Categoria = cat["Endulzantes Naturales"],    Precio = 1300, ImpuestoPorc = 13, Stock = 65,  Activo = true },
                new Producto { Nombre = "Stevia en Gotas 30ml",      id_Categoria = cat["Endulzantes Naturales"],    Precio = 2800, ImpuestoPorc = 13, Stock = 30,  Activo = true },
                new Producto { Nombre = "Mix de Frutos Secos 250g",  id_Categoria = cat["Snacks Saludables"],        Precio = 2950, ImpuestoPorc = 13, Stock = 50,  Activo = true },
                new Producto { Nombre = "Chips de Plátano 150g",     id_Categoria = cat["Snacks Saludables"],        Precio = 1750, ImpuestoPorc = 13, Stock = 75,  Activo = true },
                new Producto { Nombre = "Barra de Granola 40g",      id_Categoria = cat["Snacks Saludables"],        Precio = 950,  ImpuestoPorc = 13, Stock = 150, Activo = true },
                new Producto { Nombre = "Té Verde Orgánico 20 saq.", id_Categoria = cat["Tés e Infusiones"],         Precio = 2200, ImpuestoPorc = 13, Stock = 60,  Activo = true },
                new Producto { Nombre = "Manzanilla 20 saq.",        id_Categoria = cat["Tés e Infusiones"],         Precio = 1600, ImpuestoPorc = 13, Stock = 60,  Activo = true },
                new Producto { Nombre = "Té de Jengibre y Limón 20 saq.", id_Categoria = cat["Tés e Infusiones"],    Precio = 2100, ImpuestoPorc = 13, Stock = 50,  Activo = true }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Clientes.AnyAsync())
        {
            db.Clientes.AddRange(
                new Cliente { Nombre = "Supermercado Verde S.A.",   Cedula = "3101123456", Correo = "compras@superverde.cr",   Telefono = "22334455", Direccion = "San José, Costa Rica" },
                new Cliente { Nombre = "María Fernández Solano",    Cedula = "109870123",  Correo = "maria.fernandez@mail.com", Telefono = "88991122", Direccion = "Heredia, Costa Rica" },
                new Cliente { Nombre = "Distribuidora Natural CR",  Cedula = "3101987654", Correo = "pedidos@distnatural.cr",   Telefono = "24567890", Direccion = "Alajuela, Costa Rica" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Pedidos.AnyAsync())
        {
            var cliente  = await db.Clientes.FirstAsync();
            var admin    = await db.Users.FirstAsync(u => u.Email == "admin@labendicion.local");
            var productos = await db.Productos.OrderBy(p => p.id_Producto).Take(3).ToListAsync();

            var pedido = new Pedido
            {
                id_Cliente = cliente.id_Cliente,
                id_Usuario = admin.Id,
                Fecha      = DateTime.Now,
                Estado     = "Pendiente"
            };

            decimal subtotal = 0, impuestos = 0;
            foreach (var p in productos)
            {
                var lineaSubtotal = p.Precio * 2;
                var lineaImpuesto = lineaSubtotal * (p.ImpuestoPorc / 100);
                subtotal  += lineaSubtotal;
                impuestos += lineaImpuesto;

                pedido.Detalles.Add(new PedidoDetalle
                {
                    id_Producto    = p.id_Producto,
                    Cantidad       = 2,
                    PrecioUnitario = p.Precio,
                    Descuento      = 0,
                    Porcentaje_Imp = p.ImpuestoPorc,
                    Total_Linea    = lineaSubtotal + lineaImpuesto
                });
            }

            pedido.Subtotal = subtotal;
            pedido.Impuestos = impuestos;
            pedido.Total = subtotal + impuestos;

            db.Pedidos.Add(pedido);
            await db.SaveChangesAsync();
        }
    }
}
