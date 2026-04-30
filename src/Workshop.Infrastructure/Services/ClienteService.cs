using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Clientes;
using Workshop.Application.DTOs.Historial;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class ClienteService : IClienteService
    {
        private readonly WorkshopDbContext _context;

        public ClienteService(WorkshopDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClienteDto>> GetAllAsync()
        {
            return await _context.Clientes
                .Select(c => new ClienteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    Telefono = c.Telefono
                })
                .ToListAsync();
        }

        public async Task<ClienteDto?> GetByIdAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return null;

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono
            };
        }

        public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Email = dto.Email,
                Telefono = dto.Telefono
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono
            };
        }

        public async Task<ClienteDto?> UpdateAsync(UpdateClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.Id);
            if (cliente == null) return null;

            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;
            cliente.Telefono = dto.Telefono;

            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ClienteHistorialDto?> GetHistorialAsync(Guid id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Vehiculos)
                    .ThenInclude(v => v.Mantenimientos)
                        .ThenInclude(m => m.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null) return null;

            var vehiculos = cliente.Vehiculos.Select(v =>
            {
                var ordenes = v.Mantenimientos
                    .OrderByDescending(m => m.Fecha)
                    .Select(m => new OrdenResumenDto
                    {
                        Id = m.Id,
                        Folio = m.Folio,
                        Fecha = m.Fecha,
                        FechaEntrega = m.FechaEntrega,
                        Estado = m.Estado,
                        Descripcion = m.Descripcion,
                        TotalItems = m.Items.Count,
                        Total = m.Items.Sum(i => i.Subtotal)
                    })
                    .ToList();

                return new VehiculoEnHistorialDto
                {
                    Id = v.Id,
                    Placa = v.Placa,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Anio = v.Anio,
                    TotalOrdenes = ordenes.Count,
                    Ordenes = ordenes
                };
            }).ToList();

            return new ClienteHistorialDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                TotalOrdenes = vehiculos.Sum(v => v.TotalOrdenes),
                TotalFacturado = vehiculos.Sum(v => v.Ordenes.Sum(o => o.Total)),
                Vehiculos = vehiculos
            };
        }
    }
}