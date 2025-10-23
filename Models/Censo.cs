namespace ARSAN_Web.Models
{
    public class Censo
    {
        public int IdCenso { get; set; }
        public int IdResidencia { get; set; }
        public DateTime? FechaCenso { get; set; }
        public int NumeroHabitantes { get; set; }
        public string? Observaciones { get; set; }

        // Navegación
        public Residencia Residencia { get; set; } = null!;
    }
}