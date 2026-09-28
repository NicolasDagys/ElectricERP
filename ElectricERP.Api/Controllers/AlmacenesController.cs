using ElectricERP.Application.DTOs.Almacenes;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/almacenes")]
    public class AlmacenesController : ControllerBase
    {
        private readonly IAlmacenService _almacenService;

        public AlmacenesController(IAlmacenService almacenService)
        {
            _almacenService = almacenService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AlmacenDto>>> Get()
        {
            var almacenes = await _almacenService.ObtenerTodosAsync();
            return Ok(almacenes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlmacenDto>> GetById(int id)
        {
            var almacen = await _almacenService.ObtenerPorIdAsync(id);
            if (almacen == null) return NotFound();
            return Ok(almacen);
        }

        [HttpPost]
        public async Task<ActionResult<AlmacenDto>> Post([FromBody] CrearAlmacenDto dto)
        {
            var creado = await _almacenService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdAlmacen }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CrearAlmacenDto dto)
        {
            var actualizado = await _almacenService.ActualizarAsync(id, dto);
            if (!actualizado) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _almacenService.EliminarAsync(id);

            if (!resultado.Exito)
            {
                if (resultado.Mensaje == "No encontrado")
                    return NotFound();

                return BadRequest(new { message = resultado.Mensaje });
            }

            return NoContent();
        }
    }
}