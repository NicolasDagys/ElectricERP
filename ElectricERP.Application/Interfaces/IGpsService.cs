using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Transferencias;

namespace ElectricERP.Application.Interfaces
{
    public interface IGpsService
    {
        Task<(bool Exito, string? Mensaje)> RegistrarUbicacionAsync(string idTransportista, RegistrarUbicacionDto dto);
        Task<List<UbicacionDto>> ObtenerRecorridoAsync(int idTransferencia);
        Task<UbicacionDto?> ObtenerUbicacionActualAsync(string idTransportista);
    }
}