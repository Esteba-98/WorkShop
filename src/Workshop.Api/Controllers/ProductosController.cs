using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workshop.Application.Services;
using Workshop.Application.DTOs.Productos;


namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,OperarioAlmacen,Mecanico")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productoService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,OperarioAlmacen,Mecanico")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([FromBody] CreateProductoDto dto)
        {
            var producto = await _productoService.CreateAsync(dto);
            return Ok(producto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,OperarioAlmacen")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductoDto dto)
        {
            if (id != dto.Id) return BadRequest();
            var producto = await _productoService.UpdateAsync(dto);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productoService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
