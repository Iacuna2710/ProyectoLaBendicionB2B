import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Workspace único para todas las "islas" de React del sitio: cada pantalla
// interactiva (Pedido, Productos...) tiene su propio entry point, pero
// comparten dependencias y utilidades (src/shared). Cada una compila a su
// propia carpeta en wwwroot/dist/<pantalla>/main.js y se monta por separado
// desde su vista Razor correspondiente — el resto del sitio sigue MVC normal.
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: '../wwwroot/dist',
    emptyOutDir: true,
    rollupOptions: {
      input: {
        pedido: 'src/pedido/main.jsx',
        'productos-index': 'src/productos-index/main.jsx'
      },
      output: {
        entryFileNames: (chunk) => `${chunk.name}/main.js`,
        chunkFileNames: 'chunks/[name]-[hash].js',
        assetFileNames: '[name]/asset-[name][extname]'
      }
    }
  }
})
