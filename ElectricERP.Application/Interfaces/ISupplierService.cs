using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectricERP.Application.DTOs.Suppliers;

namespace ElectricERP.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> ObtenerTodosAsync();
        Task<SupplierDto?> ObtenerPorIdAsync(int id);
        Task<SupplierDto> CrearAsync(CrearSupplierDto dto);
        Task<bool> ActualizarAsync(int id, CrearSupplierDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
