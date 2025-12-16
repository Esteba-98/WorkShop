using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Vehiculos;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly WorkshopDbContext _context;

        public VehiculoService(WorkshopDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehiculoDto>> GetAllAsync()
        {
            return await _context.Vehiculos
                .Select(v => new VehiculoDto
                {
                    Id = v.Id,
                    Placa = v.Placa,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Anio = v.Anio,
                    ClienteId = v.ClienteId
                })
                .ToListAsync();
        }

        public async Task<VehiculoDto?> GetByIdAsync(Guid id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return null;

            return new VehiculoDto
            {
                Id = vehiculo.Id,
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Anio = vehiculo.Anio,
                ClienteId = vehiculo.ClienteId
            };
        }

        public async Task<VehiculoDto> CreateAsync(CreateVehiculoDto dto)
        {
            var vehiculo = new Vehiculo
            {
                Id = Guid.NewGuid(),
                Placa = dto.Placa,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Anio = dto.Anio,
                ClienteId = dto.ClienteId
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return new VehiculoDto
            {
                Id = vehiculo.Id,
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Anio = vehiculo.Anio,
                ClienteId = vehiculo.ClienteId
            };
        }

        public async Task<VehiculoDto?> UpdateAsync(UpdateVehiculoDto dto)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(dto.Id);
            if (vehiculo == null) return null;

            vehiculo.Placa = dto.Placa;
            vehiculo.Marca = dto.Marca;
            vehiculo.Modelo = dto.Modelo;
            vehiculo.Anio = dto.Anio;
            vehiculo.ClienteId = dto.ClienteId;

            await _context.SaveChangesAsync();

            return new VehiculoDto
            {
                Id = vehiculo.Id,
                Placa = vehiculo.Placa,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Anio = vehiculo.Anio,
                ClienteId = vehiculo.ClienteId
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return false;

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
