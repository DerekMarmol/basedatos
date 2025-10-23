namespace ARSAN_Web.Models
{
    public class PersonaNoGrata
    {
        public int IdPNG { get; set; }
        public string? Dpi { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;
        public int IdResidencial { get; set; } 

        // Navegación
        public Residencial Residencial { get; set; } = null!; 
    }
}