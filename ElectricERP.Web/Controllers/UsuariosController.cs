using ElectricERP.Application.DTOs.Users;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IApiClient _apiClient;

        public UsuariosController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string GetJwtToken() => User.FindFirst("JWToken")?.Value ?? string.Empty;

        public async Task<IActionResult> Index()
        {
            var usuarios = await _apiClient.GetUsuariosAsync(GetJwtToken());
            return View(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CreateUserDto dto)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _apiClient.CrearUsuarioAsync(dto, GetJwtToken());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Editar(string id, UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));
            await _apiClient.ActualizarUsuarioAsync(id, dto, GetJwtToken());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(string id, bool activo)
        {
            await _apiClient.CambiarEstadoUsuarioAsync(id, activo, GetJwtToken());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ResetearPassword(string id, string nuevaPassword)
        {
            var token = User.FindFirst("JWToken")?.Value;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.ResetearPasswordUsuarioAsync(id, nuevaPassword, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Contraseña reseteada correctamente."
                : $"No se pudo resetear la contraseña: {mensaje}";

            return RedirectToAction("Index");
        }
    }
}