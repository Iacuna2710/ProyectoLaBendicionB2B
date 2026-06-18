using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacrobioticaLaBendicion.Models
// clase modelo de detalle de pedido
{
    public partial class PedidoDetalle
    {
        public int id_detalleP { get; set; }

        [Required]
        public int id_Pedido { get; set; }

        [Required]
        public int id_Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Descuento (%)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Impuesto (%)")]
        public decimal Porcentaje_Imp { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Línea")]
        public decimal Total_Linea { get; set; }

        // Navegación
        public Pedido? Pedido { get; set; }
        public Producto? Producto { get; set; }
    }
}
