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
    public class TurnoGuardiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TurnoGuardiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TurnoGuardia
        public async Task<IActionResult> Index()
        {
            var turnos = await _context.TurnosGuardia
                .Include(t => t.Garita)
                    .ThenInclude(g => g.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(t => t.Guardia)
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();

            return View(turnos);
        }

        // GET: TurnoGuardia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turnoGuardia = await _context.TurnosGuardia
                .Include(t => t.Garita)
                    .ThenInclude(g => g.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(t => t.Guardia)
                .FirstOrDefaultAsync(m => m.IdTurno == id);

            if (turnoGuardia == null)
            {
                return NotFound();
            }

            return View(turnoGuardia);
        }

        // GET: TurnoGuardia/Create
        public IActionResult Create()
        {
            // Obtener solo garitas activas
            var garitas = _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Where(g => g.Activa)
                .Select(g => new
                {
                    g.IdGarita,
                    Texto = g.Cluster.Residencial.Nombre + " - " + g.Cluster.Nombre + " - " + g.Nombre
                })
                .ToList();

            // Obtener solo guardias activos
            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " - DPI: " + g.Dpi
                })
                .ToList();

            ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Texto");
            ViewData["IdGuardia"] = new SelectList(guardias, "IdGuardia", "Texto");

            return View();
        }

        // POST: TurnoGuardia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTurno,IdGarita,IdGuardia,FechaInicio,FechaFin,Observaciones")] TurnoGuardia turnoGuardia)
        {
            // Remover validación de navegación
            ModelState.Remove("Garita");
            ModelState.Remove("Guardia");

            // Validación 1: Verificar que el guardia esté activo
            var guardia = await _context.Guardias.FindAsync(turnoGuardia.IdGuardia);
            if (guardia == null || !guardia.Activo)
            {
                ModelState.AddModelError("IdGuardia", "El guardia seleccionado no está activo o no existe.");
            }

            // Validación 2: Verificar que la garita esté activa
            var garita = await _context.Garitas.FindAsync(turnoGuardia.IdGarita);
            if (garita == null || !garita.Activa)
            {
                ModelState.AddModelError("IdGarita", "La garita seleccionada no está activa o no existe.");
            }

            // Validación 3: Si hay FechaFin, validar que no exceda 24 horas
            if (turnoGuardia.FechaFin.HasValue)
            {
                var duracion = turnoGuardia.FechaFin.Value - turnoGuardia.FechaInicio;
                if (duracion.TotalHours > 24)
                {
                    ModelState.AddModelError("FechaFin", "El turno no puede exceder 24 horas según disposiciones municipales.");
                }

                if (turnoGuardia.FechaFin.Value < turnoGuardia.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio.");
                }
            }

            // Validación 4: Verificar que el guardia no tenga otro turno en el mismo período
            var turnoTraslape = await _context.TurnosGuardia
                .Where(t => t.IdGuardia == turnoGuardia.IdGuardia && t.IdTurno != turnoGuardia.IdTurno)
                .Where(t =>
                    // Turno sin fin (aún activo) que inicia antes o durante el nuevo turno
                    (!t.FechaFin.HasValue && t.FechaInicio <= turnoGuardia.FechaInicio) ||
                    // Turno con fin que se traslapa
                    (t.FechaFin.HasValue &&
                        ((t.FechaInicio <= turnoGuardia.FechaInicio && t.FechaFin.Value >= turnoGuardia.FechaInicio) ||
                         (turnoGuardia.FechaFin.HasValue && t.FechaInicio <= turnoGuardia.FechaFin.Value && t.FechaFin.Value >= turnoGuardia.FechaInicio))))
                .AnyAsync();

            if (turnoTraslape)
            {
                ModelState.AddModelError("IdGuardia", "El guardia ya tiene un turno asignado en este período de tiempo.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(turnoGuardia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar datos para el dropdown
            var garitas = _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Where(g => g.Activa)
                .Select(g => new
                {
                    g.IdGarita,
                    Texto = g.Cluster.Residencial.Nombre + " - " + g.Cluster.Nombre + " - " + g.Nombre
                })
                .ToList();

            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " - DPI: " + g.Dpi
                })
                .ToList();

            ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Texto", turnoGuardia.IdGarita);
            ViewData["IdGuardia"] = new SelectList(guardias, "IdGuardia", "Texto", turnoGuardia.IdGuardia);

            return View(turnoGuardia);
        }

        // GET: TurnoGuardia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turnoGuardia = await _context.TurnosGuardia.FindAsync(id);
            if (turnoGuardia == null)
            {
                return NotFound();
            }

            var garitas = _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Where(g => g.Activa)
                .Select(g => new
                {
                    g.IdGarita,
                    Texto = g.Cluster.Residencial.Nombre + " - " + g.Cluster.Nombre + " - " + g.Nombre
                })
                .ToList();

            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " - DPI: " + g.Dpi
                })
                .ToList();

            ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Texto", turnoGuardia.IdGarita);
            ViewData["IdGuardia"] = new SelectList(guardias, "IdGuardia", "Texto", turnoGuardia.IdGuardia);

            return View(turnoGuardia);
        }

        // POST: TurnoGuardia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTurno,IdGarita,IdGuardia,FechaInicio,FechaFin,Observaciones")] TurnoGuardia turnoGuardia)
        {
            if (id != turnoGuardia.IdTurno)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Garita");
            ModelState.Remove("Guardia");

            // Validación 1: Si hay FechaFin, validar que no exceda 24 horas
            if (turnoGuardia.FechaFin.HasValue)
            {
                var duracion = turnoGuardia.FechaFin.Value - turnoGuardia.FechaInicio;
                if (duracion.TotalHours > 24)
                {
                    ModelState.AddModelError("FechaFin", "El turno no puede exceder 24 horas según disposiciones municipales.");
                }

                if (turnoGuardia.FechaFin.Value < turnoGuardia.FechaInicio)
                {
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio.");
                }
            }

            // Validación 2: Verificar que el guardia no tenga otro turno en el mismo período
            var turnoTraslape = await _context.TurnosGuardia
                .Where(t => t.IdGuardia == turnoGuardia.IdGuardia && t.IdTurno != id)
                .Where(t =>
                    (!t.FechaFin.HasValue && t.FechaInicio <= turnoGuardia.FechaInicio) ||
                    (t.FechaFin.HasValue &&
                        ((t.FechaInicio <= turnoGuardia.FechaInicio && t.FechaFin.Value >= turnoGuardia.FechaInicio) ||
                         (turnoGuardia.FechaFin.HasValue && t.FechaInicio <= turnoGuardia.FechaFin.Value && t.FechaFin.Value >= turnoGuardia.FechaInicio))))
                .AnyAsync();

            if (turnoTraslape)
            {
                ModelState.AddModelError("IdGuardia", "El guardia ya tiene un turno asignado en este período de tiempo.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turnoGuardia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoGuardiaExists(turnoGuardia.IdTurno))
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

            var garitas = _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Where(g => g.Activa)
                .Select(g => new
                {
                    g.IdGarita,
                    Texto = g.Cluster.Residencial.Nombre + " - " + g.Cluster.Nombre + " - " + g.Nombre
                })
                .ToList();

            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " - DPI: " + g.Dpi
                })
                .ToList();

            ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Texto", turnoGuardia.IdGarita);
            ViewData["IdGuardia"] = new SelectList(guardias, "IdGuardia", "Texto", turnoGuardia.IdGuardia);

            return View(turnoGuardia);
        }

        // GET: TurnoGuardia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turnoGuardia = await _context.TurnosGuardia
                .Include(t => t.Garita)
                    .ThenInclude(g => g.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(t => t.Guardia)
                .FirstOrDefaultAsync(m => m.IdTurno == id);

            if (turnoGuardia == null)
            {
                return NotFound();
            }

            return View(turnoGuardia);
        }

        // POST: TurnoGuardia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turnoGuardia = await _context.TurnosGuardia.FindAsync(id);
            if (turnoGuardia != null)
            {
                _context.TurnosGuardia.Remove(turnoGuardia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TurnoGuardiaExists(int id)
        {
            return _context.TurnosGuardia.Any(e => e.IdTurno == id);
        }
    }
}