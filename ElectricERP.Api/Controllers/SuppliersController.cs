using ElectricERP.Application.DTOs.Suppliers;
using ElectricERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricERP.Api.Controllers
{
    [Authorize]
    [Route("api/suppliers")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupplierDto>>> Get()
        {
            var suppliers = await _supplierService.ObtenerTodosAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            var supplier = await _supplierService.ObtenerPorIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Post([FromBody] CrearSupplierDto dto)
        {
            var nuevoSupplier = await _supplierService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nuevoSupplier.IdProveedor }, nuevoSupplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CrearSupplierDto dto)
        {
            var actualizado = await _supplierService.ActualizarAsync(id, dto);
            if (!actualizado) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _supplierService.EliminarAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}