using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Secciones;

namespace ElectricERP.Application.Interfaces
{
    public interface ISeccionService
    {
        Task<List<SeccionDto>> ObtenerTodosAsync();
        Task<SeccionDto?> ObtenerPorIdAsync(int id);
        Task<SeccionDto> CrearAsync(CrearSeccionDto dto);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
    }
}