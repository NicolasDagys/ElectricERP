using ElectricERP.Application.DTOs.Inventarios;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class InventariosController : Controller
    {
        private readonly IApiClient _apiClient;

        public InventariosController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var inventarios = await _apiClient.GetInventariosAsync(token);
            ViewBag.Secciones = await _apiClient.GetSeccionesAsync(token);
            ViewBag.Productos = await _apiClient.GetProductosAsync(token);

            return View(inventarios);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(int idSeccion)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CrearInventarioAsync(idSeccion, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Inventario iniciado correctamente."
                : $"No se pudo iniciar el inventario: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AgregarConteo(int id, int idProducto, int cantidadInventario)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var productos = new List<ItemInventarioDto>
            {
                new() { IdProducto = idProducto, CantidadInventario = cantidadInventario }
            };

            var (exito, mensaje) = await _apiClient.ActualizarDetalleInventarioAsync(id, productos, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Conteo registrado correctamente."
                : $"No se pudo registrar el conteo: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cerrar(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CerrarInventarioAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Inventario cerrado correctamente."
                : $"No se pudo cerrar el inventario: {mensaje}";

            return RedirectToAction("Index");
        }
    }
}