using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MacrobioticaLaBendicion.Services;
using MacrobioticaLaBendicion.ViewModels.Api;

namespace MacrobioticaLaBendicion.Controllers.Api
{
    [ApiController]
    [Route("api/pedidos")]
    [Authorize] // solo usuarios autenticados pueden consumir el endpoint
    public class PedidosApiController : ControllerBase
    {
        private readonly CalculoPedidoService _calculo;

        public PedidosApiController(CalculoPedidoService calculo)
        {
            _calculo = calculo;
        }

        // POST /api/pedidos/calcular
        [HttpPost("calcular")]
        public async Task<ActionResult<CalcularPedidoResponse>> Calcular([FromBody] CalcularPedidoRequest request)
        {
            if (request?.Lineas is null || request.Lineas.Count == 0)
                return Ok(new CalcularPedidoResponse());

            var resultado = await _calculo.CalcularAsync(request.Lineas);
            return Ok(resultado);
        }
    }
}
