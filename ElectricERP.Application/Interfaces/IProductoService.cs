using ElectricERP.Application.DTOs.Productos;

namespace ElectricERP.Application.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> ObtenerTodosAsync();
        Task<ProductoDto?> ObtenerPorIdAsync(int id);
        Task<List<ProductoDto>> BuscarAsync(string texto);
        Task<(bool Exito, string? Mensaje, ProductoDto? Creado)> CrearAsync(CrearProductoDto dto);
        Task<(bool Exito, string? Mensaje)> ActualizarAsync(int id, ActualizarProductoDto dto);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
    }
}