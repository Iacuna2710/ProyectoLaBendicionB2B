namespace Pedidos360.Models
{
    public class ErrorViewModel
    // clase modelo para el manejo de errores en la aplicación
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
