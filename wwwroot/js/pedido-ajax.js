// Autosuggest de productos + cálculo de totales en vivo para la pantalla Pedido/Create.
// Consume los endpoints GET /api/productos/buscar y POST /api/pedidos/calcular.
(function () {
    "use strict";

    const inputBuscar   = document.getElementById("buscarProducto");
    const resultadosBox = document.getElementById("resultadosBusqueda");
    const tablaLineas    = document.getElementById("tablaLineas");
    const hiddenInputs   = document.getElementById("hiddenInputsLineas");
    const btnConfirmar   = document.getElementById("btnConfirmar");

    const selectCategoria    = document.getElementById("selectCategoria");
    const selectProducto     = document.getElementById("selectProducto");
    const btnAgregarProducto = document.getElementById("btnAgregarProducto");

    const lblSubtotal  = document.getElementById("lblSubtotal");
    const lblImpuestos = document.getElementById("lblImpuestos");
    const lblTotal      = document.getElementById("lblTotal");

    // Estado en memoria de las líneas del pedido: { productoId, nombre, cantidad, descuento, stock }
    let lineas = [];
    let debounceTimer = null;

    function formatoColones(valor) {
        return "₡" + Number(valor).toLocaleString("es-CR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    // ── Autosuggest ─────────────────────────────────────────────────────────
    inputBuscar.addEventListener("input", function () {
        clearTimeout(debounceTimer);
        const q = inputBuscar.value.trim();

        if (q.length < 2) {
            resultadosBox.style.display = "none";
            return;
        }

        debounceTimer = setTimeout(() => buscarProductos(q), 300);
    });

    document.addEventListener("click", function (e) {
        if (!resultadosBox.contains(e.target) && e.target !== inputBuscar) {
            resultadosBox.style.display = "none";
        }
    });

    async function buscarProductos(q) {
        try {
            const resp = await fetch("/api/productos/buscar?q=" + encodeURIComponent(q));
            if (!resp.ok) return;

            const productos = await resp.json();
            resultadosBox.innerHTML = "";

            if (productos.length === 0) {
                resultadosBox.style.display = "none";
                return;
            }

            productos.forEach(p => {
                const item = document.createElement("button");
                item.type = "button";
                item.className = "list-group-item list-group-item-action";
                item.innerHTML =
                    `<div class="d-flex justify-content-between">
                        <span>${p.nombre}</span>
                        <span class="text-muted small">₡${p.precio.toFixed(2)} · stock ${p.stock}</span>
                     </div>`;
                item.addEventListener("click", () => agregarLinea(p));
                resultadosBox.appendChild(item);
            });

            resultadosBox.style.display = "block";
        } catch (err) {
            console.error("Error al buscar productos:", err);
        }
    }

    // ── Dropdowns de Categoría / Producto ──────────────────────────────────
    selectCategoria.addEventListener("change", function () {
        const categoriaId = selectCategoria.value;

        [...selectProducto.options].forEach(option => {
            if (option.value === "") return; // opción "— Seleccione un producto —"
            const coincide = !categoriaId || option.dataset.categoria === categoriaId;
            option.hidden = !coincide;
        });

        // Si el producto seleccionado quedó oculto por el filtro, se limpia la selección
        const seleccionado = selectProducto.selectedOptions[0];
        if (seleccionado && seleccionado.hidden) {
            selectProducto.value = "";
        }
    });

    btnAgregarProducto.addEventListener("click", function () {
        const opcion = selectProducto.selectedOptions[0];
        if (!opcion || !opcion.value) return;

        agregarLinea({
            id: Number(opcion.value),
            nombre: opcion.dataset.nombre,
            precio: Number(opcion.dataset.precio),
            stock: Number(opcion.dataset.stock)
        });

        selectProducto.value = "";
    });

    // ── Líneas del pedido ───────────────────────────────────────────────────
    function agregarLinea(producto) {
        const existente = lineas.find(l => l.productoId === producto.id);
        if (existente) {
            existente.cantidad += 1;
        } else {
            lineas.push({
                productoId: producto.id,
                nombre: producto.nombre,
                cantidad: 1,
                descuento: 0,
                stock: producto.stock
            });
        }

        inputBuscar.value = "";
        resultadosBox.style.display = "none";
        renderTabla();
        recalcular();
    }

    function quitarLinea(productoId) {
        lineas = lineas.filter(l => l.productoId !== productoId);
        renderTabla();
        recalcular();
    }

    function renderTabla() {
        tablaLineas.innerHTML = "";

        if (lineas.length === 0) {
            tablaLineas.innerHTML =
                '<tr id="filaVacia"><td colspan="6" class="text-center py-4 text-muted">Aún no ha agregado productos al pedido.</td></tr>';
            btnConfirmar.disabled = true;
            return;
        }

        btnConfirmar.disabled = false;

        lineas.forEach(l => {
            const fila = document.createElement("tr");
            fila.innerHTML = `
                <td>${l.nombre}</td>
                <td>
                    <input type="number" min="1" max="${l.stock}" value="${l.cantidad}"
                           class="form-control form-control-sm input-cantidad" data-producto-id="${l.productoId}" />
                </td>
                <td class="precio-unit">-</td>
                <td>
                    <input type="number" min="0" max="100" value="${l.descuento}"
                           class="form-control form-control-sm input-descuento" data-producto-id="${l.productoId}" />
                </td>
                <td class="total-linea">-</td>
                <td>
                    <button type="button" class="btn btn-sm btn-outline-danger btn-quitar" data-producto-id="${l.productoId}">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>`;
            tablaLineas.appendChild(fila);
        });

        tablaLineas.querySelectorAll(".input-cantidad").forEach(input => {
            input.addEventListener("change", onCambiarLinea);
        });
        tablaLineas.querySelectorAll(".input-descuento").forEach(input => {
            input.addEventListener("change", onCambiarLinea);
        });
        tablaLineas.querySelectorAll(".btn-quitar").forEach(btn => {
            btn.addEventListener("click", () => quitarLinea(Number(btn.dataset.productoId)));
        });
    }

    function onCambiarLinea(e) {
        const productoId = Number(e.target.dataset.productoId);
        const linea = lineas.find(l => l.productoId === productoId);
        if (!linea) return;

        if (e.target.classList.contains("input-cantidad")) {
            linea.cantidad = Math.max(1, Number(e.target.value) || 1);
        } else {
            linea.descuento = Math.min(100, Math.max(0, Number(e.target.value) || 0));
        }

        recalcular();
    }

    // ── Cálculo en vivo vía POST /api/pedidos/calcular ─────────────────────
    async function recalcular() {
        if (lineas.length === 0) {
            lblSubtotal.textContent  = formatoColones(0);
            lblImpuestos.textContent = formatoColones(0);
            lblTotal.textContent     = formatoColones(0);
            hiddenInputs.innerHTML   = "";
            return;
        }

        const body = {
            lineas: lineas.map(l => ({
                productoId: l.productoId,
                cantidad: l.cantidad,
                descuento: l.descuento
            }))
        };

        try {
            const resp = await fetch("/api/pedidos/calcular", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body)
            });

            if (!resp.ok) return;

            const resultado = await resp.json();

            lblSubtotal.textContent  = formatoColones(resultado.subtotal);
            lblImpuestos.textContent = formatoColones(resultado.impuestos);
            lblTotal.textContent     = formatoColones(resultado.total);

            let stockInsuficiente = false;

            resultado.lineas.forEach(r => {
                const fila = [...tablaLineas.querySelectorAll(".input-cantidad")]
                    .find(input => Number(input.dataset.productoId) === r.productoId)
                    ?.closest("tr");

                if (fila) {
                    fila.querySelector(".precio-unit").textContent = formatoColones(r.precioUnitario);
                    fila.querySelector(".total-linea").textContent = formatoColones(r.totalLinea);
                    fila.classList.toggle("table-danger", r.stockInsuficiente);
                }

                if (r.stockInsuficiente) stockInsuficiente = true;
            });

            btnConfirmar.disabled = stockInsuficiente;
            actualizarHiddenInputs();
        } catch (err) {
            console.error("Error al calcular totales:", err);
        }
    }

    // Inputs ocultos con el formato de binding de listas de ASP.NET Core: Lineas[i].Propiedad
    function actualizarHiddenInputs() {
        hiddenInputs.innerHTML = "";
        lineas.forEach((l, i) => {
            hiddenInputs.insertAdjacentHTML("beforeend", `
                <input type="hidden" name="Lineas[${i}].id_Producto" value="${l.productoId}" />
                <input type="hidden" name="Lineas[${i}].Cantidad" value="${l.cantidad}" />
                <input type="hidden" name="Lineas[${i}].Descuento" value="${l.descuento}" />
            `);
        });
    }
})();
