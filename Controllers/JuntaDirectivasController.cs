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
    public class JuntaDirectivasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JuntaDirectivasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JuntaDirectivas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.JuntasDirectivas.Include(j => j.Cluster);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: JuntaDirectivas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juntaDirectiva = await _context.JuntasDirectivas
                .Include(j => j.Cluster)
                .FirstOrDefaultAsync(m => m.IdJuntaDirectiva == id);
            if (juntaDirectiva == null)
            {
                return NotFound();
            }

            return View(juntaDirectiva);
        }

        // GET: JuntaDirectivas/Create
        public IActionResult Create()
        {
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre");
            return View();
        }

        // POST: JuntaDirectivas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJuntaDirectiva,IdCluster,AnioInicio,AnioFin")] JuntaDirectiva juntaDirectiva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(juntaDirectiva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", juntaDirectiva.IdCluster);
            return View(juntaDirectiva);
        }

        // GET: JuntaDirectivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juntaDirectiva = await _context.JuntasDirectivas.FindAsync(id);
            if (juntaDirectiva == null)
            {
                return NotFound();
            }
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", juntaDirectiva.IdCluster);
            return View(juntaDirectiva);
        }

        // POST: JuntaDirectivas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdJuntaDirectiva,IdCluster,AnioInicio,AnioFin")] JuntaDirectiva juntaDirectiva)
        {
            if (id != juntaDirectiva.IdJuntaDirectiva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(juntaDirectiva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JuntaDirectivaExists(juntaDirectiva.IdJuntaDirectiva))
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
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", juntaDirectiva.IdCluster);
            return View(juntaDirectiva);
        }

        // GET: JuntaDirectivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juntaDirectiva = await _context.JuntasDirectivas
                .Include(j => j.Cluster)
                .FirstOrDefaultAsync(m => m.IdJuntaDirectiva == id);
            if (juntaDirectiva == null)
            {
                return NotFound();
            }

            return View(juntaDirectiva);
        }

        // POST: JuntaDirectivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juntaDirectiva = await _context.JuntasDirectivas.FindAsync(id);
            if (juntaDirectiva != null)
            {
                _context.JuntasDirectivas.Remove(juntaDirectiva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JuntaDirectivaExists(int id)
        {
            return _context.JuntasDirectivas.Any(e => e.IdJuntaDirectiva == id);
        }
    }
}
