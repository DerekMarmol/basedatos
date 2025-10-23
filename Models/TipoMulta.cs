namespace ARSAN_Web.Models
{
    public class TipoMulta
    {
        public int IdTipoMulta { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal MontoBase { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Multa> Multas { get; set; } = new List<Multa>();
    }
}