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
    public class ClustersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClustersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clusters
        public async Task<IActionResult> Index()
        {
            var clusters = await _context.Clusters
                .Include(c => c.Residencial)
                .ToListAsync();
            return View(clusters);
        }

        // GET: Clusters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cluster = await _context.Clusters
                .Include(c => c.Residencial)
                .FirstOrDefaultAsync(m => m.IdCluster == id);
            if (cluster == null)
            {
                return NotFound();
            }

            return View(cluster);
        }

        // GET: Clusters/Create
        public IActionResult Create()
        {
            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre");
            return View();
        }

        // POST: Clusters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCluster,Nombre,IdResidencial")] Cluster cluster)
        {
            // Remover validación de propiedades de navegación
            ModelState.Remove("Residencial");
            ModelState.Remove("Residencias");
            ModelState.Remove("JuntasDirectivas");
            ModelState.Remove("Garitas");

            if (ModelState.IsValid)
            {
                _context.Add(cluster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", cluster.IdResidencial);
            return View(cluster);
        }

        // GET: Clusters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cluster = await _context.Clusters.FindAsync(id);
            if (cluster == null)
            {
                return NotFound();
            }

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", cluster.IdResidencial);
            return View(cluster);
        }

        // POST: Clusters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCluster,Nombre,IdResidencial")] Cluster cluster)
        {
            if (id != cluster.IdCluster)
            {
                return NotFound();
            }

            // Remover validación de propiedades de navegación
            ModelState.Remove("Residencial");
            ModelState.Remove("Residencias");
            ModelState.Remove("JuntasDirectivas");
            ModelState.Remove("Garitas");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cluster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClusterExists(cluster.IdCluster))
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

            ViewData["IdResidencial"] = new SelectList(_context.Residenciales, "IdResidencial", "Nombre", cluster.IdResidencial);
            return View(cluster);
        }

        // GET: Clusters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cluster = await _context.Clusters
                .Include(c => c.Residencial)
                .FirstOrDefaultAsync(m => m.IdCluster == id);
            if (cluster == null)
            {
                return NotFound();
            }

            return View(cluster);
        }

        // POST: Clusters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cluster = await _context.Clusters.FindAsync(id);
            if (cluster != null)
            {
                _context.Clusters.Remove(cluster);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClusterExists(int id)
        {
            return _context.Clusters.Any(e => e.IdCluster == id);
        }
    }
}