using ElectricERP.Application.DTOs.Suppliers;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class ProveedoresController : Controller
    {
        private readonly IApiClient _apiClient;

        public ProveedoresController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var proveedores = await _apiClient.GetProveedoresAsync(token);
            return View(proveedores);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearSupplierDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CrearProveedorAsync(dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Proveedor creado correctamente."
                : $"No se pudo crear el proveedor: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, CrearSupplierDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.ActualizarProveedorAsync(id, dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Proveedor actualizado correctamente."
                : $"No se pudo actualizar el proveedor: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var exito = await _apiClient.EliminarProveedorAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Proveedor eliminado correctamente."
                : "No se pudo eliminar el proveedor: puede tener productos asociados.";

            return RedirectToAction("Index");
        }
    }
}