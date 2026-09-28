using System.Security.Claims;
using ElectricERP.Application.DTOs.Transferencias;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/gps")]
    public class GpsController : ControllerBase
    {
        private readonly IGpsService _gpsService;

        public GpsController(IGpsService gpsService)
        {
            _gpsService = gpsService;
        }

        [HttpPost("location")]
        public async Task<IActionResult> RegisterLocation([FromBody] RegistrarUbicacionDto dto)
        {
            var idTransportista = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idTransportista)) return Unauthorized();

            var (exito, mensaje) = await _gpsService.RegistrarUbicacionAsync(idTransportista, dto);
            if (!exito) return BadRequest(new { message = mensaje });
            return NoContent();
        }

        [HttpGet("route/{idTransferencia}")]
        public async Task<ActionResult<List<UbicacionDto>>> GetRoute(int idTransferencia)
        {
            return Ok(await _gpsService.ObtenerRecorridoAsync(idTransferencia));
        }

        [HttpGet("current")]
        public async Task<ActionResult<UbicacionDto>> GetCurrent([FromQuery] string idTransportista)
        {
            var ubicacion = await _gpsService.ObtenerUbicacionActualAsync(idTransportista);
            if (ubicacion == null) return NotFound();
            return Ok(ubicacion);
        }

        // Endpoint solo para pruebas: permite simular la posición de CUALQUIER transportista
        // sin depender de quién esté logueado. No debería usarse en producción con la app móvil real,
        // ahí cada transportista reporta su propia ubicación vía POST /api/gps/location.
        [HttpPost("simulate")]
        public async Task<IActionResult> SimulateLocation([FromBody] SimularUbicacionDto dto)
        {
            var (exito, mensaje) = await _gpsService.RegistrarUbicacionAsync(dto.IdTransportista, new RegistrarUbicacionDto
            {
                Latitud = dto.Latitud,
                Longitud = dto.Longitud
            });
            if (!exito) return BadRequest(new { message = mensaje });
            return NoContent();
        }
    }
}