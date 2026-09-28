using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Transferencias;

namespace ElectricERP.Application.Interfaces
{
    public interface ITransferenciaService
    {
        Task<List<TransferenciaDto>> ObtenerTodosAsync();
        Task<TransferenciaDto?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string? Mensaje, TransferenciaDto? Creada)> SolicitarAsync(CrearTransferenciaDto dto, string idSupervisor);
        Task<(bool Exito, string? Mensaje)> AutorizarAsync(int id, AutorizarTransferenciaDto dto, string idAdmin);
        Task<(bool Exito, string? Mensaje)> AsignarAsync(int id, AsignarTransferenciaDto dto);
        Task<(bool Exito, string? Mensaje, UbicacionDto? Ubicacion)> IniciarAsync(int id, string idTransportista, RegistrarUbicacionDto ubicacionInicial);
        Task<(bool Exito, string? Mensaje)> EntregarAsync(int id, string idTransportista);
        Task<(bool Exito, string? Mensaje)> RecibirAsync(int id);
    }
}