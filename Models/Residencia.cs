namespace ARSAN_Web.Models
{
    public class Residencia
    {
        public int IdResidencia { get; set; }
        public int Numero { get; set; }
        public int IdCluster { get; set; }
        public int IdPropietario { get; set; }
        public int? IdInquilino { get; set; }
        public string Estado { get; set; } = "ocupada";

        // Navegación
        public Cluster Cluster { get; set; } = null!;
        public Propietario Propietario { get; set; } = null!;
        public Inquilino? Inquilino { get; set; }
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public ICollection<Censo> Censos { get; set; } = new List<Censo>();
    }
}