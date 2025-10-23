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
    public class ResidencialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResidencialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Residencial
        public async Task<IActionResult> Index()
        {
            return View(await _context.Residenciales.ToListAsync());
        }

        // GET: Residencial/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residencial = await _context.Residenciales
                .FirstOrDefaultAsync(m => m.IdResidencial == id);
            if (residencial == null)
            {
                return NotFound();
            }

            return View(residencial);
        }

        // GET: Residencial/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Residencial/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdResidencial,Codigo,Nombre,Direccion,Municipio,Departamento,Telefono,Email,CantidadViviendas,FechaInicioAdministracion,Activo")] Residencial residencial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(residencial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(residencial);
        }

        // GET: Residencial/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residencial = await _context.Residenciales.FindAsync(id);
            if (residencial == null)
            {
                return NotFound();
            }
            return View(residencial);
        }

        // POST: Residencial/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdResidencial,Codigo,Nombre,Direccion,Municipio,Departamento,Telefono,Email,CantidadViviendas,FechaInicioAdministracion,Activo")] Residencial residencial)
        {
            if (id != residencial.IdResidencial)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(residencial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResidencialExists(residencial.IdResidencial))
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
            return View(residencial);
        }

        // GET: Residencial/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residencial = await _context.Residenciales
                .FirstOrDefaultAsync(m => m.IdResidencial == id);
            if (residencial == null)
            {
                return NotFound();
            }

            return View(residencial);
        }

        // POST: Residencial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var residencial = await _context.Residenciales.FindAsync(id);
            if (residencial != null)
            {
                _context.Residenciales.Remove(residencial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResidencialExists(int id)
        {
            return _context.Residenciales.Any(e => e.IdResidencial == id);
        }
    }
}
