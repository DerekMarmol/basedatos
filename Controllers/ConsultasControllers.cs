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

        // Consulta #2: Visitas de hoy
        public async Task<IActionResult> Consulta02()
        {
            var hoy = DateTime.Today;
            var visitas = await _context.RegistrosVisita
                .Include(r => r.Visitante)
                .Include(r => r.Residencia)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.Garita)
                .Where(r => r.FechaIngreso.Date == hoy)
                .OrderByDescending(r => r.FechaIngreso)
                .Select(r => new
                {
                    r.IdRegistro,
                    Visitante = r.Visitante.NombreCompleto,
                    r.IdResidencia,
                    Residencia = r.Residencia.Numero,
                    Cluster = r.Residencia.Cluster.Nombre,
                    GuardiaIngreso = r.GuardiaIngreso.NombreCompleto,
                    Garita = r.Garita != null ? r.Garita.Nombre : "N/A",
                    r.FechaIngreso,
                    r.FechaSalida,
                    r.Motivo
                })
                .ToListAsync();

            ViewBag.Visitas = visitas;
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

        // Consulta #4: Intentos de personas no gratas
        public async Task<IActionResult> Consulta04(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var intentos = await _context.RegistrosVisita
                .Include(r => r.Visitante)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .Join(_context.PersonasNoGratas.Where(p => p.Activo),
                    rv => rv.Visitante.Dpi ?? rv.Visitante.NombreCompleto,
                    png => png.Dpi ?? png.NombreCompleto,
                    (rv, png) => new
                    {
                        rv.IdRegistro,
                        Visitante = rv.Visitante.NombreCompleto,
                        Dpi = rv.Visitante.Dpi,
                        MotivoBloqueo = png.Motivo,
                        rv.FechaIngreso
                    })
                .OrderByDescending(x => x.FechaIngreso)
                .ToListAsync();

            ViewBag.Intentos = intentos;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #5: Vecino que más ha pagado (CORREGIDA)
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
                    IdCasa = g.Key.IdResidencia, // Agregamos esto para compatibilidad con la vista
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

        // Consulta #9: Top casas con más visitas
        public async Task<IActionResult> Consulta09(DateTime? fechaIni, DateTime? fechaFin, int topN = 5)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var topCasas = await _context.RegistrosVisita
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .GroupBy(r => new {
                    r.IdResidencia,
                    Cluster = r.Residencia.Cluster.Nombre,
                    Casa = r.Residencia.Numero
                })
                .Select(g => new
                {
                    g.Key.IdResidencia,
                    g.Key.Cluster,
                    g.Key.Casa,
                    TotalVisitas = g.Count()
                })
                .OrderByDescending(x => x.TotalVisitas)
                .Take(topN)
                .ToListAsync();

            ViewBag.TopCasas = topCasas;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            ViewBag.TopN = topN;
            return View();
        }

        // Consulta #10: Guardia con más ingresos
        public async Task<IActionResult> Consulta10(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var resultado = await _context.RegistrosVisita
                .Include(r => r.GuardiaIngreso)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .GroupBy(r => new { r.GuardiaIngreso.IdGuardia, r.GuardiaIngreso.NombreCompleto })
                .Select(g => new
                {
                    g.Key.IdGuardia,
                    g.Key.NombreCompleto,
                    IngresosAtendidos = g.Count()
                })
                .OrderByDescending(x => x.IngresosAtendidos)
                .FirstOrDefaultAsync();

            ViewBag.Resultado = resultado;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #11: Visitas abiertas (sin salida)
        public async Task<IActionResult> Consulta11()
        {
            var visitasAbiertas = await _context.RegistrosVisita
                .Include(r => r.Visitante)
                .Include(r => r.GuardiaIngreso)
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Where(r => r.FechaSalida == null)
                .OrderByDescending(r => r.FechaIngreso)
                .Select(r => new
                {
                    r.IdRegistro,
                    Visitante = r.Visitante.NombreCompleto,
                    r.IdResidencia,
                    Cluster = r.Residencia.Cluster.Nombre,
                    Casa = r.Residencia.Numero,
                    GuardiaIngreso = r.GuardiaIngreso.NombreCompleto,
                    r.FechaIngreso
                })
                .ToListAsync();

            ViewBag.VisitasAbiertas = visitasAbiertas;
            return View();
        }

        // Consulta #12: Vivienda más atrasada en pagos (CORREGIDA - SQLite decimal fix)
        public async Task<IActionResult> Consulta12()
        {
            var estadosCuenta = await _context.EstadosCuenta
                .Include(e => e.Residencia)
                    .ThenInclude(r => r.Cluster)
                .Include(e => e.Residencia)
                    .ThenInclude(r => r.Propietario)
                .ToListAsync(); // Traemos todo a memoria primero para evitar el error de SQLite

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

        // Consulta #13: Día del mes con mayor recaudación (CORREGIDA - SQLite decimal fix)
        public async Task<IActionResult> Consulta13(int anio)
        {
            if (anio == 0) anio = DateTime.Today.Year;

            var recibos = await _context.Recibos
                .Where(r => r.Fecha.Year == anio)
                .ToListAsync(); // Traemos todo a memoria primero para evitar el error de SQLite

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

        // Consulta #15: Vehículos no permitidos detectados
        public async Task<IActionResult> Consulta15(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var vehiculosDetectados = await _context.RegistrosVisita
                .Include(r => r.Visitante)
                .Include(r => r.Residencia)
                    .ThenInclude(res => res.Cluster)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date
                         && r.Visitante.Placa != null)
                .Join(_context.VehiculosNoPermitidos.Where(v => v.Activo),
                    rv => rv.Visitante.Placa,
                    vnp => vnp.Placa,
                    (rv, vnp) => new
                    {
                        rv.IdRegistro,
                        Visitante = rv.Visitante.NombreCompleto,
                        Placa = rv.Visitante.Placa,
                        Cluster = rv.Residencia.Cluster.Nombre,
                        Casa = rv.Residencia.Numero,
                        rv.FechaIngreso,
                        Motivo = vnp.Motivo
                    })
                .OrderByDescending(x => x.FechaIngreso)
                .ToListAsync();

            ViewBag.VehiculosDetectados = vehiculosDetectados;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            return View();
        }

        // Consulta #16: Visitantes frecuentes
        public async Task<IActionResult> Consulta16(DateTime? fechaIni, DateTime? fechaFin, int minVisitas = 2)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddMonths(-1);
                fechaFin = DateTime.Today;
            }

            var visitantesFrecuentes = await _context.RegistrosVisita
                .Include(r => r.Visitante)
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .GroupBy(r => new { r.Visitante.IdVisitante, r.Visitante.NombreCompleto })
                .Select(g => new
                {
                    g.Key.IdVisitante,
                    g.Key.NombreCompleto,
                    TotalVisitas = g.Count()
                })
                .Where(x => x.TotalVisitas >= minVisitas)
                .OrderByDescending(x => x.TotalVisitas)
                .ToListAsync();

            ViewBag.VisitantesFrecuentes = visitantesFrecuentes;
            ViewBag.FechaIni = fechaIni;
            ViewBag.FechaFin = fechaFin;
            ViewBag.MinVisitas = minVisitas;
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

        // Consulta #20: Bitácora diaria (ingresos y salidas)
        public async Task<IActionResult> Consulta20(DateTime? fechaIni, DateTime? fechaFin)
        {
            if (!fechaIni.HasValue || !fechaFin.HasValue)
            {
                fechaIni = DateTime.Today.AddDays(-7);
                fechaFin = DateTime.Today;
            }

            var bitacora = await _context.RegistrosVisita
                .Where(r => r.FechaIngreso.Date >= fechaIni.Value.Date
                         && r.FechaIngreso.Date <= fechaFin.Value.Date)
                .GroupBy(r => r.FechaIngreso.Date)
                .Select(g => new
                {
                    Dia = g.Key,
                    Ingresos = g.Count(),
                    Salidas = g.Count(r => r.FechaSalida != null)
                })
                .OrderBy(x => x.Dia)
                .ToListAsync();

            ViewBag.Bitacora = bitacora;
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

        // NUEVAS CONSULTAS ADICIONALES

        // Consulta Extra: Guardia que ha trabajado más de 24 horas
        public async Task<IActionResult> Consulta09Extra(DateTime? fechaIni, DateTime? fechaFin)
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

        // Consulta Extra: Guardias por género
        public async Task<IActionResult> Consulta10Extra()
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
            return View();
        }
    }
}