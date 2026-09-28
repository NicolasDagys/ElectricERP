using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Ajustes;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class AjusteService : IAjusteService
    {
        private readonly ElectricERPDbContext _context;

        public AjusteService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<SolicitudAjusteDto>> ObtenerTodosAsync()
        {
            return await BaseQuery()
                .OrderByDescending(s => s.FechaSolicitud)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<SolicitudAjusteDto?> ObtenerPorIdAsync(int id)
        {
            var s = await BaseQuery().FirstOrDefaultAsync(s => s.IdSolicitud == id);
            return s == null ? null : MapToDto(s);
        }

        public async Task<(bool Exito, string? Mensaje, SolicitudAjusteDto? Creada)> CrearAsync(CrearSolicitudAjusteDto dto, string idEmpleado)
        {
            if (dto.Tipo != "STOCK" && dto.Tipo != "INVENTARIO")
                return (false, "El tipo tiene que ser STOCK o INVENTARIO.", null);

            if (dto.CantidadSolicitada < 0)
                return (false, "La cantidad solicitada no puede ser negativa.", null);

            if (string.IsNullOrWhiteSpace(dto.Motivo))
                return (false, "Tenés que indicar un motivo.", null);

            var solicitud = new SolicitudAjuste
            {
                IdProducto = dto.IdProducto,
                IdEmpleadoSolicitante = idEmpleado,
                Tipo = dto.Tipo,
                Motivo = dto.Motivo,
                CantidadSolicitada = dto.CantidadSolicitada,
                Estado = "PENDIENTE",
                FechaSolicitud = DateTime.Now
            };

            try
            {
                _context.Set<SolicitudAjuste>().Add(solicitud);
                await _context.SaveChangesAsync();

                var creada = await BaseQuery().FirstAsync(s => s.IdSolicitud == solicitud.IdSolicitud);
                return (true, null, MapToDto(creada));
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex), null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> ResolverAsync(int id, ResolverSolicitudAjusteDto dto, string idSupervisor)
        {
            var solicitud = await _context.Set<SolicitudAjuste>().FirstOrDefaultAsync(s => s.IdSolicitud == id);
            if (solicitud == null) return (false, "No encontrado");

            if (solicitud.Estado != "PENDIENTE")
                return (false, $"Esta solicitud ya fue {(solicitud.Estado == "APROBADO" ? "aprobada" : "rechazada")}.");

            solicitud.Estado = dto.Aprobado ? "APROBADO" : "RECHAZADO";
            solicitud.IdSupervisorResolutor = idSupervisor;
            solicitud.FechaResolucion = DateTime.Now;

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

        private IQueryable<SolicitudAjuste> BaseQuery()
        {
            return _context.Set<SolicitudAjuste>()
                .Include(s => s.IdProductoNavigation)
                .Include(s => s.IdEmpleadoSolicitanteNavigation)
                .Include(s => s.IdSupervisorResolutorNavigation);
        }

        private static string TraducirErrorSql(DbUpdateException ex)
        {
            var mensaje = ex.InnerException?.Message ?? ex.Message;

            if (mensaje.Contains("CK_Ajuste_CantidadSolicitada"))
                return "La cantidad solicitada no puede ser negativa.";
            if (mensaje.Contains("CK_Ajuste_FechaResolucion"))
                return "La fecha de resolución no puede ser anterior a la solicitud.";
            if (mensaje.Contains("CK_Ajuste_Tipo"))
                return "El tipo tiene que ser STOCK o INVENTARIO.";

            return $"No se pudo guardar la solicitud. Detalle: {mensaje}";
        }

        private static SolicitudAjusteDto MapToDto(SolicitudAjuste s) => new()
        {
            IdSolicitud = s.IdSolicitud,
            IdProducto = s.IdProducto,
            NombreProducto = s.IdProductoNavigation?.Nombre ?? string.Empty,
            CodigoSku = s.IdProductoNavigation?.CodigoSku ?? string.Empty,
            NombreEmpleadoSolicitante = s.IdEmpleadoSolicitanteNavigation?.UserName ?? string.Empty,
            NombreSupervisorResolutor = s.IdSupervisorResolutorNavigation?.UserName,
            Tipo = s.Tipo,
            Motivo = s.Motivo,
            CantidadSolicitada = s.CantidadSolicitada,
            Estado = s.Estado,
            FechaSolicitud = s.FechaSolicitud,
            FechaResolucion = s.FechaResolucion
        };
    }
}