using ElectricERP.Application.DTOs.Inventarios;
using ElectricERP.Application.DTOs.Ajustes;
using ElectricERP.Application.DTOs.Almacenes;
using ElectricERP.Application.DTOs.Auth;
using ElectricERP.Application.DTOs.Productos;
using ElectricERP.Application.DTOs.Secciones;
using ElectricERP.Application.DTOs.Stock;
using ElectricERP.Application.DTOs.Suppliers;
using ElectricERP.Application.DTOs.Users;
using ElectricERP.Application.DTOs.Vehiculos;
using ElectricERP.Application.DTOs.Transferencias;

namespace ElectricERP.Web.Services
{
    public interface IApiClient
    {
        // Auth
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<TwoFactorSetupDto?> SetupTwoFactorAsync(string token);
        Task<bool> EnableTwoFactorAsync(string code, string token);
        Task<AuthResponseDto?> VerifyTwoFactorAsync(VerifyTwoFactorDto dto);

        // Usuarios
        Task<List<UserDto>> GetUsuariosAsync(string token);
        Task<bool> CrearUsuarioAsync(CreateUserDto dto, string token);
        Task<bool> ActualizarUsuarioAsync(string id, UpdateUserDto dto, string token);
        Task<bool> CambiarPasswordAsync(ChangePasswordDto dto, string token);
        Task CambiarEstadoUsuarioAsync(string id, bool activo, string token);
        Task<(bool Exito, string? Mensaje)> ResetearPasswordUsuarioAsync(string id, string nuevaPassword, string token);

        // Almacenes
        Task<List<AlmacenDto>> GetAlmacenesAsync(string token);
        Task<bool> CrearAlmacenAsync(CrearAlmacenDto dto, string token);
        Task<bool> EliminarAlmacenAsync(int id, string token);
        Task<List<SeccionDto>> GetSeccionesAsync(string token);
        Task<bool> CrearSeccionAsync(CrearSeccionDto dto, string token);
        Task<bool> EliminarSeccionAsync(int id, string token);
        Task<bool> ActualizarAlmacenAsync(int id, CrearAlmacenDto dto, string token);

        // Proveedores
        Task<List<SupplierDto>> GetProveedoresAsync(string token);
        Task<(bool Exito, string? Mensaje)> CrearProveedorAsync(CrearSupplierDto dto, string token);
        Task<(bool Exito, string? Mensaje)> ActualizarProveedorAsync(int id, CrearSupplierDto dto, string token);
        Task<bool> EliminarProveedorAsync(int id, string token);

        // Productos
        Task<List<ProductoDto>> GetProductosAsync(string token);
        Task<List<ProductoDto>> BuscarProductosAsync(string texto, string token);
        Task<(bool Exito, string? Mensaje)> CrearProductoAsync(CrearProductoDto dto, string token);
        Task<(bool Exito, string? Mensaje)> ActualizarProductoAsync(int id, ActualizarProductoDto dto, string token);
        Task<bool> EliminarProductoAsync(int id, string token);

        // Stock
        Task<List<StockDto>> GetStockAsync(string token);
        Task<List<StockDto>> GetAlertasStockAsync(string token);
        Task<(bool Exito, string? Mensaje)> CrearStockAsync(CrearStockDto dto, string token);
        Task<(bool Exito, string? Mensaje)> RegistrarCantidadAsync(int id, int cantidadActual, string token);
        Task<(bool Exito, string? Mensaje)> ActualizarLimitesStockAsync(int id, int minimo, int maximo, string token);

        // Vehículos
        Task<List<VehiculoDto>> GetVehiculosAsync(string token);
        Task<(bool Exito, string? Mensaje)> CrearVehiculoAsync(CrearVehiculoDto dto, string token);
        Task<(bool Exito, string? Mensaje)> ActualizarVehiculoAsync(int id, CrearVehiculoDto dto, string token);
        Task<(bool Exito, string? Mensaje)> EliminarVehiculoAsync(int id, string token);

        // Transferencias
        Task<List<TransferenciaDto>> GetTransferenciasAsync(string token);
        Task<TransferenciaDto?> GetTransferenciaPorIdAsync(int id, string token);
        Task<(bool Exito, string? Mensaje)> SolicitarTransferenciaAsync(CrearTransferenciaDto dto, string token);
        Task<(bool Exito, string? Mensaje)> AutorizarTransferenciaAsync(int id, bool aprobado, string? motivo, string token);
        Task<(bool Exito, string? Mensaje)> AsignarTransferenciaAsync(int id, int idVehiculo, string idTransportista, string token);

        // Ejecución de transferencias
        Task<(bool Exito, string? Mensaje)> IniciarTransferenciaAsync(int id, decimal latitud, decimal longitud, string token);
        Task<(bool Exito, string? Mensaje)> EntregarTransferenciaAsync(int id, string token);
        Task<(bool Exito, string? Mensaje)> RecibirTransferenciaAsync(int id, string token);

        // GPS
        Task<List<UbicacionDto>> GetRecorridoAsync(int idTransferencia, string token);
        Task<(bool Exito, string? Mensaje)> SimularUbicacionAsync(string idTransportista, decimal latitud, decimal longitud, string token);

        // Inventarios
        Task<List<InventarioDto>> GetInventariosAsync(string token);
        Task<InventarioDto?> GetInventarioPorIdAsync(int id, string token);
        Task<(bool Exito, string? Mensaje)> CrearInventarioAsync(int idSeccion, string token);
        Task<(bool Exito, string? Mensaje)> ActualizarDetalleInventarioAsync(int id, List<ItemInventarioDto> productos, string token);
        Task<(bool Exito, string? Mensaje)> CerrarInventarioAsync(int id, string token);

        // Solicitudes de ajuste
        Task<List<SolicitudAjusteDto>> GetAjustesAsync(string token);
        Task<(bool Exito, string? Mensaje)> CrearAjusteAsync(int idProducto, string tipo, string motivo, int cantidadSolicitada, string token);
        Task<(bool Exito, string? Mensaje)> ResolverAjusteAsync(int id, bool aprobado, string token);
    }
}