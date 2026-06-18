using System.ComponentModel.DataAnnotations;

namespace MacrobioticaLaBendicion.Models
// clase modelo de cliente  
{
    public partial class Cliente
    {
        public int id_Cliente { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(200)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La cédula / jurídica es obligatoria.")]
        [StringLength(20)]
        [Display(Name = "Cédula / Jurídica")]
        public string Cedula { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(200)]
        [Display(Name = "Correo")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = null!;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(500)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = null!;


        // Relación uno a muchos con Pedido
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
