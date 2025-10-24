using ARSAN_Web.Models;
using System;
using System.Collections.Generic;

namespace ARSAN_Web.ViewModels
{
    // ViewModel para el Estado de Cuenta
    public class EstadoCuentaViewModel
    {
        public Residencia Residencia { get; set; } = null!;

        // Listas de pagos y multas
        public List<PagoMantenimiento> PagosMantenimiento { get; set; } = new List<PagoMantenimiento>();
        public List<Multa> MultasPendientes { get; set; } = new List<Multa>();
        public List<Multa> MultasPagadas { get; set; } = new List<Multa>();

        // Totales
        public decimal TotalPagosMantenimiento { get; set; }
        public decimal TotalMultasPendientes { get; set; }
        public decimal TotalMultasPagadas { get; set; }
        public decimal CuotaMensual { get; set; }

        // Información de meses
        public int MesesDeberíaPagar { get; set; }
        public int MesesPagados { get; set; }
        public int MesesDebe { get; set; }

        // Saldos
        public decimal MontoDebeMantenimiento { get; set; }
        public decimal SaldoTotalPendiente { get; set; }

        // Estado
        public bool EstaMoroso { get; set; }
        public int DiaSinPagar { get; set; }
        public string Estado { get; set; } = string.Empty;
        public PagoMantenimiento? UltimoPago { get; set; }

        // Año de consulta
        public int AnioConsulta { get; set; }
    }
}