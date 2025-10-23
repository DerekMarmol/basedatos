namespace ARSAN_Web.Models
{
    public class Guardia
    {
        public int IdGuardia { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dpi { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public DateTime? FechaContratacion { get; set; }
        public string? Turno { get; set; }
        public bool Activo { get; set; } = true;
        public int IdResidencial { get; set; }
        public string? Genero { get; set; } 

        // Navegación
        public Residencial Residencial { get; set; } = null!; 
        public ICollection<RegistroVisita> RegistrosIngreso { get; set; } = new List<RegistroVisita>();
        public ICollection<RegistroVisita> RegistrosSalida { get; set; } = new List<RegistroVisita>();
        public ICollection<TurnoGuardia> Turnos { get; set; } = new List<TurnoGuardia>();
        public ICollection<AccesoVehicular> AccesosIngreso { get; set; } = new List<AccesoVehicular>();
        public ICollection<AccesoVehicular> AccesosSalida { get; set; } = new List<AccesoVehicular>();
    }
}