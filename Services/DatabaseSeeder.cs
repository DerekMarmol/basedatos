using ARSAN_Web.Data;
using ARSAN_Web.Models;

namespace ARSAN_Web.Services
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Verificar si ya hay datos
            if (context.Residenciales.Any()) return;

            // 1. Crear Residencial (Multi-tenant)
            var residencial = new Residencial
            {
                Codigo = "RSAN-F",
                Nombre = "Residenciales San Francisco",
                Direccion = "Zona 8, Mixco",
                Municipio = "Mixco",
                Departamento = "Guatemala",
                Telefono = "2222-3333",
                Email = "admin@sanfrancisco.com",
                CantidadViviendas = 420,
                FechaInicioAdministracion = new DateTime(2022, 1, 1),
                Activo = true
            };
            context.Residenciales.Add(residencial);
            context.SaveChanges();

            // 2. Crear Clusters
            var clusters = new List<Cluster>
            {
                new Cluster { Nombre = "Andalucía", IdResidencial = residencial.IdResidencial },
                new Cluster { Nombre = "Bernarda", IdResidencial = residencial.IdResidencial },
                new Cluster { Nombre = "Catalina", IdResidencial = residencial.IdResidencial },
                new Cluster { Nombre = "Diana I", IdResidencial = residencial.IdResidencial },
                new Cluster { Nombre = "Diana II", IdResidencial = residencial.IdResidencial },
                new Cluster { Nombre = "Esmeralda", IdResidencial = residencial.IdResidencial }
            };
            context.Clusters.AddRange(clusters);
            context.SaveChanges();

            // 3. Crear Garitas
            var garitas = new List<Garita>
            {
                new Garita { Nombre = "Garita Principal", IdCluster = null, Ubicacion = "Entrada principal", Activa = true },
                new Garita { Nombre = "Garita Andalucía", IdCluster = clusters[0].IdCluster, Ubicacion = "Cluster Andalucía", Activa = true },
                new Garita { Nombre = "Garita Esmeralda", IdCluster = clusters[5].IdCluster, Ubicacion = "Cluster Esmeralda", Activa = true }
            };
            context.Garitas.AddRange(garitas);
            context.SaveChanges();

            // 4. Crear Cargos
            var cargos = new List<Cargo>
            {
                new Cargo { Nombre = "Presidente" },
                new Cargo { Nombre = "Vicepresidente" },
                new Cargo { Nombre = "Tesorero" },
                new Cargo { Nombre = "Secretario" }
            };
            context.Cargos.AddRange(cargos);
            context.SaveChanges();

            // 5. Crear Propietarios
            var propietarios = new List<Propietario>
            {
                new Propietario { NombreCompleto = "Carlos Méndez López", Dpi = "2001567890101", Telefono = "55551234", FechaNacimiento = new DateTime(1996, 3, 15), EstadoCivil = "Casado", TipoLicencia = "A" },
                new Propietario { NombreCompleto = "María García Pérez", Dpi = "2002567890102", Telefono = "55551235", FechaNacimiento = new DateTime(1988, 7, 20), EstadoCivil = "Soltera", TipoLicencia = "B" },
                new Propietario { NombreCompleto = "Pedro Hernández López", Dpi = "1001567890105", Telefono = "55551238", FechaNacimiento = new DateTime(1985, 5, 15), EstadoCivil = "Casado", TipoLicencia = "A" },
                new Propietario { NombreCompleto = "Laura González Ruiz", Dpi = "1002567890106", Telefono = "55551239", FechaNacimiento = new DateTime(1988, 8, 20), EstadoCivil = "Soltera", TipoLicencia = "B" },
                new Propietario { NombreCompleto = "Roberto Díaz Morales", Dpi = "1003567890107", Telefono = "55551240", FechaNacimiento = new DateTime(1992, 2, 10), EstadoCivil = "Casado", TipoLicencia = "A" }
            };
            context.Propietarios.AddRange(propietarios);
            context.SaveChanges();

            // 6. Crear Inquilinos
            var inquilinos = new List<Inquilino>
            {
                new Inquilino { NombreCompleto = "Luis Pérez Gómez", Dpi = "3001567890201", Telefono = "55552001", FechaNacimiento = new DateTime(1994, 3, 10), EstadoCivil = "Soltero", TipoLicencia = "B" },
                new Inquilino { NombreCompleto = "Elena Rodríguez Salazar", Dpi = "3002567890202", Telefono = "55552002", FechaNacimiento = new DateTime(1996, 7, 15), EstadoCivil = "Soltera", TipoLicencia = "A" }
            };
            context.Inquilinos.AddRange(inquilinos);
            context.SaveChanges();

            // 7. Crear Residencias
            var residencias = new List<Residencia>
            {
                new Residencia { Numero = 5, IdCluster = clusters[4].IdCluster, IdPropietario = propietarios[0].IdPropietario, Estado = "ocupada" },
                new Residencia { Numero = 10, IdCluster = clusters[4].IdCluster, IdPropietario = propietarios[1].IdPropietario, Estado = "ocupada" },
                new Residencia { Numero = 15, IdCluster = clusters[0].IdCluster, IdPropietario = propietarios[2].IdPropietario, Estado = "ocupada" },
                new Residencia { Numero = 25, IdCluster = clusters[0].IdCluster, IdPropietario = propietarios[3].IdPropietario, IdInquilino = inquilinos[0].IdInquilino, Estado = "alquilada" },
                new Residencia { Numero = 8, IdCluster = clusters[1].IdCluster, IdPropietario = propietarios[4].IdPropietario, IdInquilino = inquilinos[1].IdInquilino, Estado = "alquilada" }
            };
            context.Residencias.AddRange(residencias);
            context.SaveChanges();

            // 8. Crear Vehículos
            var vehiculos = new List<Vehiculo>
            {
                new Vehiculo { IdResidencia = residencias[0].IdResidencia, Marca = "Toyota", Linea = "Corolla", Anio = 2020, Color = "Blanco", Placa = "P-100ABC", NumeroTarjeta = "T-001" },
                new Vehiculo { IdResidencia = residencias[0].IdResidencia, Marca = "Honda", Linea = "Civic", Anio = 2019, Color = "Negro", Placa = "P-101DEF", NumeroTarjeta = "T-002" },
                new Vehiculo { IdResidencia = residencias[2].IdResidencia, Marca = "Hyundai", Linea = "Elantra", Anio = 2021, Color = "Plata", Placa = "P-300ABC", NumeroTarjeta = "T-020" },
                new Vehiculo { IdResidencia = residencias[3].IdResidencia, Marca = "Toyota", Linea = "RAV4", Anio = 2021, Color = "Gris", Placa = "P-200ABC", NumeroTarjeta = "T-010" },
                new Vehiculo { IdResidencia = residencias[3].IdResidencia, Marca = "Nissan", Linea = "Sentra", Anio = 2020, Color = "Rojo", Placa = "P-201DEF", NumeroTarjeta = "T-011" },
                new Vehiculo { IdResidencia = residencias[3].IdResidencia, Marca = "Mazda", Linea = "CX-5", Anio = 2022, Color = "Azul", Placa = "P-202GHI", NumeroTarjeta = "T-012" },
                new Vehiculo { IdResidencia = residencias[3].IdResidencia, Marca = "Ford", Linea = "Escape", Anio = 2019, Color = "Verde", Placa = "P-203JKL", NumeroTarjeta = "T-013" },
                new Vehiculo { IdResidencia = residencias[3].IdResidencia, Marca = "Kia", Linea = "Sportage", Anio = 2023, Color = "Blanco", Placa = "P-204MNO", NumeroTarjeta = "T-014" }
            };
            context.Vehiculos.AddRange(vehiculos);
            context.SaveChanges();

            // 9. Crear Guardias
            var guardias = new List<Guardia>
            {
                new Guardia { NombreCompleto = "Luis Gómez", Dpi = "1111111111111", Telefono = "5551-0001", FechaContratacion = new DateTime(2024, 1, 10), Turno = "D", Activo = true, IdResidencial = residencial.IdResidencial, Genero = "M" },
                new Guardia { NombreCompleto = "Ana Díaz", Dpi = "2222222222222", Telefono = "5551-0002", FechaContratacion = new DateTime(2024, 2, 15), Turno = "N", Activo = true, IdResidencial = residencial.IdResidencial, Genero = "F" },
                new Guardia { NombreCompleto = "Carlos Pérez", Dpi = "3333333333333", Telefono = "5551-0003", FechaContratacion = new DateTime(2024, 3, 20), Turno = "D", Activo = true, IdResidencial = residencial.IdResidencial, Genero = "M" }
            };
            context.Guardias.AddRange(guardias);
            context.SaveChanges();

            // 10. Crear Turnos de Guardia
            var turnos = new List<TurnoGuardia>
            {
                new TurnoGuardia { IdGarita = garitas[0].IdGarita, IdGuardia = guardias[0].IdGuardia, FechaInicio = DateTime.Now.AddDays(-1), FechaFin = DateTime.Now.AddHours(-12), Observaciones = "Turno diurno" },
                new TurnoGuardia { IdGarita = garitas[0].IdGarita, IdGuardia = guardias[1].IdGuardia, FechaInicio = DateTime.Now.AddHours(-12), FechaFin = null, Observaciones = "Turno nocturno activo" }
            };
            context.TurnosGuardia.AddRange(turnos);
            context.SaveChanges();

            // 11. Crear Visitantes
            var visitantes = new List<Visitante>
            {
                new Visitante { NombreCompleto = "Pedro Ruiz", Dpi = "4444444444444", Telefono = "5552-1111", Placa = "P123ABC", Empresa = "Mensajería Centro" },
                new Visitante { NombreCompleto = "Marta León", Dpi = "5555555555555", Telefono = "5552-2222", Placa = "M777XYZ", Empresa = "Mensajería X" },
                new Visitante { NombreCompleto = "Carlos Méndez", Telefono = "5552-3333", Empresa = "Contratista Eléctrico" }
            };
            context.Visitantes.AddRange(visitantes);
            context.SaveChanges();

            // 12. Crear Registros de Visita
            var registros = new List<RegistroVisita>
            {
                new RegistroVisita { IdVisitante = visitantes[0].IdVisitante, IdCasa = 1, IdResidencia = residencias[0].IdResidencia, IdGarita = garitas[0].IdGarita, FechaIngreso = DateTime.Now.AddHours(-2), FechaSalida = DateTime.Now.AddHours(-1), IdGuardiaIngreso = guardias[0].IdGuardia, IdGuardiaSalida = guardias[1].IdGuardia, Motivo = "Entrega de paquete" },
                new RegistroVisita { IdVisitante = visitantes[1].IdVisitante, IdCasa = 2, IdResidencia = residencias[1].IdResidencia, IdGarita = garitas[0].IdGarita, FechaIngreso = DateTime.Now.AddHours(-3), IdGuardiaIngreso = guardias[1].IdGuardia, Motivo = "Visita familiar" },
                new RegistroVisita { IdVisitante = visitantes[2].IdVisitante, IdCasa = 3, IdResidencia = residencias[2].IdResidencia, IdGarita = garitas[0].IdGarita, FechaIngreso = DateTime.Now.AddHours(-1), FechaSalida = DateTime.Now, IdGuardiaIngreso = guardias[2].IdGuardia, IdGuardiaSalida = guardias[1].IdGuardia, Motivo = "Mantenimiento eléctrico" }
            };
            context.RegistrosVisita.AddRange(registros);
            context.SaveChanges();

            // 13. Crear Accesos Vehiculares
            var accesos = new List<AccesoVehicular>
            {
                new AccesoVehicular { IdVehiculo = vehiculos[0].IdVehiculo, Placa = vehiculos[0].Placa, IdGarita = garitas[0].IdGarita, FechaIngreso = DateTime.Now.AddHours(-3), HoraIngreso = new TimeSpan(8, 30, 0), FechaSalida = DateTime.Now.AddHours(-2), HoraSalida = new TimeSpan(9, 30, 0), IdGuardiaIngreso = guardias[0].IdGuardia, IdGuardiaSalida = guardias[0].IdGuardia, TipoAcceso = "Residente" },
                new AccesoVehicular { IdVisitante = visitantes[0].IdVisitante, Placa = visitantes[0].Placa!, IdGarita = garitas[0].IdGarita, FechaIngreso = DateTime.Now.AddHours(-1), HoraIngreso = new TimeSpan(14, 15, 0), IdGuardiaIngreso = guardias[1].IdGuardia, TipoAcceso = "Visitante", Observaciones = "Delivery" }
            };
            context.AccesosVehiculares.AddRange(accesos);
            context.SaveChanges();

            // 14. Crear Tipos de Multa
            var tiposMulta = new List<TipoMulta>
            {
                new TipoMulta { Codigo = "DES-MAS", Nombre = "Desechos de mascotas", Descripcion = "No recoger desechos de mascotas en áreas comunes", MontoBase = 200.00m, Activo = true },
                new TipoMulta { Codigo = "DESORDEN", Nombre = "Desorden", Descripcion = "Actividades ruidosas fuera de horario permitido", MontoBase = 175.00m, Activo = true },
                new TipoMulta { Codigo = "EXC-AGUA", Nombre = "Exceso de consumo de agua", Descripcion = "Consumo de agua por encima del límite establecido", MontoBase = 300.00m, Activo = true }
            };
            context.TiposMulta.AddRange(tiposMulta);
            context.SaveChanges();

            // 15. Crear Conceptos de Pago
            var conceptosPago = new List<ConceptoPago>
            {
                new ConceptoPago { Codigo = "AGUA", Nombre = "Servicio de Agua", Descripcion = "Cuota mensual servicio de agua", Monto = 210.00m, Activo = true },
                new ConceptoPago { Codigo = "JARDIN", Nombre = "Jardinería y ornato", Descripcion = "Mantenimiento de áreas verdes", Monto = 80.00m, Activo = true },
                new ConceptoPago { Codigo = "SEGURIDAD", Nombre = "Servicio de seguridad", Descripcion = "Vigilancia 24/7", Monto = 115.00m, Activo = true },
                new ConceptoPago { Codigo = "ADMIN", Nombre = "Administración", Descripcion = "Gastos administrativos", Monto = 75.00m, Activo = true }
            };
            context.ConceptosPago.AddRange(conceptosPago);
            context.SaveChanges();

            // 16. Crear Multas
            var multas = new List<Multa>
            {
                new Multa { IdCasa = 1, IdResidencia = residencias[0].IdResidencia, IdTipoMulta = tiposMulta[1].IdTipoMulta, Concepto = "Desorden", Monto = 175.00m, Fecha = DateTime.Now.AddDays(-10), Pagada = false },
                new Multa { IdCasa = 1, IdResidencia = residencias[0].IdResidencia, IdTipoMulta = tiposMulta[0].IdTipoMulta, Concepto = "Desechos de mascotas", Monto = 200.00m, Fecha = DateTime.Now.AddDays(-5), Pagada = true },
                new Multa { IdCasa = 2, IdResidencia = residencias[1].IdResidencia, IdTipoMulta = tiposMulta[1].IdTipoMulta, Concepto = "Desorden", Monto = 120.00m, Fecha = DateTime.Now.AddDays(-3), Pagada = false }
            };
            context.Multas.AddRange(multas);
            context.SaveChanges();

            // 17. Crear Pagos de Mantenimiento
            var pagos = new List<PagoMantenimiento>
            {
                new PagoMantenimiento { IdCasa = 1, IdResidencia = residencias[0].IdResidencia, FechaPago = DateTime.Now.AddDays(-5), Monto = 480.00m, FormaPago = "Efectivo", Observaciones = "Pago puntual" },
                new PagoMantenimiento { IdCasa = 2, IdResidencia = residencias[1].IdResidencia, FechaPago = DateTime.Now.AddDays(-6), Monto = 480.00m, FormaPago = "Transferencia", Observaciones = "Pago enero" },
                new PagoMantenimiento { IdCasa = 3, IdResidencia = residencias[2].IdResidencia, FechaPago = DateTime.Now.AddDays(-10), Monto = 960.00m, FormaPago = "POS", Observaciones = "Pago doble mes" }
            };
            context.PagosMantenimiento.AddRange(pagos);
            context.SaveChanges();

            // 18. Crear Recibos
            var recibos = new List<Recibo>
            {
                new Recibo { NumeroRecibo = "R-001", Fecha = pagos[0].FechaPago, IdPago = pagos[0].IdPago, Total = 480.00m },
                new Recibo { NumeroRecibo = "R-002", Fecha = pagos[1].FechaPago, IdPago = pagos[1].IdPago, Total = 480.00m },
                new Recibo { NumeroRecibo = "R-003", Fecha = pagos[2].FechaPago, IdPago = pagos[2].IdPago, Total = 960.00m }
            };
            context.Recibos.AddRange(recibos);
            context.SaveChanges();

            // 19. Crear Detalles de Recibo
            var detalles = new List<DetalleRecibo>
            {
                new DetalleRecibo { IdRecibo = recibos[0].IdRecibo, IdConceptoPago = conceptosPago[0].IdConceptoPago, Cantidad = 1, MontoUnitario = 210.00m, Subtotal = 210.00m },
                new DetalleRecibo { IdRecibo = recibos[0].IdRecibo, IdConceptoPago = conceptosPago[1].IdConceptoPago, Cantidad = 1, MontoUnitario = 80.00m, Subtotal = 80.00m },
                new DetalleRecibo { IdRecibo = recibos[0].IdRecibo, IdConceptoPago = conceptosPago[2].IdConceptoPago, Cantidad = 1, MontoUnitario = 115.00m, Subtotal = 115.00m },
                new DetalleRecibo { IdRecibo = recibos[0].IdRecibo, IdConceptoPago = conceptosPago[3].IdConceptoPago, Cantidad = 1, MontoUnitario = 75.00m, Subtotal = 75.00m }
            };
            context.DetallesRecibo.AddRange(detalles);
            context.SaveChanges();

            // 20. Crear Personas No Gratas
            var personasNoGratas = new List<PersonaNoGrata>
            {
                new PersonaNoGrata { Dpi = "4444444444444", NombreCompleto = "Pedro Ruiz", Motivo = "Incidente de mal comportamiento", FechaRegistro = DateTime.Now.AddDays(-30), Activo = true, IdResidencial = residencial.IdResidencial },
                new PersonaNoGrata { NombreCompleto = "Invitado Desconocido", Motivo = "Intento de ingreso no autorizado", FechaRegistro = DateTime.Now.AddDays(-15), Activo = true, IdResidencial = residencial.IdResidencial }
            };
            context.PersonasNoGratas.AddRange(personasNoGratas);
            context.SaveChanges();

            // 21. Crear Vehículos No Permitidos
            var vehiculosNoPermitidos = new List<VehiculoNoPermitido>
            {
                new VehiculoNoPermitido { Placa = "M777XYZ", Motivo = "Reporte por estacionarse en zona prohibida", FechaRegistro = DateTime.Now.AddDays(-20), Activo = true, IdResidencial = residencial.IdResidencial },
                new VehiculoNoPermitido { Placa = "Q999AAA", Motivo = "Vehículo reportado por exceso de velocidad", FechaRegistro = DateTime.Now.AddDays(-10), Activo = true, IdResidencial = residencial.IdResidencial }
            };
            context.VehiculosNoPermitidos.AddRange(vehiculosNoPermitidos);
            context.SaveChanges();
        }
    }
}