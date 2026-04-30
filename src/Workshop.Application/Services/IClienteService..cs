using Workshop.Application.DTOs.Clientes;
using Workshop.Application.DTOs.Historial;

namespace Workshop.Application.Services
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(Guid id);
        Task<ClienteDto> CreateAsync(CreateClienteDto dto);
        Task<ClienteDto?> UpdateAsync(UpdateClienteDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ClienteHistorialDto?> GetHistorialAsync(Guid id);
    }
}
