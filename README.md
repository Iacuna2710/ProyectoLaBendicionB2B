# 🌿 Macrobiótica La Bendición — Sistema de Inventario y Pedidos B2B

Aplicación web **ASP.NET Core MVC** para la gestión de catálogo de productos, clientes e inventario de una macrobiótica, con creación de pedidos B2B, cálculo de totales en tiempo real (AJAX), autenticación con roles, auditoría, reportes de ventas, exportación de pedidos y manejo de errores personalizado.

> Proyecto final del curso **SC-601 Programación Avanzada** — Universidad Fidélitas.

---

## 🛠️ Tecnologías

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core MVC (.NET 9, C#) |
| ORM | Entity Framework Core 9 (Code-First + Migrations) |
| Base de datos | SQL Server |
| Autenticación | ASP.NET Core Identity (usuarios + roles) |
| Frontend | Razor Views, Bootstrap 5, Bootstrap Icons, jQuery + jQuery Validation Unobtrusive |
| Componentes interactivos | React 18 + Vite (autosuggest de pedido, tabla de productos) |
| Imágenes | SixLabors.ImageSharp (generación de thumbnails) |
| Exportación | QuestPDF (PDF), ClosedXML (Excel) |

---

## ⚙️ Instalación y ejecución

### Requisitos previos
- [.NET SDK 9](https://dotnet.microsoft.com/download) o superior
- [Node.js 18+](https://nodejs.org/) (para compilar los componentes React del frontend)
- SQL Server (LocalDB, Express o instancia completa)
- Visual Studio 2022+ o VS Code (opcional)

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/Iacuna2710/ProyectoLaBendicionB2B.git
cd ProyectoLaBendicionB2B

# 2. Ajustar la cadena de conexión (si es necesario)
#    Editar appsettings.json → "MacrobioticaDb"
#    Por defecto apunta a: Server=localhost;Database=MacrobioticaDB;Trusted_Connection=True
#    Si usas SQL Express, cambiar a: Server=localhost\SQLEXPRESS

# 3. Compilar los componentes React (autosuggest de pedido, tabla de productos)
cd ClientApp
npm install
npm run build
cd ..

# 4. Ejecutar
dotnet run
```

> **No se necesita `update-database` manual**: al arrancar, la aplicación aplica las migraciones automáticamente (`db.Database.Migrate()`) y siembra los roles y usuarios de prueba.

> ⚠️ **Paso 3 es obligatorio**: `Productos/Index` y `Pedido/Create` renderizan componentes React que se sirven desde `wwwroot/dist/`. Si ese directorio no existe (por ejemplo en un clon nuevo), esas pantallas no van a mostrar la tabla ni el buscador. Ejecutar `npm run build` una sola vez es suficiente — no hace falta dejar `npm run dev` corriendo.

Abrir en el navegador la URL que indique la consola (ej. `https://localhost:49685`).

---

## 👥 Usuarios de prueba

Se crean automáticamente al arrancar la aplicación (`Data/SeedData.cs`):

| Correo | Contraseña | Rol |
|---|---|---|
| `admin@labendicion.local` | `Passw0rd!` | Admin |
| `ventas@labendicion.local` | `Passw0rd!` | Ventas |
| `ops@labendicion.local` | `Passw0rd!` | Operaciones |

### ⚠️ Datos de catálogo (productos, clientes, categorías)

A diferencia de los roles y usuarios, el catálogo de productos/clientes/categorías **no se siembra automáticamente al arrancar** — es el pendiente conocido más importante antes de la entrega final, ya que el enunciado pide 10–20 productos y 5–10 clientes disponibles desde el primer `dotnet run`. Mientras eso no esté resuelto:
- Para pruebas locales, cargar datos manualmente desde la propia interfaz (Productos → Nuevo, Clientes → Nuevo), o
- Restaurar `Data/Tablas_Datos.sql` en SQL Server Management Studio (script de referencia, no pensado para correr sobre una base recién migrada — puede chocar con las tablas que EF Core ya crea).

**Pendiente para la entrega final:** convertir el catálogo de prueba en un seeder programático (`DbContext.Productos.AddRange(...)` dentro de `SeedData.cs` o un `CatalogoSeedData.cs` separado) para que el checklist de instalación (`clonar → dotnet run → navegar`) cumpla el requisito de datos de prueba sin pasos manuales.

---

## 🔐 Roles y permisos

| Módulo / Acción | Admin | Ventas | Operaciones |
|---|:---:|:---:|:---:|
| Productos — Ver catálogo | ✔ | ✔ | ✔ |
| Productos — Crear / Eliminar | ✔ | ✗ | ✗ |
| Productos — Editar (ajustar stock, thumbnail) | ✔ | ✗ | ✔ |
| Clientes — Ver / Crear / Editar | ✔ | ✔ | ✗ |
| Clientes — Eliminar | ✔ | ✗ | ✗ |
| Categorías — CRUD completo | ✔ | ✗ | ✗ |
| Pedidos — Ver / Exportar PDF-Excel | ✔ | ✔ | ✔ |
| Pedidos — Crear | ✔ | ✔ | ✗ |
| Pedidos — Cambiar estado | ✔ | ✗ | ✗ |
| Reportes de ventas (por fecha/cliente) | ✔ | ✔ | ✗ |
| Bitácora de auditoría | ✔ | ✗ | ✗ |

La protección se aplica en **dos capas**: la interfaz oculta los botones sin permiso, y los controladores validan con `[Authorize(Roles = "...")]` — el acceso por URL directa también queda bloqueado (redirige a la pantalla de acceso denegado).

---

## ✨ Funcionalidades principales

### 🛒 Flujo de pedido (núcleo del sistema)
- Selección de cliente y agregado de productos por **autosuggest AJAX** (`/api/productos/buscar`) o por dropdown filtrado por categoría.
- **Cálculo de totales en vivo** (subtotal, descuento en % y ₡, impuestos, total) sin recargar la página vía `/api/pedidos/calcular` — el servidor siempre recalcula, nunca se confía en el navegador.
- Validación de stock en tiempo real: si se pide más de lo disponible, la línea se marca en rojo y no se puede confirmar.
- Al confirmar: se **descuenta el stock**, se registra usuario/fecha (auditoría) y se persisten los totales — todo dentro de una **transacción** de base de datos.
- Cambio de estado del pedido (Confirmado → Enviado → Entregado / Cancelado) exclusivo de Admin. Los pedidos cancelados se excluyen de los reportes de ventas.

### 🔌 API (consumida con AJAX)
| Endpoint | Descripción |
|---|---|
| `GET /api/productos/buscar?q=...` | Búsqueda de productos activos por nombre (hasta 10 coincidencias) |
| `POST /api/pedidos/calcular` | Cálculo de subtotal, impuestos y total de las líneas |

Ambos requieren usuario autenticado (`[Authorize]`).

### 📦 Catálogo y gestión
- CRUD de **Productos** con imagen obligatoria al crear (validación de tipo y tamaño máx. 2 MB), filtros por nombre/categoría y paginación.
- **Miniaturas automáticas**: al subir una imagen, se genera un thumbnail `.webp` (200px de ancho) que se usa en el listado, sin afectar la imagen original que se muestra en el detalle.
- *Soft delete*: un producto con pedidos asociados se desactiva en vez de eliminarse (preserva el historial).
- CRUD de **Clientes** con validación de cédula única y búsqueda por nombre/cédula.
- CRUD de **Categorías** (protegido: no se elimina si tiene productos).

### 📝 Bitácora de auditoría
- Registra automáticamente cada creación, edición, eliminación (o desactivación) de Productos y Clientes, y cada confirmación/cambio de estado de Pedido, con usuario, fecha y detalle.
- Consultable en `/Bitacora` (solo Admin), con filtros por entidad y rango de fechas.

### 📊 Reportes de ventas
- `/Reportes/PorFecha`: ventas agrupadas por día, con filtro de rango.
- `/Reportes/PorCliente`: ventas agrupadas por cliente, con filtro por cliente y rango de fechas.
- Ambos excluyen pedidos con estado `Cancelado` de los totales.

### 📄 Exportación de pedidos
- Desde el detalle de un pedido, botones para exportar a **PDF** (QuestPDF) y **Excel** (ClosedXML) con cliente, líneas y totales.

### 🚨 Manejo de errores
- **404** — página personalizada para URLs inexistentes.
- **403** — pantalla de acceso denegado cuando un rol intenta entrar donde no tiene permiso.
- **500** — vista de error genérica ante excepciones no controladas (activa en Producción; hay un botón de prueba en el Dashboard de Admin para verla sin forzar una excepción real).

### 📋 Extras
- Registro de actividad (**logging** vía `ILogger`) además de la bitácora persistente en base de datos.
- Modo oscuro persistente entre pantallas.
- Login/Registro con diseño propio, mostrar/ocultar contraseña y recuperación de contraseña.

---

## 📂 Estructura del proyecto

```
├── Controllers/          # Controladores MVC (Productos, Clientes, Categorías, Pedido, Bitacora, Reportes, Home)
│   └── Api/               # Endpoints REST para AJAX (buscar productos, calcular totales)
├── ClientApp/             # Componentes React + Vite (autosuggest de pedido, tabla de productos)
│   └── src/
├── Models/                # Entidades del dominio (Producto, Cliente, Pedido, PedidoDetalle, Bitacora...)
├── Data/                  # ApplicationDbContext, SeedData (roles + usuarios de prueba)
├── Services/              # CalculoPedidoService, ProductoListaService, BitacoraService, PedidoExportService
├── ViewModels/            # ViewModels por pantalla y DTOs de la API
├── Views/                 # Vistas Razor (Bootstrap 5)
├── Areas/Identity/        # Páginas de autenticación (Login, Registro, Recuperar contraseña)
├── Migrations/            # Migraciones de EF Core
├── Documentos/             # Evidencias y entregables por avance
└── wwwroot/                # CSS, JS, dist/ (bundles de React), imágenes y thumbnails de productos
```

---

## 🧭 Orden de revisión sugerido

1. Iniciar sesión como **Admin** → recorrer Dashboard, Productos (crear con imagen y ver el thumbnail generado), Categorías, Clientes y Bitácora (ver los registros de las acciones anteriores).
2. Iniciar sesión como **Ventas** → crear un **pedido**: buscar producto (AJAX), cambiar cantidades/descuentos (totales en vivo), confirmar y verificar la baja de stock en Productos. Revisar `Reportes/PorFecha` y `Reportes/PorCliente`.
3. Desde el detalle de un pedido, exportar a **PDF** y **Excel**.
4. Iniciar sesión como **Operaciones** → verificar los permisos restringidos (sin Clientes, Categorías, Reportes ni Bitácora; no puede crear pedidos).
5. Probar los errores: URL inexistente (404) y acceso directo a `/Categorias` con Ventas (pantalla de acceso denegado).

---

## ✅ Checklist frente al enunciado

| Requisito | Estado |
|---|:---:|
| CRUD Productos con imagen obligatoria, filtros y paginación | ✔ |
| CRUD Clientes con validaciones y búsqueda | ✔ |
| Pedidos: autosuggest AJAX, cálculo en vivo, baja de stock, auditoría | ✔ |
| Roles Admin/Ventas/Operaciones con restricciones reales | ✔ |
| API `/api/productos/buscar` y `/api/pedidos/calcular` autenticadas | ✔ |
| Vistas 404/500 personalizadas | ✔ |
| Validación cliente/servidor (DataAnnotations + jQuery unobtrusive) | ✔ |
| Seeders de roles y usuarios de prueba | ✔ |
| **Seeders de catálogo (productos/clientes/categorías)** | ⚠️ pendiente (ver sección de arriba) |
| Bono — Exportar PDF/Excel | ✔ |
| Bono — Bitácora/auditoría | ✔ |
| Bono — Reportes de ventas por fecha/cliente | ✔ |
| Bono — Thumbnails de imágenes con validación de tamaño | ✔ |

---

## 👨‍💻 Equipo — Grupo 4

- Isaac Acuña
- Pablo Castillo
- Fabricio Quesada
- Marcelo Quevedo
- Daniel Valverde

**Curso:** SC-601 Programación Avanzada
