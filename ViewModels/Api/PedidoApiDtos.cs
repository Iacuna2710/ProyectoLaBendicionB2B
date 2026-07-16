namespace MacrobioticaLaBendicion.ViewModels.Api
{
    // Forma de cada coincidencia devuelta por GET /api/productos/buscar
    public class ProductoBusquedaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public decimal Impuesto { get; set; }
        public int Stock { get; set; }
    }

    // Una línea enviada a POST /api/pedidos/calcular
    public class CalcularLineaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }
    }

    // Cuerpo de la solicitud POST /api/pedidos/calcular
    public class CalcularPedidoRequest
    {
        public List<CalcularLineaDto> Lineas { get; set; } = [];
    }

    // Respuesta con los totales calculados en el servidor
    // (fuente de verdad; el cálculo en el navegador es solo una vista previa)
    public class CalcularPedidoResponse
    {
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public List<CalcularLineaResultDto> Lineas { get; set; } = [];
    }

    public class CalcularLineaResultDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal DescuentoMonto { get; set; }
        public decimal ImpuestoPorc { get; set; }
        public decimal TotalLinea { get; set; }
        public int StockDisponible { get; set; }
        public bool StockInsuficiente { get; set; }
    }
}
