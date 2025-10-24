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
    public class MultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Multas
        public async Task<IActionResult> Index(string filtro = "todas", string busqueda = "", string ordenar = "fecha_desc")
        {
            // Query base
            var query = _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Include(m => m.TipoMulta)
                .AsQueryable();

            // Aplicar filtro por estado de pago
            ViewData["FiltroActual"] = filtro;
            switch (filtro.ToLower())
            {
                case "pendientes":
                    query = query.Where(m => !m.Pagada);
                    break;
                case "pagadas":
                    query = query.Where(m => m.Pagada);
                    break;
                default: // "todas"
                    break;
            }

            // Aplicar búsqueda
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(m =>
                    m.Concepto.ToLower().Contains(busqueda) ||
                    m.Residencia.Numero.ToString().Contains(busqueda) ||
                    m.Residencia.Cluster.Nombre.ToLower().Contains(busqueda) ||
                    m.Residencia.Propietario.NombreCompleto.ToLower().Contains(busqueda)
                );
            }
            ViewData["BusquedaActual"] = busqueda;

            // Aplicar ordenamiento
            ViewData["OrdenActual"] = ordenar;
            query = ordenar switch
            {
                "fecha_asc" => query.OrderBy(m => m.Fecha),
                "fecha_desc" => query.OrderByDescending(m => m.Fecha),
                "monto_asc" => query.OrderBy(m => m.Monto),
                "monto_desc" => query.OrderByDescending(m => m.Monto),
                "casa_asc" => query.OrderBy(m => m.Residencia.Cluster.Nombre).ThenBy(m => m.Residencia.Numero),
                "casa_desc" => query.OrderByDescending(m => m.Residencia.Cluster.Nombre).ThenByDescending(m => m.Residencia.Numero),
                _ => query.OrderByDescending(m => m.Fecha)
            };

            var multas = await query.ToListAsync();

            // Calcular estadísticas
            ViewData["TotalMultas"] = multas.Count;
            ViewData["MultasPendientes"] = multas.Count(m => !m.Pagada);
            ViewData["MultasPagadas"] = multas.Count(m => m.Pagada);
            ViewData["MontoTotal"] = multas.Sum(m => m.Monto);
            ViewData["MontoPendiente"] = multas.Where(m => !m.Pagada).Sum(m => m.Monto);
            ViewData["MontoPagado"] = multas.Where(m => m.Pagada).Sum(m => m.Monto);

            return View(multas);
        }

        // GET: Multas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multa = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Include(m => m.TipoMulta)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (multa == null)
            {
                return NotFound();
            }

            return View(multa);
        }

        // GET: Multas/Create
        public IActionResult Create()
        {
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre");
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre");
            return View();
        }

        // POST: Multas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMulta,IdCluster,IdResidencia,Concepto,Monto,Fecha,Pagada,IdTipoMulta")] Multa multa)
        {
            // Remover validación de navegación
            ModelState.Remove("Residencia");
            ModelState.Remove("TipoMulta");

            if (ModelState.IsValid)
            {
                // Verificar que el cluster existe
                var clusterExiste = await _context.Clusters.AnyAsync(c => c.IdCluster == multa.IdCluster);
                if (!clusterExiste)
                {
                    ModelState.AddModelError("IdCluster", "El cluster especificado no existe.");
                    ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                    ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
                    return View(multa);
                }

                // Verificar que la residencia existe y pertenece al cluster
                var residencia = await _context.Residencias
                    .Include(r => r.Cluster)
                    .Include(r => r.Propietario)
                    .FirstOrDefaultAsync(r => r.IdResidencia == multa.IdResidencia && r.IdCluster == multa.IdCluster);

                if (residencia == null)
                {
                    ModelState.AddModelError("IdResidencia", "La residencia especificada no existe o no pertenece al cluster seleccionado.");
                    ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                    ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
                    return View(multa);
                }

                // Validar monto
                if (multa.Monto <= 0)
                {
                    ModelState.AddModelError("Monto", "El monto debe ser mayor a cero.");
                    ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                    ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
                    return View(multa);
                }

                _context.Add(multa);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Multa creada exitosamente para la casa #{residencia.Numero} del cluster {residencia.Cluster.Nombre}.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
            return View(multa);
        }

        // GET: Multas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multa = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (multa == null)
            {
                return NotFound();
            }

            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
            return View(multa);
        }

        // POST: Multas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMulta,IdCluster,IdResidencia,Concepto,Monto,Fecha,Pagada,IdTipoMulta")] Multa multa)
        {
            if (id != multa.IdMulta)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Residencia");
            ModelState.Remove("TipoMulta");

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el cluster existe
                    var clusterExiste = await _context.Clusters.AnyAsync(c => c.IdCluster == multa.IdCluster);
                    if (!clusterExiste)
                    {
                        ModelState.AddModelError("IdCluster", "El cluster especificado no existe.");
                        ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                        ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
                        return View(multa);
                    }

                    // Verificar que la residencia existe y pertenece al cluster
                    var residencia = await _context.Residencias
                        .Include(r => r.Cluster)
                        .Include(r => r.Propietario)
                        .FirstOrDefaultAsync(r => r.IdResidencia == multa.IdResidencia && r.IdCluster == multa.IdCluster);

                    if (residencia == null)
                    {
                        ModelState.AddModelError("IdResidencia", "La residencia especificada no existe o no pertenece al cluster seleccionado.");
                        ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                        ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
                        return View(multa);
                    }

                    // Validar monto
                    if (multa.Monto <= 0)
                    {
                        ModelState.AddModelError("Monto", "El monto debe ser mayor a cero.");
                        ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                        ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
                        return View(multa);
                    }

                    _context.Update(multa);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Multa actualizada exitosamente para la casa #{residencia.Numero} del cluster {residencia.Cluster.Nombre}.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MultaExists(multa.IdMulta))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta.Where(t => t.Activo), "IdTipoMulta", "Nombre", multa.IdTipoMulta);
            return View(multa);
        }

        // GET: Multas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multa = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Include(m => m.TipoMulta)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (multa == null)
            {
                return NotFound();
            }

            return View(multa);
        }

        // POST: Multas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var multa = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (multa != null)
            {
                var infoMulta = $"casa #{multa.Residencia.Numero} del cluster {multa.Residencia.Cluster.Nombre}";
                _context.Multas.Remove(multa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Multa eliminada exitosamente ({infoMulta}).";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para marcar multa como pagada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoPagada(int id)
        {
            var multa = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .FirstOrDefaultAsync(m => m.IdMulta == id);

            if (multa == null)
            {
                return NotFound();
            }

            multa.Pagada = true;
            _context.Update(multa);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Multa marcada como pagada para la casa #{multa.Residencia.Numero}.";
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para cargar residencias por cluster (para AJAX)
        [HttpGet]
        public async Task<JsonResult> ObtenerResidenciasPorCluster(int idCluster)
        {
            var residencias = await _context.Residencias
                .Where(r => r.IdCluster == idCluster)
                .Include(r => r.Propietario)
                .OrderBy(r => r.Numero)
                .Select(r => new
                {
                    id = r.IdResidencia,
                    numero = r.Numero,
                    propietario = r.Propietario.NombreCompleto
                })
                .ToListAsync();

            return Json(residencias);
        }

        private bool MultaExists(int id)
        {
            return _context.Multas.Any(e => e.IdMulta == id);
        }
    }
}