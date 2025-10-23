using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ARSAN_Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargo",
                columns: table => new
                {
                    id_cargo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargo", x => x.id_cargo);
                });

            migrationBuilder.CreateTable(
                name: "ConceptoPago",
                columns: table => new
                {
                    id_concepto_pago = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptoPago", x => x.id_concepto_pago);
                });

            migrationBuilder.CreateTable(
                name: "Inquilino",
                columns: table => new
                {
                    id_inquilino = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre_completo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    dpi = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    telefono = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    estado_civil = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    tipo_licencia = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inquilino", x => x.id_inquilino);
                });

            migrationBuilder.CreateTable(
                name: "Propietario",
                columns: table => new
                {
                    id_propietario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre_completo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    dpi = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    telefono = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    estado_civil = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    tipo_licencia = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propietario", x => x.id_propietario);
                });

            migrationBuilder.CreateTable(
                name: "Residencial",
                columns: table => new
                {
                    id_residencial = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    direccion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    municipio = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    departamento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    cantidad_viviendas = table.Column<int>(type: "INTEGER", nullable: true),
                    fecha_inicio_administracion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residencial", x => x.id_residencial);
                });

            migrationBuilder.CreateTable(
                name: "TipoMulta",
                columns: table => new
                {
                    id_tipo_multa = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    monto_base = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoMulta", x => x.id_tipo_multa);
                });

            migrationBuilder.CreateTable(
                name: "Visitante",
                columns: table => new
                {
                    IdVisitante = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DPI = table.Column<string>(type: "TEXT", maxLength: 13, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Empresa = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitante", x => x.IdVisitante);
                });

            migrationBuilder.CreateTable(
                name: "Cluster",
                columns: table => new
                {
                    id_cluster = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    id_residencial = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cluster", x => x.id_cluster);
                    table.ForeignKey(
                        name: "FK_Cluster_Residencial_id_residencial",
                        column: x => x.id_residencial,
                        principalTable: "Residencial",
                        principalColumn: "id_residencial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guardia",
                columns: table => new
                {
                    IdGuardia = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DPI = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FechaContratacion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Turno = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    id_residencial = table.Column<int>(type: "INTEGER", nullable: false),
                    genero = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardia", x => x.IdGuardia);
                    table.ForeignKey(
                        name: "FK_Guardia_Residencial_id_residencial",
                        column: x => x.id_residencial,
                        principalTable: "Residencial",
                        principalColumn: "id_residencial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonaNoGrata",
                columns: table => new
                {
                    IdPNG = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DPI = table.Column<string>(type: "TEXT", maxLength: 13, nullable: true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    id_residencial = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaNoGrata", x => x.IdPNG);
                    table.ForeignKey(
                        name: "FK_PersonaNoGrata_Residencial_id_residencial",
                        column: x => x.id_residencial,
                        principalTable: "Residencial",
                        principalColumn: "id_residencial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehiculoNoPermitido",
                columns: table => new
                {
                    IdVNP = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    id_residencial = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiculoNoPermitido", x => x.IdVNP);
                    table.ForeignKey(
                        name: "FK_VehiculoNoPermitido_Residencial_id_residencial",
                        column: x => x.id_residencial,
                        principalTable: "Residencial",
                        principalColumn: "id_residencial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Garita",
                columns: table => new
                {
                    id_garita = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    id_cluster = table.Column<int>(type: "INTEGER", nullable: true),
                    ubicacion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garita", x => x.id_garita);
                    table.ForeignKey(
                        name: "FK_Garita_Cluster_id_cluster",
                        column: x => x.id_cluster,
                        principalTable: "Cluster",
                        principalColumn: "id_cluster",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JuntaDirectiva",
                columns: table => new
                {
                    id_junta_directiva = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_cluster = table.Column<int>(type: "INTEGER", nullable: false),
                    anio_inicio = table.Column<int>(type: "INTEGER", nullable: false),
                    anio_fin = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuntaDirectiva", x => x.id_junta_directiva);
                    table.ForeignKey(
                        name: "FK_JuntaDirectiva_Cluster_id_cluster",
                        column: x => x.id_cluster,
                        principalTable: "Cluster",
                        principalColumn: "id_cluster",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Residencia",
                columns: table => new
                {
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    numero = table.Column<int>(type: "INTEGER", nullable: false),
                    id_cluster = table.Column<int>(type: "INTEGER", nullable: false),
                    id_propietario = table.Column<int>(type: "INTEGER", nullable: false),
                    id_inquilino = table.Column<int>(type: "INTEGER", nullable: true),
                    estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residencia", x => x.id_residencia);
                    table.ForeignKey(
                        name: "FK_Residencia_Cluster_id_cluster",
                        column: x => x.id_cluster,
                        principalTable: "Cluster",
                        principalColumn: "id_cluster",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Residencia_Inquilino_id_inquilino",
                        column: x => x.id_inquilino,
                        principalTable: "Inquilino",
                        principalColumn: "id_inquilino");
                    table.ForeignKey(
                        name: "FK_Residencia_Propietario_id_propietario",
                        column: x => x.id_propietario,
                        principalTable: "Propietario",
                        principalColumn: "id_propietario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurnoGuardia",
                columns: table => new
                {
                    id_turno = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_garita = table.Column<int>(type: "INTEGER", nullable: false),
                    id_guardia = table.Column<int>(type: "INTEGER", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    observaciones = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnoGuardia", x => x.id_turno);
                    table.ForeignKey(
                        name: "FK_TurnoGuardia_Garita_id_garita",
                        column: x => x.id_garita,
                        principalTable: "Garita",
                        principalColumn: "id_garita",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TurnoGuardia_Guardia_id_guardia",
                        column: x => x.id_guardia,
                        principalTable: "Guardia",
                        principalColumn: "IdGuardia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiembroJunta",
                columns: table => new
                {
                    id_miembro_junta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_junta_directiva = table.Column<int>(type: "INTEGER", nullable: false),
                    id_propietario = table.Column<int>(type: "INTEGER", nullable: false),
                    id_cargo = table.Column<int>(type: "INTEGER", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "TEXT", nullable: true),
                    fecha_fin = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiembroJunta", x => x.id_miembro_junta);
                    table.ForeignKey(
                        name: "FK_MiembroJunta_Cargo_id_cargo",
                        column: x => x.id_cargo,
                        principalTable: "Cargo",
                        principalColumn: "id_cargo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiembroJunta_JuntaDirectiva_id_junta_directiva",
                        column: x => x.id_junta_directiva,
                        principalTable: "JuntaDirectiva",
                        principalColumn: "id_junta_directiva",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiembroJunta_Propietario_id_propietario",
                        column: x => x.id_propietario,
                        principalTable: "Propietario",
                        principalColumn: "id_propietario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Censo",
                columns: table => new
                {
                    id_censo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    fecha_censo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    numero_habitantes = table.Column<int>(type: "INTEGER", nullable: false),
                    observaciones = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Censo", x => x.id_censo);
                    table.ForeignKey(
                        name: "FK_Censo_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCuenta",
                columns: table => new
                {
                    IdEstadoCuenta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCasa = table.Column<int>(type: "INTEGER", nullable: false),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TotalPagado = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCuenta", x => x.IdEstadoCuenta);
                    table.ForeignKey(
                        name: "FK_EstadosCuenta_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Multa",
                columns: table => new
                {
                    IdMulta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCasa = table.Column<int>(type: "INTEGER", nullable: false),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Pagada = table.Column<bool>(type: "INTEGER", nullable: false),
                    id_tipo_multa = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Multa", x => x.IdMulta);
                    table.ForeignKey(
                        name: "FK_Multa_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Multa_TipoMulta_id_tipo_multa",
                        column: x => x.id_tipo_multa,
                        principalTable: "TipoMulta",
                        principalColumn: "id_tipo_multa");
                });

            migrationBuilder.CreateTable(
                name: "PagosMantenimiento",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCasa = table.Column<int>(type: "INTEGER", nullable: false),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FormaPago = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosMantenimiento", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_PagosMantenimiento_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistroVisita",
                columns: table => new
                {
                    IdRegistro = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdVisitante = table.Column<int>(type: "INTEGER", nullable: false),
                    IdCasa = table.Column<int>(type: "INTEGER", nullable: false),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdGuardiaIngreso = table.Column<int>(type: "INTEGER", nullable: false),
                    IdGuardiaSalida = table.Column<int>(type: "INTEGER", nullable: true),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    id_garita = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroVisita", x => x.IdRegistro);
                    table.ForeignKey(
                        name: "FK_RegistroVisita_Garita_id_garita",
                        column: x => x.id_garita,
                        principalTable: "Garita",
                        principalColumn: "id_garita");
                    table.ForeignKey(
                        name: "FK_RegistroVisita_Guardia_IdGuardiaIngreso",
                        column: x => x.IdGuardiaIngreso,
                        principalTable: "Guardia",
                        principalColumn: "IdGuardia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroVisita_Guardia_IdGuardiaSalida",
                        column: x => x.IdGuardiaSalida,
                        principalTable: "Guardia",
                        principalColumn: "IdGuardia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistroVisita_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistroVisita_Visitante_IdVisitante",
                        column: x => x.IdVisitante,
                        principalTable: "Visitante",
                        principalColumn: "IdVisitante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarjetaIntegracionPagos",
                columns: table => new
                {
                    IdMovimiento = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCasa = table.Column<int>(type: "INTEGER", nullable: false),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Referencia = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarjetaIntegracionPagos", x => x.IdMovimiento);
                    table.ForeignKey(
                        name: "FK_TarjetaIntegracionPagos_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculo",
                columns: table => new
                {
                    id_vehiculo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_residencia = table.Column<int>(type: "INTEGER", nullable: false),
                    marca = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    linea = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    anio = table.Column<int>(type: "INTEGER", nullable: true),
                    color = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    placa = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    numero_tarjeta = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculo", x => x.id_vehiculo);
                    table.ForeignKey(
                        name: "FK_Vehiculo_Residencia_id_residencia",
                        column: x => x.id_residencia,
                        principalTable: "Residencia",
                        principalColumn: "id_residencia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagoMulta",
                columns: table => new
                {
                    IdPagoMulta = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdMulta = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagoMulta", x => x.IdPagoMulta);
                    table.ForeignKey(
                        name: "FK_PagoMulta_Multa_IdMulta",
                        column: x => x.IdMulta,
                        principalTable: "Multa",
                        principalColumn: "IdMulta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recibos",
                columns: table => new
                {
                    IdRecibo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroRecibo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IdPago = table.Column<int>(type: "INTEGER", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recibos", x => x.IdRecibo);
                    table.ForeignKey(
                        name: "FK_Recibos_PagosMantenimiento_IdPago",
                        column: x => x.IdPago,
                        principalTable: "PagosMantenimiento",
                        principalColumn: "IdPago",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccesoVehicular",
                columns: table => new
                {
                    id_acceso = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_vehiculo = table.Column<int>(type: "INTEGER", nullable: true),
                    id_visitante = table.Column<int>(type: "INTEGER", nullable: true),
                    placa = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    id_garita = table.Column<int>(type: "INTEGER", nullable: false),
                    fecha_ingreso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    hora_ingreso = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    fecha_salida = table.Column<DateTime>(type: "TEXT", nullable: true),
                    hora_salida = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    id_guardia_ingreso = table.Column<int>(type: "INTEGER", nullable: false),
                    id_guardia_salida = table.Column<int>(type: "INTEGER", nullable: true),
                    tipo_acceso = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    observaciones = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesoVehicular", x => x.id_acceso);
                    table.ForeignKey(
                        name: "FK_AccesoVehicular_Garita_id_garita",
                        column: x => x.id_garita,
                        principalTable: "Garita",
                        principalColumn: "id_garita",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccesoVehicular_Guardia_id_guardia_ingreso",
                        column: x => x.id_guardia_ingreso,
                        principalTable: "Guardia",
                        principalColumn: "IdGuardia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccesoVehicular_Guardia_id_guardia_salida",
                        column: x => x.id_guardia_salida,
                        principalTable: "Guardia",
                        principalColumn: "IdGuardia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccesoVehicular_Vehiculo_id_vehiculo",
                        column: x => x.id_vehiculo,
                        principalTable: "Vehiculo",
                        principalColumn: "id_vehiculo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccesoVehicular_Visitante_id_visitante",
                        column: x => x.id_visitante,
                        principalTable: "Visitante",
                        principalColumn: "IdVisitante",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleRecibo",
                columns: table => new
                {
                    id_detalle_recibo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdRecibo = table.Column<int>(type: "INTEGER", nullable: false),
                    id_concepto_pago = table.Column<int>(type: "INTEGER", nullable: false),
                    cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    monto_unitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleRecibo", x => x.id_detalle_recibo);
                    table.ForeignKey(
                        name: "FK_DetalleRecibo_ConceptoPago_id_concepto_pago",
                        column: x => x.id_concepto_pago,
                        principalTable: "ConceptoPago",
                        principalColumn: "id_concepto_pago",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleRecibo_Recibos_IdRecibo",
                        column: x => x.IdRecibo,
                        principalTable: "Recibos",
                        principalColumn: "IdRecibo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccesoVehicular_id_garita",
                table: "AccesoVehicular",
                column: "id_garita");

            migrationBuilder.CreateIndex(
                name: "IX_AccesoVehicular_id_guardia_ingreso",
                table: "AccesoVehicular",
                column: "id_guardia_ingreso");

            migrationBuilder.CreateIndex(
                name: "IX_AccesoVehicular_id_guardia_salida",
                table: "AccesoVehicular",
                column: "id_guardia_salida");

            migrationBuilder.CreateIndex(
                name: "IX_AccesoVehicular_id_vehiculo",
                table: "AccesoVehicular",
                column: "id_vehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_AccesoVehicular_id_visitante",
                table: "AccesoVehicular",
                column: "id_visitante");

            migrationBuilder.CreateIndex(
                name: "IX_Cargo_nombre",
                table: "Cargo",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Censo_id_residencia",
                table: "Censo",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_Cluster_id_residencial_nombre",
                table: "Cluster",
                columns: new[] { "id_residencial", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConceptoPago_codigo",
                table: "ConceptoPago",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRecibo_id_concepto_pago",
                table: "DetalleRecibo",
                column: "id_concepto_pago");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRecibo_IdRecibo",
                table: "DetalleRecibo",
                column: "IdRecibo");

            migrationBuilder.CreateIndex(
                name: "IX_EstadosCuenta_id_residencia",
                table: "EstadosCuenta",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_Garita_id_cluster",
                table: "Garita",
                column: "id_cluster");

            migrationBuilder.CreateIndex(
                name: "IX_Garita_nombre",
                table: "Garita",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardia_DPI",
                table: "Guardia",
                column: "DPI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardia_id_residencial",
                table: "Guardia",
                column: "id_residencial");

            migrationBuilder.CreateIndex(
                name: "IX_Inquilino_dpi",
                table: "Inquilino",
                column: "dpi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JuntaDirectiva_id_cluster_anio_inicio",
                table: "JuntaDirectiva",
                columns: new[] { "id_cluster", "anio_inicio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiembroJunta_id_cargo",
                table: "MiembroJunta",
                column: "id_cargo");

            migrationBuilder.CreateIndex(
                name: "IX_MiembroJunta_id_junta_directiva_id_cargo",
                table: "MiembroJunta",
                columns: new[] { "id_junta_directiva", "id_cargo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiembroJunta_id_junta_directiva_id_propietario",
                table: "MiembroJunta",
                columns: new[] { "id_junta_directiva", "id_propietario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiembroJunta_id_propietario",
                table: "MiembroJunta",
                column: "id_propietario");

            migrationBuilder.CreateIndex(
                name: "IX_Multa_id_residencia",
                table: "Multa",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_Multa_id_tipo_multa",
                table: "Multa",
                column: "id_tipo_multa");

            migrationBuilder.CreateIndex(
                name: "IX_PagoMulta_IdMulta",
                table: "PagoMulta",
                column: "IdMulta");

            migrationBuilder.CreateIndex(
                name: "IX_PagosMantenimiento_id_residencia",
                table: "PagosMantenimiento",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaNoGrata_id_residencial",
                table: "PersonaNoGrata",
                column: "id_residencial");

            migrationBuilder.CreateIndex(
                name: "IX_Propietario_dpi",
                table: "Propietario",
                column: "dpi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recibos_IdPago",
                table: "Recibos",
                column: "IdPago");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroVisita_id_garita",
                table: "RegistroVisita",
                column: "id_garita");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroVisita_id_residencia",
                table: "RegistroVisita",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroVisita_IdGuardiaIngreso",
                table: "RegistroVisita",
                column: "IdGuardiaIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroVisita_IdGuardiaSalida",
                table: "RegistroVisita",
                column: "IdGuardiaSalida");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroVisita_IdVisitante",
                table: "RegistroVisita",
                column: "IdVisitante");

            migrationBuilder.CreateIndex(
                name: "IX_Residencia_id_cluster_numero",
                table: "Residencia",
                columns: new[] { "id_cluster", "numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Residencia_id_inquilino",
                table: "Residencia",
                column: "id_inquilino");

            migrationBuilder.CreateIndex(
                name: "IX_Residencia_id_propietario",
                table: "Residencia",
                column: "id_propietario");

            migrationBuilder.CreateIndex(
                name: "IX_Residencial_codigo",
                table: "Residencial",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Residencial_nombre",
                table: "Residencial",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarjetaIntegracionPagos_id_residencia",
                table: "TarjetaIntegracionPagos",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_TipoMulta_codigo",
                table: "TipoMulta",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TurnoGuardia_id_garita",
                table: "TurnoGuardia",
                column: "id_garita");

            migrationBuilder.CreateIndex(
                name: "IX_TurnoGuardia_id_guardia",
                table: "TurnoGuardia",
                column: "id_guardia");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_id_residencia",
                table: "Vehiculo",
                column: "id_residencia");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_numero_tarjeta",
                table: "Vehiculo",
                column: "numero_tarjeta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_placa",
                table: "Vehiculo",
                column: "placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoNoPermitido_id_residencial",
                table: "VehiculoNoPermitido",
                column: "id_residencial");

            migrationBuilder.CreateIndex(
                name: "IX_VehiculoNoPermitido_Placa",
                table: "VehiculoNoPermitido",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccesoVehicular");

            migrationBuilder.DropTable(
                name: "Censo");

            migrationBuilder.DropTable(
                name: "DetalleRecibo");

            migrationBuilder.DropTable(
                name: "EstadosCuenta");

            migrationBuilder.DropTable(
                name: "MiembroJunta");

            migrationBuilder.DropTable(
                name: "PagoMulta");

            migrationBuilder.DropTable(
                name: "PersonaNoGrata");

            migrationBuilder.DropTable(
                name: "RegistroVisita");

            migrationBuilder.DropTable(
                name: "TarjetaIntegracionPagos");

            migrationBuilder.DropTable(
                name: "TurnoGuardia");

            migrationBuilder.DropTable(
                name: "VehiculoNoPermitido");

            migrationBuilder.DropTable(
                name: "Vehiculo");

            migrationBuilder.DropTable(
                name: "ConceptoPago");

            migrationBuilder.DropTable(
                name: "Recibos");

            migrationBuilder.DropTable(
                name: "Cargo");

            migrationBuilder.DropTable(
                name: "JuntaDirectiva");

            migrationBuilder.DropTable(
                name: "Multa");

            migrationBuilder.DropTable(
                name: "Visitante");

            migrationBuilder.DropTable(
                name: "Garita");

            migrationBuilder.DropTable(
                name: "Guardia");

            migrationBuilder.DropTable(
                name: "PagosMantenimiento");

            migrationBuilder.DropTable(
                name: "TipoMulta");

            migrationBuilder.DropTable(
                name: "Residencia");

            migrationBuilder.DropTable(
                name: "Cluster");

            migrationBuilder.DropTable(
                name: "Inquilino");

            migrationBuilder.DropTable(
                name: "Propietario");

            migrationBuilder.DropTable(
                name: "Residencial");
        }
    }
}
