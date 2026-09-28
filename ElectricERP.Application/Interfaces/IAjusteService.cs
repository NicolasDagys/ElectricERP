using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Ajustes;

namespace ElectricERP.Application.Interfaces
{
    public interface IAjusteService
    {
        Task<List<SolicitudAjusteDto>> ObtenerTodosAsync();
        Task<SolicitudAjusteDto?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string? Mensaje, SolicitudAjusteDto? Creada)> CrearAsync(CrearSolicitudAjusteDto dto, string idEmpleado);
        Task<(bool Exito, string? Mensaje)> ResolverAsync(int id, ResolverSolicitudAjusteDto dto, string idSupervisor);
    }
}