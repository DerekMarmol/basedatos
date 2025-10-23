namespace ARSAN_Web.Models
{
    public class Residencial
    {
        public int IdResidencial { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Municipio { get; set; }
        public string? Departamento { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int? CantidadViviendas { get; set; }
        public DateTime? FechaInicioAdministracion { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Cluster> Clusters { get; set; } = new List<Cluster>();
        public ICollection<Guardia> Guardias { get; set; } = new List<Guardia>();
        public ICollection<PersonaNoGrata> PersonasNoGratas { get; set; } = new List<PersonaNoGrata>();
        public ICollection<VehiculoNoPermitido> VehiculosNoPermitidos { get; set; } = new List<VehiculoNoPermitido>();
    }
}