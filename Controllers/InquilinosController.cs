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
    public class InquilinosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InquilinosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inquilinos
        public async Task<IActionResult> Index()
        {
            var inquilinos = await _context.Inquilinos.ToListAsync();

            // Obtener lista de DPIs de personas no gratas activas
            var dpisNoGratos = await _context.PersonasNoGratas
                .Where(p => p.Activo)
                .Select(p => p.Dpi)
                .ToListAsync();

            ViewBag.DpisNoGratos = dpisNoGratos;

            return View(inquilinos);
        }

        // GET: Inquilinos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquilino = await _context.Inquilinos
                .FirstOrDefaultAsync(m => m.IdInquilino == id);
            if (inquilino == null)
            {
                return NotFound();
            }

            // Verificar si es persona no grata
            var esNoGrato = await _context.PersonasNoGratas
                .AnyAsync(p => p.Dpi == inquilino.Dpi && p.Activo);

            ViewBag.EsNoGrato = esNoGrato;

            return View(inquilino);
        }

        // GET: Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdInquilino,NombreCompleto,Dpi,Telefono,FechaNacimiento,EstadoCivil,TipoLicencia")] Inquilino inquilino)
        {
            // Remover validación de navegación
            ModelState.Remove("Residencias");

            // Verificar si es persona no grata
            var esNoGrato = await _context.PersonasNoGratas
                .AnyAsync(p => p.Dpi == inquilino.Dpi && p.Activo);

            if (esNoGrato)
            {
                ModelState.AddModelError("Dpi", "⚠️ ADVERTENCIA: Esta persona está registrada como NO GRATA en el sistema.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(inquilino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        // GET: Inquilinos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquilino = await _context.Inquilinos.FindAsync(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST: Inquilinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInquilino,NombreCompleto,Dpi,Telefono,FechaNacimiento,EstadoCivil,TipoLicencia")] Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Residencias");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inquilino);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InquilinoExists(inquilino.IdInquilino))
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
            return View(inquilino);
        }

        // GET: Inquilinos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquilino = await _context.Inquilinos
                .FirstOrDefaultAsync(m => m.IdInquilino == id);
            if (inquilino == null)
            {
                return NotFound();
            }

            return View(inquilino);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inquilino = await _context.Inquilinos.FindAsync(id);
            if (inquilino != null)
            {
                _context.Inquilinos.Remove(inquilino);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InquilinoExists(int id)
        {
            return _context.Inquilinos.Any(e => e.IdInquilino == id);
        }
    }
}