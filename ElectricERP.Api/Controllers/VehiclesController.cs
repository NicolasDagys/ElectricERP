using ElectricERP.Application.DTOs.Vehiculos;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoService;

        public VehiclesController(IVehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VehiculoDto>>> Get()
        {
            return Ok(await _vehiculoService.ObtenerTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehiculoDto>> GetById(int id)
        {
            var vehiculo = await _vehiculoService.ObtenerPorIdAsync(id);
            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        [HttpPost]
        public async Task<ActionResult<VehiculoDto>> Post([FromBody] CrearVehiculoDto dto)
        {
            var (exito, mensaje, creado) = await _vehiculoService.CrearAsync(dto);
            if (!exito) return BadRequest(new { message = mensaje });
            return CreatedAtAction(nameof(GetById), new { id = creado!.IdVehiculo }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CrearVehiculoDto dto)
        {
            var (exito, mensaje) = await _vehiculoService.ActualizarAsync(id, dto);
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
            var (exito, mensaje) = await _vehiculoService.EliminarAsync(id);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }
    }
}