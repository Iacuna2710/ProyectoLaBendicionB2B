import { useEffect, useRef, useState } from 'react'
import Filtros from './components/Filtros.jsx'
import TablaProductos from './components/TablaProductos.jsx'
import Paginacion from './components/Paginacion.jsx'

export default function App({ categorias, roles, filtroNombreInicial, filtroCategoriaInicial, datosIniciales }) {
  const [filtroNombre, setFiltroNombre] = useState(filtroNombreInicial)
  const [filtroCategoria, setFiltroCategoria] = useState(filtroCategoriaInicial)
  const [pagina, setPagina] = useState(datosIniciales?.paginaActual ?? 1)
  const [datos, setDatos] = useState(
    datosIniciales ?? { productos: [], totalItems: 0, totalPaginas: 0, paginaActual: 1 }
  )
  const [cargando, setCargando] = useState(!datosIniciales)

  // El servidor ya resolvió la primera página en el mismo request que sirvió
  // el HTML (Model.DatosIniciales) — el primer render de este efecto se
  // saltea para no repetir esa misma consulta apenas React se monta.
  const primerRender = useRef(!!datosIniciales)

  const esAdmin = roles.includes('Admin')
  const esOperaciones = roles.includes('Operaciones')

  function cambiarFiltroNombre(valor) {
    setFiltroNombre(valor)
    setPagina(1) // cualquier cambio de filtro vuelve a la primera página
  }

  function cambiarFiltroCategoria(valor) {
    setFiltroCategoria(valor)
    setPagina(1)
  }

  // Consulta la API en vivo (con debounce) cada vez que cambia el filtro o la página.
  useEffect(() => {
    if (primerRender.current) {
      primerRender.current = false
      return
    }

    let cancelado = false
    setCargando(true)

    const params = new URLSearchParams()
    if (filtroNombre) params.set('filtroNombre', filtroNombre)
    if (filtroCategoria) params.set('filtroCategoria', filtroCategoria)
    params.set('pagina', String(pagina))

    const debounce = setTimeout(() => {
      fetch('/api/productos?' + params.toString())
        .then((resp) => (resp.ok ? resp.json() : null))
        .then((resultado) => {
          if (!cancelado && resultado) setDatos(resultado)
        })
        .catch((err) => console.error('Error al cargar productos:', err))
        .finally(() => {
          if (!cancelado) setCargando(false)
        })
    }, 300)

    return () => {
      cancelado = true
      clearTimeout(debounce)
    }
  }, [filtroNombre, filtroCategoria, pagina])

  function limpiarFiltros() {
    setFiltroNombre('')
    setFiltroCategoria('')
    setPagina(1)
  }

  return (
    <>
      <div className="page-header">
        <div>
          <h1>Productos</h1>
          <p className="text-muted mb-0">{datos.totalItems} productos</p>
        </div>
        {esAdmin && (
          <a href="/Productos/Create" className="btn btn-primary rounded-pill">
            <i className="bi bi-plus-lg me-1"></i> Nuevo
          </a>
        )}
      </div>

      <Filtros
        categorias={categorias}
        filtroNombre={filtroNombre}
        filtroCategoria={filtroCategoria}
        onChangeNombre={cambiarFiltroNombre}
        onChangeCategoria={cambiarFiltroCategoria}
        onLimpiar={limpiarFiltros}
      />

      <div className="card-modern">
        <div className="table-responsive">
          <table className="table table-modern">
            <thead>
              <tr>
                <th>Producto</th>
                <th>Categoría</th>
                <th>Precio</th>
                <th>Stock</th>
                <th>Estado</th>
                <th className="text-end">Acciones</th>
              </tr>
            </thead>
            <TablaProductos
              productos={datos.productos}
              cargando={cargando}
              puedeEditar={esAdmin || esOperaciones}
              puedeEliminar={esAdmin}
            />
          </table>
        </div>
        <Paginacion
          paginaActual={datos.paginaActual}
          totalPaginas={datos.totalPaginas}
          totalItems={datos.totalItems}
          onCambiarPagina={setPagina}
        />
      </div>
    </>
  )
}
