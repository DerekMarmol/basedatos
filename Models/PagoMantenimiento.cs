namespace ARSAN_Web.Models
{
    public class PagoMantenimiento
    {
        public int IdPago { get; set; }
        public int IdCasa { get; set; } 
        public int IdResidencia { get; set; } 
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? FormaPago { get; set; }
        public string? Observaciones { get; set; }

        // Navegación
        public Residencia Residencia { get; set; } = null!; 
        public ICollection<Recibo> Recibos { get; set; } = new List<Recibo>();
    }
}