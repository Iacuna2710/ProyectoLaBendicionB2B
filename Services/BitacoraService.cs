using MacrobioticaLaBendicion.Data;
using MacrobioticaLaBendicion.Models;

namespace MacrobioticaLaBendicion.Services
// Registra acciones de auditoría (crear/editar/eliminar/confirmar) sin
// interrumpir la operación principal si algo falla al guardar el registro.
{
    public class BitacoraService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BitacoraService> _logger;

        public BitacoraService(ApplicationDbContext context, ILogger<BitacoraService> logger)
        {
            _context = context;
            _logger  = logger;
        }

        public async Task RegistrarAsync(
            string usuarioId, string usuarioNombre, string accion, string entidad,
            int? entidadId = null, string? detalle = null)
        {
            try
            {
                _context.Bitacoras.Add(new Bitacora
                {
                    UsuarioId     = usuarioId,
                    UsuarioNombre = usuarioNombre,
                    Accion        = accion,
                    Entidad       = entidad,
                    EntidadId     = entidadId,
                    Detalle       = detalle,
                    Fecha         = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // La bitácora nunca debe tumbar la operación que la disparó
                _logger.LogWarning(ex, "No se pudo registrar bitácora: {Accion} {Entidad} {EntidadId}",
                    accion, entidad, entidadId);
            }
        }

        // Variante para usar dentro de una transacción ya abierta (ej. confirmar pedido):
        // agrega la entidad sin hacer su propio SaveChanges, para que quede atómica
        // con el resto de la operación.
        public void Agregar(
            string usuarioId, string usuarioNombre, string accion, string entidad,
            int? entidadId = null, string? detalle = null)
        {
            _context.Bitacoras.Add(new Bitacora
            {
                UsuarioId     = usuarioId,
                UsuarioNombre = usuarioNombre,
                Accion        = accion,
                Entidad       = entidad,
                EntidadId     = entidadId,
                Detalle       = detalle,
                Fecha         = DateTime.Now
            });
        }
    }
}
