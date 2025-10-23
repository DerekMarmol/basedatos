namespace ARSAN_Web.Models
{
    public class EstadoCuenta
    {
        public int IdEstadoCuenta { get; set; }
        public int IdCasa { get; set; } 
        public int IdResidencia { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public decimal? SaldoPendiente { get; set; }
        public decimal? TotalPagado { get; set; }

        // Navegación
        public Residencia Residencia { get; set; } = null!;
    }
}