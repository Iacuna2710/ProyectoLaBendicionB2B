# Pedidos360 — Inventario y Pedidos B2B
**SC-601 Programación Avanzada — Hito 1**

## Tecnología
- ASP.NET Core MVC (.NET 9) — mismo framework que ClaseEF del profesor
- Entity Framework Core 9 (Code-First + Migrations)
- SQL Server local
- Bootstrap 5 + jQuery (validación unobtrusive)
- ASP.NET Core Identity (roles: Admin, Ventas, Operaciones)

---

## Instalación rápida

```bash
# 1. Ajustar cadena de conexión en appsettings.json
#    "Pedidos360Db": "Server=localhost;Database=Pedidos360DB;..."

# 2. Copiar la carpeta wwwroot/lib/ del proyecto ClaseEF
#    (Bootstrap, jQuery, jquery-validation)

# 3. Ejecutar — las migraciones y seeder corren automáticamente
dotnet run
```

---

## Usuarios de prueba (creados por DbSeeder)

| Email | Contraseña | Rol |
|---|---|---|
| admin@demo.local | Passw0rd! | Admin |
| ventas@demo.local | Passw0rd! | Ventas |
| ops@demo.local | Passw0rd! | Operaciones |

---

## Datos semilla
- 4 categorías: Electrónica, Alimentos, Hogar, Oficina
- 12 productos con precios, stock e IVA costarricense
- 8 clientes con cédula, correo y dirección

---

## Estructura del proyecto

```
Pedidos360/
├── Controllers/
│   ├── HomeController.cs        # Dashboard
│   ├── CategoriasController.cs  # CRUD simple (= EspecialidadesController)
│   ├── ProductosController.cs   # CRUD + imagen + filtros + paginación (= ServiciosController)
│   └── ClientesController.cs    # CRUD + búsqueda + paginación (= PacientesController)
├── Data/
│   ├── ApplicationDbContext.cs  # partial class + virtual DbSet + fluent API (= ClaseEfContext)
│   └── DbSeeder.cs
├── Migrations/
│   └── 20240101000000_InitialCreate.cs
├── Models/
│   ├── Categoria.cs             # partial class, null! (= Paciente)
│   ├── Producto.cs
│   ├── Cliente.cs
│   ├── Pedido.cs
│   ├── PedidoDetalle.cs
│   └── ErrorViewModel.cs
├── ViewModels/
│   └── ViewModels.cs            # ProductoFormViewModel, ProductoIndexViewModel,
│                                #  ClienteIndexViewModel, HomeDashboardViewModel
├── Views/
│   ├── Shared/_Layout, _Alerts, _ValidationScriptsPartial, Error
│   ├── Home/Index               # = Home/Index de ClaseEF
│   ├── Categorias/              # = Especialidades/* de ClaseEF
│   ├── Productos/               # = Servicios/* de ClaseEF
│   └── Clientes/                # = Pacientes/* de ClaseEF
├── wwwroot/css/site.css         # = site.css de ClaseEF
├── Program.cs
├── appsettings.json
└── Pedidos360.csproj
```

---

## Patrones aplicados (los mismos que ClaseEF)
- `partial class` en modelos con `= null!`
- `virtual DbSet<>` + `partial void OnModelCreatingPartial`
- `BuildViewModelAsync()` privado en controladores con FK (ProductosController)
- `XxxFormViewModel { Entidad, SelectLists }` (igual que ServicioFormViewModel)
- Partial `_Form.cshtml` reutilizado en Create y Edit
- `TempData["SuccessMessage"]` + `_Alerts.cshtml`
- Soft-delete en Producto cuando tiene pedidos (= Paciente.ESTADO = false)
- `FechaDeRegistro` / `FechaDeModificacion` en entidades auditables
