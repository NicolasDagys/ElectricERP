using ElectricERP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ElectricERP.Infrastructure.Data;

public partial class ElectricERPDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ElectricERPDbContext()
    {
    }

    public ElectricERPDbContext(DbContextOptions<ElectricERPDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Alerta> Alertas { get; set; }

    public virtual DbSet<Almacen> Almacenes { get; set; }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public virtual DbSet<DetalleInventario> DetalleInventarios { get; set; }

    public virtual DbSet<DetalleTransferencia> DetalleTransferencias { get; set; }

    public virtual DbSet<Etiqueta> Etiquetas { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedor> Proveedores { get; set; }

    public virtual DbSet<Seccion> Secciones { get; set; }

    public virtual DbSet<SolicitudAjuste> SolicitudAjustes { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Transferencia> Transferencias { get; set; }

    public virtual DbSet<UbicacionGps> UbicacionGps { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
         //=> optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ElectricERP;Trusted_Connection=True;TrustServerCertificate=True;");
         => optionsBuilder.UseSqlServer("Server=NICOD\\SQLEXPRESS;Database=ElectricERP;Trusted_Connection=True;TrustServerCertificate=True;");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================================
        // ALERTA
        // =========================================================

        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.HasKey(e => e.IdAlerta);

            entity.ToTable("Alerta", table =>
            {
                table.HasCheckConstraint(
                    "CK_Alerta_Tipo",
                    "[TipoAlerta] IN ('MINIMO','MAXIMO')");
            });

            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.TipoAlerta)
                .HasMaxLength(6)
                .IsUnicode(false);

            entity.HasOne(d => d.IdStockNavigation)
                .WithMany(p => p.Alerta)
                .HasForeignKey(d => d.IdStock)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Alerta_Stock");
        });


        // =========================================================
        // ALMACEN
        // =========================================================

        modelBuilder.Entity<Almacen>(entity =>
        {
            entity.HasKey(e => e.IdAlmacen);

            entity.ToTable("Almacen", table =>
            {
                table.HasCheckConstraint(
                    "CK_Almacen_Nombre",
                    "LEN([Nombre]) > 4");

                table.HasCheckConstraint(
                    "CK_Almacen_Direccion",
                    "LEN([Direccion]) > 7");
            });

            entity.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("UQ_Almacen_Nombre");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("(getdate())");
        });


        // =========================================================
        // SECCION
        // =========================================================

        modelBuilder.Entity<Seccion>(entity =>
        {
            entity.HasKey(e => e.IdSeccion);

            entity.ToTable("Seccion", table =>
            {
                table.HasCheckConstraint(
                    "CK_Seccion_Nombre",
                    "LEN([Nombre]) > 3");

                table.HasCheckConstraint(
                    "CK_Seccion_Descripcion",
                    "[Descripcion] IS NULL OR LEN([Descripcion]) > 1");
            });

            entity.HasIndex(e => new
            {
                e.IdAlmacen,
                e.Nombre
            })
            .IsUnique()
            .HasDatabaseName("UQ_Seccion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAlmacenNavigation)
                .WithMany(p => p.Secciones)
                .HasForeignKey(d => d.IdAlmacen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seccion_Almacen");
        });


        // =========================================================
        // PROVEEDOR
        // =========================================================

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor);

            entity.ToTable("Proveedor", table =>
            {
                table.HasCheckConstraint(
                    "CK_Proveedor_Nombre",
                    "LEN([Nombre]) > 3");

                table.HasCheckConstraint(
                    "CK_Proveedor_Rut",
                    "LEN([Rut]) = 11 " +
                    "AND [Rut] LIKE '21%' " +
                    "AND [Rut] NOT LIKE '%[^0-9]%'");

                table.HasCheckConstraint(
                    "CK_Proveedor_Direccion",
                    "LEN([Direccion]) > 7");

                table.HasCheckConstraint(
                    "CK_Proveedor_Telefono",
                    "[Telefono] IS NULL OR " +
                    "(LEN([Telefono]) > 7 " +
                    "AND [Telefono] NOT LIKE '%[^0-9]%')");

                table.HasCheckConstraint(
                    "CK_Proveedor_Email",
                    "[Email] IS NULL OR " +
                    "(LEN([Email]) > 7 " +
                    "AND [Email] LIKE '%_@_%_.%' " +
                    "AND [Email] NOT LIKE '% %' " +
                    "AND [Email] NOT LIKE '%..%' " +
                    "AND [Email] NOT LIKE '%@%@%' " +
                    "AND [Email] NOT LIKE '%.@' " +
                    "AND [Email] NOT LIKE '@%' " +
                    "AND [Email] NOT LIKE '%.')");
            });

            entity.HasIndex(e => e.Rut)
                .IsUnique()
                .HasDatabaseName("UQ_Proveedor_Rut");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("UQ_Proveedor_Mail");

            entity.Property(e => e.Nombre)
                .HasMaxLength(75)
                .IsUnicode(false);

            entity.Property(e => e.Rut)
                .HasMaxLength(11)
                .IsUnicode(false);

            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Telefono)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
        });


        // =========================================================
        // PRODUCTO
        // =========================================================

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.ToTable("Producto", table =>
            {
                table.HasCheckConstraint(
                    "CK_Producto_Nombre",
                    "LEN([Nombre]) > 3");

                table.HasCheckConstraint(
                    "CK_Producto_CodigoSKU",
                    "LEN([CodigoSKU]) > 10 " +
                    "AND [CodigoSKU] NOT LIKE '%[^A-Z0-9]%' " +
                    "AND [CodigoSKU] LIKE '%[A-Z]%' " +
                    "AND [CodigoSKU] LIKE '%[0-9]%'");

                table.HasCheckConstraint(
                    "CK_Producto_Descripcion",
                    "[Descripcion] IS NULL OR LEN([Descripcion]) > 1");

                table.HasCheckConstraint(
                    "CK_Producto_Categoria",
                    "[Categoria] IN (" +
                    "'Resistencias','Capacitores','Inductores','Diodos'," +
                    "'Transistores','Circuitos','Integrados','Sensores'," +
                    "'Conectores','Cables','Fuentes de Alimentacion'," +
                    "'Reles','Fusibles','Herramientas','Accesorios')");

                table.HasCheckConstraint(
                    "CK_Producto_UnidadMedida",
                    "[UnidadMedida] IN ('m','cm','kg','Consumo Electrico')");
            });

            entity.HasIndex(e => e.CodigoSku)
                .IsUnique()
                .HasDatabaseName("UQ_Producto_SKU");

            entity.Property(e => e.CodigoSku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CodigoSKU");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Categoria)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdProveedorNavigation)
                .WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Proveedor");
        });


        // =========================================================
        // ETIQUETA
        // =========================================================

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.HasKey(e => e.IdEtiqueta);

            entity.ToTable("Etiqueta", table =>
            {
                table.HasCheckConstraint(
                    "CK_Etiqueta_CodigoQr",
                    "LEN([CodigoQr]) > 5");
            });

            entity.HasIndex(e => e.CodigoQr)
                .IsUnique()
                .HasDatabaseName("UQ_Etiqueta_Codigo");

            entity.Property(e => e.CodigoQr)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CodigoQr");

            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.Etiqueta)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Etiqueta_Producto");
        });


        // =========================================================
        // STOCK
        // =========================================================

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.IdStock);

            entity.ToTable("Stock", table =>
            {
                table.HasCheckConstraint(
                    "CK_Stock_Cantidad",
                    "[CantidadActual] >= 0 AND [CantidadActual] < 1999");

                table.HasCheckConstraint(
                    "CK_Stock_Minimo",
                    "[Minimo] >= 0");

                table.HasCheckConstraint(
                    "CK_Stock_Maximo",
                    "[Maximo] > [Minimo]");
            });

            entity.HasIndex(e => new
            {
                e.IdProducto,
                e.IdSeccion
            })
            .IsUnique()
            .HasDatabaseName("UQ_Stock");

            entity.Property(e => e.UltimaActualizacion)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Producto");

            entity.HasOne(d => d.IdSeccionNavigation)
                .WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdSeccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Seccion");
        });


        // =========================================================
        // ALERTA
        // =========================================================

        // Ya configurada arriba.


        // =========================================================
        // INVENTARIO
        // =========================================================

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario);

            entity.ToTable("Inventario", table =>
            {
                table.HasCheckConstraint(
                    "CK_Inventario_Estado",
                    "[Estado] IN ('ABIERTO','FINALIZADO')");

                table.HasCheckConstraint(
                    "CK_Inventario_Fechas",
                    "[FechaCierre] IS NULL OR [FechaCierre] > [FechaAlta]");
            });

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdSeccionNavigation)
                .WithMany(p => p.Inventarios) //'Seccion' does not contain a definition for 'Inventarios' and no accessible extension method 'Inventarios' accepting a first argument of type 'Seccion' could be found (are you missing a using directive or an assembly reference?)
                .HasForeignKey(d => d.IdSeccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Seccion");
        });


        // =========================================================
        // DETALLE INVENTARIO
        // =========================================================

        modelBuilder.Entity<DetalleInventario>(entity =>
        {
            entity.HasKey(e => e.IdDetalleInventario);

            entity.ToTable("DetalleInventario", table =>
            {
                table.HasCheckConstraint(
                    "CK_DetalleInventario_Cantidad",
                    "[CantidadInventario] >= 0");
            });

            entity.HasOne(d => d.IdInventarioNavigation)
                .WithMany(p => p.DetalleInventarios)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleInventario_Inventario");

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleInventarios)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleInventario_Producto");
        });


        // =========================================================
        // VEHICULO
        // =========================================================

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo);

            entity.ToTable("Vehiculo", table =>
            {
                table.HasCheckConstraint(
                    "CK_Vehiculo_Matricula",
                    "LEN([Matricula]) = 8 " +
                    "AND [Matricula] LIKE '[A-Z][A-Z][A-Z] [0-9][0-9][0-9][0-9]'");

                table.HasCheckConstraint(
                    "CK_Vehiculo_Marca",
                    "LEN([Marca]) > 3");

                table.HasCheckConstraint(
                    "CK_Vehiculo_Modelo",
                    "LEN([Modelo]) > 3");

                table.HasCheckConstraint(
                    "CK_Vehiculo_Carga",
                    "[CapacidadCarga] > 0");
            });

            entity.HasIndex(e => e.Matricula)
                .IsUnique()
                .HasDatabaseName("UQ_Vehiculo_Matricula");

            entity.Property(e => e.Matricula)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Marca)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Modelo)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Activo)
                .HasDefaultValue(true);
        });


        // =========================================================
        // SOLICITUD AJUSTE
        // =========================================================

        modelBuilder.Entity<SolicitudAjuste>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud);

            entity.ToTable("SolicitudAjuste", table =>
            {
                table.HasCheckConstraint(
                    "CK_Ajuste_Tipo",
                    "[Tipo] IN ('STOCK','INVENTARIO')");

                table.HasCheckConstraint(
                    "CK_Ajuste_Motivo",
                    "LEN([Motivo]) > 1");

                table.HasCheckConstraint(
                    "CK_Ajuste_CantidadSolicitada",
                    "[CantidadSolicitada] >= 0");

                table.HasCheckConstraint(
                    "CK_Ajuste_FechaResolucion",
                    "[FechaResolucion] > [FechaSolicitud]");

                table.HasCheckConstraint(
                    "CK_Ajuste_Estado",
                    "[Estado] IN ('PENDIENTE','APROBADO','RECHAZADO')");
            });

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IdEmpleadoSolicitante)
                .HasMaxLength(450);

            entity.Property(e => e.IdSupervisorResolutor)
                .HasMaxLength(450);

            entity.Property(e => e.Motivo)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEmpleadoSolicitanteNavigation)
                .WithMany(p => p.SolicitudAjusteIdEmpleadoSolicitanteNavigations)
                .HasForeignKey(d => d.IdEmpleadoSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ajuste_Empleado");

            entity.HasOne(d => d.IdSupervisorResolutorNavigation)
                .WithMany(p => p.SolicitudAjusteIdSupervisorResolutorNavigations)
                .HasForeignKey(d => d.IdSupervisorResolutor)
                .HasConstraintName("FK_Ajuste_Supervisor");

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.SolicitudAjustes)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ajuste_Producto");
        });


        // =========================================================
        // TRANSFERENCIA
        // =========================================================

        modelBuilder.Entity<Transferencia>(entity =>
        {
            entity.HasKey(e => e.IdTransferencia);

            entity.ToTable("Transferencia", table =>
            {
                table.HasCheckConstraint(
                    "CK_Transferencia_Estado",
                    "[Estado] IN (" +
                    "'PENDIENTE'," +
                    "'AUTORIZADA'," +
                    "'ASIGNADA'," +
                    "'EN_TRANSITO'," +
                    "'ENTREGADA'," +
                    "'FINALIZADA'," +
                    "'RECHAZADA')");

                table.HasCheckConstraint(
                    "CK_Transferencia_Almacenes",
                    "[IdAlmacenOrigen] <> [IdAlmacenDestino]");

                table.HasCheckConstraint(
                    "CK_Transferencia_FechaAutorizacion",
                    "[FechaAutorizacion] IS NULL OR " +
                    "[FechaAutorizacion] > [FechaSolicitud]");

                table.HasCheckConstraint(
                    "CK_Transferencia_FechaEjecucion",
                    "[FechaEjecucion] IS NULL OR " +
                    "[FechaEjecucion] > [FechaAutorizacion]");

                table.HasCheckConstraint(
                    "CK_Transferencia_FechaEntrega",
                    "[FechaEntrega] IS NULL OR " +
                    "[FechaEntrega] > [FechaEjecucion]");
            });

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IdAdministradorAutorizador)
                .HasMaxLength(450);

            entity.Property(e => e.IdSupervisorSolicitante)
                .HasMaxLength(450);

            entity.Property(e => e.IdTransportista)
                .HasMaxLength(450);

            entity.HasOne(d => d.IdAdministradorAutorizadorNavigation)
                .WithMany(p => p.TransferenciaIdAdministradorAutorizadorNavigations)
                .HasForeignKey(d => d.IdAdministradorAutorizador)
                .HasConstraintName("FK_Transferencia_Admin");

            entity.HasOne(d => d.IdSupervisorSolicitanteNavigation)
                .WithMany(p => p.TransferenciaIdSupervisorSolicitanteNavigations)
                .HasForeignKey(d => d.IdSupervisorSolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_Supervisor");

            entity.HasOne(d => d.IdTransportistaNavigation)
                .WithMany(p => p.TransferenciaIdTransportistaNavigations)
                .HasForeignKey(d => d.IdTransportista)
                .HasConstraintName("FK_Transferencia_Transportista");

            entity.HasOne(d => d.IdAlmacenOrigenNavigation)
                .WithMany(p => p.TransferenciaIdAlmacenOrigenNavigations)
                .HasForeignKey(d => d.IdAlmacenOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_Origen");

            entity.HasOne(d => d.IdAlmacenDestinoNavigation)
                .WithMany(p => p.TransferenciaIdAlmacenDestinoNavigations)
                .HasForeignKey(d => d.IdAlmacenDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_Destino");

            entity.HasOne(d => d.IdVehiculoNavigation)
                .WithMany(p => p.Transferencia)
                .HasForeignKey(d => d.IdVehiculo)
                .HasConstraintName("FK_Transferencia_Vehiculo");
        });


        // =========================================================
        // DETALLE TRANSFERENCIA
        // =========================================================

        modelBuilder.Entity<DetalleTransferencia>(entity =>
        {
            entity.HasKey(e => e.IdDetalleTransferencia);

            entity.ToTable("DetalleTransferencia", table =>
            {
                table.HasCheckConstraint(
                    "CK_DetalleTransferencia_Cantidad",
                    "[Cantidad] > 0");
            });

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleTransferencia)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleTransferencia_Producto");

            entity.HasOne(d => d.IdTransferenciaNavigation)
                .WithMany(p => p.DetalleTransferencia)
                .HasForeignKey(d => d.IdTransferencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleTransferencia_Transferencia");
        });


        // =========================================================
        // UBICACION GPS
        // =========================================================

        modelBuilder.Entity<UbicacionGps>(entity =>
        {
            entity.HasKey(e => e.IdUbicacion);

            entity.ToTable("UbicacionGPS", table =>
            {
                table.HasCheckConstraint(
                    "CK_UbicacionGPS_Latitud",
                    "[Latitud] BETWEEN -90 AND 90");

                table.HasCheckConstraint(
                    "CK_UbicacionGPS_Longitud",
                    "[Longitud] BETWEEN -180 AND 180");
            });

            entity.Property(e => e.IdTransportista)
                .HasMaxLength(450);

            entity.Property(e => e.Latitud)
                .HasColumnType("decimal(10, 8)");

            entity.Property(e => e.Longitud)
                .HasColumnType("decimal(11, 8)");

            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdTransportistaNavigation)
                .WithMany(p => p.UbicacionGps)
                .HasForeignKey(d => d.IdTransportista)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UbicacionGPS_Transportista");
        });


        // =========================================================
        // MOVIMIENTO
        // =========================================================

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento);

            entity.ToTable("Movimiento", table =>
            {
                table.HasCheckConstraint(
                    "CK_Movimiento_Cantidad",
                    "[CantidadUnidades] > 0");

                table.HasCheckConstraint(
                    "CK_Movimiento_TipoMovimiento",
                    "[TipoMovimiento] IN " +
                    "('INGRESO','EGRESO','TRANSFERENCIA','AJUSTE_STOCK')");

                table.HasCheckConstraint(
                    "CK_Movimiento_Resultado",
                    "[Resultado] IN ('EXITOSO','FALLIDO')");
            });

            entity.Property(e => e.IdUsuario)
                .HasMaxLength(450);

            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Resultado)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Usuario");

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Producto");

            entity.HasOne(d => d.IdSeccionNavigation)
                .WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdSeccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Seccion");
        });


        // =========================================================
        // FIN
        // =========================================================

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
