using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workshop.Application.DTOs.Clientes;
using Workshop.Application.Services;

namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos requieren estar logueados, pero con roles afinamos permisos
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // Cualquier rol logueado puede ver la lista de clientes
        [HttpGet]
        public async Task<ActionResult<List<ClienteDto>>> GetAll()
        {
            return Ok(await _clienteService.GetAllAsync());
        }

        // Cualquier rol logueado puede ver un cliente específico
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetById(Guid id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        // SOLO el Administrador puede crear clientes
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Create(CreateClienteDto dto)
        {
            var cliente = await _clienteService.CreateAsync(dto);
            return Ok(cliente);
        }

        // SOLO el Administrador puede actualizar clientes
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ClienteDto>> Update(Guid id, UpdateClienteDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var cliente = await _clienteService.UpdateAsync(dto);
            if (cliente == null) return NotFound();

            return Ok(cliente);
        }

        // SOLO el Administrador puede eliminar clientes
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _clienteService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
