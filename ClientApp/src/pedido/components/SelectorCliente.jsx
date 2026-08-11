// Select de cliente. Se manda con el <form> real de Razor gracias a name="id_Cliente",
// igual que hacía el <select> plano en la versión anterior sin React.
export default function SelectorCliente({ clientes, clienteId, onChange }) {
  return (
    <div className="col-md-12">
      <label className="form-label small text-muted">Cliente</label>
      <select
        name="id_Cliente"
        className="form-select"
        value={clienteId}
        onChange={(e) => onChange(e.target.value)}
      >
        <option value="">— Seleccione un cliente —</option>
        {clientes.map((c) => (
          <option key={c.value} value={c.value}>
            {c.text}
          </option>
        ))}
      </select>
    </div>
  )
}
