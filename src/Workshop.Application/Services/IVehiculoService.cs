using Workshop.Application.DTOs.Vehiculos;

namespace Workshop.Application.Services
{
    public interface IVehiculoService
    {
        Task<List<VehiculoDto>> GetAllAsync();
        Task<VehiculoDto?> GetByIdAsync(Guid id);
        Task<VehiculoDto> CreateAsync(CreateVehiculoDto dto);
        Task<VehiculoDto?> UpdateAsync(UpdateVehiculoDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
