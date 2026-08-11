import { useEffect, useRef, useState } from 'react'

// Autosuggest de texto: llama a GET /api/productos/buscar con un pequeño
// debounce, igual que la versión en JS puro.
export default function BuscadorProducto({ onAgregar }) {
  const [query, setQuery] = useState('')
  const [resultados, setResultados] = useState([])
  const [mostrando, setMostrando] = useState(false)
  const debounceRef = useRef(null)
  const cajaRef = useRef(null)

  useEffect(() => {
    function handleClickFuera(e) {
      if (cajaRef.current && !cajaRef.current.contains(e.target)) {
        setMostrando(false)
      }
    }
    document.addEventListener('click', handleClickFuera)
    return () => document.removeEventListener('click', handleClickFuera)
  }, [])

  function handleInput(valor) {
    setQuery(valor)
    clearTimeout(debounceRef.current)

    if (valor.trim().length < 2) {
      setMostrando(false)
      return
    }

    debounceRef.current = setTimeout(() => buscar(valor.trim()), 300)
  }

  async function buscar(q) {
    try {
      const resp = await fetch('/api/productos/buscar?q=' + encodeURIComponent(q))
      if (!resp.ok) return
      const productos = await resp.json()
      setResultados(productos)
      setMostrando(productos.length > 0)
    } catch (err) {
      console.error('Error al buscar productos:', err)
    }
  }

  function seleccionar(producto) {
    onAgregar({
      id: producto.id,
      nombre: producto.nombre,
      precio: producto.precio,
      stock: producto.stock
    })
    setQuery('')
    setMostrando(false)
  }

  return (
    <div className="row g-3 mt-1">
      <div className="col-12">
        <label className="form-label small text-muted">O buscar por nombre</label>
        <div className="position-relative" ref={cajaRef}>
          <input
            type="text"
            className="form-control"
            placeholder="Escriba nombre del producto..."
            autoComplete="off"
            value={query}
            onChange={(e) => handleInput(e.target.value)}
          />
          {mostrando && (
            <div className="list-group position-absolute w-100 shadow-sm" style={{ zIndex: 1000 }}>
              {resultados.map((p) => (
                <button
                  key={p.id}
                  type="button"
                  className="list-group-item list-group-item-action"
                  onClick={() => seleccionar(p)}
                >
                  <div className="d-flex justify-content-between">
                    <span>{p.nombre}</span>
                    <span className="text-muted small">
                      ₡{p.precio.toFixed(2)} · stock {p.stock}
                    </span>
                  </div>
                </button>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
