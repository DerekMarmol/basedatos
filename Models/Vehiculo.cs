using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ARSAN_Web.Models
{
    [Index(nameof(Placa), IsUnique = true)]
    [Index(nameof(NumeroTarjeta), IsUnique = true)]
    public class Vehiculo
    {
        [Display(Name = "ID")]
        public int IdVehiculo { get; set; }

        [Display(Name = "Residencia")]
        [Required(ErrorMessage = "Debe seleccionar una residencia")]
        public int IdResidencia { get; set; }

        [Display(Name = "Marca")]
        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres")]
        public string? Marca { get; set; }

        [Display(Name = "Línea")]
        [StringLength(50, ErrorMessage = "La línea no puede exceder los 50 caracteres")]
        public string? Linea { get; set; }

        [Display(Name = "Año")]
        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100")]
        public int? Anio { get; set; }

        [Display(Name = "Color")]
        [StringLength(30, ErrorMessage = "El color no puede exceder los 30 caracteres")]
        public string? Color { get; set; }

        [Display(Name = "Placa")]
        [Required(ErrorMessage = "La placa es requerida")]
        [StringLength(20, ErrorMessage = "La placa no puede exceder los 20 caracteres")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "La placa solo puede contener letras mayúsculas, números y guiones")]
        public string Placa { get; set; } = string.Empty;

        [Display(Name = "Número de Tarjeta")]
        [Required(ErrorMessage = "El número de tarjeta es requerido")]
        [StringLength(50, ErrorMessage = "El número de tarjeta no puede exceder los 50 caracteres")]
        public string NumeroTarjeta { get; set; } = string.Empty;

        // Navegación
        public Residencia Residencia { get; set; } = null!;
        public ICollection<AccesoVehicular> Accesos { get; set; } = new List<AccesoVehicular>();
    }
}