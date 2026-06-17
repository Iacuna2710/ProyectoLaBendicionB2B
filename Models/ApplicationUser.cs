using Microsoft.AspNetCore.Identity;

namespace Pedidos360.Models
    // clase modelo para el manejo e roles en el sistema
{
    public class ApplicationUser : IdentityUser

    {
        public string? Rol { get; set; }
    }
}
