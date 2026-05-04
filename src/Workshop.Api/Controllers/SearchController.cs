using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly WorkshopDbContext _context;

        public SearchController(WorkshopDbContext context)
        {
            _context = context;
        }

        // GET /api/Search?q=term — búsqueda global en clientes, vehículos y mantenimientos
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Ok(new { clientes = Array.Empty<object>(), vehiculos = Array.Empty<object>(), mantenimientos = Array.Empty<object>() });

            var term = q.Trim().ToLower();

            var clientes = await _context.Clientes
                .Where(c => c.Nombre.ToLower().Contains(term) ||
                            (c.Email != null && c.Email.ToLower().Contains(term)) ||
                            (c.Telefono != null && c.Telefono.ToLower().Contains(term)))
                .Take(6)
                .Select(c => new { id = c.Id, nombre = c.Nombre, email = c.Email, telefono = c.Telefono })
                .ToListAsync();

            var vehiculos = await _context.Vehiculos
                .Include(v => v.Cliente)
                .Where(v => v.Placa.ToLower().Contains(term) ||
                            v.Marca.ToLower().Contains(term) ||
                            v.Modelo.ToLower().Contains(term))
                .Take(6)
                .Select(v => new { id = v.Id, placa = v.Placa, marca = v.Marca, modelo = v.Modelo, anio = v.Anio, clienteNombre = v.Cliente != null ? v.Cliente.Nombre : "" })
                .ToListAsync();

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Cliente)
                .Include(m => m.Vehiculo)
                .Where(m => m.Folio.ToLower().Contains(term) ||
                            (m.Cliente != null && m.Cliente.Nombre.ToLower().Contains(term)) ||
                            (m.Vehiculo != null && m.Vehiculo.Placa.ToLower().Contains(term)))
                .OrderByDescending(m => m.Fecha)
                .Take(6)
                .Select(m => new
                {
                    id = m.Id,
                    folio = m.Folio,
                    estado = m.Estado,
                    clienteNombre = m.Cliente != null ? m.Cliente.Nombre : "",
                    vehiculoPlaca = m.Vehiculo != null ? m.Vehiculo.Placa : "",
                    fecha = m.Fecha
                })
                .ToListAsync();

            return Ok(new { clientes, vehiculos, mantenimientos });
        }
    }
}
