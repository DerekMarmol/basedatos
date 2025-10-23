namespace ARSAN_Web.Models
{
    public class AccesoVehicular
    {
        public int IdAcceso { get; set; }
        public int? IdVehiculo { get; set; }
        public int? IdVisitante { get; set; }
        public string Placa { get; set; } = string.Empty;
        public int IdGarita { get; set; }
        public DateTime FechaIngreso { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public int IdGuardiaIngreso { get; set; }
        public int? IdGuardiaSalida { get; set; }
        public string TipoAcceso { get; set; } = "Residente"; // 'Residente' o 'Visitante'
        public string? Observaciones { get; set; }

        // Navegación
        public Vehiculo? Vehiculo { get; set; }
        public Visitante? Visitante { get; set; }
        public Garita Garita { get; set; } = null!;
        public Guardia GuardiaIngreso { get; set; } = null!;
        public Guardia? GuardiaSalida { get; set; }
    }
}
