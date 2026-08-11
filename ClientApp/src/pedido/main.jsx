import { createRoot } from 'react-dom/client'
import App from './App.jsx'

const root = document.getElementById('pedido-root')

if (root) {
  const clientes   = JSON.parse(root.dataset.clientes || '[]')
  const categorias = JSON.parse(root.dataset.categorias || '[]')
  const productos  = JSON.parse(root.dataset.productos || '[]')

  createRoot(root).render(
    <App clientes={clientes} categorias={categorias} productos={productos} />
  )
}
