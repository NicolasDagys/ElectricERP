using ElectricERP.Application.DTOs.Secciones;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class SeccionesController : Controller
    {
        private readonly IApiClient _apiClient;

        public SeccionesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var secciones = await _apiClient.GetSeccionesAsync(token);
            ViewBag.Almacenes = await _apiClient.GetAlmacenesAsync(token);

            return View(secciones);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearSeccionDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisá los datos ingresados.";
                return RedirectToAction("Index");
            }

            var exito = await _apiClient.CrearSeccionAsync(dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Sección creada correctamente."
                : "No se pudo crear la sección.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var exito = await _apiClient.EliminarSeccionAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Sección eliminada correctamente."
                : "No se pudo eliminar la sección.";

            return RedirectToAction("Index");
        }
    }
}
