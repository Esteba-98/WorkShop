using Microsoft.EntityFrameworkCore;
using Workshop.Application.DTOs.TallerConfig;
using Workshop.Application.Services;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Persistence;

namespace Workshop.Infrastructure.Services
{
    public class TallerConfigService : ITallerConfigService
    {
        private static readonly Guid ConfigId = new Guid("00000000-0000-0000-0000-000000000001");
        private readonly WorkshopDbContext _context;

        public TallerConfigService(WorkshopDbContext context)
        {
            _context = context;
        }

        public async Task<TallerConfigDto> GetAsync()
        {
            var config = await _context.TallerConfigs.FindAsync(ConfigId);
            if (config == null)
                return new TallerConfigDto();

            return Map(config);
        }

        public async Task<TallerConfigDto> UpdateAsync(UpdateTallerConfigDto dto)
        {
            var config = await _context.TallerConfigs.FindAsync(ConfigId);
            if (config == null)
            {
                config = new TallerConfig { Id = ConfigId };
                _context.TallerConfigs.Add(config);
            }

            config.Nombre = dto.Nombre;
            config.Telefono = dto.Telefono;
            config.Direccion = dto.Direccion;
            config.Email = dto.Email;
            config.NIT = dto.NIT;

            await _context.SaveChangesAsync();
            return Map(config);
        }

        private static TallerConfigDto Map(TallerConfig c) => new()
        {
            Nombre = c.Nombre,
            Telefono = c.Telefono,
            Direccion = c.Direccion,
            Email = c.Email,
            NIT = c.NIT
        };
    }
}
