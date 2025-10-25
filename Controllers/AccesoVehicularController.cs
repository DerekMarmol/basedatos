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

        // GET: AccesoVehicular
        public async Task<IActionResult> Index(string filtro = "todos")
        {
            var query = _context.AccesosVehiculares
                .Include(a => a.Garita)
                    .ThenInclude(g => g.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(a => a.GuardiaIngreso)
                .Include(a => a.GuardiaSalida)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v.Residencia)
                        .ThenInclude(r => r.Propietario)
                .Include(a => a.Visitante)
                .AsQueryable();

            switch (filtro.ToLower())
            {
                case "dentro":
                    query = query.Where(a => a.FechaSalida == null);
                    break;
                case "fuera":
                    query = query.Where(a => a.FechaSalida != null);
                    break;
                case "hoy":
                    var hoy = DateTime.Today;
                    query = query.Where(a => a.FechaIngreso.Date == hoy);
                    break;
            }

            var accesosTemp = await query.ToListAsync();

            var accesos = accesosTemp
                .OrderByDescending(a => a.FechaIngreso)
                .ThenByDescending(a => a.HoraIngreso)
                .ToList();

            ViewBag.FiltroActual = filtro;
            ViewBag.TotalDentro = await _context.AccesosVehiculares.CountAsync(a => a.FechaSalida == null);

            return View(accesos);
        }

        // GET: AccesoVehicular/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesoVehicular = await _context.AccesosVehiculares
                .Include(a => a.Garita)
                    .ThenInclude(g => g.Cluster)
                        .ThenInclude(c => c.Residencial)
                .Include(a => a.GuardiaIngreso)
                .Include(a => a.GuardiaSalida)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v.Residencia)
                        .ThenInclude(r => r.Propietario)
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(m => m.IdAcceso == id);

            if (accesoVehicular == null)
            {
                return NotFound();
            }

            return View(accesoVehicular);
        }

        // GET: AccesoVehicular/Create
        public IActionResult Create()
        {
            var garitas = _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Where(g => g.Activa)
                .Select(g => new
                {
                    g.IdGarita,
                    Texto = g.Cluster.Residencial.Nombre + " - " + g.Nombre
                })
                .ToList();

            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " (DPI: " + g.Dpi + ")"
                })
                .ToList();

            var vehiculos = _context.Vehiculos
                .Include(v => v.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Select(v => new
                {
                    v.IdVehiculo,
                    Texto = v.Placa + " - " + v.Residencia.Propietario.NombreCompleto + " (Casa " + v.Residencia.Numero + ")"
                })
                .ToList();

            var visitantes = _context.Visitantes
                .Select(v => new
                {
                    v.IdVisitante,
                    Texto = v.NombreCompleto + " (DPI: " + v.Dpi + ")"
                })
                .ToList();

            ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Texto");
            ViewData["IdGuardiaIngreso"] = new SelectList(guardias, "IdGuardia", "Texto");
            ViewData["IdVehiculo"] = new SelectList(vehiculos, "IdVehiculo", "Texto");
            ViewData["IdVisitante"] = new SelectList(visitantes, "IdVisitante", "Texto");

            var modelo = new AccesoVehicular
            {
                FechaIngreso = DateTime.Now.Date,
                HoraIngreso = DateTime.Now.TimeOfDay
            };

            return View(modelo);
        }

        // POST: AccesoVehicular/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAcceso,IdVehiculo,IdVisitante,Placa,IdGarita,FechaIngreso,HoraIngreso,IdGuardiaIngreso,TipoAcceso,Observaciones")] AccesoVehicular accesoVehicular)
        {
            ModelState.Remove("Vehiculo");
            ModelState.Remove("Visitante");
            ModelState.Remove("Garita");
            ModelState.Remove("GuardiaIngreso");
            ModelState.Remove("GuardiaSalida");
            ModelState.Remove("FechaSalida");
            ModelState.Remove("HoraSalida");
            ModelState.Remove("IdGuardiaSalida");

            if (!accesoVehicular.IdVehiculo.HasValue && !accesoVehicular.IdVisitante.HasValue && string.IsNullOrEmpty(accesoVehicular.Placa))
            {
                ModelState.AddModelError("Placa", "Debe ingresar al menos una placa o seleccionar un vehículo/visitante.");
            }

            if (accesoVehicular.IdVehiculo.HasValue)
            {
                var vehiculo = await _context.Vehiculos.FindAsync(accesoVehicular.IdVehiculo.Value);
                if (vehiculo != null)
                {
                    accesoVehicular.Placa = vehiculo.Placa;
                }
            }

            var accesoAbierto = await _context.AccesosVehiculares
                .Where(a => a.Placa == accesoVehicular.Placa && a.FechaSalida == null)
                .AnyAsync();

            if (accesoAbierto)
            {
                ModelState.AddModelError("Placa", "Ya existe un acceso abierto (sin salida registrada) para esta placa.");
            }

            if (ModelState.IsValid)
            {
                accesoVehicular.FechaSalida = null;
                accesoVehicular.HoraSalida = null;
                accesoVehicular.IdGuardiaSalida = null;

                _context.Add(accesoVehicular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var garitas = _context.Garitas
                .Include(g => g.Cluster)
                    .ThenInclude(c => c.Residencial)
                .Where(g => g.Activa)
                .Select(g => new
                {
                    g.IdGarita,
                    Texto = g.Cluster.Residencial.Nombre + " - " + g.Nombre
                })
                .ToList();

            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " (DPI: " + g.Dpi + ")"
                })
                .ToList();

            var vehiculos = _context.Vehiculos
                .Include(v => v.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Select(v => new
                {
                    v.IdVehiculo,
                    Texto = v.Placa + " - " + v.Residencia.Propietario.NombreCompleto + " (Casa " + v.Residencia.Numero + ")"
                })
                .ToList();

            var visitantes = _context.Visitantes
                .Select(v => new
                {
                    v.IdVisitante,
                    Texto = v.NombreCompleto + " (DPI: " + v.Dpi + ")"
                })
                .ToList();

            ViewData["IdGarita"] = new SelectList(garitas, "IdGarita", "Texto", accesoVehicular.IdGarita);
            ViewData["IdGuardiaIngreso"] = new SelectList(guardias, "IdGuardia", "Texto", accesoVehicular.IdGuardiaIngreso);
            ViewData["IdVehiculo"] = new SelectList(vehiculos, "IdVehiculo", "Texto", accesoVehicular.IdVehiculo);
            ViewData["IdVisitante"] = new SelectList(visitantes, "IdVisitante", "Texto", accesoVehicular.IdVisitante);

            return View(accesoVehicular);
        }

        // GET: AccesoVehicular/RegistrarSalida/5
        public async Task<IActionResult> RegistrarSalida(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accesoVehicular = await _context.AccesosVehiculares
                .Include(a => a.Garita)
                .Include(a => a.GuardiaIngreso)
                .Include(a => a.Vehiculo)
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(a => a.IdAcceso == id);

            if (accesoVehicular == null)
            {
                return NotFound();
            }

            if (accesoVehicular.FechaSalida.HasValue)
            {
                TempData["Error"] = "Este acceso ya tiene una salida registrada.";
                return RedirectToAction(nameof(Index));
            }

            var guardias = _context.Guardias
                .Where(g => g.Activo)
                .Select(g => new
                {
                    g.IdGuardia,
                    Texto = g.NombreCompleto + " (DPI: " + g.Dpi + ")"
                })
                .ToList();

            ViewData["IdGuardiaSalida"] = new SelectList(guardias, "IdGuardia", "Texto");

            accesoVehicular.FechaSalida = DateTime.Now.Date;
            accesoVehicular.HoraSalida = DateTime.Now.TimeOfDay;

            return View(accesoVehicular);
        }

        // POST: AccesoVehicular/RegistrarSalida/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarSalida(int id, DateTime FechaSalida, TimeSpan HoraSalida, int IdGuardiaSalida, string Observaciones)
        {
            var accesoVehicular = await _context.AccesosVehiculares.FindAsync(id);

            if (accesoVehicular == null)
            {
                return NotFound();
            }

            var fechaHoraSalida = FechaSalida.Date + HoraSalida;
            var fechaHoraIngreso = accesoVehicular.FechaIngreso.Date + accesoVehicular.HoraIngreso;

            if (fechaHoraSalida < fechaHoraIngreso)
            {
                ModelState.AddModelError("", "La fecha y hora de salida no puede ser anterior a la de ingreso.");

                var guardias = _context.Guardias
                    .Where(g => g.Activo)
                    .Select(g => new
                    {
                        g.IdGuardia,
                        Texto = g.NombreCompleto + " (DPI: " + g.Dpi + ")"
                    })
                    .ToList();

                ViewData["IdGuardiaSalida"] = new SelectList(guardias, "IdGuardia", "Texto", IdGuardiaSalida);

                accesoVehicular = await _context.AccesosVehiculares
                    .Include(a => a.Garita)
                    .Include(a => a.GuardiaIngreso)
                    .Include(a => a.Vehiculo)
                    .Include(a => a.Visitante)
                    .FirstOrDefaultAsync(a => a.IdAcceso == id);

                return View(accesoVehicular);
            }

            accesoVehicular.FechaSalida = FechaSalida;
            accesoVehicular.HoraSalida = HoraSalida;
            accesoVehicular.IdGuardiaSalida = IdGuardiaSalida;
            if (!string.IsNullOrEmpty(Observaciones))
            {
                accesoVehicular.Observaciones += (string.IsNullOrEmpty(accesoVehicular.Observaciones) ? "" : " | ") + Observaciones;
            }

            _context.Update(accesoVehicular);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Salida registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: AccesoVehicular/Delete/5
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

        // POST: AccesoVehicular/Delete/5
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