using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ElectricERP.Infrastructure.Entities;
using ElectricERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ============================================
            // 1. CREAR ROLES
            // ============================================

            string[] roles =
            {
            "Administrador",
            "Supervisor",
            "Operador",
            "Transportista"
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        var errors = string.Join( ", ", result.Errors.Select(e => e.Description));

                        throw new Exception($"No se pudo crear el rol '{role}': {errors}");
                    }
                }
            }

            // ============================================
            // 2. CREAR USUARIOS ADMINISTRADORES
            // ============================================

            var administradores = new[]
            {
    new
    {
        Email = "dagysnicolas@gmail.com",
        UserName = "Nicolas",
        Password = "Admin123*"
    },
    new
    {
        Email = "alexlemos2297@gmail.com",
        UserName = "Alexander",
        Password = "Admin123*"
    }
};

            // Vamos a conservar una referencia al administrador principal.
            // Será utilizado en los datos iniciales del ERP.
            ApplicationUser? adminPrincipal = null;

            // ============================================
            // FASE 3 - USUARIOS DE PRUEBA
            // ============================================

            var usuariosPrueba = new[]
            {
    new
    {
        Email = "supervisor@electricerp.com",
        UserName = "Supervisor",
        Password = "Supervisor123*",
        Rol = "Supervisor"
    },
    new
    {
        Email = "operador@electricerp.com",
        UserName = "Operador",
        Password = "Operador123*",
        Rol = "Operador"
    },
    new
    {
        Email = "transportista@electricerp.com",
        UserName = "Transportista",
        Password = "Transportista123*",
        Rol = "Transportista"
    }
};

            var usuarios = new Dictionary<string, ApplicationUser>();

            // ============================================
            // CREAR ADMINISTRADORES
            // ============================================

            foreach (var adminData in administradores)
            {
                var admin = await userManager.FindByEmailAsync(adminData.Email);

                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = adminData.UserName,
                        Email = adminData.Email,
                        EmailConfirmed = true,
                        Activo = true
                    };

                    var userResult = await userManager.CreateAsync(admin, adminData.Password);

                    if (!userResult.Succeeded)
                    {
                        var errores = string.Join( ", ", userResult.Errors.Select(e => e.Description));

                        throw new Exception( $"No se pudo crear el usuario administrador '{adminData.Email}': {errores}");
                    }
                }

                if (!await userManager.IsInRoleAsync(admin, "Administrador"))
                {
                    var roleResult = await userManager.AddToRoleAsync(admin, "Administrador");

                    if (!roleResult.Succeeded)
                    {
                        var errores = string.Join( ", ", roleResult.Errors.Select(e => e.Description));

                        throw new Exception( $"No se pudo asignar el rol Administrador a '{adminData.Email}': {errores}");
                    }
                }

                // El primer administrador será utilizado como admin
                // para los registros iniciales del ERP.
                adminPrincipal ??= admin;
            }

            // ============================================
            // CREAR USUARIOS DE PRUEBA
            // ============================================

            foreach (var userData in usuariosPrueba)
            {
                var usuario = await userManager.FindByEmailAsync(userData.Email);

                if (usuario == null)
                {
                    usuario = new ApplicationUser
                    {
                        UserName = userData.UserName,
                        Email = userData.Email,
                        EmailConfirmed = true,
                        Activo = true
                    };

                    var userResult = await userManager.CreateAsync( usuario, userData.Password);

                    if (!userResult.Succeeded)
                    {
                        var errores = string.Join( ", ", userResult.Errors.Select(e => e.Description));

                        throw new Exception( $"No se pudo crear el usuario de prueba '{userData.Email}': {errores}");
                    }
                }

                if (!await userManager.IsInRoleAsync(usuario, userData.Rol))
                {
                    var roleResult = await userManager.AddToRoleAsync(usuario, userData.Rol);

                    if (!roleResult.Succeeded)
                    {
                        var errores = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                        throw new Exception( $"No se pudo asignar el rol {userData.Rol} a '{userData.Email}': {errores}");
                    }
                }

                usuarios[userData.Rol] = usuario;
            }

            // ============================================
            // FASE 4 - DATOS INICIALES DEL ERP
            // ============================================

            if (adminPrincipal == null)
            {
                throw new Exception("No se pudo obtener un administrador principal para inicializar el ERP.");
            }

            await SeedErpDataAsync( serviceProvider, adminPrincipal, usuarios["Supervisor"], usuarios["Operador"], usuarios["Transportista"]);
        }



        private static async Task SeedErpDataAsync(IServiceProvider serviceProvider, ApplicationUser admin, ApplicationUser supervisor, ApplicationUser operador,
ApplicationUser transportista)
        {
            var context = serviceProvider.GetRequiredService<ElectricERPDbContext>();

            // -----------------------------------------------------------------
            // ALMACENES (10)
            // -----------------------------------------------------------------
            List<Almacen> almacenes;
            if (!await context.Almacenes.AnyAsync())
            {
                almacenes = new List<Almacen>
        {
            new Almacen { Nombre = "Almacen Central",     Direccion = "Av. Italia 4523",        FechaAlta = new DateTime(2026, 11, 3) },
            new Almacen { Nombre = "Almacen Norte",       Direccion = "Ruta 8 Km 21.500",        FechaAlta = new DateTime(2026, 11, 10) },
            new Almacen { Nombre = "Almacen Sur",         Direccion = "Camino Maldonado 1290",   FechaAlta = new DateTime(2026, 12, 1) },
            new Almacen { Nombre = "Almacen Este",        Direccion = "Av. Giannattasio 6780",   FechaAlta = new DateTime(2026, 12, 15) },
            new Almacen { Nombre = "Almacen Oeste",       Direccion = "Camino Cibils 3345",      FechaAlta = new DateTime(2027, 1, 5) },
            new Almacen { Nombre = "Deposito Industrial", Direccion = "Parque Industrial 220",   FechaAlta = new DateTime(2027, 2, 8) },
            new Almacen { Nombre = "Deposito Logistico",  Direccion = "Av. Batlle y Ordoñez 990",FechaAlta = new DateTime(2027, 3, 12) },
            new Almacen { Nombre = "Sucursal Colonia",    Direccion = "Av. General Flores 455",  FechaAlta = new DateTime(2027, 4, 20) },
            new Almacen { Nombre = "Sucursal Salto",      Direccion = "Uruguay 812",             FechaAlta = new DateTime(2027, 6, 2) },
            new Almacen { Nombre = "Sucursal Rivera",     Direccion = "Agraciada 1560",          FechaAlta = new DateTime(2027, 9, 18) },
        };
                await context.Almacenes.AddRangeAsync(almacenes);
                await context.SaveChangesAsync();
            }
            else
            {
                almacenes = await context.Almacenes.OrderBy(a => a.IdAlmacen).ToListAsync();
            }

            // -----------------------------------------------------------------
            // SECCIONES (10) - una por almacén
            // -----------------------------------------------------------------
            List<Seccion> secciones;
            if (!await context.Secciones.AnyAsync())
            {
                secciones = new List<Seccion>
        {
            new Seccion { IdAlmacen = almacenes[0].IdAlmacen, Nombre = "Resistencias",  Descripcion = "Resistencias de todo tipo" },
            new Seccion { IdAlmacen = almacenes[1].IdAlmacen, Nombre = "Capacitores",   Descripcion = "Capacitores ceramicos y electroliticos" },
            new Seccion { IdAlmacen = almacenes[2].IdAlmacen, Nombre = "Semiconductor", Descripcion = "Diodos y transistores" },
            new Seccion { IdAlmacen = almacenes[3].IdAlmacen, Nombre = "Integrados",    Descripcion = "Circuitos integrados varios" },
            new Seccion { IdAlmacen = almacenes[4].IdAlmacen, Nombre = "Sensores",      Descripcion = "Sensores de proximidad y temperatura" },
            new Seccion { IdAlmacen = almacenes[5].IdAlmacen, Nombre = "Conectores",    Descripcion = "Conectores y bornes" },
            new Seccion { IdAlmacen = almacenes[6].IdAlmacen, Nombre = "Cableria",      Descripcion = "Cables de distintos calibres" },
            new Seccion { IdAlmacen = almacenes[7].IdAlmacen, Nombre = "Fuentes",       Descripcion = "Fuentes de alimentacion" },
            new Seccion { IdAlmacen = almacenes[8].IdAlmacen, Nombre = "Proteccion",    Descripcion = "Reles y fusibles" },
            new Seccion { IdAlmacen = almacenes[9].IdAlmacen, Nombre = "Herramientas",  Descripcion = "Herramientas y accesorios varios" },
        };
                await context.Secciones.AddRangeAsync(secciones);
                await context.SaveChangesAsync();
            }
            else
            {
                secciones = await context.Secciones.OrderBy(s => s.IdSeccion).ToListAsync();
            }

            // -----------------------------------------------------------------
            // PROVEEDORES (10)
            // Rut: 11 caracteres, empieza con "21", solo dígitos
            // -----------------------------------------------------------------
            List<Proveedor> proveedores;
            if (!await context.Proveedores.AnyAsync())
            {
                proveedores = new List<Proveedor>
        {
            new Proveedor { Nombre = "Electro Insumos SA",    Rut = "21000111119", Direccion = "Bvar. Artigas 1234",   Telefono = "29011234",  Email = "ventas@electroinsumos.com" },
            new Proveedor { Nombre = "Componentes del Este",  Rut = "21000222229", Direccion = "18 de Julio 2200",     Telefono = "29022345",  Email = "contacto@compeste.com" },
            new Proveedor { Nombre = "Distribuidora Voltek",  Rut = "21000333339", Direccion = "Av. Millan 3450",      Telefono = "29033456",  Email = "info@voltek.com" },
            new Proveedor { Nombre = "Suministros Amperio",   Rut = "21000444449", Direccion = "Colonia 890",          Telefono = "29044567",  Email = "compras@amperio.com" },
            new Proveedor { Nombre = "Importadora Ohmega",    Rut = "21000555559", Direccion = "Rambla Sur 4560",      Telefono = "29055678",  Email = "administracion@ohmega.com" },
            new Proveedor { Nombre = "Electronica del Plata", Rut = "21000666669", Direccion = "Av. Rivera 5670",      Telefono = "29066789",  Email = "ventas@delplata.com" },
            new Proveedor { Nombre = "TecnoComponentes SA",   Rut = "21000777779", Direccion = "Yaguaron 1122",        Telefono = "29077890",  Email = "soporte@tecnocomp.com" },
            new Proveedor { Nombre = "Fuentes y Reles SRL",   Rut = "21000888889", Direccion = "Paysandu 780",         Telefono = "29088901",  Email = "pedidos@fuentesyreles.com" },
            new Proveedor { Nombre = "Circuitos Uruguay",     Rut = "21000999999", Direccion = "Canelones 3300",       Telefono = "29099012",  Email = "info@circuitosuy.com" },
            new Proveedor { Nombre = "Sensores del Sur SA",   Rut = "21001000009", Direccion = "Ejido 1450",           Telefono = "29100123",  Email = "ventas@sensoresdelsur.com" },
        };
                await context.Proveedores.AddRangeAsync(proveedores);
                await context.SaveChangesAsync();
            }
            else
            {
                proveedores = await context.Proveedores.OrderBy(p => p.IdProveedor).ToListAsync();
            }

            // -----------------------------------------------------------------
            // PRODUCTOS (10)
            // CodigoSku: >10 caracteres, solo A-Z y 0-9, al menos una letra y un numero
            // Categoria y UnidadMedida deben pertenecer a las listas permitidas
            // -----------------------------------------------------------------
            List<Producto> productos;
            if (!await context.Productos.AnyAsync())
            {
                productos = new List<Producto>
        {
            new Producto { IdProveedor = proveedores[0].IdProveedor, Nombre = "Resistencia 1K",     CodigoSku = "RES1K000123A",  Descripcion = "1/4W 5%",           Categoria = "Resistencias",             UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[1].IdProveedor, Nombre = "Capacitor 100uF",     CodigoSku = "CAP100UF456B",  Descripcion = "Electrolitico 25V",  Categoria = "Capacitores",              UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[2].IdProveedor, Nombre = "Diodo 1N4007",        CodigoSku = "DIO1N4007789C", Descripcion = "Rectificador 1A",    Categoria = "Diodos",                   UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[3].IdProveedor, Nombre = "Transistor 2N2222",   CodigoSku = "TRA2N2222001D", Descripcion = "NPN general",        Categoria = "Transistores",             UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[4].IdProveedor, Nombre = "Chip Integrado 555",  CodigoSku = "INT555TIMER1E", Descripcion = "Timer clasico",      Categoria = "Integrados",               UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[5].IdProveedor, Nombre = "Sensor PIR",          CodigoSku = "SEN00PIR0002F", Descripcion = "Movimiento infrarrojo", Categoria = "Sensores",              UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[6].IdProveedor, Nombre = "Conector JST 2P",     CodigoSku = "CON0JST02P03G", Descripcion = "Paso 2.54mm",        Categoria = "Conectores",               UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[7].IdProveedor, Nombre = "Cable UTP Cat6",      CodigoSku = "CAB0UTPCAT6XX", Descripcion = "Bobina interior",    Categoria = "Cables",                   UnidadMedida = "m" },
            new Producto { IdProveedor = proveedores[8].IdProveedor, Nombre = "Fuente 12V 5A",       CodigoSku = "FUE12V5A0004H", Descripcion = "Conmutada",          Categoria = "Fuentes de Alimentacion",  UnidadMedida = "Consumo Electrico" },
            new Producto { IdProveedor = proveedores[9].IdProveedor, Nombre = "Rele 12V 10A",        CodigoSku = "REL12V10A005I", Descripcion = "Un contacto NA/NC",  Categoria = "Reles",                    UnidadMedida = "Consumo Electrico" },
        };
                await context.Productos.AddRangeAsync(productos);
                await context.SaveChangesAsync();
            }
            else
            {
                productos = await context.Productos.OrderBy(p => p.IdProducto).ToListAsync();
            }

            // -----------------------------------------------------------------
            // ETIQUETAS (10)
            // -----------------------------------------------------------------
            if (!await context.Etiquetas.AnyAsync())
            {
                var etiquetas = new List<Etiqueta>
        {
            new Etiqueta { IdProducto = productos[0].IdProducto, CodigoQr = "QR-RES1K-000123", FechaAlta = new DateTime(2026, 11, 5) },
            new Etiqueta { IdProducto = productos[1].IdProducto, CodigoQr = "QR-CAP100UF-456",  FechaAlta = new DateTime(2026, 11, 20) },
            new Etiqueta { IdProducto = productos[2].IdProducto, CodigoQr = "QR-DIO1N4007-789", FechaAlta = new DateTime(2026, 12, 4) },
            new Etiqueta { IdProducto = productos[3].IdProducto, CodigoQr = "QR-TRA2N2222-001", FechaAlta = new DateTime(2027, 1, 9) },
            new Etiqueta { IdProducto = productos[4].IdProducto, CodigoQr = "QR-INT555TIMER-1", FechaAlta = new DateTime(2027, 2, 14) },
            new Etiqueta { IdProducto = productos[5].IdProducto, CodigoQr = "QR-SENPIR-0002",   FechaAlta = new DateTime(2027, 3, 22) },
            new Etiqueta { IdProducto = productos[6].IdProducto, CodigoQr = "QR-CONJST2P-003",  FechaAlta = new DateTime(2027, 4, 30) },
            new Etiqueta { IdProducto = productos[7].IdProducto, CodigoQr = "QR-CABUTPCAT6-X",  FechaAlta = new DateTime(2027, 6, 11) },
            new Etiqueta { IdProducto = productos[8].IdProducto, CodigoQr = "QR-FUE12V5A-0004", FechaAlta = new DateTime(2027, 8, 3) },
            new Etiqueta { IdProducto = productos[9].IdProducto, CodigoQr = "QR-REL12V10A-005", FechaAlta = new DateTime(2027, 10, 15) },
        };
                await context.Etiquetas.AddRangeAsync(etiquetas);
                await context.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // VEHICULOS (10)
            // -----------------------------------------------------------------
            List<Vehiculo> vehiculos;
            if (!await context.Vehiculos.AnyAsync())
            {
                vehiculos = new List<Vehiculo>
        {
            new Vehiculo { Matricula = "SAB 1234", Marca = "Chevrolet", Modelo = "Montana",  CapacidadCarga = 750,  Activo = true },
            new Vehiculo { Matricula = "SAC 2345", Marca = "Fiat",      Modelo = "Fiorino",  CapacidadCarga = 650,  Activo = true },
            new Vehiculo { Matricula = "SAD 3456", Marca = "Renault",  Modelo = "Kangoo",   CapacidadCarga = 800,  Activo = true },
            new Vehiculo { Matricula = "SAE 4567", Marca = "Ford",      Modelo = "Transit",  CapacidadCarga = 1400, Activo = true },
            new Vehiculo { Matricula = "SAF 5678", Marca = "Toyota",    Modelo = "Hilux",    CapacidadCarga = 1000, Activo = true },
            new Vehiculo { Matricula = "SAG 6789", Marca = "Mercedes",  Modelo = "Sprinter", CapacidadCarga = 1800, Activo = true },
            new Vehiculo { Matricula = "SAH 7890", Marca = "Volkswagen",Modelo = "Saveiro",  CapacidadCarga = 700,  Activo = true },
            new Vehiculo { Matricula = "SAI 8901", Marca = "Peugeot",   Modelo = "Partner",  CapacidadCarga = 750,  Activo = false },
            new Vehiculo { Matricula = "SAJ 9012", Marca = "Iveco",     Modelo = "Daily",    CapacidadCarga = 2200, Activo = true },
            new Vehiculo { Matricula = "SAK 0123", Marca = "Hyundai",   Modelo = "HRRR",      CapacidadCarga = 1500, Activo = true },
        };
                await context.Vehiculos.AddRangeAsync(vehiculos);
                await context.SaveChangesAsync();
            }
            else
            {
                vehiculos = await context.Vehiculos.OrderBy(v => v.IdVehiculo).ToListAsync();
            }

            // -----------------------------------------------------------------
            // STOCK (10)
            // -----------------------------------------------------------------
            List<Stock> stocks;
            if (!await context.Stocks.AnyAsync())
            {
                stocks = new List<Stock>
        {
            new Stock { IdProducto = productos[0].IdProducto, IdSeccion = secciones[0].IdSeccion, CantidadActual = 500,  Minimo = 100, Maximo = 1000, UltimaActualizacion = new DateTime(2026, 11, 6) },
            new Stock { IdProducto = productos[1].IdProducto, IdSeccion = secciones[1].IdSeccion, CantidadActual = 300,  Minimo = 50,  Maximo = 800,  UltimaActualizacion = new DateTime(2026, 11, 25) },
            new Stock { IdProducto = productos[2].IdProducto, IdSeccion = secciones[2].IdSeccion, CantidadActual = 20,   Minimo = 50,  Maximo = 500,  UltimaActualizacion = new DateTime(2026, 12, 10) },
            new Stock { IdProducto = productos[3].IdProducto, IdSeccion = secciones[3].IdSeccion, CantidadActual = 150,  Minimo = 30,  Maximo = 400,  UltimaActualizacion = new DateTime(2027, 1, 14) },
            new Stock { IdProducto = productos[4].IdProducto, IdSeccion = secciones[4].IdSeccion, CantidadActual = 80,   Minimo = 20,  Maximo = 300,  UltimaActualizacion = new DateTime(2027, 2, 18) },
            new Stock { IdProducto = productos[5].IdProducto, IdSeccion = secciones[5].IdSeccion, CantidadActual = 1900, Minimo = 100, Maximo = 1950, UltimaActualizacion = new DateTime(2027, 3, 26) },
            new Stock { IdProducto = productos[6].IdProducto, IdSeccion = secciones[6].IdSeccion, CantidadActual = 600,  Minimo = 200, Maximo = 1200, UltimaActualizacion = new DateTime(2027, 5, 2) },
            new Stock { IdProducto = productos[7].IdProducto, IdSeccion = secciones[7].IdSeccion, CantidadActual = 250,  Minimo = 50,  Maximo = 600,  UltimaActualizacion = new DateTime(2027, 6, 20) },
            new Stock { IdProducto = productos[8].IdProducto, IdSeccion = secciones[8].IdSeccion, CantidadActual = 10,   Minimo = 15,  Maximo = 200,  UltimaActualizacion = new DateTime(2027, 8, 9) },
            new Stock { IdProducto = productos[9].IdProducto, IdSeccion = secciones[9].IdSeccion, CantidadActual = 400,  Minimo = 100, Maximo = 900,  UltimaActualizacion = new DateTime(2027, 10, 22) },
        };
                await context.Stocks.AddRangeAsync(stocks);
                await context.SaveChangesAsync();
            }
            else
            {
                stocks = await context.Stocks.OrderBy(s => s.IdStock).ToListAsync();
            }

            // -----------------------------------------------------------------
            // ALERTAS (10)
            // -----------------------------------------------------------------
            if (!await context.Alertas.AnyAsync())
            {
                var alertas = new List<Alerta>
        {
            new Alerta { IdStock = stocks[2].IdStock, TipoAlerta = "MINIMO", FechaHora = new DateTime(2026, 12, 11), Estado = true },
            new Alerta { IdStock = stocks[8].IdStock, TipoAlerta = "MINIMO", FechaHora = new DateTime(2027, 8, 10), Estado = true },
            new Alerta { IdStock = stocks[5].IdStock, TipoAlerta = "MAXIMO", FechaHora = new DateTime(2027, 3, 27), Estado = true },
            new Alerta { IdStock = stocks[0].IdStock, TipoAlerta = "MAXIMO", FechaHora = new DateTime(2026, 11, 8),  Estado = false },
            new Alerta { IdStock = stocks[1].IdStock, TipoAlerta = "MINIMO", FechaHora = new DateTime(2026, 12, 1),  Estado = false },
            new Alerta { IdStock = stocks[3].IdStock, TipoAlerta = "MAXIMO", FechaHora = new DateTime(2027, 1, 16),  Estado = true },
            new Alerta { IdStock = stocks[4].IdStock, TipoAlerta = "MINIMO", FechaHora = new DateTime(2027, 2, 20),  Estado = true },
            new Alerta { IdStock = stocks[6].IdStock, TipoAlerta = "MAXIMO", FechaHora = new DateTime(2027, 5, 4),   Estado = false },
            new Alerta { IdStock = stocks[7].IdStock, TipoAlerta = "MINIMO", FechaHora = new DateTime(2027, 6, 22),  Estado = true },
            new Alerta { IdStock = stocks[9].IdStock, TipoAlerta = "MAXIMO", FechaHora = new DateTime(2027, 10, 24), Estado = true },
        };
                await context.Alertas.AddRangeAsync(alertas);
                await context.SaveChangesAsync();
            }
            // -----------------------------------------------------------------
            // INVENTARIOS (10)
            // -----------------------------------------------------------------
            List<Inventario> inventarios;
            if (!await context.Inventarios.AnyAsync())
            {
                inventarios = new List<Inventario>
        {
            new Inventario { IdSeccion = secciones[0].IdSeccion, FechaAlta = new DateTime(2026, 11, 15), Estado = "FINALIZADO", FechaCierre = new DateTime(2026, 11, 18) },
            new Inventario { IdSeccion = secciones[1].IdSeccion, FechaAlta = new DateTime(2026, 12, 5),  Estado = "FINALIZADO", FechaCierre = new DateTime(2026, 12, 9) },
            new Inventario { IdSeccion = secciones[2].IdSeccion, FechaAlta = new DateTime(2027, 1, 10),  Estado = "FINALIZADO", FechaCierre = new DateTime(2027, 1, 13) },
            new Inventario { IdSeccion = secciones[3].IdSeccion, FechaAlta = new DateTime(2027, 2, 12),  Estado = "ABIERTO",    FechaCierre = null },
            new Inventario { IdSeccion = secciones[4].IdSeccion, FechaAlta = new DateTime(2027, 3, 20),  Estado = "FINALIZADO", FechaCierre = new DateTime(2027, 3, 24) },
            new Inventario { IdSeccion = secciones[5].IdSeccion, FechaAlta = new DateTime(2027, 4, 25),  Estado = "ABIERTO",    FechaCierre = null },
            new Inventario { IdSeccion = secciones[6].IdSeccion, FechaAlta = new DateTime(2027, 5, 30),  Estado = "FINALIZADO", FechaCierre = new DateTime(2027, 6, 3) },
            new Inventario { IdSeccion = secciones[7].IdSeccion, FechaAlta = new DateTime(2027, 7, 6),   Estado = "FINALIZADO", FechaCierre = new DateTime(2027, 7, 10) },
            new Inventario { IdSeccion = secciones[8].IdSeccion, FechaAlta = new DateTime(2027, 8, 15),  Estado = "ABIERTO",    FechaCierre = null },
            new Inventario { IdSeccion = secciones[9].IdSeccion, FechaAlta = new DateTime(2027, 10, 1),  Estado = "FINALIZADO", FechaCierre = new DateTime(2027, 10, 5) },
        };
                await context.Inventarios.AddRangeAsync(inventarios);
                await context.SaveChangesAsync();
            }
            else
            {
                inventarios = await context.Inventarios.OrderBy(i => i.IdInventario).ToListAsync();
            }

            // -----------------------------------------------------------------
            // DETALLE INVENTARIO (10)
            // -----------------------------------------------------------------
            if (!await context.DetalleInventarios.AnyAsync())
            {
                var detallesInventario = new List<DetalleInventario>
        {
            new DetalleInventario { IdInventario = inventarios[0].IdInventario, IdProducto = productos[0].IdProducto, CantidadInventario = 498 },
            new DetalleInventario { IdInventario = inventarios[1].IdInventario, IdProducto = productos[1].IdProducto, CantidadInventario = 305 },
            new DetalleInventario { IdInventario = inventarios[2].IdInventario, IdProducto = productos[2].IdProducto, CantidadInventario = 18 },
            new DetalleInventario { IdInventario = inventarios[3].IdInventario, IdProducto = productos[3].IdProducto, CantidadInventario = 150 },
            new DetalleInventario { IdInventario = inventarios[4].IdInventario, IdProducto = productos[4].IdProducto, CantidadInventario = 79 },
            new DetalleInventario { IdInventario = inventarios[5].IdInventario, IdProducto = productos[5].IdProducto, CantidadInventario = 1895 },
            new DetalleInventario { IdInventario = inventarios[6].IdInventario, IdProducto = productos[6].IdProducto, CantidadInventario = 602 },
            new DetalleInventario { IdInventario = inventarios[7].IdInventario, IdProducto = productos[7].IdProducto, CantidadInventario = 248 },
            new DetalleInventario { IdInventario = inventarios[8].IdInventario, IdProducto = productos[8].IdProducto, CantidadInventario = 9 },
            new DetalleInventario { IdInventario = inventarios[9].IdInventario, IdProducto = productos[9].IdProducto, CantidadInventario = 401 },
        };
                await context.DetalleInventarios.AddRangeAsync(detallesInventario);
                await context.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // SOLICITUDES DE AJUSTE (10)
            // IdEmpleadoSolicitante / IdSupervisorResolutor -> AspNetUsers
            // -----------------------------------------------------------------
            if (!await context.SolicitudAjustes.AnyAsync())
            {
                var solicitudesAjuste = new List<SolicitudAjuste>
        {
            new SolicitudAjuste { IdProducto = productos[0].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "STOCK",      Motivo = "Diferencia detectada en conteo fisico", CantidadSolicitada = 20, Estado = "APROBADO",  FechaSolicitud = new DateTime(2026, 11, 9),  FechaResolucion = new DateTime(2026, 11, 11) },
            new SolicitudAjuste { IdProducto = productos[1].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "INVENTARIO", Motivo = "Producto danado en almacenamiento",     CantidadSolicitada = 5,  Estado = "APROBADO",  FechaSolicitud = new DateTime(2026, 12, 6),  FechaResolucion = new DateTime(2026, 12, 8) },
            new SolicitudAjuste { IdProducto = productos[2].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = null, Tipo = "STOCK",      Motivo = "Faltante en seccion",                    CantidadSolicitada = 15, Estado = "PENDIENTE", FechaSolicitud = new DateTime(2027, 1, 11),  FechaResolucion = null },
            new SolicitudAjuste { IdProducto = productos[3].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "STOCK",      Motivo = "Error de carga en sistema",              CantidadSolicitada = 10, Estado = "RECHAZADO", FechaSolicitud = new DateTime(2027, 2, 13),  FechaResolucion = new DateTime(2027, 2, 15) },
            new SolicitudAjuste { IdProducto = productos[4].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = null, Tipo = "INVENTARIO", Motivo = "Ajuste post inventario",                 CantidadSolicitada = 8,  Estado = "PENDIENTE", FechaSolicitud = new DateTime(2027, 3, 21),  FechaResolucion = null },
            new SolicitudAjuste { IdProducto = productos[5].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "STOCK",      Motivo = "Sobrante detectado",                     CantidadSolicitada = 50, Estado = "APROBADO",  FechaSolicitud = new DateTime(2027, 4, 26),  FechaResolucion = new DateTime(2027, 4, 28) },
            new SolicitudAjuste { IdProducto = productos[6].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "STOCK",      Motivo = "Producto vencido",                       CantidadSolicitada = 12, Estado = "APROBADO",  FechaSolicitud = new DateTime(2027, 5, 31),  FechaResolucion = new DateTime(2027, 6, 2) },
            new SolicitudAjuste { IdProducto = productos[7].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = null, Tipo = "INVENTARIO", Motivo = "Diferencia en bobina de cable",          CantidadSolicitada = 25, Estado = "PENDIENTE", FechaSolicitud = new DateTime(2027, 7, 7),   FechaResolucion = null },
            new SolicitudAjuste { IdProducto = productos[8].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "STOCK",      Motivo = "Retiro por falla de fabrica",            CantidadSolicitada = 3,  Estado = "RECHAZADO", FechaSolicitud = new DateTime(2027, 8, 16),  FechaResolucion = new DateTime(2027, 8, 18) },
            new SolicitudAjuste { IdProducto = productos[9].IdProducto, IdEmpleadoSolicitante = operador.Id /* --FK A ASPNETUSERS-- */, IdSupervisorResolutor = supervisor.Id /* --FK A ASPNETUSERS-- */, Tipo = "STOCK",      Motivo = "Ajuste por recuento anual",              CantidadSolicitada = 30, Estado = "APROBADO",  FechaSolicitud = new DateTime(2027, 10, 2),  FechaResolucion = new DateTime(2027, 10, 4) },
        };
                await context.SolicitudAjustes.AddRangeAsync(solicitudesAjuste);
                await context.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // TRANSFERENCIAS (10)
            // -----------------------------------------------------------------
            List<Transferencia> transferencias;
            if (!await context.Transferencias.AnyAsync())
            {
                transferencias = new List<Transferencia>
        {
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[0].IdVehiculo, IdAlmacenOrigen = almacenes[0].IdAlmacen, IdAlmacenDestino = almacenes[1].IdAlmacen, Estado = "FINALIZADA",  FechaSolicitud = new DateTime(2026, 11, 4),  FechaAutorizacion = new DateTime(2026, 11, 5),  FechaEjecucion = new DateTime(2026, 11, 6),  FechaEntrega = new DateTime(2026, 11, 7) },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[1].IdVehiculo, IdAlmacenOrigen = almacenes[1].IdAlmacen, IdAlmacenDestino = almacenes[2].IdAlmacen, Estado = "ENTREGADA",   FechaSolicitud = new DateTime(2026, 12, 2),  FechaAutorizacion = new DateTime(2026, 12, 3),  FechaEjecucion = new DateTime(2026, 12, 4),  FechaEntrega = new DateTime(2026, 12, 5) },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[2].IdVehiculo, IdAlmacenOrigen = almacenes[2].IdAlmacen, IdAlmacenDestino = almacenes[3].IdAlmacen, Estado = "EN_TRANSITO", FechaSolicitud = new DateTime(2027, 1, 6),   FechaAutorizacion = new DateTime(2027, 1, 7),   FechaEjecucion = new DateTime(2027, 1, 8),   FechaEntrega = null },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = null, IdVehiculo = null, IdAlmacenOrigen = almacenes[3].IdAlmacen, IdAlmacenDestino = almacenes[4].IdAlmacen, Estado = "ASIGNADA",    FechaSolicitud = new DateTime(2027, 2, 9),   FechaAutorizacion = new DateTime(2027, 2, 10),  FechaEjecucion = null,                        FechaEntrega = null },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = null, IdTransportista = null, IdVehiculo = null, IdAlmacenOrigen = almacenes[4].IdAlmacen, IdAlmacenDestino = almacenes[5].IdAlmacen, Estado = "PENDIENTE",   FechaSolicitud = new DateTime(2027, 3, 15),  FechaAutorizacion = null,                        FechaEjecucion = null,                        FechaEntrega = null },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[5].IdVehiculo, IdAlmacenOrigen = almacenes[5].IdAlmacen, IdAlmacenDestino = almacenes[6].IdAlmacen, Estado = "FINALIZADA",  FechaSolicitud = new DateTime(2027, 4, 21),  FechaAutorizacion = new DateTime(2027, 4, 22),  FechaEjecucion = new DateTime(2027, 4, 23),  FechaEntrega = new DateTime(2027, 4, 25) },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[6].IdVehiculo, IdAlmacenOrigen = almacenes[6].IdAlmacen, IdAlmacenDestino = almacenes[7].IdAlmacen, Estado = "RECHAZADA",   FechaSolicitud = new DateTime(2027, 5, 25),  FechaAutorizacion = new DateTime(2027, 5, 26),  FechaEjecucion = null,                        FechaEntrega = null },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[7].IdVehiculo, IdAlmacenOrigen = almacenes[7].IdAlmacen, IdAlmacenDestino = almacenes[8].IdAlmacen, Estado = "ENTREGADA",   FechaSolicitud = new DateTime(2027, 7, 2),   FechaAutorizacion = new DateTime(2027, 7, 3),   FechaEjecucion = new DateTime(2027, 7, 4),   FechaEntrega = new DateTime(2027, 7, 6) },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[8].IdVehiculo, IdAlmacenOrigen = almacenes[8].IdAlmacen, IdAlmacenDestino = almacenes[9].IdAlmacen, Estado = "EN_TRANSITO", FechaSolicitud = new DateTime(2027, 9, 10),  FechaAutorizacion = new DateTime(2027, 9, 11),  FechaEjecucion = new DateTime(2027, 9, 12),  FechaEntrega = null },
            new Transferencia { IdSupervisorSolicitante = supervisor.Id /* --FK A ASPNETUSERS-- */, IdAdministradorAutorizador = admin.Id /* --FK A ASPNETUSERS-- */, IdTransportista = transportista.Id /* --FK A ASPNETUSERS-- */, IdVehiculo = vehiculos[9].IdVehiculo, IdAlmacenOrigen = almacenes[9].IdAlmacen, IdAlmacenDestino = almacenes[0].IdAlmacen, Estado = "FINALIZADA",  FechaSolicitud = new DateTime(2027, 10, 20), FechaAutorizacion = new DateTime(2027, 10, 21), FechaEjecucion = new DateTime(2027, 10, 22), FechaEntrega = new DateTime(2027, 10, 24) },
        };
                await context.Transferencias.AddRangeAsync(transferencias);
                await context.SaveChangesAsync();
            }
            else
            {
                transferencias = await context.Transferencias.OrderBy(t => t.IdTransferencia).ToListAsync();
            }

            // -----------------------------------------------------------------
            // DETALLE TRANSFERENCIA (10)
            // -----------------------------------------------------------------
            if (!await context.DetalleTransferencias.AnyAsync())
            {
                var detallesTransferencia = new List<DetalleTransferencia>
        {
            new DetalleTransferencia { IdTransferencia = transferencias[0].IdTransferencia, IdProducto = productos[0].IdProducto, Cantidad = 50 },
            new DetalleTransferencia { IdTransferencia = transferencias[1].IdTransferencia, IdProducto = productos[1].IdProducto, Cantidad = 30 },
            new DetalleTransferencia { IdTransferencia = transferencias[2].IdTransferencia, IdProducto = productos[2].IdProducto, Cantidad = 10 },
            new DetalleTransferencia { IdTransferencia = transferencias[3].IdTransferencia, IdProducto = productos[3].IdProducto, Cantidad = 25 },
            new DetalleTransferencia { IdTransferencia = transferencias[4].IdTransferencia, IdProducto = productos[4].IdProducto, Cantidad = 15 },
            new DetalleTransferencia { IdTransferencia = transferencias[5].IdTransferencia, IdProducto = productos[5].IdProducto, Cantidad = 100 },
            new DetalleTransferencia { IdTransferencia = transferencias[6].IdTransferencia, IdProducto = productos[6].IdProducto, Cantidad = 40 },
            new DetalleTransferencia { IdTransferencia = transferencias[7].IdTransferencia, IdProducto = productos[7].IdProducto, Cantidad = 60 },
            new DetalleTransferencia { IdTransferencia = transferencias[8].IdTransferencia, IdProducto = productos[8].IdProducto, Cantidad = 5 },
            new DetalleTransferencia { IdTransferencia = transferencias[9].IdTransferencia, IdProducto = productos[9].IdProducto, Cantidad = 35 },
        };
                await context.DetalleTransferencias.AddRangeAsync(detallesTransferencia);
                await context.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // UBICACION GPS (10)
            // -----------------------------------------------------------------
            if (!await context.UbicacionGps.AnyAsync())
            {
                var ubicacionesGps = new List<UbicacionGps>
        {
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.90111m, Longitud = -56.16452m, FechaHora = new DateTime(2026, 11, 6, 9, 15, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.85700m, Longitud = -56.02170m, FechaHora = new DateTime(2026, 12, 4, 10, 30, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.72700m, Longitud = -56.23500m, FechaHora = new DateTime(2027, 1, 8, 11, 0, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.83500m, Longitud = -55.98000m, FechaHora = new DateTime(2027, 2, 5, 8, 45, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.90500m, Longitud = -56.19000m, FechaHora = new DateTime(2027, 3, 12, 14, 20, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.78000m, Longitud = -56.05000m, FechaHora = new DateTime(2027, 4, 23, 16, 10, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.86500m, Longitud = -56.16000m, FechaHora = new DateTime(2027, 5, 30, 7, 55, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.91000m, Longitud = -56.21500m, FechaHora = new DateTime(2027, 7, 4, 12, 40, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.82000m, Longitud = -55.99500m, FechaHora = new DateTime(2027, 9, 12, 9, 5, 0) },
            new UbicacionGps { IdTransportista = transportista.Id, Latitud = -34.87500m, Longitud = -56.17800m, FechaHora = new DateTime(2027, 10, 22, 15, 25, 0) },
        };
                await context.UbicacionGps.AddRangeAsync(ubicacionesGps);
                await context.SaveChangesAsync();
            }

            // -----------------------------------------------------------------
            // MOVIMIENTOS (10)
            // -----------------------------------------------------------------
            if (!await context.Movimientos.AnyAsync())
            {
                var movimientos = new List<Movimiento>
    {
        new Movimiento { IdUsuario = operador.Id, IdProducto = productos[0].IdProducto, IdSeccion = secciones[0].IdSeccion, TipoMovimiento = "INGRESO",       CantidadUnidades = 500, FechaHora = new DateTime(2026, 11, 3, 8, 0, 0),  Estado = "PROCESADO", ValorAnterior = 0,    ValorPosterior = 500,  Resultado = "EXITOSO" },
        new Movimiento { IdUsuario = operador.Id, IdProducto = productos[1].IdProducto, IdSeccion = secciones[1].IdSeccion, TipoMovimiento = "INGRESO",       CantidadUnidades = 300, FechaHora = new DateTime(2026, 11, 10, 9, 0, 0), Estado = "PROCESADO", ValorAnterior = 0,    ValorPosterior = 300,  Resultado = "EXITOSO" },
        new Movimiento { IdUsuario = operador.Id, IdProducto = productos[2].IdProducto, IdSeccion = secciones[2].IdSeccion, TipoMovimiento = "EGRESO",        CantidadUnidades = 30,  FechaHora = new DateTime(2026, 12, 10, 10, 0, 0),Estado = "PROCESADO", ValorAnterior = 50,   ValorPosterior = 20,   Resultado = "EXITOSO" },
        new Movimiento { IdUsuario = transportista.Id, IdProducto = productos[3].IdProducto, IdSeccion = secciones[3].IdSeccion, TipoMovimiento = "TRANSFERENCIA", CantidadUnidades = 25,  FechaHora = new DateTime(2027, 1, 8, 11, 0, 0),  Estado = "PROCESADO", ValorAnterior = 175,  ValorPosterior = 150,  Resultado = "EXITOSO" }, // Corregido con 'S'
        new Movimiento { IdUsuario = supervisor.Id, IdProducto = productos[4].IdProducto, IdSeccion = secciones[4].IdSeccion, TipoMovimiento = "AJUSTE_STOCK",  CantidadUnidades = 8,   FechaHora = new DateTime(2027, 2, 15, 12, 0, 0), Estado = "PROCESADO", ValorAnterior = 72,   ValorPosterior = 80,   Resultado = "EXITOSO" },
        new Movimiento { IdUsuario = operador.Id, IdProducto = productos[5].IdProducto, IdSeccion = secciones[5].IdSeccion, TipoMovimiento = "INGRESO",       CantidadUnidades = 1900,FechaHora = new DateTime(2027, 3, 26, 13, 0, 0),Estado = "PROCESADO", ValorAnterior = 0,    ValorPosterior = 1900, Resultado = "EXITOSO" },
        new Movimiento { IdUsuario = operador.Id, IdProducto = productos[6].IdProducto, IdSeccion = secciones[6].IdSeccion, TipoMovimiento = "EGRESO",        CantidadUnidades = 40,  FechaHora = new DateTime(2027, 5, 4, 14, 0, 0),  Estado = "FALLIDO",   ValorAnterior = 640,  ValorPosterior = 640,  Resultado = "FALLIDO" },
        new Movimiento { IdUsuario = transportista.Id, IdProducto = productos[7].IdProducto, IdSeccion = secciones[7].IdSeccion, TipoMovimiento = "TRANSFERENCIA", CantidadUnidades = 60,  FechaHora = new DateTime(2027, 7, 4, 15, 0, 0),  Estado = "PROCESADO", ValorAnterior = 310,  ValorPosterior = 250,  Resultado = "EXITOSO" }, // Corregido con 'S'
        new Movimiento { IdUsuario = supervisor.Id, IdProducto = productos[8].IdProducto, IdSeccion = secciones[8].IdSeccion, TipoMovimiento = "AJUSTE_STOCK",  CantidadUnidades = 3,   FechaHora = new DateTime(2027, 8, 18, 16, 0, 0), Estado = "PROCESADO", ValorAnterior = 13,   ValorPosterior = 10,   Resultado = "EXITOSO" },
        new Movimiento { IdUsuario = operador.Id, IdProducto = productos[9].IdProducto, IdSeccion = secciones[9].IdSeccion, TipoMovimiento = "INGRESO",       CantidadUnidades = 400, FechaHora = new DateTime(2027, 10, 22, 17, 0, 0),Estado = "PROCESADO", ValorAnterior = 0,    ValorPosterior = 400,  Resultado = "EXITOSO" },
    };
                await context.Movimientos.AddRangeAsync(movimientos);
                await context.SaveChangesAsync();
            }
        }
    }        
}
