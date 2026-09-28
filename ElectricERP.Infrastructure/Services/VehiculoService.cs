using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Vehiculos;
using ElectricERP.Application.Interfaces;
using ElectricERP.Infrastructure.Data;
using ElectricERP.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElectricERP.Infrastructure.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly ElectricERPDbContext _context;

        public VehiculoService(ElectricERPDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehiculoDto>> ObtenerTodosAsync()
        {
            return await _context.Set<Vehiculo>()
                .Select(v => MapToDto(v))
                .ToListAsync();
        }

        public async Task<VehiculoDto?> ObtenerPorIdAsync(int id)
        {
            var vehiculo = await _context.Set<Vehiculo>().FirstOrDefaultAsync(v => v.IdVehiculo == id);
            return vehiculo == null ? null : MapToDto(vehiculo);
        }

        public async Task<(bool Exito, string? Mensaje, VehiculoDto? Creado)> CrearAsync(CrearVehiculoDto dto)
        {
            var vehiculo = new Vehiculo
            {
                Matricula = dto.Matricula,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                CapacidadCarga = dto.CapacidadCarga,
                Activo = dto.Activo
            };

            try
            {
                _context.Set<Vehiculo>().Add(vehiculo);
                await _context.SaveChangesAsync();
                return (true, null, MapToDto(vehiculo));
            }
            catch (DbUpdateException ex)
            {
                return (false, TraducirErrorSql(ex), null);
            }
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, CrearVehiculoDto dto)
        {
            var vehiculo = await _context.Set<Vehiculo>().FirstOrDefaultAsync(v => v.IdVehiculo == id);
            if (vehiculo == null) return (false, "No encontrado");

            vehiculo.Matricula = dto.Matricula;
            vehiculo.Marca = dto.Marca;
            vehiculo.Modelo = dto.Modelo;
            vehiculo.CapacidadCarga = dto.CapacidadCarga;
            vehiculo.Activo = dto.Activo;

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

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var vehiculo = await _context.Set<Vehiculo>().FirstOrDefaultAsync(v => v.IdVehiculo == id);
            if (vehiculo == null) return (false, "No encontrado");

            try
            {
                _context.Set<Vehiculo>().Remove(vehiculo);
                await _context.SaveChangesAsync();
                return (true, "Eliminado");
            }
            catch (DbUpdateException)
            {
                return (false, "No se puede eliminar: el vehículo tiene transferencias asociadas.");
            }
        }

        private static string TraducirErrorSql(DbUpdateException ex)
        {
            var mensaje = ex.InnerException?.Message ?? ex.Message;

            if (mensaje.Contains("UQ_Vehiculo_Matricula"))
                return "Ya existe un vehículo con esa matrícula.";
            if (mensaje.Contains("CK_Vehiculo_Matricula"))
                return "La matrícula no tiene el formato correcto (ej: ABC 1234).";
            if (mensaje.Contains("CK_Vehiculo_Marca") || mensaje.Contains("CK_Vehiculo_Modelo"))
                return "La marca y el modelo deben tener más de 3 caracteres.";
            if (mensaje.Contains("CK_Vehiculo_Carga"))
                return "La capacidad de carga debe ser mayor a 0.";

            return $"No se pudo guardar el vehículo. Detalle: {mensaje}";
        }

        private static VehiculoDto MapToDto(Vehiculo v) => new()
        {
            IdVehiculo = v.IdVehiculo,
            Matricula = v.Matricula,
            Marca = v.Marca,
            Modelo = v.Modelo,
            CapacidadCarga = v.CapacidadCarga,
            Activo = v.Activo
        };
    }
}