using Microsoft.AspNetCore.Identity;
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
}
