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
    public class RegistroVisitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistroVisitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RegistroVisita
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RegistrosVisita.Include(r => r.Garita).Include(r => r.GuardiaIngreso).Include(r => r.GuardiaSalida).Include(r => r.Residencia).Include(r => r.Visitante);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RegistroVisita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroVisita = await _context.RegistrosVisita
                .Include(r => r.Garita)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.GuardiaSalida)
                .Include(r => r.Residencia)
                .Include(r => r.Visitante)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);
            if (registroVisita == null)
            {
                return NotFound();
            }

            return View(registroVisita);
        }

        // GET: RegistroVisita/Create
        public IActionResult Create()
        {
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre");
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi");
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi");
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado");
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto");
            return View();
        }

        // POST: RegistroVisita/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRegistro,IdVisitante,IdCasa,IdResidencia,FechaIngreso,FechaSalida,IdGuardiaIngreso,IdGuardiaSalida,Motivo,IdGarita")] RegistroVisita registroVisita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registroVisita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", registroVisita.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", registroVisita.IdGuardiaIngreso);
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", registroVisita.IdGuardiaSalida);
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", registroVisita.IdResidencia);
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto", registroVisita.IdVisitante);
            return View(registroVisita);
        }

        // GET: RegistroVisita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroVisita = await _context.RegistrosVisita.FindAsync(id);
            if (registroVisita == null)
            {
                return NotFound();
            }
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", registroVisita.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", registroVisita.IdGuardiaIngreso);
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", registroVisita.IdGuardiaSalida);
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", registroVisita.IdResidencia);
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto", registroVisita.IdVisitante);
            return View(registroVisita);
        }

        // POST: RegistroVisita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRegistro,IdVisitante,IdCasa,IdResidencia,FechaIngreso,FechaSalida,IdGuardiaIngreso,IdGuardiaSalida,Motivo,IdGarita")] RegistroVisita registroVisita)
        {
            if (id != registroVisita.IdRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registroVisita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistroVisitaExists(registroVisita.IdRegistro))
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
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", registroVisita.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", registroVisita.IdGuardiaIngreso);
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", registroVisita.IdGuardiaSalida);
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", registroVisita.IdResidencia);
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto", registroVisita.IdVisitante);
            return View(registroVisita);
        }

        // GET: RegistroVisita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroVisita = await _context.RegistrosVisita
                .Include(r => r.Garita)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.GuardiaSalida)
                .Include(r => r.Residencia)
                .Include(r => r.Visitante)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);
            if (registroVisita == null)
            {
                return NotFound();
            }

            return View(registroVisita);
        }

        // POST: RegistroVisita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registroVisita = await _context.RegistrosVisita.FindAsync(id);
            if (registroVisita != null)
            {
                _context.RegistrosVisita.Remove(registroVisita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistroVisitaExists(int id)
        {
            return _context.RegistrosVisita.Any(e => e.IdRegistro == id);
        }
    }
}
