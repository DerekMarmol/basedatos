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
    public class VehiculoNoPermitidosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiculoNoPermitidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VehiculoNoPermitidoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.VehiculosNoPermitidos.ToListAsync());
        }

        // GET: VehiculoNoPermitidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos
                .FirstOrDefaultAsync(m => m.IdVNP == id);
            if (vehiculoNoPermitido == null)
            {
                return NotFound();
            }

            return View(vehiculoNoPermitido);
        }

        // GET: VehiculoNoPermitidoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehiculoNoPermitidoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVNP,Placa,Motivo,FechaRegistro,Activo")] VehiculoNoPermitido vehiculoNoPermitido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehiculoNoPermitido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehiculoNoPermitido);
        }

        // GET: VehiculoNoPermitidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos.FindAsync(id);
            if (vehiculoNoPermitido == null)
            {
                return NotFound();
            }
            return View(vehiculoNoPermitido);
        }

        // POST: VehiculoNoPermitidoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVNP,Placa,Motivo,FechaRegistro,Activo")] VehiculoNoPermitido vehiculoNoPermitido)
        {
            if (id != vehiculoNoPermitido.IdVNP)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehiculoNoPermitido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoNoPermitidoExists(vehiculoNoPermitido.IdVNP))
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
            return View(vehiculoNoPermitido);
        }

        // GET: VehiculoNoPermitidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos
                .FirstOrDefaultAsync(m => m.IdVNP == id);
            if (vehiculoNoPermitido == null)
            {
                return NotFound();
            }

            return View(vehiculoNoPermitido);
        }

        // POST: VehiculoNoPermitidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos.FindAsync(id);
            if (vehiculoNoPermitido != null)
            {
                _context.VehiculosNoPermitidos.Remove(vehiculoNoPermitido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoNoPermitidoExists(int id)
        {
            return _context.VehiculosNoPermitidos.Any(e => e.IdVNP == id);
        }
    }
}
