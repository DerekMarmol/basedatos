namespace ARSAN_Web.Models
{
    public class Multa
    {
        public int IdMulta { get; set; }
        public int IdCasa { get; set; } 
        public int IdResidencia { get; set; } 
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public bool Pagada { get; set; } = false;
        public int? IdTipoMulta { get; set; } 

        // Navegación
        public Residencia Residencia { get; set; } = null!; 
        public TipoMulta? TipoMulta { get; set; }
        public ICollection<PagoMulta> PagosMulta { get; set; } = new List<PagoMulta>();
    }
}