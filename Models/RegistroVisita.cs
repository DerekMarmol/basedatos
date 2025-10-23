namespace ARSAN_Web.Models
{
    public class RegistroVisita
    {
        public int IdRegistro { get; set; }
        public int IdVisitante { get; set; }
        public int IdCasa { get; set; } 
        public int IdResidencia { get; set; } 
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public int IdGuardiaIngreso { get; set; }
        public int? IdGuardiaSalida { get; set; }
        public string? Motivo { get; set; }
        public int? IdGarita { get; set; } 

        // Navegación
        public Visitante Visitante { get; set; } = null!;
        public Residencia Residencia { get; set; } = null!; 
        public Guardia GuardiaIngreso { get; set; } = null!;
        public Guardia? GuardiaSalida { get; set; }
        public Garita? Garita { get; set; } 
    }
}