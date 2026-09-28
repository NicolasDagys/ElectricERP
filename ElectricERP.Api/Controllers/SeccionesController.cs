using ElectricERP.Application.DTOs.Secciones;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/sections")]
    public class SectionsController : ControllerBase
    {
        private readonly ISeccionService _seccionService;

        public SectionsController(ISeccionService seccionService)
        {
            _seccionService = seccionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SeccionDto>>> Get()
        {
            return Ok(await _seccionService.ObtenerTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeccionDto>> GetById(int id)
        {
            var seccion = await _seccionService.ObtenerPorIdAsync(id);
            if (seccion == null) return NotFound();
            return Ok(seccion);
        }

        [HttpPost]
        public async Task<ActionResult<SeccionDto>> Post([FromBody] CrearSeccionDto dto)
        {
            var creado = await _seccionService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdSeccion }, creado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _seccionService.EliminarAsync(id);

            if (!resultado.Exito)
            {
                if (resultado.Mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = resultado.Mensaje });
            }

            return NoContent();
        }
    }
}