-- Si la base de datos existe, la borro. --
USE master
GO

IF EXISTS(SELECT * FROM SysDataBases WHERE name='ElectricERP')
BEGIN
    DROP DATABASE ElectricERP
END
GO

-- Creo la base de datos
CREATE DATABASE ElectricERP
GO

-- Pongo en uso la base de datos
USE ElectricERP
GO

---------- TABLA SIMULADA DE IDENTITY ----------
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    PasswordHash NVARCHAR(MAX) NULL,
    Rol VARCHAR(20) NOT NULL DEFAULT 'Operador',

    CONSTRAINT CK_AspNetUsers_Rol
        CHECK (Rol IN ('Admin','Supervisor','Operador','Transportista'))
);
GO

---------- TABLAS ----------
CREATE TABLE Almacen (
    IdAlmacen INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL CHECK(LEN(Nombre)>4), 
    Direccion VARCHAR(200) NOT NULL CHECK(LEN(Direccion)>7),
    FechaAlta DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_Almacen_Nombre UNIQUE(Nombre)
);

CREATE TABLE Seccion (
    IdSeccion INT IDENTITY(1,1) PRIMARY KEY,
    IdAlmacen INT NOT NULL,
    Nombre VARCHAR(50) NOT NULL CHECK(LEN(Nombre)>3),
    Descripcion VARCHAR(250) NULL CHECK(LEN(Descripcion)>1),

    CONSTRAINT FK_Seccion_Almacen
        FOREIGN KEY (IdAlmacen)
        REFERENCES Almacen(IdAlmacen),

    CONSTRAINT UQ_Seccion
        UNIQUE(IdAlmacen, Nombre)
);

CREATE TABLE Proveedor (
    IdProveedor INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(75) NOT NULL CHECK(LEN(Nombre)>3),
    Rut VARCHAR(11) NOT NULL CHECK(LEN(Rut)=11 AND Rut LIKE '21%' AND Rut NOT LIKE '%[^0-9]%'),
    Direccion VARCHAR(100) NOT NULL CHECK(LEN(Direccion)>7),
    Telefono VARCHAR(12) NULL CHECK(LEN(Telefono)>7 AND Telefono NOT LIKE '%[^0-9]%'),
    Email VARCHAR(30) NULL CHECK(LEN(Email)>7
        AND Email LIKE '%_@_%_.%'             -- estructura básica: algo @ algo . algo
        AND Email NOT LIKE '% %'             -- sin espacios
        AND Email NOT LIKE '%..%'            -- sin puntos consecutivos
        AND Email NOT LIKE '%@%@%'           -- solo una @
        AND Email NOT LIKE '%.@'             -- sin punto antes de @
        AND Email NOT LIKE '@%'              -- no empieza con @
        AND Email NOT LIKE '%.'),            -- no termina con punto

    CONSTRAINT UQ_Proveedor_Rut UNIQUE(Rut),
    CONSTRAINT UQ_Proveedor_Mail UNIQUE(Email)
);

CREATE TABLE Producto (
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    IdProveedor INT NOT NULL,
    Nombre VARCHAR(50) NOT NULL CHECK(LEN(Nombre)>3),
    CodigoSKU VARCHAR(50) NOT NULL CHECK(LEN(CodigoSKU)>10
        AND CodigoSKU NOT LIKE '%[^A-Z0-9]%'         -- solo mayúsculas y números
        AND CodigoSKU LIKE '%[A-Z]%'                 -- al menos una letra
        AND CodigoSKU LIKE '%[0-9]%'),                -- al menos un número
    Descripcion VARCHAR(50) NULL CHECK(LEN(Descripcion)>1),
    Categoria VARCHAR(20) NOT NULL CHECK(Categoria IN ('Resistencias','Capacitores','Inductores', 'Diodos','Transistores','Circuitos','Integrados','Sensores','Conectores',
    'Cables','Fuentes de Alimentacion','Reles','Fusibles','Herramientas','Accesorios')),
    UnidadMedida VARCHAR(50) NOT NULL CHECK(UnidadMedida IN ('m', 'cm', 'kg', 'Consumo Electrico')),

    CONSTRAINT FK_Producto_Proveedor
        FOREIGN KEY (IdProveedor)
        REFERENCES Proveedor(IdProveedor),

    CONSTRAINT UQ_Producto_SKU UNIQUE(CodigoSKU)
);

CREATE TABLE Etiqueta (
    IdEtiqueta INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto INT NOT NULL,
    CodigoQR VARCHAR(200) NOT NULL CHECK(LEN(CodigoQR)>5),
    FechaAlta DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Etiqueta_Producto
        FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto),

    CONSTRAINT UQ_Etiqueta_Codigo UNIQUE(CodigoQR)
);

