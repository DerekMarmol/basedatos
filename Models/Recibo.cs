namespace ARSAN_Web.Models
{
    public class Recibo
    {
        public int IdRecibo { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int IdPago { get; set; }
        public decimal Total { get; set; }

        // Navegación
        public PagoMantenimiento Pago { get; set; } = null!;
        public ICollection<DetalleRecibo> Detalles { get; set; } = new List<DetalleRecibo>();
    }
}