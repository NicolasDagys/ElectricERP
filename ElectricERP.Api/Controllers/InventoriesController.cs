using ElectricERP.Application.DTOs.Inventarios;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/inventories")]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventoriesController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventarioDto>>> Get()
        {
            return Ok(await _inventarioService.ObtenerTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventarioDto>> GetById(int id)
        {
            var inv = await _inventarioService.ObtenerPorIdAsync(id);
            if (inv == null) return NotFound();
            return Ok(inv);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<List<DetalleInventarioItemDto>>> GetDetails(int id)
        {
            var inv = await _inventarioService.ObtenerPorIdAsync(id);
            if (inv == null) return NotFound();
            return Ok(inv.Detalles);
        }

        [HttpPost]
        public async Task<ActionResult<InventarioDto>> Post([FromBody] CrearInventarioDto dto)
        {
            var (exito, mensaje, creado) = await _inventarioService.CrearAsync(dto);
            if (!exito) return BadRequest(new { message = mensaje });
            return CreatedAtAction(nameof(GetById), new { id = creado!.IdInventario }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ActualizarInventarioDto dto)
        {
            var (exito, mensaje) = await _inventarioService.ActualizarDetalleAsync(id, dto);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }

        [HttpPatch("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var (exito, mensaje) = await _inventarioService.CerrarAsync(id);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }
    }
}