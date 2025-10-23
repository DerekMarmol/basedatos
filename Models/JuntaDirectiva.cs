namespace ARSAN_Web.Models
{
    public class JuntaDirectiva
    {
        public int IdJuntaDirectiva { get; set; }
        public int IdCluster { get; set; }
        public int AnioInicio { get; set; }
        public int? AnioFin { get; set; }

        // Navegación
        public Cluster Cluster { get; set; } = null!;
        public ICollection<MiembroJunta> Miembros { get; set; } = new List<MiembroJunta>();
    }
}