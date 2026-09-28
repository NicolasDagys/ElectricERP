using ElectricERP.Application.DTOs.Productos;
using ElectricERP.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Web.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IApiClient _apiClient;

        public ProductosController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        private string? Token => User.FindFirst("JWToken")?.Value;

        public async Task<IActionResult> Index(string? q)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var productos = string.IsNullOrWhiteSpace(q)
                ? await _apiClient.GetProductosAsync(token)
                : await _apiClient.BuscarProductosAsync(q, token);

            ViewBag.Proveedores = await _apiClient.GetProveedoresAsync(token);
            ViewBag.Categorias = ProductoOpciones.Categorias;
            ViewBag.UnidadesMedida = ProductoOpciones.UnidadesMedida;
            ViewBag.Query = q;

            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearProductoDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.CrearProductoAsync(dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Producto creado correctamente."
                : $"No se pudo crear el producto: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, ActualizarProductoDto dto)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var (exito, mensaje) = await _apiClient.ActualizarProductoAsync(id, dto, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Producto actualizado correctamente."
                : $"No se pudo actualizar el producto: {mensaje}";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var exito = await _apiClient.EliminarProductoAsync(id, token);
            TempData[exito ? "MensajeExito" : "Error"] = exito
                ? "Producto eliminado correctamente."
                : "No se pudo eliminar el producto: puede tener stock o movimientos asociados.";

            return RedirectToAction("Index");
        }
    }
}