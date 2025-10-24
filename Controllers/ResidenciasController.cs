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
    public class ResidenciasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResidenciasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Residencias
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Inquilino)
                .Include(r => r.Propietario);

            // Obtener lista de DPIs de personas no gratas activas
            var dpisNoGratos = await _context.PersonasNoGratas
                .Where(p => p.Activo)
                .Select(p => p.Dpi)
                .ToListAsync();

            ViewBag.DpisNoGratos = dpisNoGratos;

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Residencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residencia = await _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Inquilino)
                .Include(r => r.Propietario)
                .FirstOrDefaultAsync(m => m.IdResidencia == id);
            if (residencia == null)
            {
                return NotFound();
            }

            return View(residencia);
        }

        // GET: Residencias/Create
        public IActionResult Create()
        {
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre");
            ViewData["IdInquilino"] = new SelectList(_context.Inquilinos, "IdInquilino", "NombreCompleto");
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "NombreCompleto");
            return View();
        }

        // POST: Residencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdResidencia,Numero,IdCluster,IdPropietario,IdInquilino,Estado")] Residencia residencia)
        {
            // Remover validación de propiedades de navegación
            ModelState.Remove("Cluster");
            ModelState.Remove("Propietario");
            ModelState.Remove("Inquilino");
            ModelState.Remove("Vehiculos");
            ModelState.Remove("Censos");

            if (ModelState.IsValid)
            {
                _context.Add(residencia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", residencia.IdCluster);
            ViewData["IdInquilino"] = new SelectList(_context.Inquilinos, "IdInquilino", "NombreCompleto", residencia.IdInquilino);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "NombreCompleto", residencia.IdPropietario);
            return View(residencia);
        }

        // GET: Residencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residencia = await _context.Residencias.FindAsync(id);
            if (residencia == null)
            {
                return NotFound();
            }
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", residencia.IdCluster);
            ViewData["IdInquilino"] = new SelectList(_context.Inquilinos, "IdInquilino", "NombreCompleto", residencia.IdInquilino);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "NombreCompleto", residencia.IdPropietario);
            return View(residencia);
        }

        // POST: Residencias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdResidencia,Numero,IdCluster,IdPropietario,IdInquilino,Estado")] Residencia residencia)
        {
            if (id != residencia.IdResidencia)
            {
                return NotFound();
            }

            // Remover validación de propiedades de navegación
            ModelState.Remove("Cluster");
            ModelState.Remove("Propietario");
            ModelState.Remove("Inquilino");
            ModelState.Remove("Vehiculos");
            ModelState.Remove("Censos");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(residencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResidenciaExists(residencia.IdResidencia))
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
            ViewData["IdCluster"] = new SelectList(_context.Clusters, "IdCluster", "Nombre", residencia.IdCluster);
            ViewData["IdInquilino"] = new SelectList(_context.Inquilinos, "IdInquilino", "NombreCompleto", residencia.IdInquilino);
            ViewData["IdPropietario"] = new SelectList(_context.Propietarios, "IdPropietario", "NombreCompleto", residencia.IdPropietario);
            return View(residencia);
        }

        // GET: Residencias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residencia = await _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Inquilino)
                .Include(r => r.Propietario)
                .FirstOrDefaultAsync(m => m.IdResidencia == id);
            if (residencia == null)
            {
                return NotFound();
            }

            return View(residencia);
        }

        // POST: Residencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var residencia = await _context.Residencias.FindAsync(id);
            if (residencia != null)
            {
                _context.Residencias.Remove(residencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResidenciaExists(int id)
        {
            return _context.Residencias.Any(e => e.IdResidencia == id);
        }
    }
}