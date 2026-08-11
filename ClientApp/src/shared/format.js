export function formatoColones(valor) {
  return '₡' + Number(valor).toLocaleString('es-CR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}
