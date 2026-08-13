using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;
using MacrobioticaLaBendicion.Services;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MacrobioticaDb")));

// ── Identity  ──────────────────
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ── MVC + servicios de dominio ──────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CalculoPedidoService>();
builder.Services.AddScoped<ProductoListaService>();
builder.Services.AddScoped<BitacoraService>();
builder.Services.AddScoped<PedidoExportService>();

// No hay SMTP configurado: reemplaza el envío de correo (por defecto no hace
// nada) para que "Olvidé mi contraseña" se pueda probar de verdad.
builder.Services.AddTransient<IEmailSender, ConsoleEmailSender>();

var app = builder.Build();

// ── Aplica migraciones y crea los roles (Admin/Ventas/Operaciones) al arrancar ─
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await SeedData.InicializarAsync(scope.ServiceProvider);
    await SeedData.SeedDatosPruebaAsync(scope.ServiceProvider);
}

// ── Middleware pipeline ────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Captura códigos de error (404, 403, etc.) y los re-ejecuta contra
// Home/StatusCode en vez de mostrar la pantalla en blanco por defecto.
app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();