using Workshop.Application.DTOs.Mantenimientos;

namespace Workshop.Application.Services
{
    public interface IMantenimientoService
    {
        Task<List<MantenimientoDto>> GetAllAsync();
        Task<MantenimientoDto?> GetByIdAsync(Guid id);
        Task<MantenimientoDto> CreateAsync(CreateMantenimientoDto dto);
        Task<MantenimientoDto?> UpdateAsync(UpdateMantenimientoDto dto);
        Task<bool> DeleteAsync(Guid id);

        // Items de la orden
        Task<MantenimientoDto?> AddItemAsync(Guid mantenimientoId, AddItemDto dto);
        Task<MantenimientoDto?> RemoveItemAsync(Guid mantenimientoId, Guid itemId);

        // Pago
        Task<MantenimientoDto?> TogglePagadoAsync(Guid id);

        // Exportación
        Task<byte[]> ExportExcelAsync(DateTime? desde, DateTime? hasta);
    }
}
