import { useMemo, useState } from 'react'

// Dropdown de Categoría que filtra el dropdown de Producto, + botón "Agregar".
// Equivalente al selectCategoria/selectProducto/btnAgregarProducto del pedido-ajax.js original.
export default function SelectorCategoriaProducto({ categorias, productos, onAgregar }) {
  const [categoriaId, setCategoriaId] = useState('')
  const [productoId, setProductoId] = useState('')

  const productosFiltrados = useMemo(() => {
    if (!categoriaId) return productos
    return productos.filter((p) => String(p.categoriaId) === categoriaId)
  }, [productos, categoriaId])

  function handleCategoriaChange(valor) {
    setCategoriaId(valor)
    // si el producto elegido ya no aplica al filtro nuevo, se limpia
    if (valor && !productos.some((p) => String(p.id) === productoId && String(p.categoriaId) === valor)) {
      setProductoId('')
    }
  }

  function handleAgregar() {
    const producto = productos.find((p) => String(p.id) === productoId)
    if (!producto) return
    onAgregar(producto)
    setProductoId('')
  }

  return (
    <div className="row g-3 align-items-end">
      <div className="col-md-4">
        <label className="form-label small text-muted">Categoría</label>
        <select
          className="form-select"
          value={categoriaId}
          onChange={(e) => handleCategoriaChange(e.target.value)}
        >
          <option value="">— Todas —</option>
          {categorias.map((c) => (
            <option key={c.value} value={c.value}>
              {c.text}
            </option>
          ))}
        </select>
      </div>
      <div className="col-md-5">
        <label className="form-label small text-muted">Producto</label>
        <select
          className="form-select"
          value={productoId}
          onChange={(e) => setProductoId(e.target.value)}
        >
          <option value="">— Seleccione un producto —</option>
          {productosFiltrados.map((p) => (
            <option key={p.id} value={p.id}>
              {p.nombre} (₡{p.precio.toFixed(2)} · stock {p.stock})
            </option>
          ))}
        </select>
      </div>
      <div className="col-md-3">
        <button type="button" className="btn btn-outline-primary w-100" onClick={handleAgregar}>
          <i className="bi bi-plus-lg me-1"></i> Agregar
        </button>
      </div>
    </div>
  )
}
