using System.Security.Claims;
using ElectricERP.Application.DTOs.Ajustes;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/adjustment-requests")]
    public class AdjustmentRequestsController : ControllerBase
    {
        private readonly IAjusteService _ajusteService;

        public AdjustmentRequestsController(IAjusteService ajusteService)
        {
            _ajusteService = ajusteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SolicitudAjusteDto>>> Get()
        {
            return Ok(await _ajusteService.ObtenerTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudAjusteDto>> GetById(int id)
        {
            var s = await _ajusteService.ObtenerPorIdAsync(id);
            if (s == null) return NotFound();
            return Ok(s);
        }

        [HttpPost]
        public async Task<ActionResult<SolicitudAjusteDto>> Post([FromBody] CrearSolicitudAjusteDto dto)
        {
            var idEmpleado = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idEmpleado)) return Unauthorized();

            var (exito, mensaje, creada) = await _ajusteService.CrearAsync(dto, idEmpleado);
            if (!exito) return BadRequest(new { message = mensaje });
            return CreatedAtAction(nameof(GetById), new { id = creada!.IdSolicitud }, creada);
        }

        [HttpPost("{id}/resolution")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolverSolicitudAjusteDto dto)
        {
            var idSupervisor = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idSupervisor)) return Unauthorized();

            var (exito, mensaje) = await _ajusteService.ResolverAsync(id, dto, idSupervisor);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }
    }
}