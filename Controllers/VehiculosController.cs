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
    public class VehiculosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehiculoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehiculos
                .Include(v => v.Residencia)
                    .ThenInclude(r => r.Cluster)
                .ToListAsync();
            return View(await applicationDbContext);
        }

        // GET: Vehiculoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Residencia)
                    .ThenInclude(r => r.Cluster)
                .FirstOrDefaultAsync(m => m.IdVehiculo == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // GET: Vehiculoes/Create
        public IActionResult Create()
        {
            ViewData["IdCluster"] = new SelectList(_context.Clusters.OrderBy(c => c.Nombre), "IdCluster", "Nombre");
            ViewData["IdResidencia"] = new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }

        // POST: Vehiculoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVehiculo,IdResidencia,Marca,Linea,Anio,Color,Placa,NumeroTarjeta")] Vehiculo vehiculo)
        {
            ModelState.Remove("Residencia");
            ModelState.Remove("Accesos");

            // Validar que la placa sea única
            if (await _context.Vehiculos.AnyAsync(v => v.Placa.ToUpper() == vehiculo.Placa.ToUpper()))
            {
                ModelState.AddModelError("Placa", "Ya existe un vehículo registrado con esta placa.");
            }

            // Validar que el número de tarjeta sea único
            if (await _context.Vehiculos.AnyAsync(v => v.NumeroTarjeta == vehiculo.NumeroTarjeta))
            {
                ModelState.AddModelError("NumeroTarjeta", "Ya existe un vehículo registrado con este número de tarjeta.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Convertir placa a mayúsculas antes de guardar
                    vehiculo.Placa = vehiculo.Placa.ToUpper();

                    _context.Add(vehiculo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vehículo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Manejar errores de base de datos
                    if (ex.InnerException?.Message.Contains("UNIQUE constraint failed: Vehiculo.Placa") == true)
                    {
                        ModelState.AddModelError("Placa", "Ya existe un vehículo registrado con esta placa.");
                    }
                    else if (ex.InnerException?.Message.Contains("UNIQUE constraint failed: Vehiculo.NumeroTarjeta") == true ||
                             ex.InnerException?.Message.Contains("UNIQUE constraint failed: Vehiculo.numero_tarjeta") == true)
                    {
                        ModelState.AddModelError("NumeroTarjeta", "Ya existe un vehículo registrado con este número de tarjeta.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al guardar el vehículo. Por favor, intente nuevamente.");
                    }
                }
            }

            // Si hay error, recargar los dropdowns
            var residencia = await _context.Residencias.FindAsync(vehiculo.IdResidencia);
            if (residencia != null)
            {
                ViewData["IdCluster"] = new SelectList(_context.Clusters.OrderBy(c => c.Nombre), "IdCluster", "Nombre", residencia.IdCluster);
                ViewData["IdResidencia"] = new SelectList(
                    _context.Residencias.Where(r => r.IdCluster == residencia.IdCluster).OrderBy(r => r.Numero),
                    "IdResidencia",
                    "Numero",
                    vehiculo.IdResidencia);
            }
            else
            {
                ViewData["IdCluster"] = new SelectList(_context.Clusters.OrderBy(c => c.Nombre), "IdCluster", "Nombre");
                ViewData["IdResidencia"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(vehiculo);
        }

        // GET: Vehiculoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Residencia)
                .FirstOrDefaultAsync(v => v.IdVehiculo == id);

            if (vehiculo == null)
            {
                return NotFound();
            }

            ViewData["IdCluster"] = new SelectList(_context.Clusters.OrderBy(c => c.Nombre), "IdCluster", "Nombre", vehiculo.Residencia.IdCluster);
            ViewData["IdResidencia"] = new SelectList(
                _context.Residencias.Where(r => r.IdCluster == vehiculo.Residencia.IdCluster).OrderBy(r => r.Numero),
                "IdResidencia",
                "Numero",
                vehiculo.IdResidencia);

            return View(vehiculo);
        }

        // POST: Vehiculoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVehiculo,IdResidencia,Marca,Linea,Anio,Color,Placa,NumeroTarjeta")] Vehiculo vehiculo)
        {
            if (id != vehiculo.IdVehiculo)
            {
                return NotFound();
            }

            ModelState.Remove("Residencia");
            ModelState.Remove("Accesos");

            // Validar que la placa sea única (excepto para este vehículo)
            if (await _context.Vehiculos.AnyAsync(v => v.Placa.ToUpper() == vehiculo.Placa.ToUpper() && v.IdVehiculo != vehiculo.IdVehiculo))
            {
                ModelState.AddModelError("Placa", "Ya existe otro vehículo registrado con esta placa.");
            }

            // Validar que el número de tarjeta sea único (excepto para este vehículo)
            if (await _context.Vehiculos.AnyAsync(v => v.NumeroTarjeta == vehiculo.NumeroTarjeta && v.IdVehiculo != vehiculo.IdVehiculo))
            {
                ModelState.AddModelError("NumeroTarjeta", "Ya existe otro vehículo registrado con este número de tarjeta.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Convertir placa a mayúsculas antes de guardar
                    vehiculo.Placa = vehiculo.Placa.ToUpper();

                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vehículo actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.IdVehiculo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Manejar errores de base de datos
                    if (ex.InnerException?.Message.Contains("UNIQUE constraint failed: Vehiculo.Placa") == true)
                    {
                        ModelState.AddModelError("Placa", "Ya existe otro vehículo registrado con esta placa.");
                    }
                    else if (ex.InnerException?.Message.Contains("UNIQUE constraint failed: Vehiculo.NumeroTarjeta") == true ||
                             ex.InnerException?.Message.Contains("UNIQUE constraint failed: Vehiculo.numero_tarjeta") == true)
                    {
                        ModelState.AddModelError("NumeroTarjeta", "Ya existe otro vehículo registrado con este número de tarjeta.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al actualizar el vehículo. Por favor, intente nuevamente.");
                    }
                }
            }

            // Si hay error, recargar los dropdowns
            var residencia = await _context.Residencias.FindAsync(vehiculo.IdResidencia);
            if (residencia != null)
            {
                ViewData["IdCluster"] = new SelectList(_context.Clusters.OrderBy(c => c.Nombre), "IdCluster", "Nombre", residencia.IdCluster);
                ViewData["IdResidencia"] = new SelectList(
                    _context.Residencias.Where(r => r.IdCluster == residencia.IdCluster).OrderBy(r => r.Numero),
                    "IdResidencia",
                    "Numero",
                    vehiculo.IdResidencia);
            }
            else
            {
                ViewData["IdCluster"] = new SelectList(_context.Clusters.OrderBy(c => c.Nombre), "IdCluster", "Nombre");
                ViewData["IdResidencia"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(vehiculo);
        }

        // GET: Vehiculoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehiculo = await _context.Vehiculos
                .Include(v => v.Residencia)
                    .ThenInclude(r => r.Cluster)
                .FirstOrDefaultAsync(m => m.IdVehiculo == id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            return View(vehiculo);
        }

        // POST: Vehiculoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vehículo eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // API para obtener residencias por cluster (para AJAX)
        [HttpGet]
        public JsonResult GetResidenciasByCluster(int idCluster)
        {
            var residencias = _context.Residencias
                .Where(r => r.IdCluster == idCluster)
                .OrderBy(r => r.Numero)
                .Select(r => new {
                    value = r.IdResidencia,
                    text = r.Numero
                })
                .ToList();

            return Json(residencias);
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.IdVehiculo == id);
        }
    }
}