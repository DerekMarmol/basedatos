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
            var multas = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)  
                .Include(m => m.TipoMulta)
                .ToListAsync();
            return View(multas);
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
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre");
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo");
            return View();
        }

        // POST: Multas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMulta,IdCluster,IdResidencia,Concepto,Monto,Fecha,Pagada,IdTipoMulta")] Multa multa)
        {
            // Remover validación de navegación
            ModelState.Remove("Residencia");
            ModelState.Remove("TipoMulta");

            if (ModelState.IsValid)
            {
                // Verificar que el cluster existe
                var clusterExiste = await _context.Clusters.AnyAsync(c => c.IdCluster == multa.IdCluster);
                if (!clusterExiste)
                {
                    ModelState.AddModelError("IdCluster", "El cluster especificado no existe.");
                    ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                    ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
                    return View(multa);
                }

                // Verificar que la residencia existe
                var residenciaExiste = await _context.Residencias
                    .AnyAsync(r => r.IdResidencia == multa.IdResidencia && r.IdCluster == multa.IdCluster);

                if (!residenciaExiste)
                {
                    ModelState.AddModelError("IdResidencia", "La residencia especificada no existe o no pertenece al cluster seleccionado.");
                    ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                    ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
                    return View(multa);
                }

                _context.Add(multa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
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

            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
            ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
            return View(multa);
        }

        // POST: Multas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMulta,IdCluster,IdResidencia,Concepto,Monto,Fecha,Pagada,IdTipoMulta")] Multa multa)
        {
            if (id != multa.IdMulta)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Residencia");
            ModelState.Remove("TipoMulta");

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el cluster existe
                    var clusterExiste = await _context.Clusters.AnyAsync(c => c.IdCluster == multa.IdCluster);
                    if (!clusterExiste)
                    {
                        ModelState.AddModelError("IdCluster", "El cluster especificado no existe.");
                        ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                        ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
                        return View(multa);
                    }

                    // Verificar que la residencia existe y pertenece al cluster
                    var residenciaExiste = await _context.Residencias
                        .AnyAsync(r => r.IdResidencia == multa.IdResidencia && r.IdCluster == multa.IdCluster);

                    if (!residenciaExiste)
                    {
                        ModelState.AddModelError("IdResidencia", "La residencia especificada no existe o no pertenece al cluster seleccionado.");
                        ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
                        ViewData["IdTipoMulta"] = new SelectList(_context.TiposMulta, "IdTipoMulta", "Codigo", multa.IdTipoMulta);
                        return View(multa);
                    }

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

            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", multa.IdCluster);
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