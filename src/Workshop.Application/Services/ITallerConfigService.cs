using Workshop.Application.DTOs.TallerConfig;

namespace Workshop.Application.Services
{
    public interface ITallerConfigService
    {
        Task<TallerConfigDto> GetAsync();
        Task<TallerConfigDto> UpdateAsync(UpdateTallerConfigDto dto);
    }
}
