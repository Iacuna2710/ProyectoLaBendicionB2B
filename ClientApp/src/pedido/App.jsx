import { useEffect, useMemo, useState } from 'react'
import SelectorCliente from './components/SelectorCliente.jsx'
import SelectorCategoriaProducto from './components/SelectorCategoriaProducto.jsx'
import BuscadorProducto from './components/BuscadorProducto.jsx'
import TablaLineas from './components/TablaLineas.jsx'
import ResumenTotales from './components/ResumenTotales.jsx'

export default function App({ clientes, categorias, productos }) {
  const [clienteId, setClienteId] = useState('')
  const [lineas, setLineas] = useState([]) // { productoId, nombre, cantidad, descuento, stock }
  const [calculo, setCalculo] = useState(null) // respuesta de POST /api/pedidos/calcular
  const [calculando, setCalculando] = useState(false)

  // Mapa productoId -> línea calculada por el servidor, para que TablaLineas
  // pinte precio/descuento/total con la fuente de verdad (nunca calculado en el navegador).
  const resultadoPorProducto = useMemo(() => {
    const mapa = {}
    for (const r of calculo?.lineas ?? []) mapa[r.productoId] = r
    return mapa
  }, [calculo])

  const stockInsuficiente = (calculo?.lineas ?? []).some((r) => r.stockInsuficiente)

  useEffect(() => {
    if (lineas.length === 0) {
      setCalculo(null)
      return
    }

    let cancelado = false
    setCalculando(true)

    fetch('/api/pedidos/calcular', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        lineas: lineas.map((l) => ({
          productoId: l.productoId,
          cantidad: l.cantidad,
          descuento: l.descuento
        }))
      })
    })
      .then((resp) => (resp.ok ? resp.json() : null))
      .then((resultado) => {
        if (!cancelado && resultado) setCalculo(resultado)
      })
      .catch((err) => console.error('Error al calcular totales:', err))
      .finally(() => {
        if (!cancelado) setCalculando(false)
      })

    return () => {
      cancelado = true
    }
  }, [lineas])

  function agregarLinea(producto) {
    setLineas((prev) => {
      const existente = prev.find((l) => l.productoId === producto.id)
      if (existente) {
        return prev.map((l) =>
          l.productoId === producto.id ? { ...l, cantidad: l.cantidad + 1 } : l
        )
      }
      return [
        ...prev,
        { productoId: producto.id, nombre: producto.nombre, cantidad: 1, descuento: 0, stock: producto.stock }
      ]
    })
  }

  function cambiarCantidad(productoId, cantidad) {
    setLineas((prev) =>
      prev.map((l) => (l.productoId === productoId ? { ...l, cantidad: Math.max(1, cantidad) } : l))
    )
  }

  function cambiarDescuento(productoId, descuento) {
    setLineas((prev) =>
      prev.map((l) =>
        l.productoId === productoId ? { ...l, descuento: Math.min(100, Math.max(0, descuento)) } : l
      )
    )
  }

  function quitarLinea(productoId) {
    setLineas((prev) => prev.filter((l) => l.productoId !== productoId))
  }

  // El botón solo se habilita cuando ya hay un cálculo fresco del servidor,
  // sin problemas de stock — evita enviar el formulario con datos desactualizados.
  const puedeConfirmar = lineas.length > 0 && !calculando && !!calculo && !stockInsuficiente

  return (
    <>
      <div className="card-modern mb-4 p-3">
        <div className="row g-3">
          <SelectorCliente clientes={clientes} clienteId={clienteId} onChange={setClienteId} />
        </div>

        <hr className="my-3" />

        <SelectorCategoriaProducto categorias={categorias} productos={productos} onAgregar={agregarLinea} />
        <BuscadorProducto onAgregar={agregarLinea} />
      </div>

      <div className="card-modern">
        <div className="table-responsive">
          <table className="table table-modern align-middle mb-0">
            <thead>
              <tr>
                <th>Producto</th>
                <th style={{ width: 110 }}>Cantidad</th>
                <th style={{ width: 130 }}>Precio unit.</th>
                <th style={{ width: 110 }}>Descuento %</th>
                <th style={{ width: 130 }}>Descuento (₡)</th>
                <th style={{ width: 130 }}>Total línea</th>
                <th style={{ width: 50 }}></th>
              </tr>
            </thead>
            <TablaLineas
              lineas={lineas}
              resultadoPorProducto={resultadoPorProducto}
              onCambiarCantidad={cambiarCantidad}
              onCambiarDescuento={cambiarDescuento}
              onQuitar={quitarLinea}
            />
          </table>
        </div>
        <ResumenTotales calculo={calculo} />
      </div>

      {/* Inputs ocultos con el formato de binding de listas de ASP.NET Core */}
      {lineas.map((l, i) => (
        <span key={l.productoId}>
          <input type="hidden" name={`Lineas[${i}].id_Producto`} value={l.productoId} />
          <input type="hidden" name={`Lineas[${i}].Cantidad`} value={l.cantidad} />
          <input type="hidden" name={`Lineas[${i}].Descuento`} value={l.descuento} />
        </span>
      ))}

      <div className="mt-3 text-end">
        <button type="submit" className="btn btn-primary rounded-pill px-4" disabled={!puedeConfirmar}>
          <i className="bi bi-check-lg me-1"></i> Confirmar pedido
        </button>
      </div>
    </>
  )
}
