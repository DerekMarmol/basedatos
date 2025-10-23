namespace ARSAN_Web.Models
{
    public class Garita
    {
        public int IdGarita { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdCluster { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activa { get; set; } = true;

        // Navegación
        public Cluster? Cluster { get; set; }
        public ICollection<TurnoGuardia> Turnos { get; set; } = new List<TurnoGuardia>();
        public ICollection<AccesoVehicular> Accesos { get; set; } = new List<AccesoVehicular>();
        public ICollection<RegistroVisita> RegistrosVisita { get; set; } = new List<RegistroVisita>();
    }
}