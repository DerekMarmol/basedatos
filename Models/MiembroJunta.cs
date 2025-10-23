namespace ARSAN_Web.Models
{
    public class MiembroJunta
    {
        public int IdMiembroJunta { get; set; }
        public int IdJuntaDirectiva { get; set; }
        public int IdPropietario { get; set; }
        public int IdCargo { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Navegación
        public JuntaDirectiva JuntaDirectiva { get; set; } = null!;
        public Propietario Propietario { get; set; } = null!;
        public Cargo Cargo { get; set; } = null!;
    }
}