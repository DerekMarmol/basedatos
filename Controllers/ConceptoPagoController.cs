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
    public class ConceptoPagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConceptoPagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ConceptoPago
        public async Task<IActionResult> Index()
        {
            var conceptos = await _context.ConceptosPago
                .OrderBy(c => c.Activo ? 0 : 1) 
                .ThenBy(c => c.Codigo)
                .ToListAsync();

            var totalCuotaMensual = await _context.ConceptosPago
                .Where(c => c.Activo)
                .SumAsync(c => c.Monto);

            ViewBag.TotalCuotaMensual = totalCuotaMensual;

            return View(conceptos);
        }

        // GET: ConceptoPago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conceptoPago = await _context.ConceptosPago
                .FirstOrDefaultAsync(m => m.IdConceptoPago == id);

            if (conceptoPago == null)
            {
                return NotFound();
            }

            return View(conceptoPago);
        }

        // GET: ConceptoPago/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ConceptoPago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConceptoPago,Codigo,Nombre,Descripcion,Monto,Activo")] ConceptoPago conceptoPago)
        {
            var conceptoExistente = await _context.ConceptosPago
                .AnyAsync(c => c.Codigo == conceptoPago.Codigo);

            if (conceptoExistente)
            {
                ModelState.AddModelError("Codigo", "Ya existe un concepto de pago con este código.");
            }

            if (conceptoPago.Monto <= 0)
            {
                ModelState.AddModelError("Monto", "El monto debe ser mayor a 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(conceptoPago);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Concepto de pago creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(conceptoPago);
        }

        // GET: ConceptoPago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conceptoPago = await _context.ConceptosPago.FindAsync(id);
            if (conceptoPago == null)
            {
                return NotFound();
            }

            return View(conceptoPago);
        }

        // POST: ConceptoPago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConceptoPago,Codigo,Nombre,Descripcion,Monto,Activo")] ConceptoPago conceptoPago)
        {
            if (id != conceptoPago.IdConceptoPago)
            {
                return NotFound();
            }

            var conceptoExistente = await _context.ConceptosPago
                .AnyAsync(c => c.Codigo == conceptoPago.Codigo && c.IdConceptoPago != id);

            if (conceptoExistente)
            {
                ModelState.AddModelError("Codigo", "Ya existe otro concepto de pago con este código.");
            }

            if (conceptoPago.Monto <= 0)
            {
                ModelState.AddModelError("Monto", "El monto debe ser mayor a 0.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conceptoPago);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Concepto de pago actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConceptoPagoExists(conceptoPago.IdConceptoPago))
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

            return View(conceptoPago);
        }

        // GET: ConceptoPago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conceptoPago = await _context.ConceptosPago
                .FirstOrDefaultAsync(m => m.IdConceptoPago == id);

            if (conceptoPago == null)
            {
                return NotFound();
            }

            var tieneDetalles = await _context.DetallesRecibo
                .AnyAsync(d => d.IdConceptoPago == id);

            ViewBag.TieneDetalles = tieneDetalles;

            return View(conceptoPago);
        }

        // POST: ConceptoPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conceptoPago = await _context.ConceptosPago.FindAsync(id);
            if (conceptoPago != null)
            {
                _context.ConceptosPago.Remove(conceptoPago);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Concepto de pago eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ConceptoPagoExists(int id)
        {
            return _context.ConceptosPago.Any(e => e.IdConceptoPago == id);
        }
    }
}