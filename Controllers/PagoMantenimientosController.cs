using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARSAN_Web.Data;
using ARSAN_Web.Models;

namespace ARSAN_Web.Controllers
{
    public class PagoMantenimientosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagoMantenimientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PagoMantenimientos
        public async Task<IActionResult> Index(string filtro = "todos")
        {
            var query = _context.PagosMantenimiento
                .Include(p => p.Residencia)
                    .ThenInclude(r => r.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(p => p.Residencia.Propietario)
                .Include(p => p.Recibos)
                .AsQueryable();

            // Aplicar filtros
            var hoy = DateTime.Today;
            switch (filtro.ToLower())
            {
                case "mes-actual":
                    query = query.Where(p => p.FechaPago.Year == hoy.Year && p.FechaPago.Month == hoy.Month);
                    break;
                case "mes-anterior":
                    var mesAnterior = hoy.AddMonths(-1);
                    query = query.Where(p => p.FechaPago.Year == mesAnterior.Year && p.FechaPago.Month == mesAnterior.Month);
                    break;
            }

            var pagos = await query
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            ViewBag.FiltroActual = filtro;
            ViewBag.TotalMesActual = await _context.PagosMantenimiento
                .Where(p => p.FechaPago.Year == hoy.Year && p.FechaPago.Month == hoy.Month)
                .SumAsync(p => p.Monto);

            return View(pagos);
        }

        // GET: PagoMantenimientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagoMantenimiento = await _context.PagosMantenimiento
                .Include(p => p.Residencia)
                    .ThenInclude(r => r.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(p => p.Residencia.Propietario)
                .Include(p => p.Recibos)
                    .ThenInclude(r => r.Detalles)
                        .ThenInclude(d => d.ConceptoPago)
                .FirstOrDefaultAsync(m => m.IdPago == id);

            if (pagoMantenimiento == null)
            {
                return NotFound();
            }

            return View(pagoMantenimiento);
        }

        // GET: PagoMantenimientos/Create
        public async Task<IActionResult> Create()
        {
            // Obtener residencias con información completa
            var residencias = await _context.Residencias
                .Include(r => r.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Include(r => r.Propietario)
                .Select(r => new
                {
                    r.IdResidencia,
                    Texto = r.Cluster.Residencial.Nombre + " - " + r.Cluster.Nombre + " - Casa #" + r.Numero + " (" + r.Propietario.NombreCompleto + ")"
                })
                .OrderBy(r => r.Texto)
                .ToListAsync();

            ViewData["IdResidencia"] = new SelectList(residencias, "IdResidencia", "Texto");

            // Obtener conceptos activos
            var conceptosActivos = await _context.ConceptosPago
                .Where(c => c.Activo)
                .ToListAsync();

            ViewBag.ConceptosActivos = conceptosActivos;
            ViewBag.TotalCuota = conceptosActivos.Sum(c => c.Monto);

            return View();
        }

        // POST: PagoMantenimientos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int IdResidencia,
            DateTime FechaPago,
            decimal Monto,
            string FormaPago,
            string Observaciones,
            int MesesPagados = 1)
        {
            // Validaciones
            if (Monto <= 0)
            {
                ModelState.AddModelError("Monto", "El monto debe ser mayor a 0.");
            }

            if (string.IsNullOrEmpty(FormaPago))
            {
                ModelState.AddModelError("FormaPago", "Debe seleccionar una forma de pago.");
            }

            if (ModelState.IsValid)
            {
                // 1. Crear el Pago de Mantenimiento
                var pago = new PagoMantenimiento
                {
                    IdResidencia = IdResidencia,
                    FechaPago = FechaPago,
                    Monto = Monto,
                    FormaPago = FormaPago,
                    Observaciones = Observaciones
                };

                _context.PagosMantenimiento.Add(pago);
                await _context.SaveChangesAsync();

                // 2. Generar número de recibo único
                var ultimoRecibo = await _context.Recibos
                    .OrderByDescending(r => r.IdRecibo)
                    .FirstOrDefaultAsync();

                var numeroRecibo = GenerarNumeroRecibo(ultimoRecibo);

                // 3. Crear el Recibo
                var recibo = new Recibo
                {
                    NumeroRecibo = numeroRecibo,
                    Fecha = FechaPago,
                    IdPago = pago.IdPago,
                    Total = Monto
                };

                _context.Recibos.Add(recibo);
                await _context.SaveChangesAsync();

                // 4. Crear los Detalles del Recibo con conceptos activos
                var conceptosActivos = await _context.ConceptosPago
                    .Where(c => c.Activo)
                    .ToListAsync();

                foreach (var concepto in conceptosActivos)
                {
                    var detalle = new DetalleRecibo
                    {
                        IdRecibo = recibo.IdRecibo,
                        IdConceptoPago = concepto.IdConceptoPago,
                        Cantidad = MesesPagados,
                        MontoUnitario = concepto.Monto,
                        Subtotal = concepto.Monto * MesesPagados
                    };

                    _context.DetallesRecibo.Add(detalle);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Pago registrado exitosamente. Recibo #{numeroRecibo}";
                return RedirectToAction(nameof(Details), new { id = pago.IdPago });
            }

            // Recargar datos si hay error
            var residencias = await _context.Residencias
                .Include(r => r.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Include(r => r.Propietario)
                .Select(r => new
                {
                    r.IdResidencia,
                    Texto = r.Cluster.Residencial.Nombre + " - " + r.Cluster.Nombre + " - Casa #" + r.Numero + " (" + r.Propietario.NombreCompleto + ")"
                })
                .OrderBy(r => r.Texto)
                .ToListAsync();

            ViewData["IdResidencia"] = new SelectList(residencias, "IdResidencia", "Texto", IdResidencia);

            var conceptosActivosReload = await _context.ConceptosPago
                .Where(c => c.Activo)
                .ToListAsync();

            ViewBag.ConceptosActivos = conceptosActivosReload;
            ViewBag.TotalCuota = conceptosActivosReload.Sum(c => c.Monto);

            return View();
        }

        // GET: PagoMantenimientos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagoMantenimiento = await _context.PagosMantenimiento
                .Include(p => p.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Include(p => p.Residencia.Propietario)
                .Include(p => p.Recibos)
                .FirstOrDefaultAsync(m => m.IdPago == id);

            if (pagoMantenimiento == null)
            {
                return NotFound();
            }

            return View(pagoMantenimiento);
        }

        // POST: PagoMantenimientos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pagoMantenimiento = await _context.PagosMantenimiento
                .Include(p => p.Recibos)
                    .ThenInclude(r => r.Detalles)
                .FirstOrDefaultAsync(p => p.IdPago == id);

            if (pagoMantenimiento != null)
            {
                // Eliminar detalles de recibos
                foreach (var recibo in pagoMantenimiento.Recibos)
                {
                    _context.DetallesRecibo.RemoveRange(recibo.Detalles);
                }

                // Eliminar recibos
                _context.Recibos.RemoveRange(pagoMantenimiento.Recibos);

                // Eliminar pago
                _context.PagosMantenimiento.Remove(pagoMantenimiento);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Pago eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para generar número de recibo
        private string GenerarNumeroRecibo(Recibo ultimoRecibo)
        {
            if (ultimoRecibo == null)
            {
                return $"R-{DateTime.Now.Year}-00001";
            }

            // Extraer el número del último recibo
            var partes = ultimoRecibo.NumeroRecibo.Split('-');
            if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
            {
                var nuevoNumero = numero + 1;
                return $"R-{DateTime.Now.Year}-{nuevoNumero:D5}";
            }

            return $"R-{DateTime.Now.Year}-00001";
        }

        private bool PagoMantenimientoExists(int id)
        {
            return _context.PagosMantenimiento.Any(e => e.IdPago == id);
        }
    }
}