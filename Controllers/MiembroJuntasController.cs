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
    public class MiembroJuntasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MiembroJuntasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MiembroJuntas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MiembrosJunta.Include(m => m.Cargo).Include(m => m.JuntaDirectiva).Include(m => m.Propietario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MiembroJuntas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miembroJunta = await _context.MiembrosJunta
                .Include(m => m.Cargo)
                .Include(m => m.JuntaDirectiva)
                .Include(m => m.Propietario)
                .FirstOrDefaultAsync(m => m.IdMiembroJunta == id);
            if (miembroJunta == null)
            {
                return NotFound();
            }

            return View(miembroJunta);
        }

        // GET: MiembroJuntas/Create
        public IActionResult Create()
        {
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Nombre");
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.JuntasDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva");
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "Dpi");
            return View();
        }

        // POST: MiembroJuntas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMiembroJunta,IdJuntaDirectiva,IdPropietario,IdCargo,FechaInicio,FechaFin")] MiembroJunta miembroJunta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(miembroJunta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Nombre", miembroJunta.IdCargo);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.JuntasDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", miembroJunta.IdJuntaDirectiva);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "Dpi", miembroJunta.IdPropietario);
            return View(miembroJunta);
        }

        // GET: MiembroJuntas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miembroJunta = await _context.MiembrosJunta.FindAsync(id);
            if (miembroJunta == null)
            {
                return NotFound();
            }
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Nombre", miembroJunta.IdCargo);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.JuntasDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", miembroJunta.IdJuntaDirectiva);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "Dpi", miembroJunta.IdPropietario);
            return View(miembroJunta);
        }

        // POST: MiembroJuntas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMiembroJunta,IdJuntaDirectiva,IdPropietario,IdCargo,FechaInicio,FechaFin")] MiembroJunta miembroJunta)
        {
            if (id != miembroJunta.IdMiembroJunta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(miembroJunta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MiembroJuntaExists(miembroJunta.IdMiembroJunta))
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
            ViewData["IdCargo"] = new SelectList(_context.Cargos, "IdCargo", "Nombre", miembroJunta.IdCargo);
            ViewData["IdJuntaDirectiva"] = new SelectList(_context.JuntasDirectivas, "IdJuntaDirectiva", "IdJuntaDirectiva", miembroJunta.IdJuntaDirectiva);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "Dpi", miembroJunta.IdPropietario);
            return View(miembroJunta);
        }

        // GET: MiembroJuntas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miembroJunta = await _context.MiembrosJunta
                .Include(m => m.Cargo)
                .Include(m => m.JuntaDirectiva)
                .Include(m => m.Propietario)
                .FirstOrDefaultAsync(m => m.IdMiembroJunta == id);
            if (miembroJunta == null)
            {
                return NotFound();
            }

            return View(miembroJunta);
        }

        // POST: MiembroJuntas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var miembroJunta = await _context.MiembrosJunta.FindAsync(id);
            if (miembroJunta != null)
            {
                _context.MiembrosJunta.Remove(miembroJunta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MiembroJuntaExists(int id)
        {
            return _context.MiembrosJunta.Any(e => e.IdMiembroJunta == id);
        }
    }
}
