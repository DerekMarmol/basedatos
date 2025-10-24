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
    public class PropietariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropietariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Propietarios
        public async Task<IActionResult> Index()
        {
            var propietarios = await _context.Propietarios.ToListAsync();

            // Obtener lista de DPIs de personas no gratas activas
            var dpisNoGratos = await _context.PersonasNoGratas
                .Where(p => p.Activo)
                .Select(p => p.Dpi)
                .ToListAsync();

            ViewBag.DpisNoGratos = dpisNoGratos;

            return View(propietarios);
        }

        // GET: Propietarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(m => m.IdPropietario == id);
            if (propietario == null)
            {
                return NotFound();
            }

            // Verificar si es persona no grata
            var esNoGrato = await _context.PersonasNoGratas
                .AnyAsync(p => p.Dpi == propietario.Dpi && p.Activo);

            ViewBag.EsNoGrato = esNoGrato;

            return View(propietario);
        }

        // GET: Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPropietario,NombreCompleto,Dpi,Telefono,FechaNacimiento,EstadoCivil,TipoLicencia")] Propietario propietario)
        {
            // Remover validación de navegación
            ModelState.Remove("Residencias");
            ModelState.Remove("MiembrosJunta");

            // Validar si ya existe un propietario con ese DPI
            var propietarioExistente = await _context.Propietarios
                .AnyAsync(p => p.Dpi == propietario.Dpi);

            if (propietarioExistente)
            {
                ModelState.AddModelError("Dpi", "Ya existe un propietario registrado con este DPI.");
            }

            // Verificar si es persona no grata
            var esNoGrato = await _context.PersonasNoGratas
                .AnyAsync(p => p.Dpi == propietario.Dpi && p.Activo);

            if (esNoGrato)
            {
                ModelState.AddModelError("Dpi", "⚠️ ADVERTENCIA: Esta persona está registrada como NO GRATA en el sistema.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(propietario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPropietario,NombreCompleto,Dpi,Telefono,FechaNacimiento,EstadoCivil,TipoLicencia")] Propietario propietario)
        {
            if (id != propietario.IdPropietario)
            {
                return NotFound();
            }

            // Remover validación de navegación
            ModelState.Remove("Residencias");
            ModelState.Remove("MiembrosJunta");

            // Validar si ya existe otro propietario con ese DPI
            var propietarioExistente = await _context.Propietarios
                .AnyAsync(p => p.Dpi == propietario.Dpi && p.IdPropietario != id);

            if (propietarioExistente)
            {
                ModelState.AddModelError("Dpi", "Ya existe otro propietario registrado con este DPI.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propietario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropietarioExists(propietario.IdPropietario))
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
            return View(propietario);
        }

        // GET: Propietarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(m => m.IdPropietario == id);
            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario != null)
            {
                _context.Propietarios.Remove(propietario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropietarioExists(int id)
        {
            return _context.Propietarios.Any(e => e.IdPropietario == id);
        }
    }
}