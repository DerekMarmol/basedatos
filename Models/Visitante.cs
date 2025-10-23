namespace ARSAN_Web.Models
{
    public class Visitante
    {
        public int IdVisitante { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Dpi { get; set; }
        public string? Telefono { get; set; }
        public string? Placa { get; set; }
        public string? Empresa { get; set; }

        // Navegación
        public ICollection<RegistroVisita> RegistrosVisita { get; set; } = new List<RegistroVisita>();
        public ICollection<AccesoVehicular> Accesos { get; set; } = new List<AccesoVehicular>(); 
    }
}