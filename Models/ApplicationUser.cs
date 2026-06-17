using Microsoft.AspNetCore.Identity;

namespace Pedidos360.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Rol { get; set; }
    }
}
