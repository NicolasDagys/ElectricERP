using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class AjustesController : Controller
    {
        private readonly IApiClient _apiClient;

        public AjustesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var ajustes = await _apiClient.GetAjustesAsync(token);
            ViewBag.Productos = await _apiClient.GetProductosAsync(token);

            return View(ajustes);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(int idProducto, string tipo, string motivo, int cantidadSolicitada)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CrearAjusteAsync(idProducto, tipo, motivo, cantidadSolicitada, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Solicitud de ajuste creada correctamente."
                : $"No se pudo crear la solicitud: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Resolver(int id, bool aprobado)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.ResolverAjusteAsync(id, aprobado, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? (aprobado ? "Solicitud aprobada." : "Solicitud rechazada.")
                : $"No se pudo resolver la solicitud: {mensaje}";

            return RedirectToAction("Index");
        }
    }
}