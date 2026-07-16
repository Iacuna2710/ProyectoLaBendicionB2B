# 🌿 Macrobiótica La Bendición — Sistema de Inventario y Pedidos B2B

Aplicación web **ASP.NET Core MVC** para la gestión de catálogo de productos, clientes e inventario de una macrobiótica, con creación de pedidos B2B, cálculo de totales en tiempo real (AJAX), autenticación con roles y manejo de errores personalizado.

> Proyecto final del curso **SC-601 Programación Avanzada** — Universidad Fidélitas.

---

## 🛠️ Tecnologías

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core MVC (.NET 9, C#) |
| ORM | Entity Framework Core 9 (Code-First + Migrations) |
| Base de datos | SQL Server |
| Autenticación | ASP.NET Core Identity (usuarios + roles) |
| Frontend | Razor Views, Bootstrap 5, Bootstrap Icons, JavaScript (fetch/AJAX) |

---

## ⚙️ Instalación y ejecución

### Requisitos previos
- [.NET SDK 9](https://dotnet.microsoft.com/download) o superior
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

# 3. Ejecutar
dotnet run
```

> **No se necesita `update-database` manual**: al arrancar, la aplicación aplica las migraciones automáticamente (`db.Database.Migrate()`) y siembra los roles y usuarios de prueba.

Abrir en el navegador la URL que indique la consola (ej. `https://localhost:49685`).

---

## 👥 Usuarios de prueba

Se crean automáticamente al arrancar la aplicación:

| Correo | Contraseña | Rol |
|---|---|---|
| `admin@labendicion.local` | `Passw0rd!` | Admin |
| `ventas@labendicion.local` | `Passw0rd!` | Ventas |
| `ops@labendicion.local` | `Passw0rd!` | Operaciones |

---

## 🔐 Roles y permisos

| Módulo / Acción | Admin | Ventas | Operaciones |
|---|:---:|:---:|:---:|
| Productos — Ver catálogo | ✔ | ✔ | ✔ |
| Productos — Crear / Eliminar | ✔ | ✗ | ✗ |
| Productos — Editar (ajustar stock) | ✔ | ✗ | ✔ |
| Clientes — Ver / Crear / Editar | ✔ | ✔ | ✗ |
| Clientes — Eliminar | ✔ | ✗ | ✗ |
| Categorías — CRUD completo | ✔ | ✗ | ✗ |
| Pedidos — Ver | ✔ | ✔ | ✔ |
| Pedidos — Crear | ✔ | ✔ | ✗ |
| Pedidos — Cambiar estado | ✔ | ✗ | ✗ |

La protección se aplica en **dos capas**: la interfaz oculta los botones sin permiso, y los controladores validan con `[Authorize(Roles = "...")]` — el acceso por URL directa también queda bloqueado (pantalla 403 personalizada).

---

## ✨ Funcionalidades principales

### 🛒 Flujo de pedido (núcleo del sistema)
- Selección de cliente y agregado de productos por **autosuggest AJAX** o por dropdown filtrado por categoría.
- **Cálculo de totales en vivo** (subtotal, descuento en % y ₡, impuestos, total) sin recargar la página — el servidor siempre recalcula, nunca se confía en el navegador.
- Validación de stock en tiempo real: si se pide más de lo disponible, la línea se marca en rojo y no se puede confirmar.
- Al confirmar: se **descuenta el stock**, se registra usuario/fecha (auditoría) y se persisten los totales — todo dentro de una **transacción** de base de datos.
- Cambio de estado del pedido (Confirmado → Enviado → Entregado / Cancelado) exclusivo de Admin.

### 🔌 API (consumida con AJAX)
| Endpoint | Descripción |
|---|---|
| `GET /api/productos/buscar?q=...` | Búsqueda de productos (hasta 10 coincidencias) |
| `POST /api/pedidos/calcular` | Cálculo de subtotal, impuestos y total de las líneas |

Ambos requieren usuario autenticado (`[Authorize]`).

### 📦 Catálogo y gestión
- CRUD de **Productos** con imagen obligatoria al crear, filtros por nombre/categoría y paginación.
- *Soft delete*: un producto con pedidos asociados se desactiva en vez de eliminarse (preserva el historial).
- CRUD de **Clientes** con validación de cédula única y búsqueda por nombre/cédula.
- CRUD de **Categorías** (protegido: no se elimina si tiene productos).

### 🚨 Manejo de errores
- **404** — página personalizada para URLs inexistentes.
- **403** — pantalla de acceso denegado cuando un rol intenta entrar donde no tiene permiso.
- **500** — vista de error genérica ante excepciones no controladas (activa en Producción).

### 📋 Extras
- Registro de actividad (**logging**) en todos los módulos: cada creación/edición/eliminación queda en el log con el usuario que la ejecutó.
- Modo oscuro persistente entre pantallas.
- Login/Registro con diseño propio, mostrar/ocultar contraseña y recuperación de contraseña.

---

## 📂 Estructura del proyecto

```
├── Controllers/          # Controladores MVC (Productos, Clientes, Categorías, Pedido, Home)
│   └── Api/              # Endpoints REST para AJAX (buscar productos, calcular totales)
├── Models/               # Entidades del dominio (Producto, Cliente, Pedido, PedidoDetalle...)
├── Data/                 # ApplicationDbContext, SeedData (roles + usuarios de prueba)
├── Services/             # CalculoPedidoService (lógica de totales centralizada)
├── ViewModels/           # ViewModels por pantalla y DTOs de la API
├── Views/                # Vistas Razor (Bootstrap 5)
├── Areas/Identity/       # Páginas de autenticación (Login, Registro, Recuperar contraseña)
├── Migrations/           # Migraciones de EF Core
├── Documentos/           # Evidencias y entregables por avance
└── wwwroot/              # CSS, JS (pedido-ajax.js, site.js), imágenes de productos
```

---

## 🧭 Orden de revisión sugerido

1. Iniciar sesión como **Admin** → recorrer Dashboard, Productos (crear con imagen), Categorías y Clientes.
2. Iniciar sesión como **Ventas** → crear un **pedido**: buscar producto (AJAX), cambiar cantidades/descuentos (totales en vivo), confirmar y verificar la baja de stock en Productos.
3. Iniciar sesión como **Operaciones** → verificar los permisos restringidos (sin Clientes ni Categorías, no puede crear pedidos).
4. Probar los errores: URL inexistente (404) y acceso directo a `/Categorias` con Ventas (403).

---

## 👨‍💻 Equipo — Grupo 4

- Isaac Acuña
- Pablo Castillo
- Fabricio Quesada
- Marcelo Quevedo
- Daniel Valverde

**Curso:** SC-601 Programación Avanzada
