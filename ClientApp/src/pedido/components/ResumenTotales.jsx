import { formatoColones } from '../../shared/format.js'

export default function ResumenTotales({ calculo }) {
  const subtotal = calculo?.subtotal ?? 0
  const impuestos = calculo?.impuestos ?? 0
  const total = calculo?.total ?? 0

  return (
    <div className="d-flex justify-content-end gap-4 p-3 border-top">
      <div className="text-end">
        <div className="small text-muted">Subtotal</div>
        <div className="fw-semibold">{formatoColones(subtotal)}</div>
      </div>
      <div className="text-end">
        <div className="small text-muted">Impuestos</div>
        <div className="fw-semibold">{formatoColones(impuestos)}</div>
      </div>
      <div className="text-end">
        <div className="small text-muted">Total</div>
        <div className="fs-5 fw-bold text-success">{formatoColones(total)}</div>
      </div>
    </div>
  )
}
