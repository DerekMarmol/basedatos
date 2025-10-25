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
    public class RegistroVisitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistroVisitasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RegistroVisita
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RegistrosVisita
                .Include(r => r.Garita)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.GuardiaSalida)
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Include(r => r.Visitante);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RegistroVisita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroVisita = await _context.RegistrosVisita
                .Include(r => r.Garita)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.GuardiaSalida)
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Include(r => r.Visitante)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);

            if (registroVisita == null)
            {
                return NotFound();
            }

            return View(registroVisita);
        }

        // GET: RegistroVisita/Create
        public IActionResult Create()
        {
            try
            {
                var garitas = _context.Garitas.ToList();
                ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Nombre");

                var guardias = _context.Guardias.Where(g => g.Activo).ToList();
                ViewData["IdGuardiaIngreso"] = new SelectList(guardias, "IdGuardia", "NombreCompleto");
                ViewData["IdGuardiaSalida"] = new SelectList(guardias, "IdGuardia", "NombreCompleto");

                var residencias = _context.Residencias
                    .Include(r => r.Cluster)
                    .Select(r => new
                    {
                        r.IdResidencia,
                        Descripcion = "Casa " + r.Numero + " - " + r.Cluster.Nombre
                    })
                    .ToList();

                ViewData["IdResidencia"] = new SelectList(residencias, "IdResidencia", "Descripcion");

                var visitantes = _context.Visitantes.ToList();
                ViewData["IdVisitante"] = new SelectList(visitantes, "IdVisitante", "NombreCompleto");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Create GET: {ex.Message}");
                TempData["ErrorMessage"] = "Error al cargar el formulario. Por favor, contacte al administrador.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: RegistroVisita/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRegistro,IdVisitante,IdResidencia,FechaIngreso,FechaSalida,IdGuardiaIngreso,IdGuardiaSalida,Motivo,IdGarita")] RegistroVisita registroVisita)
        {
            ModelState.Remove("Garita");
            ModelState.Remove("GuardiaIngreso");
            ModelState.Remove("GuardiaSalida");
            ModelState.Remove("Residencia");
            ModelState.Remove("Visitante");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(registroVisita);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Visita registrada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al guardar: {ex.Message}");
                    ModelState.AddModelError("", "Error al guardar el registro. Por favor, intente nuevamente.");
                }
            }

            RecargarDropdowns(registroVisita);
            return View(registroVisita);
        }

        // GET: RegistroVisita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroVisita = await _context.RegistrosVisita.FindAsync(id);
            if (registroVisita == null)
            {
                return NotFound();
            }

            RecargarDropdowns(registroVisita);
            return View(registroVisita);
        }

        // POST: RegistroVisita/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRegistro,IdVisitante,IdResidencia,FechaIngreso,FechaSalida,IdGuardiaIngreso,IdGuardiaSalida,Motivo,IdGarita")] RegistroVisita registroVisita)
        {
            if (id != registroVisita.IdRegistro)
            {
                return NotFound();
            }

            ModelState.Remove("Garita");
            ModelState.Remove("GuardiaIngreso");
            ModelState.Remove("GuardiaSalida");
            ModelState.Remove("Residencia");
            ModelState.Remove("Visitante");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registroVisita);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Visita actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistroVisitaExists(registroVisita.IdRegistro))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar: {ex.Message}");
                    ModelState.AddModelError("", "Error al actualizar el registro.");
                }
            }

            RecargarDropdowns(registroVisita);
            return View(registroVisita);
        }

        // GET: RegistroVisita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registroVisita = await _context.RegistrosVisita
                .Include(r => r.Garita)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.GuardiaSalida)
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Include(r => r.Visitante)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);

            if (registroVisita == null)
            {
                return NotFound();
            }

            return View(registroVisita);
        }

        // POST: RegistroVisita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var registroVisita = await _context.RegistrosVisita.FindAsync(id);
                if (registroVisita != null)
                {
                    _context.RegistrosVisita.Remove(registroVisita);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Registro eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar: {ex.Message}");
                TempData["ErrorMessage"] = "Error al eliminar el registro.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RegistroVisitaExists(int id)
        {
            return _context.RegistrosVisita.Any(e => e.IdRegistro == id);
        }

        private void RecargarDropdowns(RegistroVisita? registroVisita = null)
        {
            var garitas = _context.Garitas.ToList();
            var guardias = _context.Guardias.Where(g => g.Activo).ToList();
            var residencias = _context.Residencias
                .Include(r => r.Cluster)
                .Select(r => new
                {
                    r.IdResidencia,
                    Descripcion = "Casa " + r.Numero + " - " + r.Cluster.Nombre
                })
                .ToList();
            var visitantes = _context.Visitantes.ToList();

            if (registroVisita != null)
            {
                ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Nombre", registroVisita.IdGarita);
                ViewData["IdGuardiaIngreso"] = new SelectList(guardias, "IdGuardia", "NombreCompleto", registroVisita.IdGuardiaIngreso);
                ViewData["IdGuardiaSalida"] = new SelectList(guardias, "IdGuardia", "NombreCompleto", registroVisita.IdGuardiaSalida);
                ViewData["IdResidencia"] = new SelectList(residencias, "IdResidencia", "Descripcion", registroVisita.IdResidencia);
                ViewData["IdVisitante"] = new SelectList(visitantes, "IdVisitante", "NombreCompleto", registroVisita.IdVisitante);
            }
            else
            {
                ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Nombre");
                ViewData["IdGuardiaIngreso"] = new SelectList(guardias, "IdGuardia", "NombreCompleto");
                ViewData["IdGuardiaSalida"] = new SelectList(guardias, "IdGuardia", "NombreCompleto");
                ViewData["IdResidencia"] = new SelectList(residencias, "IdResidencia", "Descripcion");
                ViewData["IdVisitante"] = new SelectList(visitantes, "IdVisitante", "NombreCompleto");
            }
        }
    }
}