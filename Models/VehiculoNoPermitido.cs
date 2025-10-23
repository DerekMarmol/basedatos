namespace ARSAN_Web.Models
{
    public class VehiculoNoPermitido
    {
        public int IdVNP { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; } = true;
        public int IdResidencial { get; set; } 

        // Navegación
        public Residencial Residencial { get; set; } = null!;
    }
}