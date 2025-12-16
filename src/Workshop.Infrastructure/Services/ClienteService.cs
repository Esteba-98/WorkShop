using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.Clientes;
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
            };
        }

        public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Email = dto.Email,
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email
            };
        }

        public async Task<ClienteDto?> UpdateAsync(UpdateClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.Id);
            if (cliente == null) return null;

            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;

            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Email = cliente.Email
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
    }
}
