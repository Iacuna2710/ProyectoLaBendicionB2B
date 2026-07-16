using Microsoft.AspNetCore.Identity.UI.Services;

namespace MacrobioticaLaBendicion.Services
{
    // El proyecto no tiene un servidor SMTP real configurado, así que en vez
    // de intentar enviar el correo (y fallar silenciosamente, como hace el
    // stub que trae Identity por defecto), este "sender" solo registra el
    // contenido en el log de la consola. Sirve para poder probar el flujo
    // de "olvidé mi contraseña" completo durante la demo/desarrollo:
    // el link de reseteo queda visible en la consola donde corre la app.
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogWarning(
                "\n──────── CORREO SIMULADO (no hay SMTP configurado) ────────\n" +
                "Para: {Email}\nAsunto: {Subject}\n{Mensaje}\n" +
                "─────────────────────────────────────────────────────────",
                email, subject, htmlMessage);

            return Task.CompletedTask;
        }
    }
}
