using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workshop.Application.DTOs.Mantenimientos;
using Workshop.Application.Services;

namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MantenimientosController : ControllerBase
    {
        private readonly IMantenimientoService _mantenimientoService;

        public MantenimientosController(IMantenimientoService mantenimientoService)
        {
            _mantenimientoService = mantenimientoService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mantenimientoService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var m = await _mantenimientoService.GetByIdAsync(id);
            if (m == null) return NotFound();
            return Ok(m);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> Create([FromBody] CreateMantenimientoDto dto)
        {
            var m = await _mantenimientoService.CreateAsync(dto);
            return Ok(m);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMantenimientoDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var m = await _mantenimientoService.UpdateAsync(dto);
            if (m == null) return NotFound();
            return Ok(m);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _mantenimientoService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // POST /api/Mantenimientos/{id}/items — agregar producto o servicio a la orden
        [HttpPost("{id}/items")]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] AddItemDto dto)
        {
            var m = await _mantenimientoService.AddItemAsync(id, dto);
            if (m == null) return BadRequest(new { message = "No se pudo agregar el item. Verifica el stock disponible." });
            return Ok(m);
        }

        // DELETE /api/Mantenimientos/{id}/items/{itemId} — quitar item de la orden
        [HttpDelete("{id}/items/{itemId}")]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
        {
            var m = await _mantenimientoService.RemoveItemAsync(id, itemId);
            if (m == null) return NotFound();
            return Ok(m);
        }

        // GET /api/Mantenimientos/export/excel — exportar a Excel (solo Admin)
        [HttpGet("export/excel")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var bytes = await _mantenimientoService.ExportExcelAsync(desde, hasta);
            var filename = $"ordenes_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }

        // PATCH /api/Mantenimientos/{id}/pagado — toggle estado de pago (solo Admin)
        [HttpPatch("{id}/pagado")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> TogglePagado(Guid id)
        {
            var m = await _mantenimientoService.TogglePagadoAsync(id);
            if (m == null) return NotFound();
            return Ok(m);
        }
    }
}
