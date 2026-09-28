using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectricERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Almacen",
                columns: table => new
                {
                    IdAlmacen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacen", x => x.IdAlmacen);
                    table.CheckConstraint("CK_Almacen_Direccion", "LEN([Direccion]) > 7");
                    table.CheckConstraint("CK_Almacen_Nombre", "LEN([Nombre]) > 4");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(75)", unicode: false, maxLength: 75, nullable: false),
                    Rut = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false),
                    Direccion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: true),
                    Email = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.IdProveedor);
                    table.CheckConstraint("CK_Proveedor_Direccion", "LEN([Direccion]) > 7");
                    table.CheckConstraint("CK_Proveedor_Email", "[Email] IS NULL OR (LEN([Email]) > 7 AND [Email] LIKE '%_@_%_.%' AND [Email] NOT LIKE '% %' AND [Email] NOT LIKE '%..%' AND [Email] NOT LIKE '%@%@%' AND [Email] NOT LIKE '%.@' AND [Email] NOT LIKE '@%' AND [Email] NOT LIKE '%.')");
                    table.CheckConstraint("CK_Proveedor_Nombre", "LEN([Nombre]) > 3");
                    table.CheckConstraint("CK_Proveedor_Rut", "LEN([Rut]) = 11 AND [Rut] LIKE '21%' AND [Rut] NOT LIKE '%[^0-9]%'");
                    table.CheckConstraint("CK_Proveedor_Telefono", "[Telefono] IS NULL OR (LEN([Telefono]) > 7 AND [Telefono] NOT LIKE '%[^0-9]%')");
                });

            migrationBuilder.CreateTable(
                name: "Vehiculo",
                columns: table => new
                {
                    IdVehiculo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Matricula = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Marca = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Modelo = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CapacidadCarga = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculo", x => x.IdVehiculo);
                    table.CheckConstraint("CK_Vehiculo_Carga", "[CapacidadCarga] > 0");
                    table.CheckConstraint("CK_Vehiculo_Marca", "LEN([Marca]) > 3");
                    table.CheckConstraint("CK_Vehiculo_Matricula", "LEN([Matricula]) = 8 AND [Matricula] LIKE '[A-Z][A-Z][A-Z] [0-9][0-9][0-9][0-9]'");
                    table.CheckConstraint("CK_Vehiculo_Modelo", "LEN([Modelo]) > 3");
                });

            migrationBuilder.CreateTable(
                name: "Seccion",
                columns: table => new
                {
                    IdSeccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlmacen = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seccion", x => x.IdSeccion);
                    table.CheckConstraint("CK_Seccion_Descripcion", "[Descripcion] IS NULL OR LEN([Descripcion]) > 1");
                    table.CheckConstraint("CK_Seccion_Nombre", "LEN([Nombre]) > 3");
                    table.ForeignKey(
                        name: "FK_Seccion_Almacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacen",
                        principalColumn: "IdAlmacen");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UbicacionGPS",
                columns: table => new
                {
                    IdUbicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTransportista = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UbicacionGPS", x => x.IdUbicacion);
                    table.CheckConstraint("CK_UbicacionGPS_Latitud", "[Latitud] BETWEEN -90 AND 90");
                    table.CheckConstraint("CK_UbicacionGPS_Longitud", "[Longitud] BETWEEN -180 AND 180");
                    table.ForeignKey(
                        name: "FK_UbicacionGPS_Transportista",
                        column: x => x.IdTransportista,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CodigoSKU = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Categoria = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnidadMedida = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                    table.CheckConstraint("CK_Producto_Categoria", "[Categoria] IN ('Resistencias','Capacitores','Inductores','Diodos','Transistores','Circuitos','Integrados','Sensores','Conectores','Cables','Fuentes de Alimentacion','Reles','Fusibles','Herramientas','Accesorios')");
                    table.CheckConstraint("CK_Producto_CodigoSKU", "LEN([CodigoSKU]) > 10 AND [CodigoSKU] NOT LIKE '%[^A-Z0-9]%' AND [CodigoSKU] LIKE '%[A-Z]%' AND [CodigoSKU] LIKE '%[0-9]%'");
                    table.CheckConstraint("CK_Producto_Descripcion", "[Descripcion] IS NULL OR LEN([Descripcion]) > 1");
                    table.CheckConstraint("CK_Producto_Nombre", "LEN([Nombre]) > 3");
                    table.CheckConstraint("CK_Producto_UnidadMedida", "[UnidadMedida] IN ('m','cm','kg','Consumo Electrico')");
                    table.ForeignKey(
                        name: "FK_Producto_Proveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedor",
                        principalColumn: "IdProveedor");
                });

            migrationBuilder.CreateTable(
                name: "Transferencia",
                columns: table => new
                {
                    IdTransferencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSupervisorSolicitante = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IdAdministradorAutorizador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IdTransportista = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IdVehiculo = table.Column<int>(type: "int", nullable: true),
                    IdAlmacenOrigen = table.Column<int>(type: "int", nullable: false),
                    IdAlmacenDestino = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencia", x => x.IdTransferencia);
                    table.CheckConstraint("CK_Transferencia_Almacenes", "[IdAlmacenOrigen] <> [IdAlmacenDestino]");
                    table.CheckConstraint("CK_Transferencia_Estado", "[Estado] IN ('PENDIENTE','AUTORIZADA','ASIGNADA','EN_TRANSITO','ENTREGADA','FINALIZADA','RECHAZADA')");
                    table.CheckConstraint("CK_Transferencia_FechaAutorizacion", "[FechaAutorizacion] IS NULL OR [FechaAutorizacion] > [FechaSolicitud]");
                    table.CheckConstraint("CK_Transferencia_FechaEjecucion", "[FechaEjecucion] IS NULL OR [FechaEjecucion] > [FechaAutorizacion]");
                    table.CheckConstraint("CK_Transferencia_FechaEntrega", "[FechaEntrega] IS NULL OR [FechaEntrega] > [FechaEjecucion]");
                    table.ForeignKey(
                        name: "FK_Transferencia_Admin",
                        column: x => x.IdAdministradorAutorizador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transferencia_Destino",
                        column: x => x.IdAlmacenDestino,
                        principalTable: "Almacen",
                        principalColumn: "IdAlmacen");
                    table.ForeignKey(
                        name: "FK_Transferencia_Origen",
                        column: x => x.IdAlmacenOrigen,
                        principalTable: "Almacen",
                        principalColumn: "IdAlmacen");
                    table.ForeignKey(
                        name: "FK_Transferencia_Supervisor",
                        column: x => x.IdSupervisorSolicitante,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transferencia_Transportista",
                        column: x => x.IdTransportista,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transferencia_Vehiculo",
                        column: x => x.IdVehiculo,
                        principalTable: "Vehiculo",
                        principalColumn: "IdVehiculo");
                });

            migrationBuilder.CreateTable(
                name: "Inventario",
                columns: table => new
                {
                    IdInventario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSeccion = table.Column<int>(type: "int", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventario", x => x.IdInventario);
                    table.CheckConstraint("CK_Inventario_Estado", "[Estado] IN ('ABIERTO','FINALIZADO')");
                    table.CheckConstraint("CK_Inventario_Fechas", "[FechaCierre] IS NULL OR [FechaCierre] > [FechaAlta]");
                    table.ForeignKey(
                        name: "FK_Inventario_Seccion",
                        column: x => x.IdSeccion,
                        principalTable: "Seccion",
                        principalColumn: "IdSeccion");
                });

            migrationBuilder.CreateTable(
                name: "Etiqueta",
                columns: table => new
                {
                    IdEtiqueta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    CodigoQr = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiqueta", x => x.IdEtiqueta);
                    table.CheckConstraint("CK_Etiqueta_CodigoQr", "LEN([CodigoQr]) > 5");
                    table.ForeignKey(
                        name: "FK_Etiqueta_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "Movimiento",
                columns: table => new
                {
                    IdMovimiento = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdSeccion = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    CantidadUnidades = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ValorAnterior = table.Column<int>(type: "int", nullable: false),
                    ValorPosterior = table.Column<int>(type: "int", nullable: false),
                    Resultado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimiento", x => x.IdMovimiento);
                    table.CheckConstraint("CK_Movimiento_Cantidad", "[CantidadUnidades] > 0");
                    table.CheckConstraint("CK_Movimiento_Resultado", "[Resultado] IN ('EXITOSO','FALLIDO')");
                    table.CheckConstraint("CK_Movimiento_TipoMovimiento", "[TipoMovimiento] IN ('INGRESO','EGRESO','TRANSFERENCIA','AJUSTE_STOCK')");
                    table.ForeignKey(
                        name: "FK_Movimiento_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_Movimiento_Seccion",
                        column: x => x.IdSeccion,
                        principalTable: "Seccion",
                        principalColumn: "IdSeccion");
                    table.ForeignKey(
                        name: "FK_Movimiento_Usuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAjuste",
                columns: table => new
                {
                    IdSolicitud = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdEmpleadoSolicitante = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IdSupervisorResolutor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Tipo = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Motivo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CantidadSolicitada = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAjuste", x => x.IdSolicitud);
                    table.CheckConstraint("CK_Ajuste_CantidadSolicitada", "[CantidadSolicitada] >= 0");
                    table.CheckConstraint("CK_Ajuste_Estado", "[Estado] IN ('PENDIENTE','APROBADO','RECHAZADO')");
                    table.CheckConstraint("CK_Ajuste_FechaResolucion", "[FechaResolucion] > [FechaSolicitud]");
                    table.CheckConstraint("CK_Ajuste_Motivo", "LEN([Motivo]) > 1");
                    table.CheckConstraint("CK_Ajuste_Tipo", "[Tipo] IN ('STOCK','INVENTARIO')");
                    table.ForeignKey(
                        name: "FK_Ajuste_Empleado",
                        column: x => x.IdEmpleadoSolicitante,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ajuste_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_Ajuste_Supervisor",
                        column: x => x.IdSupervisorResolutor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    IdStock = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdSeccion = table.Column<int>(type: "int", nullable: false),
                    CantidadActual = table.Column<int>(type: "int", nullable: false),
                    Minimo = table.Column<int>(type: "int", nullable: false),
                    Maximo = table.Column<int>(type: "int", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.IdStock);
                    table.CheckConstraint("CK_Stock_Cantidad", "[CantidadActual] >= 0 AND [CantidadActual] < 1999");
                    table.CheckConstraint("CK_Stock_Maximo", "[Maximo] > [Minimo]");
                    table.CheckConstraint("CK_Stock_Minimo", "[Minimo] >= 0");
                    table.ForeignKey(
                        name: "FK_Stock_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_Stock_Seccion",
                        column: x => x.IdSeccion,
                        principalTable: "Seccion",
                        principalColumn: "IdSeccion");
                });

            migrationBuilder.CreateTable(
                name: "DetalleTransferencia",
                columns: table => new
                {
                    IdDetalleTransferencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTransferencia = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleTransferencia", x => x.IdDetalleTransferencia);
                    table.CheckConstraint("CK_DetalleTransferencia_Cantidad", "[Cantidad] > 0");
                    table.ForeignKey(
                        name: "FK_DetalleTransferencia_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                    table.ForeignKey(
                        name: "FK_DetalleTransferencia_Transferencia",
                        column: x => x.IdTransferencia,
                        principalTable: "Transferencia",
                        principalColumn: "IdTransferencia");
                });

            migrationBuilder.CreateTable(
                name: "DetalleInventario",
                columns: table => new
                {
                    IdDetalleInventario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInventario = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    CantidadInventario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleInventario", x => x.IdDetalleInventario);
                    table.CheckConstraint("CK_DetalleInventario_Cantidad", "[CantidadInventario] >= 0");
                    table.ForeignKey(
                        name: "FK_DetalleInventario_Inventario",
                        column: x => x.IdInventario,
                        principalTable: "Inventario",
                        principalColumn: "IdInventario");
                    table.ForeignKey(
                        name: "FK_DetalleInventario_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto");
                });

            migrationBuilder.CreateTable(
                name: "Alerta",
                columns: table => new
                {
                    IdAlerta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStock = table.Column<int>(type: "int", nullable: false),
                    TipoAlerta = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerta", x => x.IdAlerta);
                    table.CheckConstraint("CK_Alerta_Tipo", "[TipoAlerta] IN ('MINIMO','MAXIMO')");
                    table.ForeignKey(
                        name: "FK_Alerta_Stock",
                        column: x => x.IdStock,
                        principalTable: "Stock",
                        principalColumn: "IdStock");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerta_IdStock",
                table: "Alerta",
                column: "IdStock");

            migrationBuilder.CreateIndex(
                name: "UQ_Almacen_Nombre",
                table: "Almacen",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleInventario_IdInventario",
                table: "DetalleInventario",
                column: "IdInventario");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleInventario_IdProducto",
                table: "DetalleInventario",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleTransferencia_IdProducto",
                table: "DetalleTransferencia",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleTransferencia_IdTransferencia",
                table: "DetalleTransferencia",
                column: "IdTransferencia");

            migrationBuilder.CreateIndex(
                name: "IX_Etiqueta_IdProducto",
                table: "Etiqueta",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "UQ_Etiqueta_Codigo",
                table: "Etiqueta",
                column: "CodigoQr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventario_IdSeccion",
                table: "Inventario",
                column: "IdSeccion");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_IdProducto",
                table: "Movimiento",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_IdSeccion",
                table: "Movimiento",
                column: "IdSeccion");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_IdUsuario",
                table: "Movimiento",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdProveedor",
                table: "Producto",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "UQ_Producto_SKU",
                table: "Producto",
                column: "CodigoSKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Proveedor_Mail",
                table: "Proveedor",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ_Proveedor_Rut",
                table: "Proveedor",
                column: "Rut",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Seccion",
                table: "Seccion",
                columns: new[] { "IdAlmacen", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudAjuste_IdEmpleadoSolicitante",
                table: "SolicitudAjuste",
                column: "IdEmpleadoSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudAjuste_IdProducto",
                table: "SolicitudAjuste",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudAjuste_IdSupervisorResolutor",
                table: "SolicitudAjuste",
                column: "IdSupervisorResolutor");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_IdSeccion",
                table: "Stock",
                column: "IdSeccion");

            migrationBuilder.CreateIndex(
                name: "UQ_Stock",
                table: "Stock",
                columns: new[] { "IdProducto", "IdSeccion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_IdAdministradorAutorizador",
                table: "Transferencia",
                column: "IdAdministradorAutorizador");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_IdAlmacenDestino",
                table: "Transferencia",
                column: "IdAlmacenDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_IdAlmacenOrigen",
                table: "Transferencia",
                column: "IdAlmacenOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_IdSupervisorSolicitante",
                table: "Transferencia",
                column: "IdSupervisorSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_IdTransportista",
                table: "Transferencia",
                column: "IdTransportista");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencia_IdVehiculo",
                table: "Transferencia",
                column: "IdVehiculo");

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionGPS_IdTransportista",
                table: "UbicacionGPS",
                column: "IdTransportista");

            migrationBuilder.CreateIndex(
                name: "UQ_Vehiculo_Matricula",
                table: "Vehiculo",
                column: "Matricula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerta");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DetalleInventario");

            migrationBuilder.DropTable(
                name: "DetalleTransferencia");

            migrationBuilder.DropTable(
                name: "Etiqueta");

            migrationBuilder.DropTable(
                name: "Movimiento");

            migrationBuilder.DropTable(
                name: "SolicitudAjuste");

            migrationBuilder.DropTable(
                name: "UbicacionGPS");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Inventario");

            migrationBuilder.DropTable(
                name: "Transferencia");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Seccion");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Vehiculo");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Almacen");
        }
    }
}
