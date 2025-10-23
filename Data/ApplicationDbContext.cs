using Microsoft.EntityFrameworkCore;
using ARSAN_Web.Models;

namespace ARSAN_Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Tablas de la base de datos
        public DbSet<Residencial> Residenciales { get; set; }
        public DbSet<Cluster> Clusters { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
        public DbSet<Residencia> Residencias { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<JuntaDirectiva> JuntasDirectivas { get; set; }
        public DbSet<MiembroJunta> MiembrosJunta { get; set; }
        public DbSet<Censo> Censos { get; set; }
        public DbSet<PagoMantenimiento> PagosMantenimiento { get; set; }
        public DbSet<Recibo> Recibos { get; set; }
        public DbSet<EstadoCuenta> EstadosCuenta { get; set; }
        public DbSet<Multa> Multas { get; set; }
        public DbSet<PagoMulta> PagosMulta { get; set; }
        public DbSet<Guardia> Guardias { get; set; }
        public DbSet<Visitante> Visitantes { get; set; }
        public DbSet<RegistroVisita> RegistrosVisita { get; set; }
        public DbSet<PersonaNoGrata> PersonasNoGratas { get; set; }
        public DbSet<VehiculoNoPermitido> VehiculosNoPermitidos { get; set; }
        public DbSet<TarjetaIntegracionPagos> TarjetasIntegracionPagos { get; set; }

        // NUEVAS TABLAS
        public DbSet<Garita> Garitas { get; set; }
        public DbSet<TurnoGuardia> TurnosGuardia { get; set; }
        public DbSet<AccesoVehicular> AccesosVehiculares { get; set; }
        public DbSet<TipoMulta> TiposMulta { get; set; }
        public DbSet<ConceptoPago> ConceptosPago { get; set; }
        public DbSet<DetalleRecibo> DetallesRecibo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =============== NUEVA TABLA: Residencial ===============
            modelBuilder.Entity<Residencial>(entity =>
            {
                entity.ToTable("Residencial");
                entity.HasKey(e => e.IdResidencial);
                entity.Property(e => e.IdResidencial).HasColumnName("id_residencial");
                entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Direccion).HasColumnName("direccion").HasMaxLength(200);
                entity.Property(e => e.Municipio).HasColumnName("municipio").HasMaxLength(100);
                entity.Property(e => e.Departamento).HasColumnName("departamento").HasMaxLength(100);
                entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(20);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.CantidadViviendas).HasColumnName("cantidad_viviendas");
                entity.Property(e => e.FechaInicioAdministracion).HasColumnName("fecha_inicio_administracion");
                entity.Property(e => e.Activo).HasColumnName("activo").IsRequired();

                entity.HasIndex(e => e.Codigo).IsUnique();
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // =============== NUEVA TABLA: Garita ===============
            modelBuilder.Entity<Garita>(entity =>
            {
                entity.ToTable("Garita");
                entity.HasKey(e => e.IdGarita);
                entity.Property(e => e.IdGarita).HasColumnName("id_garita");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
                entity.Property(e => e.IdCluster).HasColumnName("id_cluster");
                entity.Property(e => e.Ubicacion).HasColumnName("ubicacion").HasMaxLength(100);
                entity.Property(e => e.Activa).HasColumnName("activa").IsRequired();

                entity.HasOne(e => e.Cluster)
                    .WithMany(c => c.Garitas)
                    .HasForeignKey(e => e.IdCluster)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // =============== NUEVA TABLA: TurnoGuardia ===============
            modelBuilder.Entity<TurnoGuardia>(entity =>
            {
                entity.ToTable("TurnoGuardia");
                entity.HasKey(e => e.IdTurno);
                entity.Property(e => e.IdTurno).HasColumnName("id_turno");
                entity.Property(e => e.IdGarita).HasColumnName("id_garita").IsRequired();
                entity.Property(e => e.IdGuardia).HasColumnName("id_guardia").IsRequired();
                entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
                entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(200);

                entity.HasOne(e => e.Garita)
                    .WithMany(g => g.Turnos)
                    .HasForeignKey(e => e.IdGarita);

                entity.HasOne(e => e.Guardia)
                    .WithMany(g => g.Turnos)
                    .HasForeignKey(e => e.IdGuardia);
            });

            // =============== NUEVA TABLA: AccesoVehicular ===============
            modelBuilder.Entity<AccesoVehicular>(entity =>
            {
                entity.ToTable("AccesoVehicular");
                entity.HasKey(e => e.IdAcceso);
                entity.Property(e => e.IdAcceso).HasColumnName("id_acceso");
                entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
                entity.Property(e => e.IdVisitante).HasColumnName("id_visitante");
                entity.Property(e => e.Placa).HasColumnName("placa").HasMaxLength(20).IsRequired();
                entity.Property(e => e.IdGarita).HasColumnName("id_garita").IsRequired();
                entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso").IsRequired();
                entity.Property(e => e.HoraIngreso).HasColumnName("hora_ingreso").IsRequired();
                entity.Property(e => e.FechaSalida).HasColumnName("fecha_salida");
                entity.Property(e => e.HoraSalida).HasColumnName("hora_salida");
                entity.Property(e => e.IdGuardiaIngreso).HasColumnName("id_guardia_ingreso").IsRequired();
                entity.Property(e => e.IdGuardiaSalida).HasColumnName("id_guardia_salida");
                entity.Property(e => e.TipoAcceso).HasColumnName("tipo_acceso").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(200);

                entity.HasOne(e => e.Vehiculo)
                    .WithMany(v => v.Accesos)
                    .HasForeignKey(e => e.IdVehiculo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Visitante)
                    .WithMany(v => v.Accesos)
                    .HasForeignKey(e => e.IdVisitante)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Garita)
                    .WithMany(g => g.Accesos)
                    .HasForeignKey(e => e.IdGarita);

                entity.HasOne(e => e.GuardiaIngreso)
                    .WithMany(g => g.AccesosIngreso)
                    .HasForeignKey(e => e.IdGuardiaIngreso)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.GuardiaSalida)
                    .WithMany(g => g.AccesosSalida)
                    .HasForeignKey(e => e.IdGuardiaSalida)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // =============== NUEVA TABLA: TipoMulta ===============
            modelBuilder.Entity<TipoMulta>(entity =>
            {
                entity.ToTable("TipoMulta");
                entity.HasKey(e => e.IdTipoMulta);
                entity.Property(e => e.IdTipoMulta).HasColumnName("id_tipo_multa");
                entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
                entity.Property(e => e.MontoBase).HasColumnName("monto_base").HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Activo).HasColumnName("activo").IsRequired();

                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // =============== NUEVA TABLA: ConceptoPago ===============
            modelBuilder.Entity<ConceptoPago>(entity =>
            {
                entity.ToTable("ConceptoPago");
                entity.HasKey(e => e.IdConceptoPago);
                entity.Property(e => e.IdConceptoPago).HasColumnName("id_concepto_pago");
                entity.Property(e => e.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(500);
                entity.Property(e => e.Monto).HasColumnName("monto").HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Activo).HasColumnName("activo").IsRequired();

                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // =============== NUEVA TABLA: DetalleRecibo ===============
            modelBuilder.Entity<DetalleRecibo>(entity =>
            {
                entity.ToTable("DetalleRecibo");
                entity.HasKey(e => e.IdDetalleRecibo);
                entity.Property(e => e.IdDetalleRecibo).HasColumnName("id_detalle_recibo");
                entity.Property(e => e.IdRecibo).HasColumnName("IdRecibo").IsRequired();
                entity.Property(e => e.IdConceptoPago).HasColumnName("id_concepto_pago").IsRequired();
                entity.Property(e => e.Cantidad).HasColumnName("cantidad").IsRequired();
                entity.Property(e => e.MontoUnitario).HasColumnName("monto_unitario").HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(10,2)").IsRequired();

                entity.HasOne(e => e.Recibo)
                    .WithMany(r => r.Detalles)
                    .HasForeignKey(e => e.IdRecibo);

                entity.HasOne(e => e.ConceptoPago)
                    .WithMany(c => c.DetallesRecibo)
                    .HasForeignKey(e => e.IdConceptoPago);
            });

            // =============== ACTUALIZADO: Cluster ===============
            modelBuilder.Entity<Cluster>(entity =>
            {
                entity.ToTable("Cluster");
                entity.HasKey(e => e.IdCluster);
                entity.Property(e => e.IdCluster).HasColumnName("id_cluster");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
                entity.Property(e => e.IdResidencial).HasColumnName("id_residencial").IsRequired(); // NUEVO

                entity.HasOne(e => e.Residencial) // NUEVO
                    .WithMany(r => r.Clusters)
                    .HasForeignKey(e => e.IdResidencial);

                entity.HasIndex(e => new { e.IdResidencial, e.Nombre }).IsUnique();
            });

            // =============== ACTUALIZADO: Guardia ===============
            modelBuilder.Entity<Guardia>(entity =>
            {
                entity.ToTable("Guardia");
                entity.HasKey(e => e.IdGuardia);
                entity.Property(e => e.IdGuardia).HasColumnName("IdGuardia");
                entity.Property(e => e.NombreCompleto).HasColumnName("NombreCompleto").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Dpi).HasColumnName("DPI").HasMaxLength(13).IsRequired();
                entity.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(20);
                entity.Property(e => e.FechaContratacion).HasColumnName("FechaContratacion");
                entity.Property(e => e.Turno).HasColumnName("Turno").HasMaxLength(1);
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired();
                entity.Property(e => e.IdResidencial).HasColumnName("id_residencial").IsRequired(); // NUEVO
                entity.Property(e => e.Genero).HasColumnName("genero").HasMaxLength(1); // NUEVO

                entity.HasOne(e => e.Residencial) // NUEVO
                    .WithMany(r => r.Guardias)
                    .HasForeignKey(e => e.IdResidencial);

                entity.HasIndex(e => e.Dpi).IsUnique();
            });

            // Configuración de Propietario
            modelBuilder.Entity<Propietario>(entity =>
            {
                entity.ToTable("Propietario");
                entity.HasKey(e => e.IdPropietario);
                entity.Property(e => e.IdPropietario).HasColumnName("id_propietario");
                entity.Property(e => e.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Dpi).HasColumnName("dpi").HasMaxLength(13).IsRequired();
                entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(15);
                entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
                entity.Property(e => e.EstadoCivil).HasColumnName("estado_civil").HasMaxLength(20);
                entity.Property(e => e.TipoLicencia).HasColumnName("tipo_licencia").HasMaxLength(1);
                entity.HasIndex(e => e.Dpi).IsUnique();
            });

            // Configuración de Inquilino
            modelBuilder.Entity<Inquilino>(entity =>
            {
                entity.ToTable("Inquilino");
                entity.HasKey(e => e.IdInquilino);
                entity.Property(e => e.IdInquilino).HasColumnName("id_inquilino");
                entity.Property(e => e.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Dpi).HasColumnName("dpi").HasMaxLength(13).IsRequired();
                entity.Property(e => e.Telefono).HasColumnName("telefono").HasMaxLength(15);
                entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
                entity.Property(e => e.EstadoCivil).HasColumnName("estado_civil").HasMaxLength(20);
                entity.Property(e => e.TipoLicencia).HasColumnName("tipo_licencia").HasMaxLength(1);
                entity.HasIndex(e => e.Dpi).IsUnique();
            });

            // Configuración de Residencia
            modelBuilder.Entity<Residencia>(entity =>
            {
                entity.ToTable("Residencia");
                entity.HasKey(e => e.IdResidencia);
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia");
                entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
                entity.Property(e => e.IdCluster).HasColumnName("id_cluster").IsRequired();
                entity.Property(e => e.IdPropietario).HasColumnName("id_propietario").IsRequired();
                entity.Property(e => e.IdInquilino).HasColumnName("id_inquilino");
                entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).IsRequired();

                entity.HasOne(e => e.Cluster)
                    .WithMany(c => c.Residencias)
                    .HasForeignKey(e => e.IdCluster);

                entity.HasOne(e => e.Propietario)
                    .WithMany(p => p.Residencias)
                    .HasForeignKey(e => e.IdPropietario);

                entity.HasOne(e => e.Inquilino)
                    .WithMany(i => i.Residencias)
                    .HasForeignKey(e => e.IdInquilino);

                entity.HasIndex(e => new { e.IdCluster, e.Numero }).IsUnique();
            });

            // Configuración de Vehiculo
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.ToTable("Vehiculo");
                entity.HasKey(e => e.IdVehiculo);
                entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired();
                entity.Property(e => e.Marca).HasColumnName("marca").HasMaxLength(20);
                entity.Property(e => e.Linea).HasColumnName("linea").HasMaxLength(20);
                entity.Property(e => e.Anio).HasColumnName("anio");
                entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(15);
                entity.Property(e => e.Placa).HasColumnName("placa").HasMaxLength(20).IsRequired();
                entity.Property(e => e.NumeroTarjeta).HasColumnName("numero_tarjeta").HasMaxLength(20).IsRequired();

                entity.HasOne(e => e.Residencia)
                    .WithMany(r => r.Vehiculos)
                    .HasForeignKey(e => e.IdResidencia);

                entity.HasIndex(e => e.Placa).IsUnique();
                entity.HasIndex(e => e.NumeroTarjeta).IsUnique();
            });

            // Configuración de Cargo
            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.ToTable("Cargo");
                entity.HasKey(e => e.IdCargo);
                entity.Property(e => e.IdCargo).HasColumnName("id_cargo");
                entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de JuntaDirectiva
            modelBuilder.Entity<JuntaDirectiva>(entity =>
            {
                entity.ToTable("JuntaDirectiva");
                entity.HasKey(e => e.IdJuntaDirectiva);
                entity.Property(e => e.IdJuntaDirectiva).HasColumnName("id_junta_directiva");
                entity.Property(e => e.IdCluster).HasColumnName("id_cluster").IsRequired();
                entity.Property(e => e.AnioInicio).HasColumnName("anio_inicio").IsRequired();
                entity.Property(e => e.AnioFin).HasColumnName("anio_fin");

                entity.HasOne(e => e.Cluster)
                    .WithMany(c => c.JuntasDirectivas)
                    .HasForeignKey(e => e.IdCluster);

                entity.HasIndex(e => new { e.IdCluster, e.AnioInicio }).IsUnique();
            });

            // Configuración de MiembroJunta
            modelBuilder.Entity<MiembroJunta>(entity =>
            {
                entity.ToTable("MiembroJunta");
                entity.HasKey(e => e.IdMiembroJunta);
                entity.Property(e => e.IdMiembroJunta).HasColumnName("id_miembro_junta");
                entity.Property(e => e.IdJuntaDirectiva).HasColumnName("id_junta_directiva").IsRequired();
                entity.Property(e => e.IdPropietario).HasColumnName("id_propietario").IsRequired();
                entity.Property(e => e.IdCargo).HasColumnName("id_cargo").IsRequired();
                entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
                entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");

                entity.HasOne(e => e.JuntaDirectiva)
                    .WithMany(j => j.Miembros)
                    .HasForeignKey(e => e.IdJuntaDirectiva);

                entity.HasOne(e => e.Propietario)
                    .WithMany(p => p.MiembrosJunta)
                    .HasForeignKey(e => e.IdPropietario);

                entity.HasOne(e => e.Cargo)
                    .WithMany(c => c.MiembrosJunta)
                    .HasForeignKey(e => e.IdCargo);

                entity.HasIndex(e => new { e.IdJuntaDirectiva, e.IdCargo }).IsUnique();
                entity.HasIndex(e => new { e.IdJuntaDirectiva, e.IdPropietario }).IsUnique();
            });

            // Configuración de Censo
            modelBuilder.Entity<Censo>(entity =>
            {
                entity.ToTable("Censo");
                entity.HasKey(e => e.IdCenso);
                entity.Property(e => e.IdCenso).HasColumnName("id_censo");
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired();
                entity.Property(e => e.FechaCenso).HasColumnName("fecha_censo");
                entity.Property(e => e.NumeroHabitantes).HasColumnName("numero_habitantes").IsRequired();
                entity.Property(e => e.Observaciones).HasColumnName("observaciones").HasMaxLength(100);

                entity.HasOne(e => e.Residencia)
                    .WithMany(r => r.Censos)
                    .HasForeignKey(e => e.IdResidencia);
            });

            // =============== ACTUALIZADO: PagoMantenimiento ===============
            modelBuilder.Entity<PagoMantenimiento>(entity =>
            {
                entity.ToTable("PagosMantenimiento");
                entity.HasKey(e => e.IdPago);
                entity.Property(e => e.IdPago).HasColumnName("IdPago");
                entity.Property(e => e.IdCasa).HasColumnName("IdCasa"); // Mantener por compatibilidad
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired(); // NUEVO
                entity.Property(e => e.FechaPago).HasColumnName("FechaPago").IsRequired();
                entity.Property(e => e.Monto).HasColumnName("Monto").HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.FormaPago).HasColumnName("FormaPago").HasMaxLength(20);
                entity.Property(e => e.Observaciones).HasColumnName("Observaciones").HasMaxLength(200);

                entity.HasOne(e => e.Residencia) // NUEVO
                    .WithMany()
                    .HasForeignKey(e => e.IdResidencia);
            });

            // Configuración de Recibo
            modelBuilder.Entity<Recibo>(entity =>
            {
                entity.ToTable("Recibos");
                entity.HasKey(e => e.IdRecibo);
                entity.Property(e => e.IdRecibo).HasColumnName("IdRecibo");
                entity.Property(e => e.NumeroRecibo).HasColumnName("NumeroRecibo").HasMaxLength(20).IsRequired();
                entity.Property(e => e.Fecha).HasColumnName("Fecha").IsRequired();
                entity.Property(e => e.IdPago).HasColumnName("IdPago").IsRequired();
                entity.Property(e => e.Total).HasColumnName("Total").HasColumnType("decimal(10,2)").IsRequired();

                entity.HasOne(e => e.Pago)
                    .WithMany(p => p.Recibos)
                    .HasForeignKey(e => e.IdPago);
            });

            // =============== ACTUALIZADO: EstadoCuenta ===============
            modelBuilder.Entity<EstadoCuenta>(entity =>
            {
                entity.ToTable("EstadosCuenta");
                entity.HasKey(e => e.IdEstadoCuenta);
                entity.Property(e => e.IdEstadoCuenta).HasColumnName("IdEstadoCuenta");
                entity.Property(e => e.IdCasa).HasColumnName("IdCasa"); // Mantener por compatibilidad
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired(); // NUEVO
                entity.Property(e => e.FechaGeneracion).HasColumnName("FechaGeneracion").IsRequired();
                entity.Property(e => e.SaldoPendiente).HasColumnName("SaldoPendiente").HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalPagado).HasColumnName("TotalPagado").HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Residencia) // NUEVO
                    .WithMany()
                    .HasForeignKey(e => e.IdResidencia);
            });

            // =============== ACTUALIZADO: Multa ===============
            modelBuilder.Entity<Multa>(entity =>
            {
                entity.ToTable("Multa");
                entity.HasKey(e => e.IdMulta);
                entity.Property(e => e.IdMulta).HasColumnName("IdMulta");
                entity.Property(e => e.IdCasa).HasColumnName("IdCasa"); // Mantener por compatibilidad
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired(); // NUEVO
                entity.Property(e => e.Concepto).HasColumnName("Concepto").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Monto).HasColumnName("Monto").HasColumnType("decimal(12,2)").IsRequired();
                entity.Property(e => e.Fecha).HasColumnName("Fecha").IsRequired();
                entity.Property(e => e.Pagada).HasColumnName("Pagada").IsRequired();
                entity.Property(e => e.IdTipoMulta).HasColumnName("id_tipo_multa"); // NUEVO

                entity.HasOne(e => e.Residencia) // NUEVO
                    .WithMany()
                    .HasForeignKey(e => e.IdResidencia);

                entity.HasOne(e => e.TipoMulta) // NUEVO
                    .WithMany(t => t.Multas)
                    .HasForeignKey(e => e.IdTipoMulta);
            });

            // Configuración de PagoMulta
            modelBuilder.Entity<PagoMulta>(entity =>
            {
                entity.ToTable("PagoMulta");
                entity.HasKey(e => e.IdPagoMulta);
                entity.Property(e => e.IdPagoMulta).HasColumnName("IdPagoMulta");
                entity.Property(e => e.IdMulta).HasColumnName("IdMulta").IsRequired();
                entity.Property(e => e.FechaPago).HasColumnName("FechaPago").IsRequired();
                entity.Property(e => e.MontoPagado).HasColumnName("MontoPagado").HasColumnType("decimal(12,2)").IsRequired();

                entity.HasOne(e => e.Multa)
                    .WithMany(m => m.PagosMulta)
                    .HasForeignKey(e => e.IdMulta);
            });

            // Configuración de Visitante
            modelBuilder.Entity<Visitante>(entity =>
            {
                entity.ToTable("Visitante");
                entity.HasKey(e => e.IdVisitante);
                entity.Property(e => e.IdVisitante).HasColumnName("IdVisitante");
                entity.Property(e => e.NombreCompleto).HasColumnName("NombreCompleto").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Dpi).HasColumnName("DPI").HasMaxLength(13);
                entity.Property(e => e.Telefono).HasColumnName("Telefono").HasMaxLength(20);
                entity.Property(e => e.Placa).HasColumnName("Placa").HasMaxLength(15);
                entity.Property(e => e.Empresa).HasColumnName("Empresa").HasMaxLength(100);
            });

            // =============== ACTUALIZADO: RegistroVisita ===============
            modelBuilder.Entity<RegistroVisita>(entity =>
            {
                entity.ToTable("RegistroVisita");
                entity.HasKey(e => e.IdRegistro);
                entity.Property(e => e.IdRegistro).HasColumnName("IdRegistro");
                entity.Property(e => e.IdVisitante).HasColumnName("IdVisitante").IsRequired();
                entity.Property(e => e.IdCasa).HasColumnName("IdCasa"); // Mantener por compatibilidad
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired(); // NUEVO
                entity.Property(e => e.FechaIngreso).HasColumnName("FechaIngreso").IsRequired();
                entity.Property(e => e.FechaSalida).HasColumnName("FechaSalida");
                entity.Property(e => e.IdGuardiaIngreso).HasColumnName("IdGuardiaIngreso").IsRequired();
                entity.Property(e => e.IdGuardiaSalida).HasColumnName("IdGuardiaSalida");
                entity.Property(e => e.Motivo).HasColumnName("Motivo").HasMaxLength(200);
                entity.Property(e => e.IdGarita).HasColumnName("id_garita"); // NUEVO

                entity.HasOne(e => e.Visitante)
                    .WithMany(v => v.RegistrosVisita)
                    .HasForeignKey(e => e.IdVisitante);

                entity.HasOne(e => e.Residencia) // NUEVO
                    .WithMany()
                    .HasForeignKey(e => e.IdResidencia);

                entity.HasOne(e => e.GuardiaIngreso)
                    .WithMany(g => g.RegistrosIngreso)
                    .HasForeignKey(e => e.IdGuardiaIngreso)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.GuardiaSalida)
                    .WithMany(g => g.RegistrosSalida)
                    .HasForeignKey(e => e.IdGuardiaSalida)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Garita) // NUEVO
                    .WithMany(g => g.RegistrosVisita)
                    .HasForeignKey(e => e.IdGarita);
            });

            // =============== ACTUALIZADO: PersonaNoGrata ===============
            modelBuilder.Entity<PersonaNoGrata>(entity =>
            {
                entity.ToTable("PersonaNoGrata");
                entity.HasKey(e => e.IdPNG);
                entity.Property(e => e.IdPNG).HasColumnName("IdPNG");
                entity.Property(e => e.Dpi).HasColumnName("DPI").HasMaxLength(13);
                entity.Property(e => e.NombreCompleto).HasColumnName("NombreCompleto").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Motivo).HasColumnName("Motivo").HasMaxLength(200).IsRequired();
                entity.Property(e => e.FechaRegistro).HasColumnName("FechaRegistro").IsRequired();
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired();
                entity.Property(e => e.IdResidencial).HasColumnName("id_residencial").IsRequired(); // NUEVO

                entity.HasOne(e => e.Residencial) // NUEVO
                    .WithMany(r => r.PersonasNoGratas)
                    .HasForeignKey(e => e.IdResidencial);
            });

            // =============== ACTUALIZADO: VehiculoNoPermitido ===============
            modelBuilder.Entity<VehiculoNoPermitido>(entity =>
            {
                entity.ToTable("VehiculoNoPermitido");
                entity.HasKey(e => e.IdVNP);
                entity.Property(e => e.IdVNP).HasColumnName("IdVNP");
                entity.Property(e => e.Placa).HasColumnName("Placa").HasMaxLength(15).IsRequired();
                entity.Property(e => e.Motivo).HasColumnName("Motivo").HasMaxLength(200).IsRequired();
                entity.Property(e => e.FechaRegistro).HasColumnName("FechaRegistro").IsRequired();
                entity.Property(e => e.Activo).HasColumnName("Activo").IsRequired();
                entity.Property(e => e.IdResidencial).HasColumnName("id_residencial").IsRequired(); // NUEVO

                entity.HasOne(e => e.Residencial) // NUEVO
                    .WithMany(r => r.VehiculosNoPermitidos)
                    .HasForeignKey(e => e.IdResidencial);

                entity.HasIndex(e => e.Placa).IsUnique();
            });

            // =============== ACTUALIZADO: TarjetaIntegracionPagos ===============
            modelBuilder.Entity<TarjetaIntegracionPagos>(entity =>
            {
                entity.ToTable("TarjetaIntegracionPagos");
                entity.HasKey(e => e.IdMovimiento);
                entity.Property(e => e.IdMovimiento).HasColumnName("IdMovimiento");
                entity.Property(e => e.IdCasa).HasColumnName("IdCasa"); // Mantener por compatibilidad
                entity.Property(e => e.IdResidencia).HasColumnName("id_residencia").IsRequired(); // NUEVO
                entity.Property(e => e.Fecha).HasColumnName("Fecha").IsRequired();
                entity.Property(e => e.TipoMovimiento).HasColumnName("TipoMovimiento").HasMaxLength(20);
                entity.Property(e => e.Referencia).HasColumnName("Referencia").HasMaxLength(50);
                entity.Property(e => e.Monto).HasColumnName("Monto").HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Residencia) // NUEVO
                    .WithMany()
                    .HasForeignKey(e => e.IdResidencia);
            });
        }
    }
}