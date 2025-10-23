namespace ARSAN_Web.Models
{
    public class PagoMulta
    {
        public int IdPagoMulta { get; set; }
        public int IdMulta { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoPagado { get; set; }

        // Navegación
        public Multa Multa { get; set; } = null!;
    }
}