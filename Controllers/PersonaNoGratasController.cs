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
    public class PersonaNoGratasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonaNoGratasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PersonaNoGratas
        public async Task<IActionResult> Index()
        {
            var personasNoGratas = await _context.PersonasNoGratas
                .Include(p => p.Residencial)
                .OrderByDescending(p => p.Activo)
                .ThenByDescending(p => p.FechaRegistro)
                .ToListAsync();
            return View(personasNoGratas);
        }

        // GET: PersonaNoGratas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personaNoGrata = await _context.PersonasNoGratas
                .Include(p => p.Residencial)
                .FirstOrDefaultAsync(m => m.IdPNG == id);
            if (personaNoGrata == null)
            {
                return NotFound();
            }

            return View(personaNoGrata);
        }

        // GET: PersonaNoGratas/Create
        public IActionResult Create()
        {
            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre");
            return View();
        }

        // POST: PersonaNoGratas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPNG,Dpi,NombreCompleto,Motivo,FechaRegistro,Activo,IdResidencial")] PersonaNoGrata personaNoGrata)
        {
            // Remover validación de navegación
            ModelState.Remove("Residencial");

            // Validar si ya existe una persona no grata ACTIVA con el mismo DPI
            var personaNoGrataExistente = await _context.PersonasNoGratas
                .Include(p => p.Residencial)
                .FirstOrDefaultAsync(p => p.Dpi == personaNoGrata.Dpi && p.Activo);

            if (personaNoGrataExistente != null)
            {
                ModelState.AddModelError("Dpi",
                    $"Esta persona ya está registrada como NO GRATA (activa) en el residencial '{personaNoGrataExistente.Residencial?.Nombre}'. " +
                    $"Si desea actualizar la información, edite el registro existente.");
            }

            if (ModelState.IsValid)
            {
                // Asegurar que se registre como activo
                personaNoGrata.Activo = true;

                _context.Add(personaNoGrata);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", personaNoGrata.IdResidencial);
            return View(personaNoGrata);
        }

        // GET: PersonaNoGratas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personaNoGrata = await _context.PersonasNoGratas.FindAsync(id);
            if (personaNoGrata == null)
            {
                return NotFound();
            }

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", personaNoGrata.IdResidencial);
            return View(personaNoGrata);
        }

        // POST: PersonaNoGratas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPNG,Dpi,NombreCompleto,Motivo,FechaRegistro,Activo,IdResidencial")] PersonaNoGrata personaNoGrata)
        {
            if (id != personaNoGrata.IdPNG)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Residencial");

            // Validar si ya existe otra persona no grata ACTIVA con el mismo DPI
            var personaNoGrataExistente = await _context.PersonasNoGratas
                .Include(p => p.Residencial)
                .FirstOrDefaultAsync(p => p.Dpi == personaNoGrata.Dpi && p.Activo && p.IdPNG != id);

            if (personaNoGrataExistente != null && personaNoGrata.Activo)
            {
                ModelState.AddModelError("Dpi",
                    $"Ya existe otra persona no grata activa con este DPI en el residencial '{personaNoGrataExistente.Residencial?.Nombre}'.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personaNoGrata);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaNoGrataExists(personaNoGrata.IdPNG))
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

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", personaNoGrata.IdResidencial);
            return View(personaNoGrata);
        }

        // GET: PersonaNoGratas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personaNoGrata = await _context.PersonasNoGratas
                .Include(p => p.Residencial)
                .FirstOrDefaultAsync(m => m.IdPNG == id);
            if (personaNoGrata == null)
            {
                return NotFound();
            }

            return View(personaNoGrata);
        }

        // POST: PersonaNoGratas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personaNoGrata = await _context.PersonasNoGratas.FindAsync(id);
            if (personaNoGrata != null)
            {
                _context.PersonasNoGratas.Remove(personaNoGrata);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaNoGrataExists(int id)
        {
            return _context.PersonasNoGratas.Any(e => e.IdPNG == id);
        }
    }
}