using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Inventarios;

namespace ElectricERP.Application.Interfaces
{
    public interface IInventarioService
    {
        Task<List<InventarioDto>> ObtenerTodosAsync();
        Task<InventarioDto?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string? Mensaje, InventarioDto? Creado)> CrearAsync(CrearInventarioDto dto);
        Task<(bool Exito, string? Mensaje)> ActualizarDetalleAsync(int id, ActualizarInventarioDto dto);
        Task<(bool Exito, string? Mensaje)> CerrarAsync(int id);
    }
}