namespace ARSAN_Web.Models
{
    public class ConceptoPago
    {
        public int IdConceptoPago { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Monto { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<DetalleRecibo> DetallesRecibo { get; set; } = new List<DetalleRecibo>();
    }
}