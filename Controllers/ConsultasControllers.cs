using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ARSAN_Web.Data;

namespace ARSAN_Web.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Consultas
        public IActionResult Index()
        {
            return View();
        }

        // Consulta #1: Vehículos por vivienda
        public async Task<IActionResult> Consulta01(string? cluster, int? numeroCasa)
        {
            if (string.IsNullOrEmpty(cluster) || !numeroCasa.HasValue)
            {
                ViewBag.Clusters = await _context.Clusters.OrderBy(c => c.Nombre).ToListAsync();
                return View();
            }

            var resultado = await _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Propietario)
                .Include(r => r.Vehiculos)
                .Where(r => r.Cluster.Nombre == cluster && r.Numero == numeroCasa)
                .Select(r => new
                {
                    Cluster = r.Cluster.Nombre,
                    NumeroCasa = r.Numero,
                    Propietario = r.Propietario.NombreCompleto,
                    CantidadVehiculos = r.Vehiculos.Count,
                    CargoExtra = r.Vehiculos.Count > 4
                        ? $"Debe pagar Q150 adicionales por {r.Vehiculos.Count - 4} vehículo(s) extra"
                        : "Sin cargo adicional"
                })
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.Clusters = await _context.Clusters.OrderBy(c => c.Nombre).ToListAsync();
            return View();
        }

        // Consulta #2: Vehículos de visitantes por hora
        public async Task<IActionResult> Consulta02(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddDays(-7);
                fechaFin = DateTime.Today;
            }

            // Obtener accesos vehiculares de visitantes
            var accesos = await _context.AccesosVehiculares
                .Include(a => a.Visitante)
                .Where(a => a.FechaIngreso.Date >= fechaIni.Value.Date
                         && a.FechaIngreso.Date <= fechaFin.Value.Date
                         && a.TipoAcceso == "Visitante")
                .ToListAsync();

            // Agrupar por hora
            var vehiculosPorHora = accesos
                .GroupBy(a => a.HoraIngreso.Hours)
                .Select(g => new
                {
                    Hora = g.Key,
                    CantidadVehiculos = g.Count()
                })
                .OrderBy(x => x.Hora)
                .ToList();

            ViewBag.VehiculosPorHora = vehiculosPorHora;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            ViewBag.TotalVehiculos = accesos.Count;
            return View();
        }

        // Consulta #3: Reporte de viviendas
        public async Task<IActionResult> Consulta03()
        {
            var viviendas = await _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Propietario)
                .Include(r => r.Inquilino)
                .OrderBy(r => r.Cluster.Nombre)
                .ThenBy(r => r.Numero)
                .Select(r => new
                {
                    Cluster = r.Cluster.Nombre,
                    NumeroCasa = r.Numero,
                    r.Estado,
                    Propietario = r.Propietario.NombreCompleto,
                    DpiPropietario = r.Propietario.Dpi,
                    TelefonoPropietario = r.Propietario.Telefono,
                    Inquilino = r.Inquilino != null ? r.Inquilino.NombreCompleto : "Sin inquilino",
                    DpiInquilino = r.Inquilino != null ? r.Inquilino.Dpi : "N/A",
                    TelefonoInquilino = r.Inquilino != null ? r.Inquilino.Telefono : "N/A"
                })
                .ToListAsync();

            ViewBag.Viviendas = viviendas;
            return View();
        }

        // Consulta #4: Vehículos que ingresan entre 23:00 y 01:00
        public async Task<IActionResult> Consulta04(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var accesos = await _context.AccesosVehiculares
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v.Residencia)
                        .ThenInclude(r => r.Propietario)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v.Residencia)
                        .ThenInclude(r => r.Cluster)
                .Where(a => a.FechaIngreso.Date >= fechaIni.Value.Date
                         && a.FechaIngreso.Date <= fechaFin.Value.Date
                         && a.Vehiculo != null)
                .ToListAsync();

            // Filtrar en memoria por hora (23:00 a 01:00)
            var vehiculosNocturnos = accesos
                .Where(a => a.HoraIngreso.Hours >= 23 || a.HoraIngreso.Hours <= 1)
                .GroupBy(a => new
                {
                    a.Placa,
                    Propietario = a.Vehiculo.Residencia.Propietario.NombreCompleto,
                    Cluster = a.Vehiculo.Residencia.Cluster.Nombre,
                    Casa = a.Vehiculo.Residencia.Numero
                })
                .Select(g => new
                {
                    g.Key.Placa,
                    g.Key.Propietario,
                    g.Key.Cluster,
                    g.Key.Casa,
                    TotalIngresos = g.Count(),
                    Ingresos = g.Select(a => new
                    {
                        Fecha = a.FechaIngreso,
                        Hora = a.HoraIngreso
                    }).OrderByDescending(x => x.Fecha).ToList()
                })
                .OrderByDescending(x => x.TotalIngresos)
                .ToList();

            ViewBag.VehiculosNocturnos = vehiculosNocturnos;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #5: Vecino que más ha pagado
        public async Task<IActionResult> Consulta05(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = new DateTime(DateTime.Today.Year, 1, 1);
                fechaFin = DateTime.Today;
            }

            var pagos = await _context.PagosMantenimiento
                .Include(p => p.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Include(p => p.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Where(p => p.FechaPago.Date >= fechaIni.Value.Date
                         && p.FechaPago.Date <= fechaFin.Value.Date)
                .ToListAsync();

            var resultado = pagos
                .GroupBy(p => new {
                    p.IdResidencia,
                    Propietario = p.Residencia.Propietario.NombreCompleto,
                    Cluster = p.Residencia.Cluster.Nombre,
                    Casa = p.Residencia.Numero
                })
                .Select(g => new
                {
                    IdResidencia = g.Key.IdResidencia,
                    IdCasa = g.Key.IdResidencia,
                    Propietario = g.Key.Propietario,
                    Cluster = g.Key.Cluster,
                    Casa = g.Key.Casa,
                    TotalPagado = g.Sum(p => p.Monto)
                })
                .OrderByDescending(x => x.TotalPagado)
                .FirstOrDefault();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #6: Propietarios Diana II casados menores de 30
        public async Task<IActionResult> Consulta06()
        {
            var propietarios = await _context.Residencias
                .Include(r => r.Propietario)
                .Include(r => r.Cluster)
                .Where(r => r.Cluster.Nombre == "Diana II"
                         && r.Propietario.EstadoCivil == "Casado"
                         && r.Propietario.FechaNacimiento.HasValue
                         && DateTime.Today.Year - r.Propietario.FechaNacimiento.Value.Year < 30)
                .Select(r => new
                {
                    r.Propietario.IdPropietario,
                    r.Propietario.NombreCompleto,
                    r.Propietario.Dpi,
                    r.Propietario.Telefono,
                    r.Propietario.FechaNacimiento,
                    Edad = DateTime.Today.Year - r.Propietario.FechaNacimiento.Value.Year,
                    r.Propietario.EstadoCivil,
                    NumeroCasa = r.Numero,
                    Cluster = r.Cluster.Nombre
                })
                .ToListAsync();

            ViewBag.Propietarios = propietarios;
            return View();
        }

        // Consulta #7: Más veces presidente
        public async Task<IActionResult> Consulta07()
        {
            var resultado = await _context.MiembrosJunta
                .Include(m => m.Propietario)
                .Include(m => m.Cargo)
                .Include(m => m.JuntaDirectiva)
                .Where(m => m.Cargo.Nombre == "Presidente")
                .GroupBy(m => new {
                    m.Propietario.IdPropietario,
                    m.Propietario.NombreCompleto,
                    m.Propietario.Dpi,
                    m.Propietario.Telefono
                })
                .Select(g => new
                {
                    g.Key.IdPropietario,
                    g.Key.NombreCompleto,
                    g.Key.Dpi,
                    g.Key.Telefono,
                    VecesPresidente = g.Count(),
                    Anios = string.Join(", ", g.Select(m => m.JuntaDirectiva.AnioInicio))
                })
                .OrderByDescending(x => x.VecesPresidente)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            return View();
        }

        // Consulta #8: Nunca miembro de junta
        public async Task<IActionResult> Consulta08()
        {
            var propietariosConJunta = await _context.MiembrosJunta
                .Select(m => m.IdPropietario)
                .Distinct()
                .ToListAsync();

            var propietarios = await _context.Residencias
                .Include(r => r.Propietario)
                .Include(r => r.Cluster)
                .Where(r => !propietariosConJunta.Contains(r.IdPropietario))
                .Select(r => new
                {
                    r.Propietario.IdPropietario,
                    r.Propietario.NombreCompleto,
                    r.Propietario.Dpi,
                    r.Propietario.Telefono,
                    Cluster = r.Cluster.Nombre,
                    NumeroCasa = r.Numero
                })
                .Distinct()
                .OrderBy(x => x.NombreCompleto)
                .ToListAsync();

            ViewBag.Propietarios = propietarios;
            return View();
        }

        // Consulta #9: Guardia que ha trabajado más de 24 horas
        public async Task<IActionResult> Consulta09(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var turnos = await _context.TurnosGuardia
                .Include(t => t.Guardia)
                .Include(t => t.Garita)
                .Where(t => t.FechaInicio.Date >= fechaIni.Value.Date
                         && t.FechaInicio.Date <= fechaFin.Value.Date
                         && t.FechaFin != null)
                .ToListAsync();

            var guardiasSobretiempo = turnos
                .Select(t => new
                {
                    t.IdGuardia,
                    Guardia = t.Guardia.NombreCompleto,
                    Garita = t.Garita.Nombre,
                    t.FechaInicio,
                    t.FechaFin,
                    HorasTrabajadas = (t.FechaFin.Value - t.FechaInicio).TotalHours
                })
                .Where(x => x.HorasTrabajadas > 24)
                .OrderByDescending(x => x.HorasTrabajadas)
                .ToList();

            ViewBag.GuardiasSobretiempo = guardiasSobretiempo;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #10: Guardias por género
        public async Task<IActionResult> Consulta10()
        {
            var guardiasPorGenero = await _context.Guardias
                .Where(g => g.Activo)
                .GroupBy(g => g.Genero ?? "No especificado")
                .Select(g => new
                {
                    Genero = g.Key == "M" ? "Masculino" : g.Key == "F" ? "Femenino" : "No especificado",
                    Cantidad = g.Count()
                })
                .ToListAsync();

            ViewBag.GuardiasPorGenero = guardiasPorGenero;
            ViewBag.TotalGuardias = guardiasPorGenero.Sum(g => g.Cantidad);
            return View();
        }

        // Consulta #11: Vecino que más ingresa/sale los domingos
        public async Task<IActionResult> Consulta11(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-3);
                fechaFin = DateTime.Today;
            }

            var accesosDomingos = await _context.AccesosVehiculares
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v.Residencia)
                        .ThenInclude(r => r.Propietario)
                .Include(a => a.Vehiculo)
                    .ThenInclude(v => v.Residencia)
                        .ThenInclude(r => r.Cluster)
                .Where(a => a.FechaIngreso.Date >= fechaIni.Value.Date
                         && a.FechaIngreso.Date <= fechaFin.Value.Date
                         && a.Vehiculo != null
                         && a.TipoAcceso == "Residente")
                .ToListAsync();

            // Filtrar solo domingos
            var accesosDomingosFiltrados = accesosDomingos
                .Where(a => a.FechaIngreso.DayOfWeek == DayOfWeek.Sunday)
                .ToList();

            var resultado = accesosDomingosFiltrados
                .GroupBy(a => new
                {
                    IdResidencia = a.Vehiculo.IdResidencia,
                    Propietario = a.Vehiculo.Residencia.Propietario.NombreCompleto,
                    Cluster = a.Vehiculo.Residencia.Cluster.Nombre,
                    Casa = a.Vehiculo.Residencia.Numero
                })
                .Select(g => new
                {
                    g.Key.IdResidencia,
                    g.Key.Propietario,
                    g.Key.Cluster,
                    g.Key.Casa,
                    TotalMovimientos = g.Count(),
                    Domingos = g.Select(a => a.FechaIngreso.Date).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalMovimientos)
                .FirstOrDefault();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #12: Vivienda más atrasada en pagos
        public async Task<IActionResult> Consulta12()
        {
            var estadosCuenta = await _context.EstadosCuenta
                .Include(e => e.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Include(e => e.Residencia)
                    .ThenInclude(r => r.Propietario)
                .ToListAsync();

            var resultado = estadosCuenta
                .OrderByDescending(e => e.SaldoPendiente)
                .Select(e => new
                {
                    e.IdResidencia,
                    Cluster = e.Residencia.Cluster.Nombre,
                    Casa = e.Residencia.Numero,
                    Propietario = e.Residencia.Propietario.NombreCompleto,
                    e.SaldoPendiente
                })
                .FirstOrDefault();

            ViewBag.Resultado = resultado;
            return View();
        }

        // Consulta #13: Día del mes con mayor recaudación
        public async Task<IActionResult> Consulta13(int anio)
        {
            if (anio == 0) anio = DateTime.Today.Year;

            var recibos = await _context.Recibos
                .Where(r => r.Fecha.Year == anio)
                .ToListAsync();

            var resultado = recibos
                .GroupBy(r => r.Fecha.Day)
                .Select(g => new
                {
                    DiaDeMes = g.Key,
                    Recaudado = g.Sum(r => r.Total)
                })
                .OrderByDescending(x => x.Recaudado)
                .FirstOrDefault();

            ViewBag.Resultado = resultado;
            ViewBag.Anio = anio;
            return View();
        }

        // Consulta #14: Residencia con más recibos
        public async Task<IActionResult> Consulta14(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = new DateTime(DateTime.Today.Year, 1, 1);
                fechaFin = DateTime.Today;
            }

            var resultado = await _context.Recibos
                .Include(r => r.Pago)
                    .ThenInclude(p => p.Residencia)
                        .ThenInclude(res => res.Cluster)
                .Where(r => r.Fecha.Date >= fechaIni.Value.Date
                         && r.Fecha.Date <= fechaFin.Value.Date)
                .GroupBy(r => new {
                    r.Pago.IdResidencia,
                    Cluster = r.Pago.Residencia.Cluster.Nombre,
                    Casa = r.Pago.Residencia.Numero
                })
                .Select(g => new
                {
                    g.Key.IdResidencia,
                    g.Key.Cluster,
                    g.Key.Casa,
                    TotalRecibos = g.Count()
                })
                .OrderByDescending(x => x.TotalRecibos)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #15: Carros por mes y día
        public async Task<IActionResult> Consulta15(int? anio, int? mes)
        {
            if (!anio.HasValue) anio = DateTime.Today.Year;
            if (!mes.HasValue) mes = DateTime.Today.Month;

            var accesos = await _context.AccesosVehiculares
                .Where(a => a.FechaIngreso.Year == anio.Value
                         && a.FechaIngreso.Month == mes.Value)
                .ToListAsync();

            var carrosPorDia = accesos
                .GroupBy(a => a.FechaIngreso.Day)
                .Select(g => new
                {
                    Dia = g.Key,
                    CantidadCarros = g.Count()
                })
                .OrderBy(x => x.Dia)
                .ToList();

            ViewBag.CarrosPorDia = carrosPorDia;
            ViewBag.Anio = anio;
            ViewBag.Mes = mes;
            ViewBag.TotalCarros = accesos.Count;

            var meses = new[] { "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            ViewBag.NombreMes = meses[mes.Value];

            return View();
        }

        // Consulta #16: Persona que más visita el condominio
        public async Task<IActionResult> Consulta16(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-3);
                fechaFin = DateTime.Today;
            }

            var resultado = await _context.RegistrosVisita
                .Include(r => r.Visitante)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .GroupBy(r => new {
                    r.Visitante.IdVisitante,
                    r.Visitante.NombreCompleto,
                    r.Visitante.Dpi,
                    r.Visitante.Telefono
                })
                .Select(g => new
                {
                    g.Key.IdVisitante,
                    g.Key.NombreCompleto,
                    g.Key.Dpi,
                    g.Key.Telefono,
                    TotalVisitas = g.Count(),
                    UltimaVisita = g.Max(r => r.FechaIngreso)
                })
                .OrderByDescending(x => x.TotalVisitas)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #17: Inquilinos que son propietarios
        public async Task<IActionResult> Consulta17()
        {
            var inquilinosPropietarios = await _context.Inquilinos
                .Join(_context.Propietarios,
                    i => i.Dpi,
                    p => p.Dpi,
                    (i, p) => new { Inquilino = i, Propietario = p })
                .SelectMany(
                    x => _context.Residencias.Include(r => r.Cluster)
                        .Where(r => r.IdInquilino == x.Inquilino.IdInquilino),
                    (x, r1) => new { x.Inquilino, x.Propietario, ResidenciaInquilino = r1 })
                .SelectMany(
                    x => _context.Residencias.Include(r => r.Cluster)
                        .Where(r => r.IdPropietario == x.Propietario.IdPropietario),
                    (x, r2) => new
                    {
                        IdInquilino = x.Inquilino.IdInquilino,
                        NombreInquilino = x.Inquilino.NombreCompleto,
                        Dpi = x.Inquilino.Dpi,
                        ClusterInquilino = x.ResidenciaInquilino.Cluster.Nombre,
                        CasaInquilino = x.ResidenciaInquilino.Numero,
                        IdPropietario = x.Propietario.IdPropietario,
                        ClusterPropietario = r2.Cluster.Nombre,
                        CasaPropietario = r2.Numero
                    })
                .OrderBy(x => x.NombreInquilino)
                .ToListAsync();

            ViewBag.InquilinosPropietarios = inquilinosPropietarios;
            return View();
        }

        // Consulta #18: Propietarios con licencia tipo A
        public async Task<IActionResult> Consulta18()
        {
            var propietarios = await _context.Residencias
                .Include(r => r.Propietario)
                .Include(r => r.Cluster)
                .Where(r => r.Propietario.TipoLicencia == "A")
                .Select(r => new
                {
                    r.Propietario.IdPropietario,
                    r.Propietario.NombreCompleto,
                    r.Propietario.Dpi,
                    r.Propietario.Telefono,
                    r.Propietario.TipoLicencia,
                    Cluster = r.Cluster.Nombre,
                    NumeroCasa = r.Numero
                })
                .Distinct()
                .OrderBy(x => x.NombreCompleto)
                .ToListAsync();

            ViewBag.Total = propietarios.Count;
            ViewBag.Propietarios = propietarios;
            return View();
        }

        // Consulta #19: Debe cobro extra por vehículos
        public async Task<IActionResult> Consulta19(string? cluster, int? numeroCasa)
        {
            if (string.IsNullOrEmpty(cluster) || !numeroCasa.HasValue)
            {
                ViewBag.Clusters = await _context.Clusters.OrderBy(c => c.Nombre).ToListAsync();
                return View();
            }

            var residencia = await _context.Residencias
                .Include(r => r.Cluster)
                .Include(r => r.Propietario)
                .Include(r => r.Vehiculos)
                .Where(r => r.Cluster.Nombre == cluster && r.Numero == numeroCasa)
                .FirstOrDefaultAsync();

            if (residencia != null)
            {
                var cantidadVehiculos = residencia.Vehiculos.Count;
                var vehiculosExtra = cantidadVehiculos > 4 ? cantidadVehiculos - 4 : 0;
                var montoExtra = vehiculosExtra * 150.00m;

                ViewBag.Cluster = cluster;
                ViewBag.NumeroCasa = numeroCasa;
                ViewBag.Propietario = residencia.Propietario.NombreCompleto;
                ViewBag.TotalVehiculos = cantidadVehiculos;
                ViewBag.DebePagarExtra = cantidadVehiculos > 4 ? "Sí" : "No";
                ViewBag.VehiculosExtra = vehiculosExtra;
                ViewBag.MontoExtra = montoExtra;
                ViewBag.Vehiculos = residencia.Vehiculos;
            }

            ViewBag.Clusters = await _context.Clusters.OrderBy(c => c.Nombre).ToListAsync();
            return View();
        }

        // Consulta #20: Residencia con más visitas
        public async Task<IActionResult> Consulta20(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var resultado = await _context.RegistrosVisita
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Propietario)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .GroupBy(r => new {
                    r.IdResidencia,
                    Cluster = r.Residencia.Cluster.Nombre,
                    Casa = r.Residencia.Numero,
                    Propietario = r.Residencia.Propietario.NombreCompleto
                })
                .Select(g => new
                {
                    g.Key.IdResidencia,
                    g.Key.Cluster,
                    g.Key.Casa,
                    g.Key.Propietario,
                    TotalVisitas = g.Count()
                })
                .OrderByDescending(x => x.TotalVisitas)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #21: Concepto de multa más utilizado
        public async Task<IActionResult> Consulta21(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = new DateTime(DateTime.Today.Year, 1, 1);
                fechaFin = DateTime.Today;
            }

            var resultado = await _context.Multas
                .Where(m => m.Fecha.Date >= fechaIni.Value.Date
                         && m.Fecha.Date <= fechaFin.Value.Date)
                .GroupBy(m => m.Concepto)
                .Select(g => new
                {
                    Concepto = g.Key,
                    Veces = g.Count()
                })
                .OrderByDescending(x => x.Veces)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #22: Casas con multas pendientes
        public async Task<IActionResult> Consulta22()
        {
            var multasPendientes = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Where(m => !m.Pagada)
                .OrderBy(m => m.IdResidencia)
                .ThenByDescending(m => m.Fecha)
                .Select(m => new
                {
                    m.IdMulta,
                    m.IdResidencia,
                    Cluster = m.Residencia.Cluster.Nombre,
                    Casa = m.Residencia.Numero,
                    m.Concepto,
                    m.Monto,
                    m.Fecha
                })
                .ToListAsync();

            ViewBag.MultasPendientes = multasPendientes;
            return View();
        }

        // Consulta #23: Mes con más multas por desorden
        public async Task<IActionResult> Consulta23(int anio)
        {
            if (anio == 0) anio = DateTime.Today.Year;

            var resultado = await _context.Multas
                .Where(m => m.Concepto == "Desorden" && m.Fecha.Year == anio)
                .GroupBy(m => m.Fecha.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.Anio = anio;
            return View();
        }

        // Consulta #24: Total recaudado por multas
        public async Task<IActionResult> Consulta24(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = new DateTime(DateTime.Today.Year, 1, 1);
                fechaFin = DateTime.Today;
            }

            var totalRecaudado = await _context.PagosMulta
                .Where(p => p.FechaPago.Date >= fechaIni.Value.Date
                         && p.FechaPago.Date <= fechaFin.Value.Date)
                .SumAsync(p => (decimal?)p.MontoPagado) ?? 0;

            ViewBag.TotalRecaudado = totalRecaudado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #25: Propietario con más multas
        public async Task<IActionResult> Consulta25(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = new DateTime(DateTime.Today.Year, 1, 1);
                fechaFin = DateTime.Today;
            }

            var resultado = await _context.Multas
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Propietario)
                .Include(m => m.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Where(m => m.Fecha.Date >= fechaIni.Value.Date
                         && m.Fecha.Date <= fechaFin.Value.Date)
                .GroupBy(m => new {
                    m.IdResidencia,
                    Propietario = m.Residencia.Propietario.NombreCompleto,
                    Cluster = m.Residencia.Cluster.Nombre,
                    Casa = m.Residencia.Numero
                })
                .Select(g => new
                {
                    g.Key.IdResidencia,
                    g.Key.Propietario,
                    g.Key.Cluster,
                    g.Key.Casa,
                    TotalMultas = g.Count()
                })
                .OrderByDescending(x => x.TotalMultas)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }
    }
}