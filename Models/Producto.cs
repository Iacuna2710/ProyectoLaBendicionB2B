using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MacrobioticaLaBendicion.Models
// clase modelo de producto
{
    public partial class Producto
    {
        public int id_Producto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int id_Categoria { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 9999999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio (₡)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El porcentaje de impuesto es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El impuesto debe estar entre 0% y 100%.")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Impuesto (%)")]
        public decimal ImpuestoPorc { get; set; } = 13;

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Imagen")]
        public string? Url_Imagen { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        

        // Navegación
        public Categoria? Categoria { get; set; }
        public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
    }
}
