using Microsoft.EntityFrameworkCore;
using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.ViewModels.Api;

namespace MacrobioticaLaBendicion.Services
{
    // Calcula subtotal/impuestos/total de un pedido a partir de las líneas.
    // Es la única fuente de verdad para el cálculo: la usan tanto el endpoint
    // de AJAX (vista previa en vivo) como la confirmación del pedido (persistencia),
    // así nunca se desincronizan.
    public class CalculoPedidoService
    {
        private readonly ApplicationDbContext _context;

        public CalculoPedidoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CalcularPedidoResponse> CalcularAsync(IEnumerable<CalcularLineaDto> lineas)
        {
            var respuesta = new CalcularPedidoResponse();
            var productoIds = lineas.Select(l => l.ProductoId).Distinct().ToList();

            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.id_Producto))
                .ToDictionaryAsync(p => p.id_Producto);

            foreach (var linea in lineas)
            {
                if (!productos.TryGetValue(linea.ProductoId, out var producto))
                    continue; // producto inexistente: se ignora la línea

                var cantidad  = Math.Max(0, linea.Cantidad);
                var descuento = Math.Clamp(linea.Descuento, 0, 100);

                var baseLinea      = producto.Precio * cantidad;
                var descuentoMonto = baseLinea * (descuento / 100m);
                var baseConDesc    = baseLinea - descuentoMonto;
                var impuestoLinea  = baseConDesc * (producto.ImpuestoPorc / 100m);
                var totalLinea     = baseConDesc + impuestoLinea;

                respuesta.Lineas.Add(new CalcularLineaResultDto
                {
                    ProductoId        = producto.id_Producto,
                    Nombre            = producto.Nombre,
                    Cantidad          = cantidad,
                    PrecioUnitario    = producto.Precio,
                    Descuento         = descuento,
                    DescuentoMonto    = Math.Round(descuentoMonto, 2),
                    ImpuestoPorc      = producto.ImpuestoPorc,
                    TotalLinea        = Math.Round(totalLinea, 2),
                    StockDisponible   = producto.Stock,
                    StockInsuficiente = cantidad > producto.Stock
                });

                respuesta.Subtotal  += Math.Round(baseConDesc, 2);
                respuesta.Impuestos += Math.Round(impuestoLinea, 2);
            }

            respuesta.Total = respuesta.Subtotal + respuesta.Impuestos;
            return respuesta;
        }
    }
}
