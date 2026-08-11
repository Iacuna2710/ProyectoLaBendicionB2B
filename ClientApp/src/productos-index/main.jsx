import { createRoot } from 'react-dom/client'
import App from './App.jsx'

const root = document.getElementById('productos-root')

if (root) {
  const categorias = JSON.parse(root.dataset.categorias || '[]')
  const roles = JSON.parse(root.dataset.roles || '[]')
  const filtroNombreInicial = root.dataset.filtroNombre || ''
  const filtroCategoriaInicial = root.dataset.filtroCategoria || ''
  // Primera página ya resuelta por el servidor (mismo request que sirvió el
  // HTML) — evita que React tenga que hacer un primer fetch solo para
  // mostrar lo mismo que el servidor ya sabía.
  const datosIniciales = JSON.parse(root.dataset.datosIniciales || 'null')

  createRoot(root).render(
    <App
      categorias={categorias}
      roles={roles}
      filtroNombreInicial={filtroNombreInicial}
      filtroCategoriaInicial={filtroCategoriaInicial}
      datosIniciales={datosIniciales}
    />
  )
}
