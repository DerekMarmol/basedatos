namespace ARSAN_Web.Models
{
    public class Cargo
    {
        public int IdCargo { get; set; }
        public string Nombre { get; set; } = string.Empty;

        // Navegación
        public ICollection<MiembroJunta> MiembrosJunta { get; set; } = new List<MiembroJunta>();
    }
}