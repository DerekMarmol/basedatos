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
    public class GaritasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GaritasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Garitas
        public async Task<IActionResult> Index()
        {
            var garitas = await _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .OrderBy(g => g.Activa ? 0 : 1) // Activas primero
                .ThenBy(g => g.Nombre)
                .ToListAsync();

            return View(garitas);
        }

        // GET: Garitas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garita = await _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Include(g => g.Turnos)
                    .ThenInclude(t => t.Guardia)
                .FirstOrDefaultAsync(m => m.IdGarita == id);

            if (garita == null)
            {
                return NotFound();
            }

            return View(garita);
        }

        // GET: Garitas/Create
        public IActionResult Create()
        {
            // Obtener clusters con su residencial para mostrar información completa
            var clusters = _context.Clusters
                .Include(c => c.Residencial)
                .Select(c => new
                {
                    c.IdCluster,
                    Texto = c.Residencial.Nombre + " - " + c.Nombre
                })
                .ToList();

            ViewData["IdCluster"] = new SelectList(clusters, "IdCluster", "Texto");
            return View();
        }

        // POST: Garitas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGarita,Nombre,IdCluster,Ubicacion,Activa")] Garita garita)
        {
            // Remover validación de navegación
            ModelState.Remove("Cluster");
            ModelState.Remove("Turnos");

            // Validar que no exista una garita con el mismo nombre
            var garitaExistente = await _context.Garitas
                .AnyAsync(g => g.Nombre == garita.Nombre);

            if (garitaExistente)
            {
                ModelState.AddModelError("Nombre", "Ya existe una garita con este nombre.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(garita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var clusters = _context.Clusters
                .Include(c => c.Residencial)
                .Select(c => new
                {
                    c.IdCluster,
                    Texto = c.Residencial.Nombre + " - " + c.Nombre
                })
                .ToList();

            ViewData["IdCluster"] = new SelectList(clusters, "IdCluster", "Texto", garita.IdCluster);
            return View(garita);
        }

        // GET: Garitas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garita = await _context.Garitas.FindAsync(id);
            if (garita == null)
            {
                return NotFound();
            }

            var clusters = _context.Clusters
                .Include(c => c.Residencial)
                .Select(c => new
                {
                    c.IdCluster,
                    Texto = c.Residencial.Nombre + " - " + c.Nombre
                })
                .ToList();

            ViewData["IdCluster"] = new SelectList(clusters, "IdCluster", "Texto", garita.IdCluster);
            return View(garita);
        }

        // POST: Garitas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGarita,Nombre,IdCluster,Ubicacion,Activa")] Garita garita)
        {
            if (id != garita.IdGarita)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Cluster");
            ModelState.Remove("Turnos");

            // Validar que no exista otra garita con el mismo nombre
            var garitaExistente = await _context.Garitas
                .AnyAsync(g => g.Nombre == garita.Nombre && g.IdGarita != id);

            if (garitaExistente)
            {
                ModelState.AddModelError("Nombre", "Ya existe otra garita con este nombre.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(garita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GaritaExists(garita.IdGarita))
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

            var clusters = _context.Clusters
                .Include(c => c.Residencial)
                .Select(c => new
                {
                    c.IdCluster,
                    Texto = c.Residencial.Nombre + " - " + c.Nombre
                })
                .ToList();

            ViewData["IdCluster"] = new SelectList(clusters, "IdCluster", "Texto", garita.IdCluster);
            return View(garita);
        }

        // GET: Garitas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garita = await _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .FirstOrDefaultAsync(m => m.IdGarita == id);

            if (garita == null)
            {
                return NotFound();
            }

            // Verificar si tiene turnos asignados
            var tieneTurnos = await _context.TurnosGuardia
                .AnyAsync(t => t.IdGarita == id);

            ViewBag.TieneTurnos = tieneTurnos;

            return View(garita);
        }

        // POST: Garitas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var garita = await _context.Garitas.FindAsync(id);
            if (garita != null)
            {
                _context.Garitas.Remove(garita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GaritaExists(int id)
        {
            return _context.Garitas.Any(e => e.IdGarita == id);
        }
    }
}