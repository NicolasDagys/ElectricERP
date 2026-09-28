using ElectricERP.Application.DTOs.Stock;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly IApiClient _apiClient;

        public StockController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index(bool soloAlertas = false)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var stock = soloAlertas
                ? await _apiClient.GetAlertasStockAsync(token)
                : await _apiClient.GetStockAsync(token);

            ViewBag.Productos = await _apiClient.GetProductosAsync(token);
            ViewBag.Secciones = await _apiClient.GetSeccionesAsync(token);
            ViewBag.SoloAlertas = soloAlertas;

            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearStockDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CrearStockAsync(dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Stock creado correctamente."
                : $"No se pudo crear el stock: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int id, int cantidadActual)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.RegistrarCantidadAsync(id, cantidadActual, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Cantidad actualizada correctamente."
                : $"No se pudo registrar el stock: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarLimites(int id, int minimo, int maximo)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.ActualizarLimitesStockAsync(id, minimo, maximo, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Límites actualizados correctamente."
                : $"No se pudieron actualizar los límites: {mensaje}";

            return RedirectToAction("Index");
        }
    }
}