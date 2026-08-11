using System.ComponentModel.DataAnnotations;

namespace MacrobioticaLaBendicion.Models
// Clase modelo para el registro de auditoría de acciones sobre el sistema
{
    public class Bitacora
    {
        public int id_Bitacora { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioId { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string UsuarioNombre { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Entidad { get; set; } = null!;

        public int? EntidadId { get; set; }

        [StringLength(500)]
        public string? Detalle { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
