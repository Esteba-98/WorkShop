using Workshop.Application.DTOs.Stats;

namespace Workshop.Application.Services
{
    public interface IStatsService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(string periodo = "mes");
        Task<List<MecanicoStatsDto>> GetMecanicoStatsAsync(string periodo = "mes");
    }
}
