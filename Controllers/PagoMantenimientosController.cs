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
    public class PagoMantenimientosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagoMantenimientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PagoMantenimiento
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PagosMantenimiento.Include(p => p.Residencia);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PagoMantenimiento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagoMantenimiento = await _context.PagosMantenimiento
                .Include(p => p.Residencia)
                .FirstOrDefaultAsync(m => m.IdPago == id);
            if (pagoMantenimiento == null)
            {
                return NotFound();
            }

            return View(pagoMantenimiento);
        }

        // GET: PagoMantenimiento/Create
        public IActionResult Create()
        {
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado");
            return View();
        }

        // POST: PagoMantenimiento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPago,IdCasa,IdResidencia,FechaPago,Monto,FormaPago,Observaciones")] PagoMantenimiento pagoMantenimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pagoMantenimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", pagoMantenimiento.IdResidencia);
            return View(pagoMantenimiento);
        }

        // GET: PagoMantenimiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagoMantenimiento = await _context.PagosMantenimiento.FindAsync(id);
            if (pagoMantenimiento == null)
            {
                return NotFound();
            }
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", pagoMantenimiento.IdResidencia);
            return View(pagoMantenimiento);
        }

        // POST: PagoMantenimiento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPago,IdCasa,IdResidencia,FechaPago,Monto,FormaPago,Observaciones")] PagoMantenimiento pagoMantenimiento)
        {
            if (id != pagoMantenimiento.IdPago)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pagoMantenimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoMantenimientoExists(pagoMantenimiento.IdPago))
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
            ViewData["IdResidencia"] = new SelectList(_context.Residencias, "IdResidencia", "Estado", pagoMantenimiento.IdResidencia);
            return View(pagoMantenimiento);
        }

        // GET: PagoMantenimiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pagoMantenimiento = await _context.PagosMantenimiento
                .Include(p => p.Residencia)
                .FirstOrDefaultAsync(m => m.IdPago == id);
            if (pagoMantenimiento == null)
            {
                return NotFound();
            }

            return View(pagoMantenimiento);
        }

        // POST: PagoMantenimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pagoMantenimiento = await _context.PagosMantenimiento.FindAsync(id);
            if (pagoMantenimiento != null)
            {
                _context.PagosMantenimiento.Remove(pagoMantenimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoMantenimientoExists(int id)
        {
            return _context.PagosMantenimiento.Any(e => e.IdPago == id);
        }
    }
}
