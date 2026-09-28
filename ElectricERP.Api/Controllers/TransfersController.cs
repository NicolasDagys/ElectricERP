using System.Security.Claims;
using ElectricERP.Application.DTOs.Transferencias;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transfers")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferenciaService _transferenciaService;

        public TransfersController(ITransferenciaService transferenciaService)
        {
            _transferenciaService = transferenciaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TransferenciaDto>>> Get()
        {
            return Ok(await _transferenciaService.ObtenerTodosAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransferenciaDto>> GetById(int id)
        {
            var t = await _transferenciaService.ObtenerPorIdAsync(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<List<DetalleTransferenciaItemDto>>> GetDetails(int id)
        {
            var t = await _transferenciaService.ObtenerPorIdAsync(id);
            if (t == null) return NotFound();
            return Ok(t.Detalles);
        }

        [HttpPost]
        public async Task<ActionResult<TransferenciaDto>> Post([FromBody] CrearTransferenciaDto dto)
        {
            var idSupervisor = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idSupervisor)) return Unauthorized();

            var (exito, mensaje, creada) = await _transferenciaService.SolicitarAsync(dto, idSupervisor);
            if (!exito) return BadRequest(new { message = mensaje });
            return CreatedAtAction(nameof(GetById), new { id = creada!.IdTransferencia }, creada);
        }

        [HttpPost("{id}/authorization")]
        public async Task<IActionResult> Authorize(int id, [FromBody] AutorizarTransferenciaDto dto)
        {
            var idAdmin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idAdmin)) return Unauthorized();

            var (exito, mensaje) = await _transferenciaService.AutorizarAsync(id, dto, idAdmin);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }

        [HttpPost("{id}/assignment")]
        public async Task<IActionResult> Assign(int id, [FromBody] AsignarTransferenciaDto dto)
        {
            var (exito, mensaje) = await _transferenciaService.AsignarAsync(id, dto);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }

        [HttpPatch("{id}/start")]
        public async Task<IActionResult> Start(int id, [FromBody] RegistrarUbicacionDto dto)
        {
            var idTransportista = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idTransportista)) return Unauthorized();

            var (exito, mensaje, ubicacion) = await _transferenciaService.IniciarAsync(id, idTransportista, dto);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return Ok(ubicacion);
        }

        [HttpPatch("{id}/deliver")]
        public async Task<IActionResult> Deliver(int id)
        {
            var idTransportista = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idTransportista)) return Unauthorized();

            var (exito, mensaje) = await _transferenciaService.EntregarAsync(id, idTransportista);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }

        [HttpPatch("{id}/receive")]
        public async Task<IActionResult> Receive(int id)
        {
            var (exito, mensaje) = await _transferenciaService.RecibirAsync(id);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }
    }
}