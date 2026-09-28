using ElectricERP.Application.DTOs.Vehiculos;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class VehiculosController : Controller
    {
        private readonly IApiClient _apiClient;

        public VehiculosController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var vehiculos = await _apiClient.GetVehiculosAsync(token);
            return View(vehiculos);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearVehiculoDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CrearVehiculoAsync(dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Vehículo creado correctamente."
                : $"No se pudo crear el vehículo: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, CrearVehiculoDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.ActualizarVehiculoAsync(id, dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Vehículo actualizado correctamente."
                : $"No se pudo actualizar el vehículo: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.EliminarVehiculoAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Vehículo eliminado correctamente."
                : $"No se pudo eliminar el vehículo: {mensaje}";

            return RedirectToAction("Index");
        }
    }
}