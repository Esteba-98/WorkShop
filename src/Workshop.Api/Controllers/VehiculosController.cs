using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workshop.Application.DTOs.Vehiculos;
using Workshop.Application.Services;

namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiculosController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoService;

        public VehiculosController(IVehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _vehiculoService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var vehiculo = await _vehiculoService.GetByIdAsync(id);
            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,User")]
        public async Task<IActionResult> Create([FromBody] CreateVehiculoDto dto)
        {
            var vehiculo = await _vehiculoService.CreateAsync(dto);
            return Ok(vehiculo);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,User")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehiculoDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var vehiculo = await _vehiculoService.UpdateAsync(dto);
            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _vehiculoService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/historial")]
        [Authorize(Roles = "Administrador,Mecanico,User")]
        public async Task<IActionResult> GetHistorial(Guid id)
        {
            var historial = await _vehiculoService.GetHistorialAsync(id);
            if (historial == null) return NotFound();
            return Ok(historial);
        }
    }
}
