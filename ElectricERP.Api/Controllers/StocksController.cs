using Microsoft.AspNetCore.Mvc;
using ElectricERP.Application.DTOs.Stock;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/stocks")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StocksController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StockDto>>> Get()
        {
            return Ok(await _stockService.ObtenerTodosAsync());
        }

        [HttpGet("alerts")]
        public async Task<ActionResult<List<StockDto>>> GetAlerts()
        {
            return Ok(await _stockService.ObtenerAlertasAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockDto>> GetById(int id)
        {
            var stock = await _stockService.ObtenerPorIdAsync(id);
            if (stock == null) return NotFound();
            return Ok(stock);
        }

        [HttpPost]
        public async Task<ActionResult<StockDto>> Post([FromBody] CrearStockDto dto)
        {
            var (exito, mensaje, creado) = await _stockService.CrearAsync(dto);
            if (!exito) return BadRequest(new { message = mensaje });
            return CreatedAtAction(nameof(GetById), new { id = creado!.IdStock }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RegistrarStockDto dto)
        {
            var (exito, mensaje) = await _stockService.RegistrarCantidadAsync(id, dto);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }

        [HttpPut("{id}/limits")]
        public async Task<IActionResult> PutLimits(int id, [FromBody] ActualizarLimitesDto dto)
        {
            var (exito, mensaje) = await _stockService.ActualizarLimitesAsync(id, dto);
            if (!exito)
            {
                if (mensaje == "No encontrado") return NotFound();
                return BadRequest(new { message = mensaje });
            }
            return NoContent();
        }
    }
}