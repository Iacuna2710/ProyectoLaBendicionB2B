export default function Paginacion({ paginaActual, totalPaginas, totalItems, onCambiarPagina }) {
  if (totalPaginas <= 1) return null

  const paginas = Array.from({ length: totalPaginas }, (_, i) => i + 1)

  return (
    <div className="d-flex justify-content-between align-items-center p-3 border-top">
      <small className="text-muted">
        Pág. {paginaActual} de {totalPaginas} ({totalItems} productos)
      </small>
      <nav>
        <ul className="pagination pagination-modern mb-0">
          <li className={`page-item ${paginaActual === 1 ? 'disabled' : ''}`}>
            <button className="page-link" onClick={() => onCambiarPagina(paginaActual - 1)}>
              <i className="bi bi-chevron-left"></i>
            </button>
          </li>
          {paginas.map((i) => (
            <li key={i} className={`page-item ${i === paginaActual ? 'active' : ''}`}>
              <button className="page-link" onClick={() => onCambiarPagina(i)}>
                {i}
              </button>
            </li>
          ))}
          <li className={`page-item ${paginaActual === totalPaginas ? 'disabled' : ''}`}>
            <button className="page-link" onClick={() => onCambiarPagina(paginaActual + 1)}>
              <i className="bi bi-chevron-right"></i>
            </button>
          </li>
        </ul>
      </nav>
    </div>
  )
}
