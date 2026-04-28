using Workshop.Application.DTOs.Productos;

namespace Workshop.Application.Services
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> GetAllAsync();
        Task<ProductoDto?> GetByIdAsync(Guid id);
        Task<ProductoDto> CreateAsync(CreateProductoDto dto);
        Task<ProductoDto?> UpdateAsync(UpdateProductoDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<byte[]> GenerarPlantillaAsync();
        Task<ImportarResultadoDto> ImportarAsync(Stream excelStream);
    }
}
