#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using MacrobioticaLaBendicion.Models;

namespace MacrobioticaLaBendicion.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly IUserStore<ApplicationUser>   _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel>        _logger;
        private readonly IEmailSender                  _emailSender;
        private readonly RoleManager<IdentityRole>     _roleManager;

        private const string RolAdmin = "Admin";

        public RegisterModel(
            UserManager<ApplicationUser>  userManager,
            IUserStore<ApplicationUser>   userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel>        logger,
            IEmailSender                  emailSender,
            RoleManager<IdentityRole>     roleManager)
        {
            _userManager  = userManager;
            _userStore    = userStore;
            _emailStore   = GetEmailStore();
            _signInManager = signInManager;
            _logger       = logger;
            _emailSender  = emailSender;
            _roleManager  = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public List<SelectListItem> RolesDisponibles { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un rol.")]
            [Display(Name = "Rol")]
            public string Rol { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            RolesDisponibles = ObtenerRolesAutoasignables();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            RolesDisponibles = ObtenerRolesAutoasignables();

            // Blindaje: aunque alguien manipule el POST y mande "Admin" a mano,
            // el servidor lo rechaza igual (nunca confiar solo en el dropdown del cliente)
            if (string.Equals(Input.Rol, RolAdmin, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(Input.Rol), "No es posible autoasignarse el rol Admin.");
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Guarda el rol en la propiedad del ApplicationUser
                user.Rol = Input.Rol;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario creado con contraseña.");

                    // Asigna el rol en la tabla de Identity
                    await _userManager.AddToRoleAsync(user, Input.Rol);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private List<SelectListItem> ObtenerRolesAutoasignables()
        {
            return _roleManager.Roles
                .Where(r => r.Name != RolAdmin)
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"No se puede crear una instancia de '{nameof(ApplicationUser)}'.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("La UI requiere un store con soporte de email.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}