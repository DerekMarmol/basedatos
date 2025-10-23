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
    public class TipoMultaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoMultaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoMulta
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposMulta.ToListAsync());
        }

        // GET: TipoMulta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMulta = await _context.TiposMulta
                .FirstOrDefaultAsync(m => m.IdTipoMulta == id);
            if (tipoMulta == null)
            {
                return NotFound();
            }

            return View(tipoMulta);
        }

        // GET: TipoMulta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoMulta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoMulta,Codigo,Nombre,Descripcion,MontoBase,Activo")] TipoMulta tipoMulta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoMulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoMulta);
        }

        // GET: TipoMulta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMulta = await _context.TiposMulta.FindAsync(id);
            if (tipoMulta == null)
            {
                return NotFound();
            }
            return View(tipoMulta);
        }

        // POST: TipoMulta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoMulta,Codigo,Nombre,Descripcion,MontoBase,Activo")] TipoMulta tipoMulta)
        {
            if (id != tipoMulta.IdTipoMulta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoMulta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoMultaExists(tipoMulta.IdTipoMulta))
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
            return View(tipoMulta);
        }

        // GET: TipoMulta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMulta = await _context.TiposMulta
                .FirstOrDefaultAsync(m => m.IdTipoMulta == id);
            if (tipoMulta == null)
            {
                return NotFound();
            }

            return View(tipoMulta);
        }

        // POST: TipoMulta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoMulta = await _context.TiposMulta.FindAsync(id);
            if (tipoMulta != null)
            {
                _context.TiposMulta.Remove(tipoMulta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoMultaExists(int id)
        {
            return _context.TiposMulta.Any(e => e.IdTipoMulta == id);
        }
    }
}
