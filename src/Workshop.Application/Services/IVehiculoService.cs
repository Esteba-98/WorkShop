using Workshop.Application.DTOs.Vehiculos;
using Workshop.Application.DTOs.Historial;

namespace Workshop.Application.Services
{
    public interface IVehiculoService
    {
        Task<List<VehiculoDto>> GetAllAsync();
        Task<VehiculoDto?> GetByIdAsync(Guid id);
        Task<VehiculoDto> CreateAsync(CreateVehiculoDto dto);
        Task<VehiculoDto?> UpdateAsync(UpdateVehiculoDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<VehiculoHistorialDto?> GetHistorialAsync(Guid id);
    }
}
