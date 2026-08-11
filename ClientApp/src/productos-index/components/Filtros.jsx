export default function Filtros({ categorias, filtroNombre, filtroCategoria, onChangeNombre, onChangeCategoria, onLimpiar }) {
  return (
    <div className="card-modern mb-4 p-3">
      <div className="row g-2 align-items-end">
        <div className="col-md-4">
          <label className="form-label small text-muted">Nombre</label>
          <div className="input-group">
            <span className="input-group-text bg-transparent">
              <i className="bi bi-search"></i>
            </span>
            <input
              type="text"
              className="form-control"
              placeholder="Buscar producto..."
              value={filtroNombre}
              onChange={(e) => onChangeNombre(e.target.value)}
            />
          </div>
        </div>
        <div className="col-md-3">
          <label className="form-label small text-muted">Categoría</label>
          <select
            className="form-select"
            value={filtroCategoria}
            onChange={(e) => onChangeCategoria(e.target.value)}
          >
            <option value="">Todas</option>
            {categorias.map((c) => (
              <option key={c.value} value={c.value}>
                {c.text}
              </option>
            ))}
          </select>
        </div>
        <div className="col-md-3 d-flex gap-2 pt-3">
          <button type="button" className="btn btn-outline-secondary flex-grow-1" onClick={onLimpiar}>
            <i className="bi bi-x-lg me-1"></i> Limpiar filtros
          </button>
        </div>
      </div>
    </div>
  )
}
