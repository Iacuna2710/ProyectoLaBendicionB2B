# Checklist de pruebas funcionales — Macrobiotica La Bendición

Guía de casos manuales a correr antes de la demo del Hito 2. Márquenlo con
`[x]` a medida que se prueba, y anoten en "Notas" cualquier cosa que falle.

Usuarios de prueba (ya sembrados automáticamente al arrancar la app):

| Correo | Contraseña | Rol |
|---|---|---|
| admin@labendicion.local | Passw0rd! | Admin |
| ventas@labendicion.local | Passw0rd! | Ventas |
| ops@labendicion.local | Passw0rd! | Operaciones |

---

## 1. Autenticación

- [ ] Login con `admin@labendicion.local` funciona y redirige al Dashboard.
- [ ] Login con `ventas@labendicion.local` funciona.
- [ ] Login con `ops@labendicion.local` funciona.
- [ ] Login con contraseña incorrecta muestra error, no deja entrar.
- [ ] El ojito de mostrar/ocultar contraseña aparece solo cuando hay texto escrito, en Login y en Registro.
- [ ] Registrar un usuario nuevo: el dropdown de "Rol" muestra Ventas y Operaciones, pero **no** Admin.
- [ ] Registrar un usuario nuevo funciona y loguea automáticamente.
- [ ] "Cerrar sesión" funciona y vuelve a pedir login al intentar entrar a un módulo protegido.
- [ ] La barra superior muestra "Hola, [correo]" + el badge con el rol correcto.

## 2. Menú según rol

- [ ] **Admin**: ve Dashboard, Productos, Categorías, Clientes, Pedidos (todo).
- [ ] **Ventas**: ve Dashboard, Productos, Clientes, Pedidos — **no** ve Categorías.
- [ ] **Operaciones**: ve Dashboard, Productos, Pedidos — **no** ve Categorías ni Clientes.

## 3. CRUD Productos

- [ ] Listado con filtro por nombre y por categoría funciona.
- [ ] Paginación funciona con más de 8 productos.
- [ ] **Admin** puede crear un producto (la imagen es obligatoria — probar sin imagen y confirmar que da error).
- [ ] **Admin** puede editar y eliminar un producto.
- [ ] Eliminar un producto que ya tiene pedidos asociados lo desactiva en vez de borrarlo (soft delete) — mostrar mensaje.
- [ ] **Operaciones** puede editar (ajustar stock) pero no puede crear ni eliminar.
- [ ] **Ventas** solo puede ver el listado, sin botones de crear/editar/eliminar visibles o accesibles.
- [ ] Precio ≤ 0 o stock negativo son rechazados por el formulario.

## 4. CRUD Clientes

- [ ] **Admin** y **Ventas** pueden crear/editar clientes.
- [ ] Solo **Admin** puede eliminar un cliente.
- [ ] Cédula duplicada muestra error al crear o editar.
- [ ] Búsqueda por nombre y por cédula funciona.
- [ ] Eliminar un cliente con pedidos asociados lo bloquea con mensaje claro.
- [ ] **Operaciones** no puede entrar a `/Clientes` en absoluto (cae en la pantalla de acceso denegado).

## 5. CRUD Categorías

- [ ] Solo **Admin** puede entrar a `/Categorias`. Ventas y Operaciones caen en acceso denegado.
- [ ] Crear, editar y eliminar categoría funciona.
- [ ] Eliminar una categoría con productos asociados lo bloquea con mensaje.

## 6. Flujo de Pedido (el más importante para la demo)

- [ ] **Admin** y **Ventas** pueden entrar a "Nuevo pedido"; **Operaciones** no puede crear (solo puede ver el listado/detalle).
- [ ] Seleccionar cliente desde el dropdown.
- [ ] Buscar producto por nombre (autosuggest AJAX) y agregarlo como línea.
- [ ] Elegir categoría en el dropdown filtra correctamente el dropdown de producto.
- [ ] Agregar producto vía dropdown + botón "Agregar" funciona igual que el buscador.
- [ ] Cambiar cantidad de una línea recalcula el total en vivo (sin recargar la página).
- [ ] Cambiar el % de descuento de una línea recalcula el total en vivo.
- [ ] Pedir más cantidad que el stock disponible: la línea se marca en rojo y el botón "Confirmar" se deshabilita.
- [ ] Quitar una línea del pedido funciona y recalcula los totales.
- [ ] Confirmar el pedido: redirige al detalle, muestra Subtotal/Impuestos/Total correctos.
- [ ] Después de confirmar, el **stock del producto bajó** exactamente la cantidad pedida (verificar en Productos).
- [ ] El detalle del pedido (`Pedido/Details/{id}`) muestra cliente, fecha, líneas y totales correctos.
- [ ] El listado de pedidos (`Pedido/Index`) muestra el pedido recién creado, ordenado por fecha descendente.

## 7. API

- [ ] `GET /api/productos/buscar?q=...` devuelve resultados en formato JSON estando logueado.
- [ ] Llamar `GET /api/productos/buscar` **sin sesión iniciada** (ej. en una ventana de incógnito) responde 401/redirige a login, no expone datos.
- [ ] `POST /api/pedidos/calcular` devuelve subtotal/impuestos/total correctos para un set de líneas de prueba.
- [ ] Llamar `POST /api/pedidos/calcular` sin sesión iniciada también está bloqueado.

## 8. Manejo de errores

- [ ] Entrar a una URL que no existe (ej. `/algo-que-no-existe`) muestra la pantalla 404 personalizada (ícono de mapa verde), no la página en blanco del servidor.
- [ ] Loguearse con un rol sin permiso e intentar entrar directo a un módulo restringido (ej. Ventas → `/Categorias`) muestra la pantalla de acceso denegado (403, ícono rojo), no un error feo.
- [ ] Forzar un error 500 (por ejemplo, con la base de datos apagada) muestra la vista de error genérica, no un stack trace crudo — **solo aplica en modo Production**, revisar con `ASPNETCORE_ENVIRONMENT=Production`.

## 9. Validaciones generales

- [ ] Todos los formularios (Producto, Cliente, Categoría) muestran los mensajes de validación en rojo debajo de cada campo, sin necesidad de recargar (validación cliente con jQuery unobtrusive).
- [ ] Ningún formulario permite guardar con campos requeridos vacíos.
- [ ] El formato de correo inválido en Cliente es rechazado.

---

## Resultado de la corrida

| Fecha | Quién probó | Resultado general | Bugs encontrados |
|---|---|---|---|
| | | | |
