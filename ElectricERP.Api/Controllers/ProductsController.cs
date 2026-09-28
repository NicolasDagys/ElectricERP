using ElectricERP.Application.DTOs.Productos;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductsController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDto>>> Get()
        {
            return Ok(await _productoService.ObtenerTodosAsync());
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductoDto>>> Search([FromQuery] string text)
        {
            return Ok(await _productoService.BuscarAsync(text ?? string.Empty));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetById(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Post([FromBody] CrearProductoDto dto)
        {
            var (exito, mensaje, creado) = await _productoService.CrearAsync(dto);
            if (!exito) return BadRequest(new { message = mensaje });
            return CreatedAtAction(nameof(GetById), new { id = creado!.IdProducto }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActualizarProductoDto dto)
        {
            var (exito, mensaje) = await _productoService.ActualizarAsync(id, dto);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _productoService.EliminarAsync(id);

            if (!resultado.Exito)
            {
                if (resultado.Mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = resultado.Mensaje });
            }

            return NoContent();
        }
    }
}