CREATE TABLE Stock (
    IdStock INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto INT NOT NULL,
    IdSeccion INT NOT NULL,
    CantidadActual INT NOT NULL,
    Minimo INT NOT NULL,
    Maximo INT NOT NULL,
    UltimaActualizacion DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Stock_Producto
        FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto),

    CONSTRAINT FK_Stock_Seccion
        FOREIGN KEY (IdSeccion)
        REFERENCES Seccion(IdSeccion),

    CONSTRAINT CK_Stock_Cantidad
        CHECK (CantidadActual >= 0 AND CantidadActual < 1999),

    CONSTRAINT CK_Stock_Minimo
        CHECK (Minimo >= 0),

    CONSTRAINT CK_Stock_Maximo
        CHECK (Maximo > Minimo),

    CONSTRAINT UQ_Stock
        UNIQUE(IdProducto, IdSeccion)
);

CREATE TABLE Alerta (
    IdAlerta INT IDENTITY(1,1) PRIMARY KEY,
    IdStock INT NOT NULL,
    TipoAlerta VARCHAR(6) NOT NULL,
    FechaHora DATETIME2 NOT NULL DEFAULT GETDATE(),
    Estado BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Alerta_Stock
        FOREIGN KEY (IdStock)
        REFERENCES Stock(IdStock),

    CONSTRAINT CK_Alerta_Tipo
        CHECK (TipoAlerta IN ('MINIMO','MAXIMO'))
);

CREATE TABLE Inventario (
    IdInventario INT IDENTITY(1,1) PRIMARY KEY,
    IdSeccion INT NOT NULL,
    FechaAlta DATETIME2 NOT NULL DEFAULT GETDATE(),
    Estado VARCHAR(20) NOT NULL,
    FechaCierre DATETIME2 NULL,

    CONSTRAINT FK_Inventario_Seccion
        FOREIGN KEY (IdSeccion)
        REFERENCES Seccion(IdSeccion),

    CONSTRAINT CK_Inventario_Estado
        CHECK (Estado IN ('ABIERTO','FINALIZADO')),

    CONSTRAINT CK_Inventario_Fechas
        CHECK (FechaCierre IS NULL OR FechaCierre > FechaAlta)
);

CREATE TABLE DetalleInventario (
    IdDetalleInventario INT IDENTITY(1,1) PRIMARY KEY,
    IdInventario INT NOT NULL,
    IdProducto INT NOT NULL,
    CantidadInventario INT NOT NULL,

    CONSTRAINT FK_DetalleInventario_Inventario
        FOREIGN KEY (IdInventario)
        REFERENCES Inventario(IdInventario),

    CONSTRAINT FK_DetalleInventario_Producto
        FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto),

    CONSTRAINT CK_DetalleInventario_Cantidad
        CHECK (CantidadInventario >= 0)
);

CREATE TABLE SolicitudAjuste (
    IdSolicitud INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto INT NOT NULL,
    IdEmpleadoSolicitante NVARCHAR(450) NOT NULL,
    IdSupervisorResolutor NVARCHAR(450) NULL,
    Tipo VARCHAR(20) NOT NULL,
    Motivo VARCHAR(100) NOT NULL,
    CantidadSolicitada INT NOT NULL,
    Estado VARCHAR(20) NOT NULL,
    FechaSolicitud DATETIME2 NOT NULL DEFAULT GETDATE(),
    FechaResolucion DATETIME2 NULL,

    CONSTRAINT FK_Ajuste_Producto
        FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto),

    CONSTRAINT FK_Ajuste_Empleado
        FOREIGN KEY (IdEmpleadoSolicitante)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT FK_Ajuste_Supervisor
        FOREIGN KEY (IdSupervisorResolutor)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT CK_Ajuste_Tipo
        CHECK (Tipo IN ('STOCK','INVENTARIO')),

    CONSTRAINT CK_Ajuste_Motivo
        CHECK (LEN(Motivo) > 1),

    CONSTRAINT CK_Ajuste_CantidadSolicitada
        CHECK (CantidadSolicitada >= 0),

    CONSTRAINT CK_Ajuste_FechaResolucion
        CHECK (FechaResolucion > FechaSolicitud),

    CONSTRAINT CK_Ajuste_Estado
        CHECK (Estado IN ('PENDIENTE','APROBADO','RECHAZADO'))
);

CREATE TABLE Vehiculo (
    IdVehiculo INT IDENTITY(1,1) PRIMARY KEY,
    Matricula VARCHAR(20) NOT NULL,
    Marca VARCHAR(20) NOT NULL,
    Modelo VARCHAR(20) NOT NULL,
    CapacidadCarga INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT UQ_Vehiculo_Matricula UNIQUE(Matricula),

    CONSTRAINT CK_Vehiculo_Matricula
        CHECK (
            LEN(Matricula) = 8
            AND Matricula LIKE '[A-Z][A-Z][A-Z] [0-9][0-9][0-9][0-9]'),

    CONSTRAINT CK_Vehiculo_Marca
        CHECK (LEN(Marca) > 3),

    CONSTRAINT CK_Vehiculo_Modelo
        CHECK (LEN(Modelo) > 3),

    CONSTRAINT CK_Vehiculo_Carga
        CHECK (CapacidadCarga > 0)
);

