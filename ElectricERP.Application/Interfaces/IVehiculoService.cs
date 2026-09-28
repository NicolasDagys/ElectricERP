using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Vehiculos;

namespace ElectricERP.Application.Interfaces
{
    public interface IVehiculoService
    {
        Task<List<VehiculoDto>> ObtenerTodosAsync();
        Task<VehiculoDto?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string? Mensaje, VehiculoDto? Creado)> CrearAsync(CrearVehiculoDto dto);
        Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, CrearVehiculoDto dto);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
    }
}