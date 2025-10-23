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
            return View(await _context.Guardias.ToListAsync());
        }

        // GET: Guardias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guardia = await _context.Guardias
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
            return View();
        }

        // POST: Guardias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGuardia,NombreCompleto,Dpi,Telefono,FechaContratacion,Turno,Activo")] Guardia guardia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(guardia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            return View(guardia);
        }

        // POST: Guardias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGuardia,NombreCompleto,Dpi,Telefono,FechaContratacion,Turno,Activo")] Guardia guardia)
        {
            if (id != guardia.IdGuardia)
            {
                return NotFound();
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
