namespace ARSAN_Web.Models
{
    public class Cluster
    {
        public int IdCluster { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdResidencial { get; set; } 

        // Navegación
        public Residencial Residencial { get; set; } = null!; 
        public ICollection<Residencia> Residencias { get; set; } = new List<Residencia>();
        public ICollection<JuntaDirectiva> JuntasDirectivas { get; set; } = new List<JuntaDirectiva>();
        public ICollection<Garita> Garitas { get; set; } = new List<Garita>();
    }
}