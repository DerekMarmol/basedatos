namespace ARSAN_Web.Models
{
    public class TurnoGuardia
    {
        public int IdTurno { get; set; }
        public int IdGarita { get; set; }
        public int IdGuardia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Observaciones { get; set; }

        // Navegación
        public Garita Garita { get; set; } = null!;
        public Guardia Guardia { get; set; } = null!;
    }
}