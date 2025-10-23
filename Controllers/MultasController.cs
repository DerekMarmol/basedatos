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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Multas.Include(m => m.Residencia).Include(m => m.TipoMulta);
            return View(await applicationDbContext.ToListAsync());
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
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado");
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo");
            return View();
        }

        // POST: Multas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMulta,IdCasa,IdResidencia,Concepto,Monto,Fecha,Pagada,IdTipoMulta")] Multa multa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(multa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", multa.IdResidencia);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
            return View(multa);
        }

        // GET: Multas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var multa = await _context.Multas.FindAsync(id);
            if (multa == null)
            {
                return NotFound();
            }
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", multa.IdResidencia);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
            return View(multa);
        }

        // POST: Multas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMulta,IdCasa,IdResidencia,Concepto,Monto,Fecha,Pagada,IdTipoMulta")] Multa multa)
        {
            if (id != multa.IdMulta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(multa);
                    await _context.SaveChangesAsync();
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
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", multa.IdResidencia);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
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
            var multa = await _context.Multas.FindAsync(id);
            if (multa != null)
            {
                _context.Multas.Remove(multa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MultaExists(int id)
        {
            return _context.Multas.Any(e => e.IdMulta == id);
        }
    }
}
