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

        public async Task<IActionResult> Index()
        {
            var vehiculos = await _context.VehiculosNoPermitidos
                .Include(v => v.Residencial)
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync();
            return View(vehiculos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos
                .Include(v => v.Residencial)
                .FirstOrDefaultAsync(m => m.IdVNP == id);

            if (vehiculoNoPermitido == null)
            {
                return NotFound();
            }

            return View(vehiculoNoPermitido);
        }

        public IActionResult Create()
        {
            ViewBag.IdResidencial = new SelectList(
                _context.Residenciales.OrderBy(r => r.Nombre),
                "IdResidencial",
                "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehiculoNoPermitido vehiculoNoPermitido)
        {
            if (vehiculoNoPermitido.IdResidencial == 0)
            {
                vehiculoNoPermitido.IdResidencial = null;
            }

            ModelState.Remove("Residencial");

            if (ModelState.IsValid)
            {
                _context.Add(vehiculoNoPermitido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdResidencial = new SelectList(
                _context.Residenciales.OrderBy(r => r.Nombre),
                "IdResidencial",
                "Nombre",
                vehiculoNoPermitido.IdResidencial);
            return View(vehiculoNoPermitido);
        }

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

            ViewBag.IdResidencial = new SelectList(
                _context.Residenciales.OrderBy(r => r.Nombre),
                "IdResidencial",
                "Nombre",
                vehiculoNoPermitido.IdResidencial);
            return View(vehiculoNoPermitido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehiculoNoPermitido vehiculoNoPermitido)
        {
            if (id != vehiculoNoPermitido.IdVNP)
            {
                return NotFound();
            }

            if (vehiculoNoPermitido.IdResidencial == 0)
            {
                vehiculoNoPermitido.IdResidencial = null;
            }

            ModelState.Remove("Residencial");

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

            ViewBag.IdResidencial = new SelectList(
                _context.Residenciales.OrderBy(r => r.Nombre),
                "IdResidencial",
                "Nombre",
                vehiculoNoPermitido.IdResidencial);
            return View(vehiculoNoPermitido);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos
                .Include(v => v.Residencial)
                .FirstOrDefaultAsync(m => m.IdVNP == id);

            if (vehiculoNoPermitido == null)
            {
                return NotFound();
            }

            return View(vehiculoNoPermitido);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculoNoPermitido = await _context.VehiculosNoPermitidos.FindAsync(id);
            if (vehiculoNoPermitido != null)
            {
                _context.VehiculosNoPermitidos.Remove(vehiculoNoPermitido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoNoPermitidoExists(int id)
        {
            return _context.VehiculosNoPermitidos.Any(e => e.IdVNP == id);
        }
    }
}