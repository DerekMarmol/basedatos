namespace ARSAN_Web.Models
{
    public class TarjetaIntegracionPagos
    {
        public int IdMovimiento { get; set; }
        public int IdCasa { get; set; } 
        public int IdResidencia { get; set; } 
        public DateTime Fecha { get; set; }
        public string? TipoMovimiento { get; set; }
        public string? Referencia { get; set; }
        public decimal? Monto { get; set; }

        // Navegación
        public Residencia Residencia { get; set; } = null!; // NUEVA RELACIÓN
    }
}