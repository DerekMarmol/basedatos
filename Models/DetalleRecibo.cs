namespace ARSAN_Web.Models
{
    public class DetalleRecibo
    {
        public int IdDetalleRecibo { get; set; }
        public int IdRecibo { get; set; }
        public int IdConceptoPago { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal MontoUnitario { get; set; }
        public decimal Subtotal { get; set; }

        // Navegación
        public Recibo Recibo { get; set; } = null!;
        public ConceptoPago ConceptoPago { get; set; } = null!;
    }
}