using ElectricERP.Application.DTOs.Almacenes;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class AlmacenesController : Controller
    {
        private readonly IApiClient _apiClient;

        public AlmacenesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string GetJwtToken() => User.FindFirst("JWToken")?.Value ?? string.Empty;

        public async Task<IActionResult> Index()
        {
            var almacenes = await _apiClient.GetAlmacenesAsync(GetJwtToken());
            return View(almacenes);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearAlmacenDto dto)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _apiClient.CrearAlmacenAsync(dto, GetJwtToken());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _apiClient.EliminarAlmacenAsync(id, GetJwtToken());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, CrearAlmacenDto dto)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _apiClient.ActualizarAlmacenAsync(id, dto, GetJwtToken());
            return RedirectToAction(nameof(Index));
        }
    }
}