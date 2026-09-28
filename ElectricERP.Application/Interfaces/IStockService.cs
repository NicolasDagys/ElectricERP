using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Stock;

namespace ElectricERP.Application.Interfaces
{
    public interface IStockService
    {
        Task<List<StockDto>> ObtenerTodosAsync();
        Task<StockDto?> ObtenerPorIdAsync(int id);
        Task<List<StockDto>> ObtenerAlertasAsync();
        Task<(bool Exito, string? Mensaje, StockDto? Creado)> CrearAsync(CrearStockDto dto);
        Task<(bool Exito, string? Mensaje)> RegistrarCantidadAsync(int id, RegistrarStockDto dto);
        Task<(bool Exito, string? Mensaje)> ActualizarLimitesAsync(int id, ActualizarLimitesDto dto);
    }
}