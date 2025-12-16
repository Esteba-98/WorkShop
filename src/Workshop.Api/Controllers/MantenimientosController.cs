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
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mantenimientoService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var mantenimiento = await _mantenimientoService.GetByIdAsync(id);
            if (mantenimiento == null) return NotFound();
            return Ok(mantenimiento);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Mecanico")]
        public async Task<IActionResult> Create([FromBody] CreateMantenimientoDto dto)
        {
            var mantenimiento = await _mantenimientoService.CreateAsync(dto);
            return Ok(mantenimiento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMantenimientoDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var mantenimiento = await _mantenimientoService.UpdateAsync(dto);
            if (mantenimiento == null) return NotFound();
            return Ok(mantenimiento);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _mantenimientoService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
