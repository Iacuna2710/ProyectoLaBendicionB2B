using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacrobioticaLaBendicion.Models
// clase modelo de pedido
{
    public partial class Pedido
    {
        public int id_Pedido { get; set; }

        [Required]
        public int id_Cliente { get; set; }

        [Required]
        public string id_Usuario { get; set; } = null!;

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Impuestos")]
        public decimal Impuestos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [StringLength(50)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";

        // Navegación
        public Cliente? Cliente { get; set; }
        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
