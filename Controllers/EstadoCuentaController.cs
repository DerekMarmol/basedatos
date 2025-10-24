using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARSAN_Web.Data;
using ARSAN_Web.Models;
using ARSAN_Web.ViewModels;

namespace ARSAN_Web.Controllers
{
    public class EstadoCuentaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EstadoCuentaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EstadoCuenta (Vista de búsqueda)
        public IActionResult Index()
        {
            ViewData["Clusters"] = new SelectList(_context.Clusters, "IdCluster", "Nombre");
            return View();
        }

        // GET: EstadoCuenta/Buscar - Buscar residencias por diferentes criterios
        [HttpGet]
        public async Task<JsonResult> BuscarResidencias(string criterio, string valor, int? idCluster = null)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return Json(new { success = false, message = "Debe ingresar un valor de búsqueda" });
            }

            List<object> residencias = new List<object>();

            switch (criterio.ToLower())
            {
                case "numero":
                    if (!idCluster.HasValue)
                    {
                        return Json(new { success = false, message = "Debe seleccionar un cluster" });
                    }

                    if (int.TryParse(valor, out int numeroCasa))
                    {
                        residencias = await _context.Residencias
                            .Include(r => r.Cluster)
                            .Include(r => r.Propietario)
                            .Where(r => r.Numero == numeroCasa && r.IdCluster == idCluster.Value)
                            .Select(r => new
                            {
                                idResidencia = r.IdResidencia,
                                numero = r.Numero,
                                cluster = r.Cluster.Nombre,
                                propietario = r.Propietario.NombreCompleto,
                                idPropietario = r.IdPropietario
                            })
                            .Cast<object>()
                            .ToListAsync();
                    }
                    break;

                case "propietario":
                    residencias = await _context.Residencias
                        .Include(r => r.Cluster)
                        .Include(r => r.Propietario)
                        .Where(r => r.Propietario.NombreCompleto.Contains(valor))
                        .Select(r => new
                        {
                            idResidencia = r.IdResidencia,
                            numero = r.Numero,
                            cluster = r.Cluster.Nombre,
                            propietario = r.Propietario.NombreCompleto,
                            idPropietario = r.IdPropietario
                        })
                        .Cast<object>()
                        .ToListAsync();
                    break;

                case "cluster":
                    if (int.TryParse(valor, out int idClusterBusqueda))
                    {
                        residencias = await _context.Residencias
                            .Include(r => r.Cluster)
                            .Include(r => r.Propietario)
                            .Where(r => r.IdCluster == idClusterBusqueda)
                            .Select(r => new
                            {
                                idResidencia = r.IdResidencia,
                                numero = r.Numero,
                                cluster = r.Cluster.Nombre,
                                propietario = r.Propietario.NombreCompleto,
                                idPropietario = r.IdPropietario
                            })
                            .Cast<object>()
                            .ToListAsync();
                    }
                    break;
            }

            if (residencias.Any())
            {
                return Json(new { success = true, data = residencias });
            }

            return Json(new { success = false, message = "No se encontraron residencias con ese criterio" });
        }

        // GET: EstadoCuenta/Ver/{idResidencia}?año=2024
        public async Task<IActionResult> Ver(int id, int? anio)
        {
            var anioConsulta = anio ?? DateTime.Now.Year;

            // Obtener la residencia con sus relaciones
            var residencia = await _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Propietario)
                .FirstOrDefaultAsync(r => r.IdResidencia == id);

            if (residencia == null)
            {
                TempData["ErrorMessage"] = "Residencia no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener todos los pagos de mantenimiento del año
            var pagosMantenimiento = await _context.PagosMantenimiento
                .Where(p => p.IdResidencia == id && p.FechaPago.Year == anioConsulta)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            // Obtener todas las multas (pendientes y pagadas) del año
            var multas = await _context.Multas
                .Include(m => m.TipoMulta)
                .Where(m => m.IdResidencia == id && m.Fecha.Year == anioConsulta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            // Calcular estadísticas
            var multasPendientes = multas.Where(m => !m.Pagada).ToList();
            var multasPagadas = multas.Where(m => m.Pagada).ToList();

            var totalPagosMantenimiento = pagosMantenimiento.Sum(p => p.Monto);
            var totalMultasPendientes = multasPendientes.Sum(m => m.Monto);
            var totalMultasPagadas = multasPagadas.Sum(m => m.Monto);

            // Calcular meses que debe (asumiendo cuota mensual)
            // Por defecto, asumimos que debe pagar todos los meses del año hasta el mes actual
            var mesActual = DateTime.Now.Month;
            var mesesDeberíaPagar = anioConsulta == DateTime.Now.Year ? mesActual : 12;
            var mesesPagados = pagosMantenimiento.Count;
            var mesesDebe = Math.Max(0, mesesDeberíaPagar - mesesPagados);

            // Obtener el monto de cuota mensual del concepto de pago
            var conceptoPago = await _context.ConceptosPago
                .FirstOrDefaultAsync(c => (c.Codigo == "MANT" || c.Nombre.Contains("Mantenimiento")) && c.Activo);
            var cuotaMensual = conceptoPago?.Monto ?? 0;

            var montoDebeMantenimiento = mesesDebe * cuotaMensual;

            // Calcular saldo total pendiente
            var saldoTotalPendiente = montoDebeMantenimiento + totalMultasPendientes;

            // Determinar si está moroso (más de 10 días sin pagar desde el inicio del mes)
            var ultimoPago = pagosMantenimiento.OrderByDescending(p => p.FechaPago).FirstOrDefault();
            var diasSinPagar = 0;
            var estaMoroso = false;

            if (ultimoPago != null)
            {
                // Calcular días desde el último pago
                diasSinPagar = (DateTime.Now - ultimoPago.FechaPago).Days;

                // Si han pasado más de 40 días (más de un mes + 10 días de gracia), está moroso
                estaMoroso = diasSinPagar > 40;
            }
            else
            {
                // Si nunca ha pagado y ya pasaron 10 días del primer mes del año
                if (DateTime.Now.Year == anioConsulta && DateTime.Now.Day > 10)
                {
                    estaMoroso = true;
                    diasSinPagar = DateTime.Now.Day + ((DateTime.Now.Month - 1) * 30);
                }
            }

            // Calcular estado: "Al día" o "Moroso"
            var estado = estaMoroso ? "Moroso" : (saldoTotalPendiente > 0 ? "Pendiente" : "Al día");

            // Crear el ViewModel
            var viewModel = new EstadoCuentaViewModel
            {
                // Información de la residencia
                Residencia = residencia,

                // Pagos y multas
                PagosMantenimiento = pagosMantenimiento,
                MultasPendientes = multasPendientes,
                MultasPagadas = multasPagadas,

                // Estadísticas financieras
                TotalPagosMantenimiento = totalPagosMantenimiento,
                TotalMultasPendientes = totalMultasPendientes,
                TotalMultasPagadas = totalMultasPagadas,
                CuotaMensual = cuotaMensual,

                // Información de morosidad
                MesesDeberíaPagar = mesesDeberíaPagar,
                MesesPagados = mesesPagados,
                MesesDebe = mesesDebe,
                MontoDebeMantenimiento = montoDebeMantenimiento,
                SaldoTotalPendiente = saldoTotalPendiente,

                // Estado
                EstaMoroso = estaMoroso,
                DiaSinPagar = diasSinPagar,
                Estado = estado,
                UltimoPago = ultimoPago,

                // Año de consulta
                AnioConsulta = anioConsulta
            };

            return View(viewModel);
        }

        // GET: EstadoCuenta/VerPorPropietario/{idPropietario}?año=2024
        public async Task<IActionResult> VerPorPropietario(int id, int? anio)
        {
            var anioConsulta = anio ?? DateTime.Now.Year;

            // Obtener el propietario
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null)
            {
                TempData["ErrorMessage"] = "Propietario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Obtener todas las residencias del propietario
            var residencias = await _context.Residencias
                .Include(r => r.Cluster)
                .Where(r => r.IdPropietario == id)
                .ToListAsync();

            if (!residencias.Any())
            {
                TempData["ErrorMessage"] = "El propietario no tiene residencias registradas.";
                return RedirectToAction(nameof(Index));
            }

            var estadosCuenta = new List<EstadoCuentaViewModel>();

            // Generar estado de cuenta para cada residencia
            foreach (var residencia in residencias)
            {
                var pagosMantenimiento = await _context.PagosMantenimiento
                    .Where(p => p.IdResidencia == residencia.IdResidencia && p.FechaPago.Year == anioConsulta)
                    .OrderByDescending(p => p.FechaPago)
                    .ToListAsync();

                var multas = await _context.Multas
                    .Include(m => m.TipoMulta)
                    .Where(m => m.IdResidencia == residencia.IdResidencia && m.Fecha.Year == anioConsulta)
                    .OrderByDescending(m => m.Fecha)
                    .ToListAsync();

                var multasPendientes = multas.Where(m => !m.Pagada).ToList();
                var multasPagadas = multas.Where(m => m.Pagada).ToList();

                var totalPagosMantenimiento = pagosMantenimiento.Sum(p => p.Monto);
                var totalMultasPendientes = multasPendientes.Sum(m => m.Monto);
                var totalMultasPagadas = multasPagadas.Sum(m => m.Monto);

                var mesActual = DateTime.Now.Month;
                var mesesDeberíaPagar = anioConsulta == DateTime.Now.Year ? mesActual : 12;
                var mesesPagados = pagosMantenimiento.Count;
                var mesesDebe = Math.Max(0, mesesDeberíaPagar - mesesPagados);

                var conceptoPago = await _context.ConceptosPago
                    .FirstOrDefaultAsync(c => (c.Codigo == "MANT" || c.Nombre.Contains("Mantenimiento")) && c.Activo);
                var cuotaMensual = conceptoPago?.Monto ?? 0;

                var montoDebeMantenimiento = mesesDebe * cuotaMensual;
                var saldoTotalPendiente = montoDebeMantenimiento + totalMultasPendientes;

                var ultimoPago = pagosMantenimiento.OrderByDescending(p => p.FechaPago).FirstOrDefault();
                var diasSinPagar = 0;
                var estaMoroso = false;

                if (ultimoPago != null)
                {
                    diasSinPagar = (DateTime.Now - ultimoPago.FechaPago).Days;
                    estaMoroso = diasSinPagar > 40;
                }
                else if (DateTime.Now.Year == anioConsulta && DateTime.Now.Day > 10)
                {
                    estaMoroso = true;
                    diasSinPagar = DateTime.Now.Day + ((DateTime.Now.Month - 1) * 30);
                }

                var estado = estaMoroso ? "Moroso" : (saldoTotalPendiente > 0 ? "Pendiente" : "Al día");

                estadosCuenta.Add(new EstadoCuentaViewModel
                {
                    Residencia = residencia,
                    PagosMantenimiento = pagosMantenimiento,
                    MultasPendientes = multasPendientes,
                    MultasPagadas = multasPagadas,
                    TotalPagosMantenimiento = totalPagosMantenimiento,
                    TotalMultasPendientes = totalMultasPendientes,
                    TotalMultasPagadas = totalMultasPagadas,
                    CuotaMensual = cuotaMensual,
                    MesesDeberíaPagar = mesesDeberíaPagar,
                    MesesPagados = mesesPagados,
                    MesesDebe = mesesDebe,
                    MontoDebeMantenimiento = montoDebeMantenimiento,
                    SaldoTotalPendiente = saldoTotalPendiente,
                    EstaMoroso = estaMoroso,
                    DiaSinPagar = diasSinPagar,
                    Estado = estado,
                    UltimoPago = ultimoPago,
                    AnioConsulta = anioConsulta
                });
            }

            ViewData["Propietario"] = propietario;
            ViewData["AnioConsulta"] = anioConsulta;

            return View("VerPorPropietario", estadosCuenta);
        }
    }
}