namespace ARSAN_Web.Models
{
    public class Vehiculo
    {
        public int IdVehiculo { get; set; }
        public int IdResidencia { get; set; }
        public string? Marca { get; set; }
        public string? Linea { get; set; }
        public int? Anio { get; set; }
        public string? Color { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string NumeroTarjeta { get; set; } = string.Empty;

        // Navegación
        public Residencia Residencia { get; set; } = null!;
        public ICollection<AccesoVehicular> Accesos { get; set; } = new List<AccesoVehicular>();
    }
}