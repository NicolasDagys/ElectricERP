using ElectricERP.Application.DTOs.Inventarios;
using ElectricERP.Application.DTOs.Ajustes;
using ElectricERP.Application.DTOs.Almacenes;
using ElectricERP.Application.DTOs.Auth;
using ElectricERP.Application.DTOs.Productos;
using ElectricERP.Application.DTOs.Secciones;
using ElectricERP.Application.DTOs.Stock;
using ElectricERP.Application.DTOs.Suppliers;
using ElectricERP.Application.DTOs.Transferencias;
using ElectricERP.Application.DTOs.Users;
using ElectricERP.Application.DTOs.Vehiculos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ElectricERP.Web.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        private void SetAuthHeader(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<string> ExtraerMensajeError(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return msg.GetString() ?? "Error desconocido.";
                return json;
            }
            catch
            {
                return "Error desconocido.";
            }
        }

        // --- AUTH ---
        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/login", content);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions);
        }

        public async Task<TwoFactorSetupDto?> SetupTwoFactorAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.PostAsync("api/auth/2fa/setup", null);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwoFactorSetupDto>(json, _jsonOptions);
        }

        public async Task<bool> EnableTwoFactorAsync( string code, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(new { Code = code }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/2fa/enable", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<AuthResponseDto?> VerifyTwoFactorAsync(VerifyTwoFactorDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8,"application/json");
            var response = await _httpClient.PostAsync("api/auth/2fa/verify", content);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthResponseDto>(json, _jsonOptions);
        }

        // --- USUARIOS---
        public async Task<List<UserDto>> GetUsuariosAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/users");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UserDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<bool> CrearUsuarioAsync(CreateUserDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/users", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarUsuarioAsync(string id, UpdateUserDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/users/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CambiarPasswordAsync(ChangePasswordDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/change-password", content);
            return response.IsSuccessStatusCode;
        }

        public async Task CambiarEstadoUsuarioAsync(string id, bool activo, string token)
        {
            SetAuthHeader(token);

            string endpoint = activo ? $"api/users/{id}/activate" : $"api/users/{id}/deactivate";

            var response = await _httpClient.PatchAsync(endpoint, null);

            response.EnsureSuccessStatusCode();
        }

        public async Task<(bool Exito, string? Mensaje)> ResetearPasswordUsuarioAsync(string id, string nuevaPassword, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { NewPassword = nuevaPassword }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/users/{id}/reset-password", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }


        // --- ALMACENES ---
        public async Task<List<AlmacenDto>> GetAlmacenesAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/almacenes");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AlmacenDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<bool> CrearAlmacenAsync(CrearAlmacenDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/almacenes", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarAlmacenAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.DeleteAsync($"api/almacenes/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<SeccionDto>> GetSeccionesAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/sections");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SeccionDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<bool> CrearSeccionAsync(CrearSeccionDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/sections", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarSeccionAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.DeleteAsync($"api/sections/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarAlmacenAsync(int id, CrearAlmacenDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/almacenes/{id}", content);
            return response.IsSuccessStatusCode;
        }

        

        

        // --- PROVEEDORES ---
        public async Task<List<SupplierDto>> GetProveedoresAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/suppliers");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SupplierDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<(bool Exito, string? Mensaje)> CrearProveedorAsync(CrearSupplierDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/suppliers", content);

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarProveedorAsync(int id, CrearSupplierDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/suppliers/{id}", content);

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<bool> EliminarProveedorAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.DeleteAsync($"api/suppliers/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- PRODUCTOS ---
        public async Task<List<ProductoDto>> GetProductosAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/products");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductoDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<List<ProductoDto>> BuscarProductosAsync(string texto, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync($"api/products/search?text={Uri.EscapeDataString(texto ?? string.Empty)}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductoDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<(bool Exito, string? Mensaje)> CrearProductoAsync(CrearProductoDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/products", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarProductoAsync(int id, ActualizarProductoDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/products/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<bool> EliminarProductoAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.DeleteAsync($"api/products/{id}");
            return response.IsSuccessStatusCode;
        }

        // --- STOCK ---
        public async Task<List<StockDto>> GetStockAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/stocks");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<StockDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<List<StockDto>> GetAlertasStockAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/stocks/alerts");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<StockDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<(bool Exito, string? Mensaje)> CrearStockAsync(CrearStockDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/stocks", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> RegistrarCantidadAsync(int id, int cantidadActual, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { CantidadActual = cantidadActual }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/stocks/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarLimitesStockAsync(int id, int minimo, int maximo, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { Minimo = minimo, Maximo = maximo }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/stocks/{id}/limits", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        // --- VEHÍCULOS ---
        public async Task<List<VehiculoDto>> GetVehiculosAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/vehicles");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<VehiculoDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<(bool Exito, string? Mensaje)> CrearVehiculoAsync(CrearVehiculoDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/vehicles", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarVehiculoAsync(int id, CrearVehiculoDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/vehicles/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> EliminarVehiculoAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.DeleteAsync($"api/vehicles/{id}");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        // --- TRANSFERENCIAS ---
        public async Task<List<TransferenciaDto>> GetTransferenciasAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/transfers");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TransferenciaDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<TransferenciaDto?> GetTransferenciaPorIdAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync($"api/transfers/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TransferenciaDto>(json, _jsonOptions);
        }

        public async Task<(bool Exito, string? Mensaje)> SolicitarTransferenciaAsync(CrearTransferenciaDto dto, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/transfers", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> AutorizarTransferenciaAsync(int id, bool aprobado, string? motivo, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { Aprobado = aprobado, Motivo = motivo }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/transfers/{id}/authorization", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> AsignarTransferenciaAsync(int id, int idVehiculo, string idTransportista, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { IdVehiculo = idVehiculo, IdTransportista = idTransportista }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/transfers/{id}/assignment", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        // --- EJECUCIÓN DE TRANSFERENCIAS ---
        public async Task<(bool Exito, string? Mensaje)> IniciarTransferenciaAsync(int id, decimal latitud, decimal longitud, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { Latitud = latitud, Longitud = longitud }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/transfers/{id}/start", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> EntregarTransferenciaAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.PatchAsync($"api/transfers/{id}/deliver", null);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> RecibirTransferenciaAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.PatchAsync($"api/transfers/{id}/receive", null);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        // --- GPS ---
        public async Task<(bool Exito, string? Mensaje)> SimularUbicacionAsync(string idTransportista, decimal latitud, decimal longitud, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { IdTransportista = idTransportista, Latitud = latitud, Longitud = longitud }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/gps/simulate", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<List<UbicacionDto>> GetRecorridoAsync(int idTransferencia, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync($"api/gps/route/{idTransferencia}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UbicacionDto>>(json, _jsonOptions) ?? new();
        }

        // --- INVENTARIOS ---
        public async Task<List<InventarioDto>> GetInventariosAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/inventories");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<InventarioDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<InventarioDto?> GetInventarioPorIdAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync($"api/inventories/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InventarioDto>(json, _jsonOptions);
        }

        public async Task<(bool Exito, string? Mensaje)> CrearInventarioAsync(int idSeccion, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { IdSeccion = idSeccion }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/inventories", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> ActualizarDetalleInventarioAsync(int id, List<ItemInventarioDto> productos, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { Productos = productos }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/inventories/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> CerrarInventarioAsync(int id, string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.PatchAsync($"api/inventories/{id}/close", null);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        // --- SOLICITUDES DE AJUSTE ---
        public async Task<List<SolicitudAjusteDto>> GetAjustesAsync(string token)
        {
            SetAuthHeader(token);
            var response = await _httpClient.GetAsync("api/adjustment-requests");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SolicitudAjusteDto>>(json, _jsonOptions) ?? new();
        }

        public async Task<(bool Exito, string? Mensaje)> CrearAjusteAsync(int idProducto, string tipo, string motivo, int cantidadSolicitada, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { IdProducto = idProducto, Tipo = tipo, Motivo = motivo, CantidadSolicitada = cantidadSolicitada }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/adjustment-requests", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }

        public async Task<(bool Exito, string? Mensaje)> ResolverAjusteAsync(int id, bool aprobado, string token)
        {
            SetAuthHeader(token);
            var content = new StringContent(
                JsonSerializer.Serialize(new { Aprobado = aprobado }),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/adjustment-requests/{id}/resolution", content);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtraerMensajeError(response));
        }
    }
}