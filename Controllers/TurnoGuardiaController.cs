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
            var applicationDbContext = _context.TurnosGuardia.Include(t => t.Garita).Include(t => t.Guardia);
            return View(await applicationDbContext.ToListAsync());
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
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre");
            ViewData["IdGuardia"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi");
            return View();
        }

        // POST: TurnoGuardia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTurno,IdGarita,IdGuardia,FechaInicio,FechaFin,Observaciones")] TurnoGuardia turnoGuardia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(turnoGuardia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", turnoGuardia.IdGarita);
            ViewData["IdGuardia"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", turnoGuardia.IdGuardia);
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
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", turnoGuardia.IdGarita);
            ViewData["IdGuardia"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", turnoGuardia.IdGuardia);
            return View(turnoGuardia);
        }

        // POST: TurnoGuardia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTurno,IdGarita,IdGuardia,FechaInicio,FechaFin,Observaciones")] TurnoGuardia turnoGuardia)
        {
            if (id != turnoGuardia.IdTurno)
            {
                return NotFound();
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
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", turnoGuardia.IdGarita);
            ViewData["IdGuardia"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", turnoGuardia.IdGuardia);
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
