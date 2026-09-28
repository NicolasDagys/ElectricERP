using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Transferencias;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class GpsService : IGpsService
    {
        private readonly ElectricERPDbContext _context;

        public GpsService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Exito, string? Mensaje)> RegistrarUbicacionAsync(string idTransportista, RegistrarUbicacionDto dto)
        {
            if (dto.Latitud < -90 || dto.Latitud > 90)
                return (false, "La latitud tiene que estar entre -90 y 90.");
            if (dto.Longitud < -180 || dto.Longitud > 180)
                return (false, "La longitud tiene que estar entre -180 y 180.");

            var ubicacion = new UbicacionGps
            {
                IdTransportista = idTransportista,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
                FechaHora = DateTime.Now
            };

            try
            {
                _context.Set<UbicacionGps>().Add(ubicacion);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo registrar la ubicación.");
            }
        }

        public async Task<List<UbicacionDto>> ObtenerRecorridoAsync(int idTransferencia)
        {
            var transferencia = await _context.Set<Transferencia>()
                .FirstOrDefaultAsync(t => t.IdTransferencia == idTransferencia);

            if (transferencia == null || string.IsNullOrEmpty(transferencia.IdTransportista))
                return new();

            var desde = transferencia.FechaEjecucion ?? transferencia.FechaSolicitud;
            var hasta = transferencia.FechaEntrega ?? DateTime.Now;

            return await _context.Set<UbicacionGps>()
                .Where(u => u.IdTransportista == transferencia.IdTransportista
                            && u.FechaHora >= desde
                            && u.FechaHora <= hasta)
                .OrderBy(u => u.FechaHora)
                .Select(u => new UbicacionDto
                {
                    Latitud = u.Latitud,
                    Longitud = u.Longitud,
                    FechaHora = u.FechaHora
                })
                .ToListAsync();
        }

        public async Task<UbicacionDto?> ObtenerUbicacionActualAsync(string idTransportista)
        {
            var ultima = await _context.Set<UbicacionGps>()
                .Where(u => u.IdTransportista == idTransportista)
                .OrderByDescending(u => u.FechaHora)
                .FirstOrDefaultAsync();

            if (ultima == null) return null;

            return new UbicacionDto
            {
                Latitud = ultima.Latitud,
                Longitud = ultima.Longitud,
                FechaHora = ultima.FechaHora
            };
        }
    }
}