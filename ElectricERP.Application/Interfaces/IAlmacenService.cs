using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ElectricERP.Application.DTOs.Almacenes;

namespace ElectricERP.Application.Interfaces
{
    public interface IAlmacenService
    {
        Task<List<AlmacenDto>> ObtenerTodosAsync();
        Task<AlmacenDto?> ObtenerPorIdAsync(int id);
        Task<AlmacenDto> CrearAsync(CrearAlmacenDto dto);
        Task<bool> ActualizarAsync(int id, CrearAlmacenDto dto);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
    }
}