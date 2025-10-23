namespace ARSAN_Web.Models
{
    public class Casa
    {
        public int IdCasa { get; set; }
        public string? Direccion { get; set; }
        public int? IdPropietario { get; set; }

        // Navegación
        public ICollection<PagoMantenimiento> Pagos { get; set; } = new List<PagoMantenimiento>();
        public ICollection<Multa> Multas { get; set; } = new List<Multa>();
        public ICollection<RegistroVisita> RegistrosVisita { get; set; } = new List<RegistroVisita>();
        public ICollection<EstadoCuenta> EstadosCuenta { get; set; } = new List<EstadoCuenta>();
    }
}