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
    public class AccesoVehicularController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccesoVehicularController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AccesoVehiculars
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AccesosVehiculares.Include(a => a.Garita).Include(a => a.GuardiaIngreso).Include(a => a.GuardiaSalida).Include(a => a.Vehiculo).Include(a => a.Visitante);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AccesoVehiculars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesoVehicular = await _context.AccesosVehiculares
                .Include(a => a.Garita)
                .Include(a => a.GuardiaIngreso)
                .Include(a => a.GuardiaSalida)
                .Include(a => a.Vehiculo)
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(m => m.IdAcceso == id);
            if (accesoVehicular == null)
            {
                return NotFound();
            }

            return View(accesoVehicular);
        }

        // GET: AccesoVehiculars/Create
        public IActionResult Create()
        {
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre");
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi");
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi");
            ViewData["IdVehiculo"] = new SelectList(_context.Vehiculos, "IdVehiculo", "NumeroTarjeta");
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto");
            return View();
        }

        // POST: AccesoVehiculars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAcceso,IdVehiculo,IdVisitante,Placa,IdGarita,FechaIngreso,HoraIngreso,FechaSalida,HoraSalida,IdGuardiaIngreso,IdGuardiaSalida,TipoAcceso,Observaciones")] AccesoVehicular accesoVehicular)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accesoVehicular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", accesoVehicular.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", accesoVehicular.IdGuardiaIngreso);
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", accesoVehicular.IdGuardiaSalida);
            ViewData["IdVehiculo"] = new SelectList(_context.Vehiculos, "IdVehiculo", "NumeroTarjeta", accesoVehicular.IdVehiculo);
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto", accesoVehicular.IdVisitante);
            return View(accesoVehicular);
        }

        // GET: AccesoVehiculars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesoVehicular = await _context.AccesosVehiculares.FindAsync(id);
            if (accesoVehicular == null)
            {
                return NotFound();
            }
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", accesoVehicular.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", accesoVehicular.IdGuardiaIngreso);
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", accesoVehicular.IdGuardiaSalida);
            ViewData["IdVehiculo"] = new SelectList(_context.Vehiculos, "IdVehiculo", "NumeroTarjeta", accesoVehicular.IdVehiculo);
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto", accesoVehicular.IdVisitante);
            return View(accesoVehicular);
        }

        // POST: AccesoVehiculars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAcceso,IdVehiculo,IdVisitante,Placa,IdGarita,FechaIngreso,HoraIngreso,FechaSalida,HoraSalida,IdGuardiaIngreso,IdGuardiaSalida,TipoAcceso,Observaciones")] AccesoVehicular accesoVehicular)
        {
            if (id != accesoVehicular.IdAcceso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accesoVehicular);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccesoVehicularExists(accesoVehicular.IdAcceso))
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
            ViewData["IdGarita"] = new SelectList(_context.Garitas, "IdGarita", "Nombre", accesoVehicular.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", accesoVehicular.IdGuardiaIngreso);
            ViewData["IdGuardiaSalida"] = new SelectList(_context.Guardias, "IdGuardia", "Dpi", accesoVehicular.IdGuardiaSalida);
            ViewData["IdVehiculo"] = new SelectList(_context.Vehiculos, "IdVehiculo", "NumeroTarjeta", accesoVehicular.IdVehiculo);
            ViewData["IdVisitante"] = new SelectList(_context.Visitantes, "IdVisitante", "NombreCompleto", accesoVehicular.IdVisitante);
            return View(accesoVehicular);
        }

        // GET: AccesoVehiculars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesoVehicular = await _context.AccesosVehiculares
                .Include(a => a.Garita)
                .Include(a => a.GuardiaIngreso)
                .Include(a => a.GuardiaSalida)
                .Include(a => a.Vehiculo)
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(m => m.IdAcceso == id);
            if (accesoVehicular == null)
            {
                return NotFound();
            }

            return View(accesoVehicular);
        }

        // POST: AccesoVehiculars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accesoVehicular = await _context.AccesosVehiculares.FindAsync(id);
            if (accesoVehicular != null)
            {
                _context.AccesosVehiculares.Remove(accesoVehicular);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccesoVehicularExists(int id)
        {
            return _context.AccesosVehiculares.Any(e => e.IdAcceso == id);
        }
    }
}
