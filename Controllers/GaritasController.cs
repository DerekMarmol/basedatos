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
            var applicationDbContext = _context.Garitas.Include(g => g.Cluster);
            return View(await applicationDbContext.ToListAsync());
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
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre");
            return View();
        }

        // POST: Garitas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGarita,Nombre,IdCluster,Ubicacion,Activa")] Garita garita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(garita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", garita.IdCluster);
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
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", garita.IdCluster);
            return View(garita);
        }

        // POST: Garitas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGarita,Nombre,IdCluster,Ubicacion,Activa")] Garita garita)
        {
            if (id != garita.IdGarita)
            {
                return NotFound();
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
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", garita.IdCluster);
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
                .FirstOrDefaultAsync(m => m.IdGarita == id);
            if (garita == null)
            {
                return NotFound();
            }

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
