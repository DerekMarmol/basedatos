namespace ARSAN_Web.Models
{
    public class Propietario
    {
        public int IdPropietario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dpi { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? EstadoCivil { get; set; }
        public string? TipoLicencia { get; set; }

        // Navegación
        public ICollection<Residencia> Residencias { get; set; } = new List<Residencia>();
        public ICollection<MiembroJunta> MiembrosJunta { get; set; } = new List<MiembroJunta>();
    }
}