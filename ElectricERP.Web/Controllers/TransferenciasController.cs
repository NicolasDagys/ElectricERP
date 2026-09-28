using ElectricERP.Application.DTOs.Transferencias;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class TransferenciasController : Controller
    {
        private readonly IApiClient _apiClient;

        public TransferenciasController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var transferencias = await _apiClient.GetTransferenciasAsync(token);

            ViewBag.Almacenes = await _apiClient.GetAlmacenesAsync(token);
            ViewBag.Productos = await _apiClient.GetProductosAsync(token);
            ViewBag.Vehiculos = await _apiClient.GetVehiculosAsync(token);
            ViewBag.Usuarios = await _apiClient.GetUsuariosAsync(token);

            return View(transferencias);
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar(CrearTransferenciaDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.SolicitarTransferenciaAsync(dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Transferencia solicitada correctamente."
                : $"No se pudo solicitar la transferencia: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Autorizar(int id, bool aprobado, string? motivo)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.AutorizarTransferenciaAsync(id, aprobado, motivo, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? (aprobado ? "Transferencia autorizada." : "Transferencia rechazada.")
                : $"No se pudo resolver la autorización: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Asignar(int id, int idVehiculo, string idTransportista)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.AsignarTransferenciaAsync(id, idVehiculo, idTransportista, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Vehículo y transportista asignados correctamente."
                : $"No se pudo asignar: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Iniciar(int id, decimal latitud, decimal longitud)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.IniciarTransferenciaAsync(id, latitud, longitud, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Transferencia iniciada. Ya está en tránsito."
                : $"No se pudo iniciar la transferencia: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Entregar(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.EntregarTransferenciaAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Entrega confirmada."
                : $"No se pudo confirmar la entrega: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Recibir(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.RecibirTransferenciaAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Recepción confirmada. Transferencia finalizada."
                : $"No se pudo confirmar la recepción: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SimularAvance(int id, decimal latitud, decimal longitud)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var transferencia = await _apiClient.GetTransferenciaPorIdAsync(id, token);
            if (transferencia?.IdTransportista != null)
            {
                await _apiClient.SimularUbicacionAsync(transferencia.IdTransportista, latitud, longitud, token);
            }

            return RedirectToAction("Mapa", new { id });
        }

        public async Task<IActionResult> Mapa(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var transferencia = await _apiClient.GetTransferenciaPorIdAsync(id, token);
            if (transferencia == null) return NotFound();

            ViewBag.Recorrido = await _apiClient.GetRecorridoAsync(id, token);

            return View(transferencia);
        }
    }
}