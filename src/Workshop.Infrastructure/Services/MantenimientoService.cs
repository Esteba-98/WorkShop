using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Mantenimientos;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly WorkshopDbContext _context;

        public MantenimientoService(WorkshopDbContext context)
        {
            _context = context;
        }

        public async Task<List<MantenimientoDto>> GetAllAsync()
        {
            return await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Select(m => new MantenimientoDto
                {
                    Id = m.Id,
                    Fecha = m.Fecha,
                    Estado = m.Estado,
                    VehiculoId = m.VehiculoId,
                    ClienteId = m.ClienteId
                })
                .ToListAsync();
        }

        public async Task<MantenimientoDto?> GetByIdAsync(Guid id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento == null) return null;

            return new MantenimientoDto
            {
                Id = mantenimiento.Id,
                Fecha = mantenimiento.Fecha,
                Estado = mantenimiento.Estado,
                VehiculoId = mantenimiento.VehiculoId,
                ClienteId = mantenimiento.ClienteId
            };
        }

        public async Task<MantenimientoDto> CreateAsync(CreateMantenimientoDto dto)
        {
            var mantenimiento = new Mantenimiento
            {
                Id = Guid.NewGuid(),
                VehiculoId = dto.VehiculoId,
                ClienteId = dto.ClienteId,
                Fecha = DateTime.UtcNow,
                Estado = "Abierto"
            };

            _context.Mantenimientos.Add(mantenimiento);
            await _context.SaveChangesAsync();

            return new MantenimientoDto
            {
                Id = mantenimiento.Id,
                Fecha = mantenimiento.Fecha,
                Estado = mantenimiento.Estado,
                VehiculoId = mantenimiento.VehiculoId,
                ClienteId = mantenimiento.ClienteId
            };
        }

        public async Task<MantenimientoDto?> UpdateAsync(UpdateMantenimientoDto dto)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(dto.Id);
            if (mantenimiento == null) return null;

            mantenimiento.Estado = dto.Estado;
            mantenimiento.VehiculoId = dto.VehiculoId;
            mantenimiento.ClienteId = dto.ClienteId;

            await _context.SaveChangesAsync();

            return new MantenimientoDto
            {
                Id = mantenimiento.Id,
                Fecha = mantenimiento.Fecha,
                Estado = mantenimiento.Estado,
                VehiculoId = mantenimiento.VehiculoId,
                ClienteId = mantenimiento.ClienteId
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var mantenimiento = await _context.Mantenimientos.FindAsync(id);
            if (mantenimiento == null) return false;

            _context.Mantenimientos.Remove(mantenimiento);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
