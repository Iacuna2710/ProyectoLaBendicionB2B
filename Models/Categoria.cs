using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
    //  clase modelo de categoria
{
    public partial class Categoria
    {
        public int id_Categoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Categoría")]
        public string Nombre { get; set; } = null!;

        // Navegación
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
