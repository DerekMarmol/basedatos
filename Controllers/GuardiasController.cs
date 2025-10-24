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
    public class GuardiasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuardiasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Guardias
        public async Task<IActionResult> Index()
        {
            var guardias = await _context.Guardias
                .Include(g => g.Residencial)
                .OrderBy(g => g.Activo ? 0 : 1) // Activos primero
                .ThenBy(g => g.NombreCompleto)
                .ToListAsync();
            return View(guardias);
        }

        // GET: Guardias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guardia = await _context.Guardias
                .Include(g => g.Residencial)
                .FirstOrDefaultAsync(m => m.IdGuardia == id);
            if (guardia == null)
            {
                return NotFound();
            }

            return View(guardia);
        }

        // GET: Guardias/Create
        public IActionResult Create()
        {
            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre");
            return View();
        }

        // POST: Guardias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGuardia,NombreCompleto,Dpi,Telefono,FechaContratacion,Turno,Activo,IdResidencial,Genero")] Guardia guardia)
        {
            // Remover validación de navegación
            ModelState.Remove("Residencial");
            ModelState.Remove("Turnos");
            ModelState.Remove("AccesosIngreso");
            ModelState.Remove("AccesosSalida");

            // Validar si ya existe un guardia con ese DPI
            var guardiaExistente = await _context.Guardias
                .AnyAsync(g => g.Dpi == guardia.Dpi);

            if (guardiaExistente)
            {
                ModelState.AddModelError("Dpi", "Ya existe un guardia registrado con este DPI.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(guardia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", guardia.IdResidencial);
            return View(guardia);
        }

        // GET: Guardias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guardia = await _context.Guardias.FindAsync(id);
            if (guardia == null)
            {
                return NotFound();
            }

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", guardia.IdResidencial);
            return View(guardia);
        }

        // POST: Guardias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGuardia,NombreCompleto,Dpi,Telefono,FechaContratacion,Turno,Activo,IdResidencial,Genero")] Guardia guardia)
        {
            if (id != guardia.IdGuardia)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Residencial");
            ModelState.Remove("Turnos");
            ModelState.Remove("AccesosIngreso");
            ModelState.Remove("AccesosSalida");

            // Validar si ya existe otro guardia con ese DPI
            var guardiaExistente = await _context.Guardias
                .AnyAsync(g => g.Dpi == guardia.Dpi && g.IdGuardia != id);

            if (guardiaExistente)
            {
                ModelState.AddModelError("Dpi", "Ya existe otro guardia registrado con este DPI.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guardia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuardiaExists(guardia.IdGuardia))
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

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", guardia.IdResidencial);
            return View(guardia);
        }

        // GET: Guardias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guardia = await _context.Guardias
                .Include(g => g.Residencial)
                .FirstOrDefaultAsync(m => m.IdGuardia == id);
            if (guardia == null)
            {
                return NotFound();
            }

            return View(guardia);
        }

        // POST: Guardias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var guardia = await _context.Guardias.FindAsync(id);
            if (guardia != null)
            {
                _context.Guardias.Remove(guardia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuardiaExists(int id)
        {
            return _context.Guardias.Any(e => e.IdGuardia == id);
        }
    }
}