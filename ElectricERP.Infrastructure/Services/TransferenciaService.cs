using ElectricERP.Application.DTOs.Transferencias;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ElectricERP.Infrastructure.Services
{
    public class TransferenciaService : ITransferenciaService
    {
        private readonly ElectricERPDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransferenciaService(ElectricERPDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<TransferenciaDto>> ObtenerTodosAsync()
        {
            var lista = await BaseQuery().OrderByDescending(t => t.FechaSolicitud).ToListAsync();
            return lista.Select(MapToDto).ToList();
        }

        public async Task<TransferenciaDto?> ObtenerPorIdAsync(int id)
        {
            var t = await BaseQuery().FirstOrDefaultAsync(t => t.IdTransferencia == id);
            return t == null ? null : MapToDto(t);
        }

        public async Task<(bool Exito, string? Mensaje, TransferenciaDto? Creada)> SolicitarAsync(CrearTransferenciaDto dto, string idSupervisor)
        {
            if (dto.IdAlmacenOrigen == dto.IdAlmacenDestino) return (false, "El almacén de origen y destino no pueden ser el mismo.", null);
            if (dto.Productos == null || !dto.Productos.Any()) return (false, "Tenés que agregar al menos un producto a la transferencia.", null);
            if (dto.Productos.Any(p => p.Cantidad <= 0)) return (false, "Todas las cantidades deben ser mayores a 0.", null);

            var transferencia = new Transferencia
            {
                IdSupervisorSolicitante = idSupervisor,
                IdAlmacenOrigen = dto.IdAlmacenOrigen,
                IdAlmacenDestino = dto.IdAlmacenDestino,
                Estado = "PENDIENTE",
                FechaSolicitud = DateTime.Now
            };

            foreach (var item in dto.Productos)
            {
                transferencia.DetalleTransferencia.Add(new DetalleTransferencia
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                });
            }

            try
            {
                _context.Set<Transferencia>().Add(transferencia);
                await _context.SaveChangesAsync();
                var creada = await BaseQuery().FirstAsync(t => t.IdTransferencia == transferencia.IdTransferencia);
                return (true, null, MapToDto(creada));
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex), null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> AutorizarAsync(int id, AutorizarTransferenciaDto dto, string idAdmin)
        {
            var t = await _context.Set<Transferencia>().FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (t == null) return (false, "No encontrado");
            if (t.Estado != "PENDIENTE") return (false, $"No se puede autorizar: la transferencia está en estado {t.Estado}.");
            t.Estado = dto.Aprobado ? "AUTORIZADA" : "RECHAZADA";
            t.IdAdministradorAutorizador = idAdmin;
            t.FechaAutorizacion = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex));
            }
        }

        public async Task<(bool Exito, string? Mensaje)> AsignarAsync(
    int id,
    AsignarTransferenciaDto dto)
        {
            var t = await _context.Set<Transferencia>()
                .FirstOrDefaultAsync(t => t.IdTransferencia == id);

            if (t == null)
                return (false, "No encontrado");

            if (t.Estado != "AUTORIZADA")
                return (false,
                    $"No se puede asignar: la transferencia está en estado {t.Estado}, tiene que estar AUTORIZADA primero.");

            var vehiculo = await _context.Set<Vehiculo>()
                .FirstOrDefaultAsync(v => v.IdVehiculo == dto.IdVehiculo);

            if (vehiculo == null)
                return (false, "El vehículo seleccionado no existe.");

            if (!vehiculo.Activo)
                return (false, "El vehículo seleccionado no está activo.");

            var transportista = await _userManager.FindByIdAsync(dto.IdTransportista);

            if (transportista == null)
                return (false, "El transportista seleccionado no existe.");

            if (!transportista.Activo)
                return (false, "El transportista seleccionado no está activo.");

            var esTransportista = await _userManager.IsInRoleAsync(
                transportista,
                "Transportista");

            if (!esTransportista)
                return (false,
                    "El usuario seleccionado no tiene rol Transportista.");

            t.IdVehiculo = dto.IdVehiculo;
            t.IdTransportista = dto.IdTransportista;
            t.Estado = "ASIGNADA";

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex));
            }
        }

        private IQueryable<Transferencia> BaseQuery()
        {
            return _context.Set<Transferencia>().Include(t => t.IdAlmacenOrigenNavigation).Include(t => t.IdAlmacenDestinoNavigation).Include(t => t.IdSupervisorSolicitanteNavigation)
                .Include(t => t.IdAdministradorAutorizadorNavigation).Include(t => t.IdTransportistaNavigation).Include(t => t.IdVehiculoNavigation)
                .Include(t => t.DetalleTransferencia).ThenInclude(d => d.IdProductoNavigation);
        }

        public async Task<(bool Exito, string? Mensaje, UbicacionDto? Ubicacion)> IniciarAsync(int id, string idTransportista, RegistrarUbicacionDto ubicacionInicial)
        {
            var t = await _context.Set<Transferencia>().FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (t == null) return (false, "No encontrado", null);
            if (t.Estado != "ASIGNADA") return (false, $"No se puede iniciar: la transferencia está en estado {t.Estado}, tiene que estar ASIGNADA primero.", null);
            if (t.IdTransportista != idTransportista) return (false, "Esta transferencia no está asignada a tu usuario.", null);
            t.Estado = "EN_TRANSITO";
            t.FechaEjecucion = DateTime.Now;
            var ubicacion = new UbicacionGps
            {
                IdTransportista = idTransportista,
                Latitud = ubicacionInicial.Latitud,
                Longitud = ubicacionInicial.Longitud,
                FechaHora = DateTime.Now
            };

            try
            {
                _context.Set<UbicacionGps>().Add(ubicacion);
                await _context.SaveChangesAsync();
                return (true, null, new UbicacionDto
                {
                    Latitud = ubicacion.Latitud,
                    Longitud = ubicacion.Longitud,
                    FechaHora = ubicacion.FechaHora
                });
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex), null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> EntregarAsync(int id, string idTransportista)
        {
            var t = await _context.Set<Transferencia>().FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (t == null) return (false, "No encontrado");
            if (t.Estado != "EN_TRANSITO") return (false, $"No se puede confirmar la entrega: la transferencia está en estado {t.Estado}, tiene que estar EN_TRANSITO.");
            if (t.IdTransportista != idTransportista) return (false, "Esta transferencia no está asignada a tu usuario.");
            t.Estado = "ENTREGADA";
            t.FechaEntrega = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex));
            }
        }

        public async Task<(bool Exito, string? Mensaje)> RecibirAsync(int id)
        {
            var t = await _context.Set<Transferencia>().FirstOrDefaultAsync(t => t.IdTransferencia == id);
            if (t == null) return (false, "No encontrado");
            if (t.Estado != "ENTREGADA") return (false, $"No se puede confirmar la recepción: la transferencia está en estado {t.Estado}, tiene que estar ENTREGADA.");
            t.Estado = "FINALIZADA";

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex));
            }
        }

        private static string TraducirErrorSql(DbUpdateException ex)
        {
            var mensaje = ex.InnerException?.Message ?? ex.Message;
            if (mensaje.Contains("CK_Transferencia_Almacenes")) return "El almacén de origen y destino no pueden ser el mismo.";
            if (mensaje.Contains("CK_Transferencia_FechaAutorizacion")) return "La fecha de autorización no puede ser anterior a la de solicitud.";
            if (mensaje.Contains("FK_Transferencia_Vehiculo")) return "El vehículo seleccionado no es válido.";
            if (mensaje.Contains("FK_Transferencia_Transportista")) return "El transportista seleccionado no es válido.";
            return $"No se pudo guardar la transferencia. Detalle: {mensaje}";
        }

        private static TransferenciaDto MapToDto(Transferencia t) => new()
        {
            IdTransferencia = t.IdTransferencia,
            IdAlmacenOrigen = t.IdAlmacenOrigen,
            NombreAlmacenOrigen = t.IdAlmacenOrigenNavigation?.Nombre ?? string.Empty,
            IdAlmacenDestino = t.IdAlmacenDestino,
            NombreAlmacenDestino = t.IdAlmacenDestinoNavigation?.Nombre ?? string.Empty,
            Estado = t.Estado,
            NombreSupervisorSolicitante = t.IdSupervisorSolicitanteNavigation?.UserName ?? string.Empty,
            NombreAdministradorAutorizador = t.IdAdministradorAutorizadorNavigation?.UserName,
            IdTransportista = t.IdTransportista,
            NombreTransportista = t.IdTransportistaNavigation?.UserName,
            Matricula = t.IdVehiculoNavigation?.Matricula,
            FechaSolicitud = t.FechaSolicitud,
            FechaAutorizacion = t.FechaAutorizacion,
            FechaEjecucion = t.FechaEjecucion,
            FechaEntrega = t.FechaEntrega,
            Detalles = t.DetalleTransferencia.Select(d => new DetalleTransferenciaItemDto
            {
                IdProducto = d.IdProducto,
                NombreProducto = d.IdProductoNavigation?.Nombre ?? string.Empty,
                CodigoSku = d.IdProductoNavigation?.CodigoSku ?? string.Empty,
                Cantidad = d.Cantidad
            }).ToList()
        };
    }
}