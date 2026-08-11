import { formatoColones } from '../../shared/format.js'

export default function TablaProductos({ productos, cargando, puedeEditar, puedeEliminar }) {
  if (cargando) {
    return (
      <tbody>
        <tr>
          <td colSpan={6} className="text-center py-4 text-muted">
            <span className="spinner-border spinner-border-sm me-2"></span>
            Cargando...
          </td>
        </tr>
      </tbody>
    )
  }

  if (productos.length === 0) {
    return (
      <tbody>
        <tr>
          <td colSpan={6} className="text-center py-4 text-muted">
            No hay productos que coincidan.
          </td>
        </tr>
      </tbody>
    )
  }

  return (
    <tbody>
      {productos.map((p) => (
        <tr key={p.id}>
          <td>
            <div className="d-flex align-items-center gap-2">
              {p.urlThumbnail || p.urlImagen ? (
                <img
                  src={p.urlThumbnail ?? p.urlImagen}
                  alt=""
                  className="rounded"
                  style={{ width: 36, height: 36, objectFit: 'cover' }}
                />
              ) : (
                <div
                  className="rounded bg-light d-flex align-items-center justify-content-center"
                  style={{ width: 36, height: 36 }}
                >
                  <i className="bi bi-image text-muted"></i>
                </div>
              )}
              <strong>{p.nombre}</strong>
            </div>
          </td>
          <td>
            <span className="badge badge-modern bg-light text-dark">{p.categoriaNombre}</span>
          </td>
          <td className="fw-semibold">₡ {p.precio.toFixed(2)}</td>
          <td>
            {p.stock <= 5 ? (
              <span className="text-danger fw-semibold">{p.stock}</span>
            ) : (
              <span>{p.stock}</span>
            )}
          </td>
          <td>
            {p.activo ? (
              <span className="badge badge-modern bg-success-subtle text-success">Activo</span>
            ) : (
              <span className="badge badge-modern bg-danger-subtle text-danger">Inactivo</span>
            )}
          </td>
          <td className="text-end">
            <a href={`/Productos/Details/${p.id}`} className="btn btn-sm btn-outline-secondary" title="Ver">
              <i className="bi bi-eye"></i>
            </a>
            {puedeEditar && (
              <a href={`/Productos/Edit/${p.id}`} className="btn btn-sm btn-outline-primary" title="Editar">
                <i className="bi bi-pencil"></i>
              </a>
            )}
            {puedeEliminar && (
              <a href={`/Productos/Delete/${p.id}`} className="btn btn-sm btn-outline-danger" title="Eliminar">
                <i className="bi bi-trash"></i>
              </a>
            )}
          </td>
        </tr>
      ))}
    </tbody>
  )
}
