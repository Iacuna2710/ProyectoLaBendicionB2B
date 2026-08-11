import { formatoColones } from '../../shared/format.js'

// Tabla editable de líneas del pedido. Los valores de precio/descuento en ₡/total
// vienen siempre del último cálculo del servidor (prop `resultadoPorProducto`),
// nunca se calculan en el navegador.
export default function TablaLineas({ lineas, resultadoPorProducto, onCambiarCantidad, onCambiarDescuento, onQuitar }) {
  if (lineas.length === 0) {
    return (
      <tbody>
        <tr>
          <td colSpan={7} className="text-center py-4 text-muted">
            Aún no ha agregado productos al pedido.
          </td>
        </tr>
      </tbody>
    )
  }

  return (
    <tbody>
      {lineas.map((l) => {
        const r = resultadoPorProducto[l.productoId]
        return (
          <tr key={l.productoId} className={r?.stockInsuficiente ? 'table-danger' : ''}>
            <td>{l.nombre}</td>
            <td>
              <input
                type="number"
                min="1"
                max={l.stock}
                className="form-control form-control-sm"
                value={l.cantidad}
                onChange={(e) => onCambiarCantidad(l.productoId, Number(e.target.value) || 1)}
              />
            </td>
            <td>{r ? formatoColones(r.precioUnitario) : '-'}</td>
            <td>
              <input
                type="number"
                min="0"
                max="100"
                className="form-control form-control-sm"
                value={l.descuento}
                onChange={(e) => onCambiarDescuento(l.productoId, Number(e.target.value) || 0)}
              />
            </td>
            <td className="text-muted">
              {r ? (r.descuentoMonto > 0 ? '-' + formatoColones(r.descuentoMonto) : formatoColones(0)) : '-'}
            </td>
            <td>{r ? formatoColones(r.totalLinea) : '-'}</td>
            <td>
              <button
                type="button"
                className="btn btn-sm btn-outline-danger"
                onClick={() => onQuitar(l.productoId)}
              >
                <i className="bi bi-trash"></i>
              </button>
            </td>
          </tr>
        )
      })}
    </tbody>
  )
}