CREATE TABLE Transferencia (
    IdTransferencia INT IDENTITY(1,1) PRIMARY KEY,
    IdSupervisorSolicitante NVARCHAR(450) NOT NULL,
    IdAdministradorAutorizador NVARCHAR(450) NULL,
    IdTransportista NVARCHAR(450) NULL,
    IdVehiculo INT NULL,
    IdAlmacenOrigen INT NOT NULL,
    IdAlmacenDestino INT NOT NULL,
    Estado VARCHAR(20) NOT NULL,
    FechaSolicitud DATETIME2 NOT NULL DEFAULT GETDATE(),
    FechaAutorizacion DATETIME2 NULL,
    FechaEjecucion DATETIME2 NULL,
    FechaEntrega DATETIME2 NULL,

    CONSTRAINT FK_Transferencia_Supervisor
        FOREIGN KEY (IdSupervisorSolicitante)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT FK_Transferencia_Admin
        FOREIGN KEY (IdAdministradorAutorizador)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT FK_Transferencia_Transportista
        FOREIGN KEY (IdTransportista)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT FK_Transferencia_Vehiculo
        FOREIGN KEY (IdVehiculo)
        REFERENCES Vehiculo(IdVehiculo),

    CONSTRAINT FK_Transferencia_Origen
        FOREIGN KEY (IdAlmacenOrigen)
        REFERENCES Almacen(IdAlmacen),

    CONSTRAINT FK_Transferencia_Destino
        FOREIGN KEY (IdAlmacenDestino)
        REFERENCES Almacen(IdAlmacen),

    CONSTRAINT CK_Transferencia_Estado
        CHECK (
            Estado IN (
                'PENDIENTE',
                'AUTORIZADA',
                'ASIGNADA',
                'EN_TRANSITO',
                'ENTREGADA',
                'FINALIZADA',
                'RECHAZADA')),

    CONSTRAINT CK_Transferencia_Almacenes
        CHECK (IdAlmacenOrigen <> IdAlmacenDestino),

    CONSTRAINT CK_Transferencia_FechaAutorizacion
        CHECK (FechaAutorizacion > FechaSolicitud),

    CONSTRAINT CK_Transferencia_FechaEjecucion
        CHECK (FechaEjecucion > FechaAutorizacion),

    CONSTRAINT CK_Transferencia_FechaEntrega
        CHECK (FechaEntrega > FechaEjecucion)
);

CREATE TABLE DetalleTransferencia (
    IdDetalleTransferencia INT IDENTITY(1,1) PRIMARY KEY,
    IdTransferencia INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,

    CONSTRAINT FK_DetalleTransferencia_Transferencia
        FOREIGN KEY (IdTransferencia)
        REFERENCES Transferencia(IdTransferencia),

    CONSTRAINT FK_DetalleTransferencia_Producto
        FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto),

    CONSTRAINT CK_DetalleTransferencia_Cantidad
        CHECK (Cantidad > 0)
);

CREATE TABLE UbicacionGPS (
    IdUbicacion INT IDENTITY(1,1) PRIMARY KEY,
    IdTransportista NVARCHAR(450) NOT NULL,
    Latitud DECIMAL(10,8) NOT NULL,
    Longitud DECIMAL(11,8) NOT NULL,
    FechaHora DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_UbicacionGPS_Transportista
        FOREIGN KEY (IdTransportista)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT CK_UbicacionGPS_Latitud
        CHECK (Latitud BETWEEN -90 AND 90),

    CONSTRAINT CK_UbicacionGPS_Longitud
        CHECK (Longitud BETWEEN -180 AND 180)
);

CREATE TABLE Movimiento (
    IdMovimiento BIGINT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario NVARCHAR(450) NOT NULL,
    IdProducto INT NOT NULL,
    IdSeccion INT NOT NULL,
    TipoMovimiento VARCHAR(30) NOT NULL,
    CantidadUnidades INT NOT NULL,
    FechaHora DATETIME2 NOT NULL DEFAULT GETDATE(),
    Estado VARCHAR(20) NOT NULL,
    ValorAnterior INT NOT NULL,
    ValorPosterior INT NOT NULL,
    Resultado VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Movimiento_Usuario
        FOREIGN KEY (IdUsuario)
        REFERENCES AspNetUsers(Id),

    CONSTRAINT FK_Movimiento_Producto
        FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto),

    CONSTRAINT FK_Movimiento_Seccion
        FOREIGN KEY (IdSeccion)
        REFERENCES Seccion(IdSeccion),

    CONSTRAINT CK_Movimiento_Cantidad
        CHECK (CantidadUnidades > 0),

    CONSTRAINT CK_Movimiento_TipoMovimiento
        CHECK (TipoMovimiento IN ('INGRESO','EGRESO','TRANFERENCIA','AJUSTE_STOCK')),

    CONSTRAINT CK_Movimiento_Resultado
        CHECK (Resultado IN ('EXITOSO','FALLIDO'))
);